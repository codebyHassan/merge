using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AlignHR.Data;
using AlignHR.Models;
using AlignHR.Models.Enums;
using AlignHR.Services;
using AlignHR.Security;

namespace AlignHR.Controllers
{
    public class LeavesController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public LeavesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? year)
        {
            var session = PermissionHelper.GetUserSession(HttpContext);

            // Check if HR or Manager is "Viewing" someone else
            int? hrSelectedEmpId = HttpContext.Session.GetInt32("HR_View_EmployeeId");
            int? hrSelectedYear = HttpContext.Session.GetInt32("HR_View_Year");

            if (session == null || session.EmployeeId == null)
            {
                return View(new LeaveDashboardViewModel());
            }

            // Fetch the logged-in employee to check permissions and department
            var currentEmployee = await _context.emp
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.Id == session.EmployeeId);

            if (currentEmployee == null) return View(new LeaveDashboardViewModel());

            int currentEmpId = hrSelectedEmpId ?? session.EmployeeId.Value;
            // HR session year takes priority, then query param, then current year
            int currentYear = hrSelectedYear ?? year ?? DateTime.Now.Year;
            var displayEmployee = currentEmpId == currentEmployee.Id
                ? currentEmployee
                : await _context.emp.FindAsync(currentEmpId);
            
            ViewBag.IsHRView = hrSelectedEmpId != null;
            ViewBag.CanApplyForOthers = currentEmployee.CanApplyForOthers;

            var vm = new LeaveDashboardViewModel();
            vm.EmployeeName = displayEmployee?.FullName ?? currentEmployee.FullName;
            vm.EmployeeCode = displayEmployee?.Code ?? currentEmployee.Code;
            vm.CurrentYear = currentYear;

            // If they have the flag, fetch their department members
            if (currentEmployee.CanApplyForOthers)
            {
                ViewBag.DepartmentMembers = await _context.emp
                    .Where(e => e.DepartmentFk == currentEmployee.DepartmentFk && e.Id != currentEmployee.Id && !e.IsServiceEnded)
                    .OrderBy(e => e.FirstName)
                    .Select(e => new { e.Id, e.FullName, e.Code })
                    .ToListAsync();
            }

            vm.ActivePoliciesCount = await _context.EmployeeLeavePolicies
                .Where(elp => elp.EmployeeId == currentEmpId && elp.IsActive)
                .CountAsync();

            // 2. Fetch Ledger (History of this year)
            vm.Ledger = await _context.LeaveTransactions
                .Include(t => t.LeaveType)
                .Where(t => t.EmployeeFk == currentEmpId && t.Year == currentYear)
                .OrderByDescending(t => t.TransactionDate)
                .ThenByDescending(t => t.Id)
                .Take(50) // Limit to latest 50 for dashboard performance
                .ToListAsync();

            var yearRequests = await _context.LeaveRequests
                .Include(r => r.LeaveType)
                .Where(r => r.EmployeeId == currentEmpId && r.FromDate.Year == currentYear)
                .ToListAsync();

            vm.TotalRequests = yearRequests.Count;
            vm.PendingRequests = yearRequests.Count(r => r.Status == "Pending");
            vm.ApprovedRequests = yearRequests.Count(r => r.Status == "Approved");
            vm.RejectedRequests = yearRequests.Count(r => r.Status == "Rejected");
            vm.CancelledRequests = yearRequests.Count(r => r.Status == "Cancelled");
            vm.RecentRequests = yearRequests
                .OrderByDescending(r => r.CreatedAt ?? r.AppliedAt ?? r.FromDate)
                .ThenByDescending(r => r.Id)
                .Take(5)
                .ToList();

            // Leave balance summary per leave type for the current year
            vm.LeaveBalances = vm.Ledger
                .Where(t => t.LeaveType != null)
                .GroupBy(t => t.LeaveType!.Name)
                .Select(g => new LeaveBalanceSummary
                {
                    LeaveTypeName = g.Key,
                    Allocated = g.Where(t => t.Debit > 0 && t.TransactionType != LeaveTransactionType.CarryForward).Sum(t => t.Debit),
                    Used = g.Where(t => t.TransactionType == LeaveTransactionType.Availed).Sum(t => t.Credit)
                })
                .Where(b => b.Allocated > 0)
                .OrderBy(b => b.LeaveTypeName)
                .ToList();

            return View(vm);
        }

        public IActionResult ClearHRView()
        {
            HttpContext.Session.Remove("HR_View_EmployeeId");
            HttpContext.Session.Remove("HR_View_Year");
            return RedirectToAction(nameof(Index));
        }

        // New action to ensure we always see OUR OWN dashboard when clicking the sidebar
        public IActionResult MyDashboard()
        {
            HttpContext.Session.Remove("HR_View_EmployeeId");
            HttpContext.Session.Remove("HR_View_Year");
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> SearchDepartmentEmployees(string term)
        {
            var session = PermissionHelper.GetUserSession(HttpContext);
            if (session == null || session.EmployeeId == null || string.IsNullOrWhiteSpace(term))
            {
                return PartialView("_EmployeeSearchResults", new List<Employee>());
            }

            var currentEmployee = await _context.emp.FindAsync(session.EmployeeId);
            if (currentEmployee == null) return PartialView("_EmployeeSearchResults", new List<Employee>());

            var results = await _context.emp
                .Where(e => e.DepartmentFk == currentEmployee.DepartmentFk && 
                           e.Id != currentEmployee.Id && 
                           !e.IsServiceEnded &&
                           (e.FirstName.Contains(term) || e.LastName.Contains(term) || e.Code.Contains(term)))
                .Take(10)
                .ToListAsync();

            return PartialView("_EmployeeSearchResults", results);
        }
    }

    public class LeaveDashboardViewModel
    {
        public string? EmployeeName { get; set; }
        public string? EmployeeCode { get; set; }
        public int CurrentYear { get; set; }
        public int ActivePoliciesCount { get; set; }
        public int TotalRequests { get; set; }
        public int PendingRequests { get; set; }
        public int ApprovedRequests { get; set; }
        public int RejectedRequests { get; set; }
        public int CancelledRequests { get; set; }
        public List<LeaveTransaction> Ledger { get; set; } = new List<LeaveTransaction>();
        public List<LeaveRequest> RecentRequests { get; set; } = new List<LeaveRequest>();
        public List<LeaveBalanceSummary> LeaveBalances { get; set; } = new List<LeaveBalanceSummary>();
    }

    public class LeaveBalanceSummary
    {
        public string LeaveTypeName { get; set; } = "";
        public decimal Allocated { get; set; }
        public decimal Used { get; set; }
        public decimal Remaining => Math.Max(0, Allocated - Used);
        public decimal UsedPercent => Allocated > 0 ? Math.Min(100, Math.Round(Used / Allocated * 100, 0)) : 0;
    }
}

