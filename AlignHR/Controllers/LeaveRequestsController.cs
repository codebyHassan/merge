using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AlignHR.Data;
using AlignHR.Models;
using AlignHR.Services;
using AlignHR.Security;
using X.PagedList;

namespace AlignHR.Controllers
{
    public class LeaveRequestsController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly ILeaveWorkflowService _workflowService;
        private readonly ILeaveAccountingService _accountingService;

        public LeaveRequestsController(
            ApplicationDbContext context,
            ILeaveWorkflowService workflowService,
            ILeaveAccountingService accountingService)
        {
            _context = context;
            _workflowService = workflowService;
            _accountingService = accountingService;
        }

        // ═══════════════════════════════════════════════════════════════
        //  INDEX — All Leave Requests
        // ═══════════════════════════════════════════════════════════════

        public async Task<IActionResult> Index(int? page, string searchQuery, string status)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            ViewData["CurrentFilter"] = searchQuery;
            ViewData["CurrentStatus"] = status;

            // Get pending inbox count for the logged-in user (for badge)
            var currentEmployeeId = await GetCurrentEmployeeIdAsync();
            if (currentEmployeeId.HasValue)
            {
                var pendingCount = await _context.LeaveRequests
                    .CountAsync(lr => lr.CurrentApproverId == currentEmployeeId.Value
                                   && lr.Status == "Pending");
                ViewData["PendingInboxCount"] = pendingCount;
            }

            var query = _context.LeaveRequests
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .Include(l => l.CurrentApprover)
                .AsQueryable();

            // Filter: Only show my own requests OR requests I applied for on behalf of others
            // Restricted as requested: "they only show who's employee i applied and self"
            if (currentEmployeeId.HasValue)
            {
                var myEmpIdStr = currentEmployeeId.Value.ToString();
                query = query.Where(l => l.EmployeeId == currentEmployeeId.Value || l.CreatedBy == myEmpIdStr);
            }

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(l => l.Employee.FirstName.Contains(searchQuery) ||
                                         l.Employee.LastName.Contains(searchQuery) ||
                                         l.Reason.Contains(searchQuery));
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(l => l.Status == status);
            }

            var pagedList = await query.OrderByDescending(l => l.AppliedAt ?? l.CreatedAt)
                .ToPagedListAsync(pageNumber, pageSize);

            // Fetch names for CreatedBy IDs to show who applied (Self vs Name)
            var createdByIds = pagedList.Select(l => l.CreatedBy).Where(id => !string.IsNullOrEmpty(id)).Distinct().ToList();
            var submitterNames = await _context.emp
                .Where(e => createdByIds.Contains(e.Id.ToString()))
                .ToDictionaryAsync(e => e.Id.ToString(), e => e.FullName);
            ViewData["SubmitterNames"] = submitterNames;

            return View(pagedList);
        }

        // ═══════════════════════════════════════════════════════════════
        //  APPROVAL INBOX — Pending requests for logged-in approver
        // ═══════════════════════════════════════════════════════════════

        public async Task<IActionResult> ApprovalInbox(int? page, string searchQuery)
        {
            int pageSize = 10;
            int pageNumber = page ?? 1;

            ViewData["CurrentFilter"] = searchQuery;

            var currentEmployeeId = await GetCurrentEmployeeIdAsync();
            if (!currentEmployeeId.HasValue)
            {
                TempData["Error"] = "Your user account is not linked to an employee record. Please contact HR.";
                return RedirectToAction(nameof(Index));
            }

            var pendingRequests = await _workflowService.GetApprovalInboxAsync(currentEmployeeId.Value);

            IEnumerable<LeaveRequest> filtered = pendingRequests;
            if (!string.IsNullOrEmpty(searchQuery))
            {
                filtered = filtered.Where(lr =>
                    (lr.Employee?.FullName?.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (lr.Employee?.Code?.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (lr.LeaveType?.Name?.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ?? false));
            }

            ViewData["CurrentEmployeeId"] = currentEmployeeId.Value;

            var pagedList = filtered.ToPagedList(pageNumber, pageSize);
            return View(pagedList);
        }

        // ═══════════════════════════════════════════════════════════════
        //  APPROVE PAGE — View request details + take action
        // ═══════════════════════════════════════════════════════════════

        [HttpGet]
        public async Task<IActionResult> Approve(decimal? id)
        {
            if (id == null) return NotFound();

            var leaveRequest = await _context.LeaveRequests
                .Include(l => l.Employee)
                    .ThenInclude(e => e.Department)
                .Include(l => l.Employee)
                    .ThenInclude(e => e.Designation)
                .Include(l => l.Employee)
                    .ThenInclude(e => e.Location)
                .Include(l => l.LeaveType)
                .Include(l => l.CurrentApprover)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (leaveRequest == null) return NotFound();

            // Load approval history
            var approvalHistory = await _workflowService.GetApprovalHistoryAsync(id.Value);
            ViewData["ApprovalHistory"] = approvalHistory;

            var currentEmployeeId = await GetCurrentEmployeeIdAsync();
            ViewData["CurrentEmployeeId"] = currentEmployeeId;

            return View(leaveRequest);
        }

        // ═══════════════════════════════════════════════════════════════
        //  APPROVE ACTION (POST)
        // ═══════════════════════════════════════════════════════════════

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveRequest(decimal leaveRequestId, string? remarks)
        {
            var currentEmployeeId = await GetCurrentEmployeeIdAsync();
            if (!currentEmployeeId.HasValue)
            {
                TempData["Error"] = "Your user account is not linked to an employee record.";
                return RedirectToAction(nameof(ApprovalInbox));
            }

            try
            {
                await _workflowService.ApproveAsync(leaveRequestId, currentEmployeeId.Value, remarks);
                TempData["Success"] = "Leave request approved successfully.";
            }
            catch (DbUpdateConcurrencyException)
            {
                TempData["Error"] = "This request was already processed by another user. Please refresh and try again.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(ApprovalInbox));
        }

        // ═══════════════════════════════════════════════════════════════
        //  REJECT ACTION (POST)
        // ═══════════════════════════════════════════════════════════════

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectRequest(decimal leaveRequestId, string reason)
        {
            var currentEmployeeId = await GetCurrentEmployeeIdAsync();
            if (!currentEmployeeId.HasValue)
            {
                TempData["Error"] = "Your user account is not linked to an employee record.";
                return RedirectToAction(nameof(ApprovalInbox));
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                TempData["Error"] = "Rejection reason is required.";
                return RedirectToAction(nameof(Approve), new { id = leaveRequestId });
            }

            try
            {
                await _workflowService.RejectAsync(leaveRequestId, currentEmployeeId.Value, reason);
                TempData["Success"] = "Leave request rejected.";
            }
            catch (DbUpdateConcurrencyException)
            {
                TempData["Error"] = "This request was already processed by another user. Please refresh.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(ApprovalInbox));
        }

        // ═══════════════════════════════════════════════════════════════
        //  DETAILS — View request with full workflow history
        // ═══════════════════════════════════════════════════════════════

        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null) return NotFound();

            var leaveRequest = await _context.LeaveRequests
                .Include(l => l.Employee)
                    .ThenInclude(e => e.Department)
                .Include(l => l.Employee)
                    .ThenInclude(e => e.Location)
                .Include(l => l.LeaveType)
                .Include(l => l.CurrentApprover)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (leaveRequest == null) return NotFound();

            // Load approval history for timeline
            var approvalHistory = await _workflowService.GetApprovalHistoryAsync(id.Value);
            ViewData["ApprovalHistory"] = approvalHistory;

            return View(leaveRequest);
        }

        // ═══════════════════════════════════════════════════════════════
        //  CREATE — Submit new leave request (starts workflow)
        // ═══════════════════════════════════════════════════════════════

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var session = PermissionHelper.GetUserSession(HttpContext);
            bool canApplyForOthers = session?.CanApplyForOthers ?? false;
            ViewData["CanApplyForOthers"] = canApplyForOthers;

            if (!canApplyForOthers)
            {
                var employeeId = await GetCurrentEmployeeIdAsync();
                if (employeeId.HasValue)
                {
                    var emp = await _context.emp
                        .Include(e => e.Department)
                        .Include(e => e.Location)
                        .Include(e => e.EmploymentType)
                        .Include(e => e.EmploymentStatus)
                        .Include(e => e.SubDepartment)
                        .Include(e => e.Grade)
                        .Include(e => e.Division)
                        .Include(e => e.Designation)
                        .FirstOrDefaultAsync(e => e.Id == employeeId.Value);
                    ViewData["DefaultEmployee"] = emp;
                    // Also useful for the hidden field
                    ViewData["CurrentEmployeeId"] = employeeId.Value;
                }
            }

            ViewData["LeaveTypeId"] = new SelectList(_context.LeaveTypes, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EmployeeId,LeaveTypeId,FromDate,ToDate,DaysCount,IsHalfDay,HalfDaySession,Reason,AttachmentUrl")] LeaveRequest leaveRequest)
        {
            var session = PermissionHelper.GetUserSession(HttpContext);
            var myEmployeeId = session?.EmployeeId ?? 0;
            
            // Security Validation
            if (!(session?.CanApplyForOthers ?? false))
            {
                // Force own employee ID if they don't have delegation permission
                leaveRequest.EmployeeId = myEmployeeId;
            }
            else
            {
                // Restrict to same department for EVERYONE (no bypass)
                var selectedEmp = await _context.emp.FindAsync(leaveRequest.EmployeeId);
                if (selectedEmp == null || selectedEmp.DepartmentFk != session.DepartmentId)
                {
                    ModelState.AddModelError("EmployeeId", "You can only request leave for employees in your own department.");
                }
            }

            // 3. Policy Rule Validation
            var (isValid, message, isWarning) = await _accountingService.ValidateLeavePolicyRulesAsync(
                leaveRequest.EmployeeId, 
                leaveRequest.LeaveTypeId, 
                leaveRequest.FromDate, 
                leaveRequest.ToDate, 
                leaveRequest.DaysCount, 
                leaveRequest.IsHalfDay);

            if (!isValid)
            {
                ModelState.AddModelError("", message);
            }
            else if (isWarning)
            {
                // Warn but allow as per "Scenario 1: Alert him when he apply"
                TempData["Warning"] = message;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Pass the applicant's ID to the workflow service so it can follow their hierarchy if applying for others
                    await _workflowService.StartWorkflowAsync(leaveRequest, myEmployeeId);
                    TempData["Success"] = "Leave request submitted and sent for approval.";
                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            
            // Re-populate ViewData for error return
            bool canApplyForOthers = session?.CanApplyForOthers ?? false;
            ViewData["CanApplyForOthers"] = canApplyForOthers;
            if (!canApplyForOthers && session?.EmployeeId != null)
            {
                ViewData["DefaultEmployee"] = await _context.emp.FindAsync(session.EmployeeId);
            }

            ViewData["LeaveTypeId"] = new SelectList(_context.LeaveTypes, "Id", "Name", leaveRequest.LeaveTypeId);
            return View(leaveRequest);
        }

        // ═══════════════════════════════════════════════════════════════
        //  EDIT (kept for HR admin corrections on pending requests)
        // ═══════════════════════════════════════════════════════════════

        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null) return NotFound();

            var session = PermissionHelper.GetUserSession(HttpContext);
            if (!(session?.CanApplyForOthers ?? false))
            {
                TempData["Error"] = "Leave requests cannot be edited after submission.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var leaveRequest = await _context.LeaveRequests.FindAsync(id);
            if (leaveRequest == null) return NotFound();

            ViewData["LeaveTypeId"] = new SelectList(_context.LeaveTypes, "Id", "Name", leaveRequest.LeaveTypeId);
            return View(leaveRequest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Id,EmployeeId,LeaveTypeId,FromDate,ToDate,DaysCount,IsHalfDay,HalfDaySession,Reason,AttachmentUrl,Status,CurrentApproverId,CurrentStage,ApprovalLevel,RowVersion")] LeaveRequest leaveRequest)
        {
            if (id != leaveRequest.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(leaveRequest);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Leave request updated.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LeaveRequestExists(leaveRequest.Id))
                        return NotFound();

                    TempData["Error"] = "Concurrency conflict. The record was modified by another user.";
                    return RedirectToAction(nameof(Edit), new { id });
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["LeaveTypeId"] = new SelectList(_context.LeaveTypes, "Id", "Name", leaveRequest.LeaveTypeId);
            return View(leaveRequest);
        }

        // ═══════════════════════════════════════════════════════════════
        //  DELETE
        // ═══════════════════════════════════════════════════════════════

        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null) return NotFound();

            var leaveRequest = await _context.LeaveRequests
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (leaveRequest == null) return NotFound();
            return View(leaveRequest);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            var leaveRequest = await _context.LeaveRequests.FindAsync(id);
            if (leaveRequest != null)
            {
                // Remove audit trail first
                var approvals = await _context.LeaveApprovals
                    .Where(la => la.LeaveRequestId == id)
                    .ToListAsync();
                _context.LeaveApprovals.RemoveRange(approvals);

                _context.LeaveRequests.Remove(leaveRequest);
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Leave request deleted.";
            return RedirectToAction(nameof(Index));
        }

        // ═══════════════════════════════════════════════════════════════
        //  EMPLOYEE SEARCH (HTMX)
        // ═══════════════════════════════════════════════════════════════

        [HttpGet]
        public async Task<IActionResult> SearchEmployees(string term)
        {
            var session = PermissionHelper.GetUserSession(HttpContext);
            if (!(session?.CanApplyForOthers ?? false))
            {
                return PartialView("_EmployeeSearchResults", new List<Employee>());
            }

            if (string.IsNullOrEmpty(term) || term.Length < 2)
            {
                return PartialView("_EmployeeSearchResults", new List<Employee>());
            }

            var query = _context.emp
                .Include(e => e.Department)
                .Include(e => e.Location)
                .Include(e => e.EmploymentType)
                .Include(e => e.EmploymentStatus)
                .Include(e => e.SubDepartment)
                .Include(e => e.Grade)
                .Include(e => e.Division)
                .Where(e => e.FirstName.Contains(term) || 
                            e.LastName.Contains(term) || 
                            e.Code.Contains(term));

            // Restriction: Only show employees from the same department as the user
            // NO Super Admin bypass as requested
            var userDeptId = session.DepartmentId;
            if (userDeptId.HasValue)
            {
                query = query.Where(e => e.DepartmentFk == userDeptId.Value);
            }
            else
            {
                // If the user doesn't have a department, they shouldn't see anyone
                return PartialView("_EmployeeSearchResults", new List<Employee>());
            }

            var employees = await query.Take(10).ToListAsync();

            return PartialView("_EmployeeSearchResults", employees);
        }

        // ═══════════════════════════════════════════════════════════════
        //  HELPERS
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// Resolves the current logged-in user's Employee ID from session.
        /// </summary>
        private async Task<int?> GetCurrentEmployeeIdAsync()
        {
            var session = PermissionHelper.GetUserSession(HttpContext);
            if (session == null) return null;

            var user = await _context.users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == session.UserId);

            return user?.EmployeeId;
        }

        private bool LeaveRequestExists(decimal id)
        {
            return _context.LeaveRequests.Any(e => e.Id == id);
        }
    }
}
