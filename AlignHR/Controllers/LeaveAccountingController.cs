using Microsoft.AspNetCore.Mvc;
using AlignHR.Services;
using AlignHR.Data;
using Microsoft.EntityFrameworkCore;
using AlignHR.Models;
using AlignHR.Models.Enums;

namespace AlignHR.Controllers
{
    public class LeaveAccountingController : BaseController
    {
        private readonly ILeaveAccountingService _leaveAccountingService;
        private readonly ApplicationDbContext _context;

        public LeaveAccountingController(ILeaveAccountingService leaveAccountingService, ApplicationDbContext context)
        {
            _leaveAccountingService = leaveAccountingService;
            _context = context;
        }

        public IActionResult Adjust()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Compensate()
        {
            ViewBag.LeaveTypes = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                _context.LeaveTypes
                    .Where(t => t.IsActive)
                    .OrderBy(t => t.Name)
                    .ToList(),
                "Id", "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessCompensation(string empCode, decimal leaveTypeId, decimal leaveDays, string narration, int year)
        {
            // ── Validate inputs ───────────────────────────────────────
            if (string.IsNullOrWhiteSpace(empCode))
            {
                TempData["Error"] = "Employee Code is required.";
                return RedirectToAction(nameof(Compensate));
            }

            if (string.IsNullOrWhiteSpace(narration))
            {
                TempData["Error"] = "Reason / Remarks is required.";
                return RedirectToAction(nameof(Compensate));
            }

            if (leaveDays <= 0)
            {
                TempData["Error"] = "Leave Days must be greater than zero.";
                return RedirectToAction(nameof(Compensate));
            }

            // ── Employee exists and active ────────────────────────────
            var employee = await _context.emp.FirstOrDefaultAsync(e => e.Code == empCode);
            if (employee == null)
            {
                TempData["Error"] = $"Employee with code '{empCode}' not found.";
                return RedirectToAction(nameof(Compensate));
            }

            if (employee.IsServiceEnded)
            {
                TempData["Error"] = $"{employee.FullName} ({empCode}) is no longer active. Compensation cannot be assigned to an inactive employee.";
                return RedirectToAction(nameof(Compensate));
            }

            // ── Active policy assignment ──────────────────────────────
            var hasPolicy = await _context.EmployeeLeavePolicies
                .AnyAsync(p => p.EmployeeId == employee.Id && p.IsActive);

            if (!hasPolicy)
            {
                TempData["Error"] = $"No active Leave Policy found for {employee.FullName} ({empCode}). Please assign a policy first.";
                return RedirectToAction(nameof(Compensate));
            }

            // ── Leave type exists ─────────────────────────────────────
            var leaveType = await _context.LeaveTypes.FindAsync(leaveTypeId);
            if (leaveType == null || !leaveType.IsActive)
            {
                TempData["Error"] = "Selected Leave Type is invalid or inactive.";
                return RedirectToAction(nameof(Compensate));
            }

            try
            {
                int createdBy = AlignHR.Security.PermissionHelper.GetCurrentUserId(HttpContext) ?? 1;

                await _leaveAccountingService.CompensateLeavesAsync(
                    employee.Id,
                    leaveTypeId,
                    leaveDays,
                    $"[Leave Compensation] {narration}",
                    year,
                    createdBy);

                TempData["Success"] = $"Successfully credited {leaveDays:0.##} day(s) of {leaveType.Name} to {employee.FullName} ({empCode}) for {year}.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error processing compensation: {ex.Message}";
            }

            return RedirectToAction(nameof(Compensate));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessYearEnd(string actionType, int year)
        {
            if (year >= DateTime.Now.Year)
            {
                TempData["Error"] = $"Cannot process year {year}. Company-wide year-end must be run for a past year (max {DateTime.Now.Year - 1}).";
                return RedirectToAction(nameof(Adjust));
            }

            var newerLock = await _context.LeaveYearLocks
                .Where(l => l.Year >= year)
                .OrderBy(l => l.Year)
                .FirstOrDefaultAsync();
            if (newerLock != null)
            {
                var msg = newerLock.Year == year
                    ? $"Year {year} has already been processed and is locked (on {newerLock.LockedAt:dd MMM yyyy})."
                    : $"Cannot process year {year}. Year {newerLock.Year} was already processed on {newerLock.LockedAt:dd MMM yyyy} — processing an earlier year would corrupt those balances.";
                TempData["Error"] = msg;
                return RedirectToAction(nameof(Adjust));
            }

            try
            {
                int createdBy = 1; // Fallback or current user ID

                if (actionType == "Lapse")
                {
                    await _leaveAccountingService.ProcessCompanyWideLapseAsync(year, createdBy, lapseAll: true);
                    TempData["Success"] = $"Successfully lapsed leaves for all employees up to year {year}.";
                }
                else if (actionType == "Transfer")
                {
                    int toYear = year + 1;
                    await _leaveAccountingService.ProcessCompanyWideCarryForwardAsync(year, toYear, createdBy);

                    _context.ChangeTracker.Clear();
                    _context.LeaveYearLocks.Add(new LeaveYearLock
                    {
                        Year = year,
                        LockedAt = DateTime.Now,
                        LockedBy = createdBy
                    });
                    await _context.SaveChangesAsync();

                    TempData["Success"] = $"Successfully transferred (carried forward) leaves for all employees from {year} to {toYear}. Year {year} is now locked.";
                }
                else
                {
                    TempData["Error"] = "Invalid action specified.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error processing leaves: {ex.Message}";
            }

            return RedirectToAction(nameof(Adjust));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MigrateLegacyBalances()
        {
            try
            {
                int createdBy = 1; // Fallback
                var legacyBalances = await _context.LeaveBalances.ToListAsync();
                int count = 0;

                foreach (var lb in legacyBalances)
                {
                    if (lb.Available > 0)
                    {
                        try 
                        {
                            await _leaveAccountingService.GenerateYearOpeningAsync(lb.EmployeeId, lb.LeaveTypeId, lb.Year, lb.Available, createdBy);
                            count++;
                        }
                        catch (InvalidOperationException)
                        {
                            // Already exists, ignore
                        }
                    }
                }

                TempData["Success"] = $"Successfully migrated {count} legacy balances as Opening balances in the new Ledger.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Migration failed: {ex.Message}";
            }
            return RedirectToAction(nameof(Adjust));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateTestData()
        {
            try
            {
                var employeePolicy = await _context.EmployeeLeavePolicies.FirstOrDefaultAsync(p => p.IsActive);
                if (employeePolicy == null) throw new Exception("No active employee policy found.");

                var rule = await _context.LeavePolicyRules.FirstOrDefaultAsync(r => r.LeavePolicyId == employeePolicy.LeavePolicyId && r.CarryForwardLimit > 0);
                if (rule == null) throw new Exception("No leave policy rule with carry forward found.");

                int year = DateTime.Now.Year;
                
                // Add 25 leaves as an opening balance
                await _leaveAccountingService.GenerateYearOpeningAsync(employeePolicy.EmployeeId, rule.LeaveTypeId, year, 25, 1);

                TempData["Success"] = $"Successfully inserted a TEST Opening Balance of 25 leaves for Employee {employeePolicy.EmployeeId} for the year {year}. Now click Transfer or Lapse!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to generate test data: {ex.Message}";
            }
            return RedirectToAction(nameof(Adjust));
        }
        [HttpGet]
        public async Task<IActionResult> SearchEmployees(string term)
        {
            if (string.IsNullOrEmpty(term) || term.Length < 2)
            {
                return PartialView("_EmployeeSearchResults", new List<Employee>());
            }

            var employees = await _context.emp
                .Include(e => e.Department)
                .Where(e => e.FirstName.Contains(term) || 
                            e.LastName.Contains(term) || 
                            e.Code.Contains(term))
                .Take(10)
                .ToListAsync();

            return PartialView("_EmployeeSearchResults", employees);
        }

        [HttpGet]
        public IActionResult IndividualTransfer()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessIndividualTransfer(string empCode, int year)
        {
            if (year > DateTime.Now.Year)
            {
                TempData["Error"] = $"Cannot process a future year ({year}). Target year cannot exceed {DateTime.Now.Year}.";
                return RedirectToAction(nameof(IndividualTransfer));
            }

            try
            {
                if (string.IsNullOrWhiteSpace(empCode))
                {
                    TempData["Error"] = "Employee Code is required.";
                    return RedirectToAction(nameof(IndividualTransfer));
                }

                var employee = await _context.emp.FirstOrDefaultAsync(e => e.Code == empCode);
                if (employee == null)
                {
                    TempData["Error"] = $"Employee with Code '{empCode}' not found.";
                    return RedirectToAction(nameof(IndividualTransfer));
                }

                if (employee.IsServiceEnded)
                {
                    TempData["Error"] = $"{employee.FullName} ({empCode}) is no longer active. Leave allocation cannot be processed for inactive employees.";
                    return RedirectToAction(nameof(IndividualTransfer));
                }

                // Check if they have an active leave policy
                var activePolicy = await _context.EmployeeLeavePolicies
                    .Include(p => p.LeavePolicy)
                    .FirstOrDefaultAsync(p => p.EmployeeId == employee.Id && p.IsActive);

                if (activePolicy == null)
                {
                    TempData["Error"] = $"No active Leave Policy found for Employee {employee.FullName} ({empCode}).";
                    return RedirectToAction(nameof(IndividualTransfer));
                }

                // Get all rules for their policy
                var policyRules = await _context.LeavePolicyRules
                    .Where(r => r.LeavePolicyId == activePolicy.LeavePolicyId && r.IsActive)
                    .ToListAsync();

                if (!policyRules.Any())
                {
                    TempData["Error"] = $"No Leave Policy Rules found for the assigned policy.";
                    return RedirectToAction(nameof(IndividualTransfer));
                }

                // Check what type of opening already exists for the target year
                var existingSource = await _context.LeaveTransactions
                    .Where(t => t.EmployeeFk == employee.Id &&
                                t.Year == year &&
                                t.TransactionType == LeaveTransactionType.Opening)
                    .Select(t => t.TransactionSource)
                    .FirstOrDefaultAsync();

                if (existingSource == "CompanyWideTransfer")
                {
                    TempData["Error"] = $"{employee.FullName} ({empCode}) already has a company-wide leave allocation for year {year}. Individual allocation is not required.";
                    return RedirectToAction(nameof(IndividualTransfer));
                }

                if (existingSource == "IndividualTransfer" || existingSource == "YearEndProcess")
                {
                    TempData["Error"] = $"{employee.FullName} ({empCode}) already has an individual leave allocation for year {year}.";
                    return RedirectToAction(nameof(IndividualTransfer));
                }

                int createdBy = 1; // Fallback or current user ID
                int fromYear = year - 1;

                foreach (var rule in policyRules)
                {
                    decimal proratedQuota = rule.AnnualQuota;

                    string prorationInfo = "";
                    // PRORATION LOGIC
                    if (employee.Dateofjoin != default(DateOnly))
                    {
                        var doj = employee.Dateofjoin.ToDateTime(TimeOnly.MinValue);
                        
                        // If they joined in the target year, prorate their quota (Daily Proration)
                        if (doj.Year == year)
                        {
                            var endOfYear = new DateTime(year, 12, 31);
                            double daysInYear = DateTime.IsLeapYear(year) ? 366.0 : 365.0;
                            double activeDays = (endOfYear - doj).TotalDays + 1;
                            
                            decimal originalQuota = rule.AnnualQuota;
                            proratedQuota = Math.Round((decimal)(activeDays / daysInYear) * originalQuota, 2);
                            prorationInfo = $" (Prorated from {originalQuota} based on Join Date {employee.Dateofjoin:dd-MM-yyyy})";
                        }
                        // If they join NEXT year (future), quota is 0 for this year
                        else if (doj.Year > year)
                        {
                            proratedQuota = 0;
                            prorationInfo = $" (Set to 0 because Join Date {employee.Dateofjoin:dd-MM-yyyy} is in the future)";
                        }
                    }

                    // Process the carry forward / initialization for this specific employee and leave type
                    await _leaveAccountingService.CarryForwardLeavesAsync(
                        employee.Id,
                        rule.LeaveTypeId,
                        fromYear,
                        year,
                        createdBy,
                        rule.CarryForwardLimit,
                        proratedQuota,
                        !string.IsNullOrEmpty(prorationInfo) ? $"[Prorated — DOJ {employee.Dateofjoin:dd-MM-yyyy}]" : null,
                        transactionSource: "IndividualTransfer");
                }

                TempData["Success"] = $"Successfully initialized/transferred leaves for {employee.FullName} ({empCode}) for the year {year}. (DOJ: {employee.Dateofjoin:dd-MMM-yyyy})";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error processing leaves: {ex.Message}";
            }

            return RedirectToAction(nameof(IndividualTransfer));
        }
        public async Task<IActionResult> CompanySummary(int? year, int page = 1)
        {
            ViewBag.ActivePage = "CompanySummary";
            int targetYear = year ?? DateTime.Now.Year;
            ViewBag.Year = targetYear;

            int pageSize = 10;

            // Show employees who have any leave transaction for the selected year
            var empIdsWithTransactions = await _context.LeaveTransactions
                .Where(t => t.Year == targetYear)
                .Select(t => t.EmployeeFk)
                .Distinct()
                .ToListAsync();

            int totalEmployees = await _context.emp
                .CountAsync(e => empIdsWithTransactions.Contains(e.Id));
            int totalPages = (int)Math.Ceiling(totalEmployees / (double)pageSize);

            page = Math.Max(1, Math.Min(page, totalPages > 0 ? totalPages : 1));
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            var employees = await _context.emp
                .Where(e => empIdsWithTransactions.Contains(e.Id))
                .OrderBy(e => e.Code)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Fetch ALL transactions for these employees to get accurate real-time balances
            var employeeIds = employees.Select(e => e.Id).ToList();
            
            var allTransactions = await _context.LeaveTransactions
                .Include(t => t.LeaveType)
                .Where(t => employeeIds.Contains(t.EmployeeFk))
                .ToListAsync();

            var summary = employees.Select(e => {
                var empTxns = allTransactions.Where(t => t.EmployeeFk == e.Id).ToList();
                var leaveTypes = empTxns.Select(t => t.LeaveType).DistinctBy(lt => lt.Id).ToList();

                return new CompanyLeaveSummaryViewModel
                {
                    EmployeeId = e.Id,
                    EmployeeCode = e.Code,
                    EmployeeName = e.FullName,
                    Year = targetYear,
                    Details = leaveTypes.Select(lt => new LeaveTypeSummary
                    {
                        LeaveTypeName = lt.Name,
                        // Total Quota is basically the sum of Debits (Accruals/Forward/Opening) for that year
                        Opening = empTxns.Where(t => t.LeaveTypeFk == lt.Id && t.Year == targetYear && t.Debit > 0).Sum(t => t.Debit),
                        // Availed is the sum of Credits for that year
                        Availed = empTxns.Where(t => t.LeaveTypeFk == lt.Id && t.Year == targetYear && t.Credit > 0 && t.TransactionType == LeaveTransactionType.Availed).Sum(t => t.Credit)
                    }).ToList()
                };
            }).ToList();

            return View(summary);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ViewEmployeeDashboard(int employeeId, int year)
        {
            // Set session variable to "Impersonate" the employee for the dashboard
            HttpContext.Session.SetInt32("HR_View_EmployeeId", employeeId);
            HttpContext.Session.SetInt32("HR_View_Year", year);
            
            // Redirect to the regular dashboard, which will now show THIS employee's data
            return RedirectToAction("Index", "Leaves");
        }
    }
}
