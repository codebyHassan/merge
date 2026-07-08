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
using X.PagedList;

namespace AlignHR.Controllers
{
    public class EmployeeLeavePoliciesController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly ILeaveAccountingService _leaveAccountingService;

        public EmployeeLeavePoliciesController(ApplicationDbContext context, ILeaveAccountingService leaveAccountingService)
        {
            _context = context;
            _leaveAccountingService = leaveAccountingService;
        }

        // GET: EmployeeLeavePolicies
        public async Task<IActionResult> Index(int? page, string searchQuery)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            ViewData["CurrentFilter"] = searchQuery;

            var baseQuery = _context.EmployeeLeavePolicies.AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                baseQuery = baseQuery.Where(e =>
                    e.Employee!.FirstName.Contains(searchQuery) ||
                    e.Employee.LastName.Contains(searchQuery));
            }

            // Paginate on IDs only — no JOIN so count and rows always match
            var pagedIds = await baseQuery
                .OrderByDescending(e => e.AssignedAt)
                .Select(e => e.Id)
                .ToPagedListAsync(pageNumber, pageSize);

            var ids = pagedIds.ToList();
            var items = await _context.EmployeeLeavePolicies
                .Include(e => e.Employee)
                .Include(e => e.LeavePolicy)
                .Where(e => ids.Contains(e.Id))
                .OrderByDescending(e => e.AssignedAt)
                .ToListAsync();

            var pagedList = new StaticPagedList<EmployeeLeavePolicy>(items, pagedIds);
            return View(pagedList);
        }

        // GET: EmployeeLeavePolicies/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeLeavePolicy = await _context.EmployeeLeavePolicies
                .Include(e => e.Employee)
                .Include(e => e.LeavePolicy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employeeLeavePolicy == null)
            {
                return NotFound();
            }

            return View(employeeLeavePolicy);
        }

        // GET: EmployeeLeavePolicies/Create
        public IActionResult Create(int? employeeId)
        {
            ViewData["EmployeeId"] = new SelectList(_context.emp, "Id", "FullName", employeeId);
            ViewData["LeavePolicyId"] = new SelectList(_context.LeavePolicy, "Id", "PolicyName");
            var model = new EmployeeLeavePolicy { EmployeeId = employeeId ?? 0 };
            return View(model);
        }

        // POST: EmployeeLeavePolicies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EmployeeId,LeavePolicyId,AssignedAt,IsActive")] EmployeeLeavePolicy employeeLeavePolicy)
        {
            if (ModelState.IsValid)
            {
                var alreadyAssigned = await _context.EmployeeLeavePolicies
                    .AnyAsync(e => e.EmployeeId == employeeLeavePolicy.EmployeeId &&
                                   e.LeavePolicyId == employeeLeavePolicy.LeavePolicyId &&
                                   e.IsActive);

                if (alreadyAssigned)
                {
                    TempData["Error"] = "This policy is already assigned to the selected employee.";
                    ViewData["EmployeeId"] = new SelectList(_context.emp, "Id", "Id", employeeLeavePolicy.EmployeeId);
                    ViewData["LeavePolicyId"] = new SelectList(_context.LeavePolicy, "Id", "PolicyName", employeeLeavePolicy.LeavePolicyId);
                    return View(employeeLeavePolicy);
                }

                _context.Add(employeeLeavePolicy);
                await _context.SaveChangesAsync();

                // AUTOMATICALLY ALLOCATE LEAVES BASED ON POLICY RULES
                try
                {
                    var rules = await _context.LeavePolicyRules
                        .Where(r => r.LeavePolicyId == employeeLeavePolicy.LeavePolicyId && r.IsActive)
                        .ToListAsync();

                    int currentYear = DateTime.Now.Year;

                    foreach (var rule in rules)
                    {
                        if (rule.AnnualQuota > 0)
                        {
                            try
                            {
                                await _leaveAccountingService.GenerateYearOpeningAsync(
                                    employeeLeavePolicy.EmployeeId,
                                    rule.LeaveTypeId,
                                    currentYear,
                                    rule.AnnualQuota,
                                    1
                                );
                            }
                            catch (InvalidOperationException)
                            {
                                // Already allocated for this year — skip
                            }
                        }
                    }
                    TempData["Success"] = "Policy assigned and annual quotas successfully deposited into the ledger.";
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Policy assigned, but failed to auto-deposit leaves: {ex.Message}";
                }

                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeId"] = new SelectList(_context.emp, "Id", "Id", employeeLeavePolicy.EmployeeId);
            ViewData["LeavePolicyId"] = new SelectList(_context.LeavePolicy, "Id", "PolicyName", employeeLeavePolicy.LeavePolicyId);
            return View(employeeLeavePolicy);
        }

        // GET: EmployeeLeavePolicies/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null) return NotFound();

            var employeeLeavePolicy = await _context.EmployeeLeavePolicies
                .Include(e => e.Employee)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employeeLeavePolicy == null) return NotFound();

            ViewData["LeavePolicyId"] = new SelectList(_context.LeavePolicy, "Id", "PolicyName", employeeLeavePolicy.LeavePolicyId);
            return View(employeeLeavePolicy);
        }

        // POST: EmployeeLeavePolicies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Id,EmployeeId,LeavePolicyId,AssignedAt,IsActive")] EmployeeLeavePolicy employeeLeavePolicy)
        {
            if (id != employeeLeavePolicy.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Block deactivation if the employee has pending leave requests
                if (!employeeLeavePolicy.IsActive)
                {
                    var existing = await _context.EmployeeLeavePolicies
                        .AsNoTracking()
                        .FirstOrDefaultAsync(e => e.Id == id);

                    if (existing?.IsActive == true)
                    {
                        var hasPending = await _context.LeaveRequests
                            .AnyAsync(r => r.EmployeeId == employeeLeavePolicy.EmployeeId &&
                                           r.Status == "Pending");

                        if (hasPending)
                        {
                            var emp = await _context.emp.FindAsync(employeeLeavePolicy.EmployeeId);
                            ModelState.AddModelError(string.Empty,
                                $"Cannot deactivate. {emp?.FullName ?? "This employee"} has pending leave requests that must be resolved first.");

                            employeeLeavePolicy.Employee = emp;
                            ViewData["LeavePolicyId"] = new SelectList(_context.LeavePolicy, "Id", "PolicyName", employeeLeavePolicy.LeavePolicyId);
                            return View(employeeLeavePolicy);
                        }
                    }
                }

                try
                {
                    _context.Update(employeeLeavePolicy);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Policy assignment updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeLeavePolicyExists(employeeLeavePolicy.Id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["LeavePolicyId"] = new SelectList(_context.LeavePolicy, "Id", "PolicyName", employeeLeavePolicy.LeavePolicyId);
            return View(employeeLeavePolicy);
        }

        // GET: EmployeeLeavePolicies/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeLeavePolicy = await _context.EmployeeLeavePolicies
                .Include(e => e.Employee)
                .Include(e => e.LeavePolicy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employeeLeavePolicy == null)
            {
                return NotFound();
            }

            return View(employeeLeavePolicy);
        }

        // POST: EmployeeLeavePolicies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            var employeeLeavePolicy = await _context.EmployeeLeavePolicies.FindAsync(id);
            if (employeeLeavePolicy != null)
            {
                _context.EmployeeLeavePolicies.Remove(employeeLeavePolicy);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: EmployeeLeavePolicies/SearchEmployees
        [HttpGet]
        public async Task<IActionResult> SearchEmployees(string term)
        {
            if (string.IsNullOrEmpty(term) || term.Length < 2)
            {
                return PartialView("_EmployeeSearchResults", new List<Employee>());
            }

            var employees = await _context.emp
                .Where(e => e.FirstName.Contains(term) || 
                            e.LastName.Contains(term) || 
                            e.Code.Contains(term))
                .Take(10)
                .ToListAsync();

            return PartialView("_EmployeeSearchResults", employees);
        }

        private bool EmployeeLeavePolicyExists(decimal id)
        {
            return _context.EmployeeLeavePolicies.Any(e => e.Id == id);
        }
    }
}
