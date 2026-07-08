using System.Text.Json;
using AlignHR.Data;
using AlignHR.Models;
using AlignHR.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AlignHR.Controllers
{
    public class ExecutionController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public ExecutionController(ApplicationDbContext context)
        {
            _context = context;
        }

        [RequireUrlPermission]
        public IActionResult Index()
        {
            ViewBag.ActivePage = "Execution";
            ViewBag.Periods = _context.SalaryPeriod.OrderByDescending(s => s.SalaryPeriodID).ToList();
            return View();
        }

        [HttpGet]
        [SkipPermissionCheck]
        public async Task<IActionResult> GetExecutionStatus(int periodId)
        {
            if (periodId <= 0) return Json(new { success = false });

            var salaryPeriod = await _context.SalaryPeriod.FindAsync(periodId);
            if (salaryPeriod == null) return Json(new { success = false });

            var execution = await _context.Executions.FirstOrDefaultAsync(e => e.Period == salaryPeriod.PeriodName);

            // Use explicit object — serialized with PascalCase to match JS data-action attribute names
            var result = new
            {
                success = true,
                GenerateIncomeTax  = execution?.IncomeTaxExecuted      ?? false,
                GenerateEOBI       = execution?.EOBIExecuted            ?? false,
                GeneratePF         = execution?.PFExecuted              ?? false,
                GenerateLoanDeduction = execution?.LoanDeductionExecuted ?? false,
                FetchBonus         = execution?.BonusFetched            ?? false,
                FetchSalaryAdjustment = execution?.SalaryAdjustmentFetched ?? false,
                GenerateSalarySlips = execution?.IsExecuted             ?? false
            };

            var json = JsonSerializer.Serialize(result, new JsonSerializerOptions
            {
                PropertyNamingPolicy = null  // preserve PascalCase
            });

            return Content(json, "application/json");
        }

        private async Task UpdateExecutionStep(int periodId, string stepName)
        {
            var salaryPeriod = await _context.SalaryPeriod.FindAsync(periodId);
            if (salaryPeriod == null) return;

            var execution = await _context.Executions.FirstOrDefaultAsync(e => e.Period == salaryPeriod.PeriodName);
            if (execution == null)
            {
                execution = new Execution
                {
                    Period = salaryPeriod.PeriodName,
                    IsExecuted = false
                };
                _context.Executions.Add(execution);
            }

            switch (stepName)
            {
                case "IncomeTax": execution.IncomeTaxExecuted = true; break;
                case "EOBI": execution.EOBIExecuted = true; break;
                case "Loan": execution.LoanDeductionExecuted = true; break;
                case "PF": execution.PFExecuted = true; break;
                case "Bonus": execution.BonusFetched = true; break;
                case "Adjustment": execution.SalaryAdjustmentFetched = true; break;
            }

            await _context.SaveChangesAsync();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SkipPermissionCheck] // Or use RequireUrlPermission if specifically needed
        public async Task<IActionResult> GenerateIncomeTax(int periodId)
        {
            if (periodId <= 0) return Json(new { success = false, message = "Invalid period selected." });

            try
            {
                // 1. Fetch Rules & Data once (Same logic as IncomeTaxDetuctionController.BulkAdd)
                var salaryPeriod = await _context.SalaryPeriod.FindAsync(periodId);
                if (salaryPeriod == null) return Json(new { success = false, message = "Salary period not found." });

                if (salaryPeriod.IsPostedToGL)
                    return Json(new { success = false, message = "Action Denied: This Salary Period is already Posted to GL and cannot be modified." });

                var periodStart = DateOnly.FromDateTime(salaryPeriod.StartDate);
                var fiscalYear = await _context.FiscalYear
                    .FirstOrDefaultAsync(fy => fy.startdate <= periodStart && fy.enddate >= periodStart);

                if (fiscalYear == null)
                    return Json(new { success = false, message = "No Fiscal Year found for the selected period." });

                var slabs = await _context.TaxSlabMaster.Where(t => t.FiscalFK == fiscalYear.id && t.IsActive).ToListAsync();
                var surcharges = await _context.TaxSurcharge.Where(t => t.FiscalFK == fiscalYear.id && t.IsActive).ToListAsync();

                var employeesWithSalaries = await _context.emp
                    .Where(e => e.SalaryStatus == "Active")
                    .Select(e => new {
                        Id = e.Id,
                        LatestSalary = _context.PayRollGenrate
                            .Where(p => p.EmployeeFK == e.Id)
                            .OrderByDescending(p => p.id)
                            .Select(p => (decimal?)p.salery)
                            .FirstOrDefault() ?? 0
                    })
                    .ToListAsync();

                // 2. Delete existing records for the period before generating new ones
                var existingRecords = await _context.IncomeTaxDetuction
                    .Where(i => i.PeriodSalery == periodId)
                    .ToListAsync();

                if (existingRecords.Any())
                {
                    _context.IncomeTaxDetuction.RemoveRange(existingRecords);
                    await _context.SaveChangesAsync();
                }

                var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;
                var recordsToSave = new List<IncomeTaxDetuction>();

                // 3. Process
                foreach (var empData in employeesWithSalaries)
                {
                    if (empData.LatestSalary > 0)
                    {
                        var taxRes = CalculateTaxInternal(empData.LatestSalary, fiscalYear.title, slabs, surcharges);
                        if (taxRes.MonthlyTax > 0)
                        {
                            recordsToSave.Add(new IncomeTaxDetuction
                            {
                                EmployeeId = empData.Id,
                                PeriodSalery = periodId,
                                incometaxdetection = taxRes.MonthlyTax,
                                createat = DateTime.Now,
                                createdby = currentUserId
                            });
                        }
                    }
                }

                if (recordsToSave.Any())
                {
                    _context.IncomeTaxDetuction.AddRange(recordsToSave);
                    await _context.SaveChangesAsync();
                    await UpdateExecutionStep(periodId, "IncomeTax");
                    return Json(new { success = true, message = $"Successfully deleted {existingRecords.Count} old records and generated {recordsToSave.Count} new Income Tax records." });
                }
                else
                {
                    await UpdateExecutionStep(periodId, "IncomeTax");
                    return Json(new { success = true, message = $"Successfully deleted {existingRecords.Count} old records. No new records generated." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SkipPermissionCheck]
        public async Task<IActionResult> GenerateEOBI(int periodId)
        {
            if (periodId <= 0) return Json(new { success = false, message = "Invalid period selected." });

            try
            {
                var salaryPeriod = await _context.SalaryPeriod.FindAsync(periodId);
                if (salaryPeriod == null) return Json(new { success = false, message = "Salary period not found." });

                if (salaryPeriod.IsPostedToGL)
                    return Json(new { success = false, message = "Action Denied: This Salary Period is already Posted to GL and cannot be modified." });
                // Same logic as EOBIDetuctionController.BulkAdd
                var employees = await _context.emp
                    .Where(e => e.SalaryStatus == "Active" && e.isEobi)
                    .Select(e => e.Id)
                    .ToListAsync();

                var existingRecords = await _context.EOBIDetuction
                    .Where(i => i.PeriodSalery == periodId)
                    .ToListAsync();
                    
                if (existingRecords.Any())
                {
                    _context.EOBIDetuction.RemoveRange(existingRecords);
                    await _context.SaveChangesAsync();
                }

                var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;
                var recordsToSave = new List<EOBIDetuction>();
                decimal? periodEOBIAmount = salaryPeriod.EOBIAmount;

                foreach (var empId in employees)
                {
                    recordsToSave.Add(new EOBIDetuction
                    {
                        EmployeeId = empId,
                        PeriodSalery = periodId,
                        Amount = periodEOBIAmount,
                        createat = DateTime.Now,
                        createdby = currentUserId
                    });
                }

                if (recordsToSave.Any())
                {
                    _context.EOBIDetuction.AddRange(recordsToSave);
                    await _context.SaveChangesAsync();
                    await UpdateExecutionStep(periodId, "EOBI");
                    return Json(new { success = true, message = $"Successfully deleted {existingRecords.Count} old records and generated {recordsToSave.Count} new EOBI records." });
                }
                else
                {
                    await UpdateExecutionStep(periodId, "EOBI");
                    return Json(new { success = true, message = $"Successfully deleted {existingRecords.Count} old records. No new records generated." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SkipPermissionCheck]
        public async Task<IActionResult> GeneratePF(int periodId)
        {
            if (periodId <= 0) return Json(new { success = false, message = "Invalid period selected." });

            try
            {
                var salaryPeriod = await _context.SalaryPeriod.FindAsync(periodId);
                if (salaryPeriod == null) return Json(new { success = false, message = "Salary period not found." });

                if (salaryPeriod.IsPostedToGL)
                    return Json(new { success = false, message = "Action Denied: This Salary Period is already Posted to GL and cannot be modified." });
                // Fetch all active employees who are marked as PF members
                var employees = await _context.emp
                    .Where(e => e.SalaryStatus == "Active" && e.isPfMember)
                    .Select(e => e.Id)
                    .ToListAsync();

                // Find existing PF records for this period
                var existingRecords = await _context.PFMembers
                    .Where(i => i.SalaryPeriodFK == periodId)
                    .ToListAsync();

                if (existingRecords.Any())
                {
                    _context.PFMembers.RemoveRange(existingRecords);
                    await _context.SaveChangesAsync();
                }

                var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;
                var recordsToSave = new List<PFMembers>();

                foreach (var empId in employees)
                {
                    recordsToSave.Add(new PFMembers
                    {
                        EmployeeId = empId,
                        SalaryPeriodFK = periodId,
                        createat = DateTime.Now,
                        createdby = currentUserId
                    });
                }

                if (recordsToSave.Any())
                {
                    _context.PFMembers.AddRange(recordsToSave);
                    await _context.SaveChangesAsync();
                    await UpdateExecutionStep(periodId, "PF");
                    return Json(new { success = true, message = $"Successfully deleted {existingRecords.Count} old records and generated {recordsToSave.Count} new PF records." });
                }
                else
                {
                    await UpdateExecutionStep(periodId, "PF");
                    return Json(new { success = true, message = $"Successfully deleted {existingRecords.Count} old records. No new PF records generated (make sure employees have 'Is PF Member' checked)." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SkipPermissionCheck]
        public async Task<IActionResult> GenerateLoanDeduction(int periodId)
        {
            if (periodId <= 0) return Json(new { success = false, message = "Invalid period selected." });

            try
            {
                var salaryPeriod = await _context.SalaryPeriod.FindAsync(periodId);
                if (salaryPeriod == null) return Json(new { success = false, message = "Salary period not found." });

                if (salaryPeriod.IsPostedToGL)
                    return Json(new { success = false, message = "Action Denied: This Salary Period is already Posted to GL and cannot be modified." });

                // Map period to "yyyy/MM" format used in MasterLoanAdvanceDetail
                string deductionMonth = salaryPeriod.StartDate.ToString("yyyy/MM");
                var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;

                // Fetch approved loans for active employees that haven't been excluded/stopped
                var activeLoans = await _context.MasterLoanAdvance
                    .Include(m => m.Employee)
                    .Include(m => m.Details)
                    .Where(m => m.Status == LoanStatus.Approved 
                             && m.TransactionType == TransactionType.Loan 
                             && !m.LoanDetuction 
                             && m.Employee!.SalaryStatus == "Active")
                    .ToListAsync();
                    
                // Find and delete previous records for the current deduction month
                var existingDetailsForMonth = activeLoans
                    .SelectMany(l => l.Details)
                    .Where(d => d.DeductionMonth == deductionMonth)
                    .ToList();
                    
                if (existingDetailsForMonth.Any())
                {
                    _context.MasterLoanAdvanceDetail.RemoveRange(existingDetailsForMonth);
                    await _context.SaveChangesAsync();
                    
                    // Reload the activeLoans so the removed details aren't in memory
                    activeLoans = await _context.MasterLoanAdvance
                        .Include(m => m.Employee)
                        .Include(m => m.Details)
                        .Where(m => m.Status == LoanStatus.Approved 
                                 && m.TransactionType == TransactionType.Loan 
                                 && !m.LoanDetuction 
                                 && m.Employee!.SalaryStatus == "Active")
                        .ToListAsync();
                }

                int createdCount = 0;

                foreach (var loan in activeLoans)
                {
                    // --- Flat Interest & Global Balance Logic ---
                    decimal principal = loan.ApprovedAmount;
                    decimal flatInterestTotal = principal * (loan.InterestRate / 100);
                    decimal initialTotalDebt = principal + flatInterestTotal;
                    decimal totalDeductedSoFar = loan.Details.Sum(d => d.DeductedAmount);
                    decimal netBalanceBeforeThis = initialTotalDebt - totalDeductedSoFar;
                    
                    int installmentNo = loan.Details.Count + 1;
                    decimal interestPerMonth = loan.TenureMonths > 0 ? (flatInterestTotal / loan.TenureMonths) : 0;

                    var detail = new MasterLoanAdvanceDetail
                    {
                        MasterId = loan.id,
                        InstallmentNo = installmentNo,
                        DeductionMonth = deductionMonth,
                        TotalDeductionAmount = loan.MonthlyInstallment,
                        DeductedAmount = loan.MonthlyInstallment,
                        InterestAmount = Math.Round(interestPerMonth, 2),
                        RemainingAmount = netBalanceBeforeThis - loan.MonthlyInstallment,
                        Status = LoanInstallmentStatus.Deducted,
                        createdby = currentUserId,
                        createat = DateTime.Now
                    };

                    _context.MasterLoanAdvanceDetail.Add(detail);
                    createdCount++;
                }

                if (createdCount > 0)
                {
                    await _context.SaveChangesAsync();
                    await UpdateExecutionStep(periodId, "Loan");
                    return Json(new { success = true, message = $"Successfully deleted {existingDetailsForMonth.Count} old records and generated {createdCount} new loan installments for {deductionMonth}." });
                }
                else
                {
                    await UpdateExecutionStep(periodId, "Loan");
                    return Json(new { success = true, message = $"Successfully deleted {existingDetailsForMonth.Count} old records. No new loan installments were pending for {deductionMonth}." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SkipPermissionCheck]
        public async Task<IActionResult> FetchBonus(int periodId)
        {
            if (periodId <= 0) return Json(new { success = false, message = "Invalid period selected." });
            
            var count = await _context.Bonuses.CountAsync(b => b.SalaryPeriodID == periodId);
            await UpdateExecutionStep(periodId, "Bonus");
            return Json(new { success = true, message = $"Found {count} existing Bonus records for this period." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SkipPermissionCheck]
        public async Task<IActionResult> FetchSalaryAdjustment(int periodId)
        {
            if (periodId <= 0) return Json(new { success = false, message = "Invalid period selected." });
            
            var count = await _context.SalaryAdjustments.CountAsync(a => a.SalaryPeriodId == periodId);
            await UpdateExecutionStep(periodId, "Adjustment");
            return Json(new { success = true, message = $"Found {count} existing Salary Adjustment records for this period." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SkipPermissionCheck]
        public async Task<IActionResult> GenerateSalarySlips(int periodId)
        {
            if (periodId <= 0) return Json(new { success = false, message = "Invalid period selected." });

            try
            {
                var salaryPeriod = await _context.SalaryPeriod.FindAsync(periodId);
                if (salaryPeriod == null) return Json(new { success = false, message = "Salary period not found." });

                if (salaryPeriod.IsPostedToGL)
                    return Json(new { success = false, message = "Action Denied: This Salary Period is already Posted to GL and cannot be modified." });

                var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;
                var now = DateTime.Now;

                // 1. Calculate Period Totals
                DateTime start = salaryPeriod.StartDate;
                DateTime end = salaryPeriod.EndDate;
                int totalDaysInPeriod = (end - start).Days + 1;
                DateOnly startDO = DateOnly.FromDateTime(start);
                DateOnly endDO = DateOnly.FromDateTime(end);

                // 2. Bulk Fetch Data
                var employees = await _context.emp
                    .Include(e => e.Department)
                    .Include(e => e.Location)
                    .Include(e => e.Designation)
                    .Include(e => e.BankName)
                    .Where(e => e.SalaryStatus == "Active")
                    .ToListAsync();

                var empIds = employees.Select(e => e.Id).ToList();

                // Payroll Definitions
                var latestPayrolls = await _context.PayRollGenrate
                    .Include(p => p.PayRollDefinationFile)
                    .Where(p => empIds.Contains(p.EmployeeFK))
                    .OrderByDescending(p => p.id)
                    .ToListAsync();
                var payrollMap = latestPayrolls.GroupBy(p => p.EmployeeFK).ToDictionary(g => g.Key, g => g.First());

                var allDefinitions = await _context.PayRollDefinationFile
                    .Include(d => d.PayRollCom)
                    .ToListAsync();
                var defMap = allDefinitions.GroupBy(d => d.DefinitionName).ToDictionary(g => g.Key, g => g.ToList());

                // Attendance Logs
                var attendanceLogs = await _context.AttendenceLogs
                    .Where(l => l.AttendenceDate >= startDO && l.AttendenceDate <= endDO)
                    .ToListAsync();
                var attendanceMap = attendanceLogs.GroupBy(l => l.EmployeeID)
                    .ToDictionary(g => g.Key, g => g.Select(l => l.AttendenceDate).Distinct().Count());

                // Employee Shifts (for rest days)
                var shifts = await _context.EmployeeShifts
                    .Where(s => empIds.Contains(s.EmployeeId))
                    .ToListAsync();
                var shiftMap = shifts.GroupBy(s => s.EmployeeId).ToDictionary(g => g.Key, g => g.OrderByDescending(s => s.id).FirstOrDefault());

                // Deductions (using GroupBy to prevent duplicate key errors)
                var taxMap = await _context.IncomeTaxDetuction.Where(t => t.PeriodSalery == periodId)
                    .GroupBy(t => t.EmployeeId).ToDictionaryAsync(g => g.Key, g => g.First());

                var eobiMap = await _context.EOBIDetuction.Where(t => t.PeriodSalery == periodId)
                    .GroupBy(t => t.EmployeeId).ToDictionaryAsync(g => g.Key, g => g.First());

                var pfMap = await _context.PFMembers.Where(t => t.SalaryPeriodFK == periodId)
                    .GroupBy(t => t.EmployeeId).ToDictionaryAsync(g => g.Key, g => g.First());
                
                string loanMonth = start.ToString("yyyy/MM");
                var loanInstallments = await _context.MasterLoanAdvanceDetail
                    .Include(d => d.MasterLoanAdvance)
                    .Where(d => d.DeductionMonth == loanMonth)
                    .GroupBy(d => d.MasterLoanAdvance.EmployeeID)
                    .ToDictionaryAsync(g => g.Key, g => g.Sum(d => d.DeductedAmount));

                // Bonuses
                var bonusMap = await _context.Bonuses
                    .Where(b => b.SalaryPeriodID == periodId && b.ApprovalStatus == "Approved")
                    .GroupBy(b => b.EmployeeID)
                    .ToDictionaryAsync(g => g.Key, g => g.Sum(b => b.NetBonus));

                // Salary Adjustments & Arrears
                var adjustments = await _context.SalaryAdjustments
                    .Where(a => a.SalaryPeriodId == periodId && a.IsApproved)
                    .ToListAsync();
                var adjustmentMap = adjustments.GroupBy(a => a.EmployeeId).ToDictionary(g => g.Key, g => g.ToList());

                // Previous Carry Forwards
                var lastPeriod = await _context.SalaryPeriod.Where(p => p.StartDate < start).OrderByDescending(p => p.StartDate).FirstOrDefaultAsync();
                var carryMap = new Dictionary<int, decimal>();
                if (lastPeriod != null)
                {
                    carryMap = await _context.SalarySlipMasters
                        .Where(s => s.SalaryPeriodID == lastPeriod.SalaryPeriodID)
                        .ToDictionaryAsync(s => s.EmployeeID, s => s.NewCarryForward);
                }

                // Cleanup existing drafts
                var existingDrafts = await _context.SalarySlipMasters.Include(m => m.Details).Where(m => m.SalaryPeriodID == periodId && m.Status == "Draft").ToListAsync();
                if (existingDrafts.Any())
                {
                    _context.SalarySlipDetails.RemoveRange(existingDrafts.SelectMany(s => s.Details));
                    _context.SalarySlipMasters.RemoveRange(existingDrafts);
                    await _context.SaveChangesAsync();
                }

                // 3. Process Slips
                int createdCount = 0;
                foreach (var emp in employees)
                {
                    if (!payrollMap.TryGetValue(emp.Id, out var payroll) || payroll.PayRollDefinationFile == null) continue;
                    if (!defMap.TryGetValue(payroll.PayRollDefinationFile.DefinitionName, out var components)) continue;

                    // A. Calculate Days
                    int presentDays = attendanceMap.TryGetValue(emp.Id, out var pDays) ? pDays : 0;
                    int restDaysCount = 0;
                    if (shiftMap.TryGetValue(emp.Id, out var shift))
                    {
                        for (var d = start; d <= end; d = d.AddDays(1))
                        {
                            if (d.DayOfWeek == shift.RestDay) restDaysCount++;
                        }
                    }
                    else
                    {
                        // Default to Sunday if no shift found
                        for (var d = start; d <= end; d = d.AddDays(1))
                        {
                            if (d.DayOfWeek == DayOfWeek.Sunday) restDaysCount++;
                        }
                    }

                    int totalPaidDays = presentDays + restDaysCount;
                    if (totalPaidDays > totalDaysInPeriod) totalPaidDays = totalDaysInPeriod;
                    decimal unpaidDays = totalDaysInPeriod - totalPaidDays;

                    // B. Financials
                    decimal grossMonthly = payroll.salery;
                    decimal dailyRate = totalDaysInPeriod > 0 ? (grossMonthly / totalDaysInPeriod) : 0;
                    
                    // Earned Salary based on attendance
                    decimal earnedSalary = Math.Round(dailyRate * totalPaidDays, 2);

                    var master = new SalarySlipMaster
                    {
                        EmployeeID = emp.Id,
                        SalaryPeriodID = periodId,
                        EmployeeNameSnapshot = $"{emp.FirstName} {emp.LastName}".Trim(),
                        DepartmentSnapshot = emp.Department?.Name ?? "N/A",
                        DesignationSnapshot = emp.Designation?.Name ?? "N/A",
                        LocationSnapshot = emp.Location?.Name ?? "N/A",
                        BankNameSnapshot = emp.BankName?.Name ?? "N/A",
                        AccountNumberSnapshot = emp.AccountNumber ?? "N/A",
                        TotalDaysInMonth = totalDaysInPeriod,
                        GrossSalaryFK = payroll.id,
                        GrossSalarySnapshot = grossMonthly,
                        UnpaidDaysSnapshot = unpaidDays,
                        NetPaidDays = totalPaidDays,
                        AdjustedNetSalary = earnedSalary, // Storing Earned Salary
                        Status = "Draft",
                        Remarks = "",
                        createdby = currentUserId,
                        createat = now
                    };

                    decimal totalEarnings = 0;
                    decimal totalDeductions = 0;

                    // Add Earnings based on EARNED Salary (LATEST USER REQUEST)
                    foreach (var comp in components)
                    {
                        decimal pct = decimal.TryParse(comp.Percentage, out decimal v) ? v : 0;
                        decimal amt = Math.Round(earnedSalary * (pct / 100), 2); // Base is now Earned Salary
                        totalEarnings += amt;

                        master.Details.Add(new SalarySlipDetail
                        {
                            ComponentName = comp.label ?? comp.PayRollCom?.Name ?? "Allowance",
                            Amount = amt,
                            Type = PayrollComponentType.Earning,
                            PayRollDefinationFK = comp.Id
                        });
                    }

                    // No longer adding a separate "Absence Deduction" because components are pro-rated

                    // Add Bonus
                    if (bonusMap.TryGetValue(emp.Id, out var bonusAmt))
                    {
                        master.BonusAmount = bonusAmt;
                        totalEarnings += bonusAmt;
                        master.Details.Add(new SalarySlipDetail { ComponentName = "Bonus", Amount = bonusAmt, Type = PayrollComponentType.Earning });
                    }

                    // Add Deductions
                    if (taxMap.TryGetValue(emp.Id, out var tax))
                    {
                        totalDeductions += tax.incometaxdetection;
                        master.Details.Add(new SalarySlipDetail { ComponentName = "Income Tax", Amount = tax.incometaxdetection, Type = PayrollComponentType.Deduction });
                    }

                    if (eobiMap.TryGetValue(emp.Id, out var eobi))
                    {
                        // Prioritize Individual Amount, fallback to Period Default
                        decimal eobiAmt = eobi.Amount ?? salaryPeriod.EOBIAmount ?? 0;
                        totalDeductions += eobiAmt;
                        master.Details.Add(new SalarySlipDetail { ComponentName = "EOBI", Amount = eobiAmt, Type = PayrollComponentType.Deduction });
                    }

                    if (pfMap.TryGetValue(emp.Id, out var pf))
                    {
                        decimal pfAmt = emp.IsFixedPFAmount ? emp.PFAmount.GetValueOrDefault() : (earnedSalary * (salaryPeriod.PFPercentage / 100));
                        totalDeductions += pfAmt;
                        master.Details.Add(new SalarySlipDetail { ComponentName = "Provident Fund", Amount = pfAmt, Type = PayrollComponentType.Deduction });
                    }

                    if (loanInstallments.TryGetValue(emp.Id, out var loanAmt))
                    {
                        totalDeductions += loanAmt;
                        master.Details.Add(new SalarySlipDetail { ComponentName = "Loan Installment", Amount = loanAmt, Type = PayrollComponentType.Deduction });
                    }

                    // Add Adjustments & Arrears
                    if (adjustmentMap.TryGetValue(emp.Id, out var empsAdjustments))
                    {
                        foreach (var adj in empsAdjustments)
                        {
                            if (!string.IsNullOrEmpty(adj.AdjustmentCategory) && adj.AdjustmentCategory.ToLower().Contains("arrear"))
                            {
                                master.ArrearAmount += adj.Amount;
                                totalEarnings += adj.Amount;
                                master.Details.Add(new SalarySlipDetail { ComponentName = adj.AdjustmentCategory, Amount = adj.Amount, Type = PayrollComponentType.Earning });
                            }
                            else if (adj.Type == PayrollComponentTypes.Earning)
                            {
                                totalEarnings += adj.Amount;
                                master.Details.Add(new SalarySlipDetail { ComponentName = adj.AdjustmentCategory ?? "Adjustment (Earning)", Amount = adj.Amount, Type = PayrollComponentType.Earning });
                            }
                            else if (adj.Type == PayrollComponentTypes.Deduction)
                            {
                                totalDeductions += adj.Amount;
                                master.Details.Add(new SalarySlipDetail { ComponentName = adj.AdjustmentCategory ?? "Adjustment (Deduction)", Amount = adj.Amount, Type = PayrollComponentType.Deduction });
                            }
                        }
                    }

                    master.TotalAllowances = totalEarnings;
                    master.TotalDeductions = totalDeductions;
                    master.NetSalary = totalEarnings - totalDeductions;

                    // 1. adjusted_salary = net_salary + previous_carry_forward
                    if (carryMap.TryGetValue(emp.Id, out var cf)) master.PreviousCarryForward = cf;
                    master.AdjustedNetSalary = master.NetSalary + master.PreviousCarryForward;

                    // 2. round_salary = floor(adjusted_salary / 100) * 100
                    master.PayableAmount = Math.Floor(master.AdjustedNetSalary / 100) * 100;

                    // 3. new_carry_forward = adjusted_salary - round_salary
                    master.NewCarryForward = master.AdjustedNetSalary - master.PayableAmount;

                    _context.SalarySlipMasters.Add(master);
                    createdCount++;
                }

                // Auto-Post to GL and Lock Period
                salaryPeriod.IsPostedToGL = true;
                _context.SalaryPeriod.Update(salaryPeriod);

                // Log execution event details
                var existingExecution = await _context.Executions.FirstOrDefaultAsync(e => e.Period == salaryPeriod.PeriodName);
                if (existingExecution != null)
                {
                    existingExecution.IsExecuted = true;
                    existingExecution.ExecutedAt = now;
                    existingExecution.ExecutedBy = currentUserId;
                    _context.Executions.Update(existingExecution);
                }
                else
                {
                    var executionLog = new Execution
                    {
                        Period = salaryPeriod.PeriodName,
                        IsExecuted = true,
                        ExecutedAt = now,
                        ExecutedBy = currentUserId
                    };
                    _context.Executions.Add(executionLog);
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = $"Successfully generated {createdCount} salary slips. This period is now LOCKED and Posted to GL." });
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null) msg += " | Inner: " + ex.InnerException.Message;
                return Json(new { success = false, message = "Execution Error: " + msg });
            }
        }

        // Helper method copied from IncomeTaxDetuctionController
        private TaxCalculationResult CalculateTaxInternal(decimal monthlyGross, string fiscalYearName, List<TaxSlabMaster> slabs, List<TaxSurcharge> surcharges)
        {
            var res = new TaxCalculationResult
            {
                MonthlyGross = monthlyGross,
                FiscalYearName = fiscalYearName,
                AnnualGross = monthlyGross * 12
            };

            decimal annualGross = res.AnnualGross;

            res.TaxSlab = slabs
                .Where(t => t.IncomeFrom <= annualGross && (t.IncomeTo == null || t.IncomeTo >= annualGross))
                .FirstOrDefault();

            res.TaxSurcharge = surcharges
                .Where(t => t.IncomeFrom <= annualGross && (t.IncomeTo == null || t.IncomeTo >= annualGross))
                .FirstOrDefault();

            if (res.TaxSlab != null)
            {
                res.FixedTax = res.TaxSlab.FixedTax;
                res.TaxRate = res.TaxSlab.RatePercent;
                res.TaxOnExceeding = (annualGross - res.TaxSlab.IncomeFrom) * (res.TaxRate / 100);
                res.AnnualTax += res.FixedTax + res.TaxOnExceeding;
            }

            if (res.TaxSurcharge != null)
            {
                res.SurchargeAmount = (annualGross - res.TaxSurcharge.IncomeFrom) * (res.TaxSurcharge.RatePercent / 100);
                res.AnnualTax += res.SurchargeAmount;
            }

            res.MonthlyTax = res.AnnualTax / 12;
            return res;
        }

        private class TaxCalculationResult
        {
            public decimal MonthlyGross { get; set; }
            public decimal AnnualGross { get; set; }
            public decimal AnnualTax { get; set; }
            public decimal MonthlyTax { get; set; }
            public decimal FixedTax { get; set; }
            public decimal TaxRate { get; set; }
            public decimal TaxOnExceeding { get; set; }
            public decimal SurchargeAmount { get; set; }
            public string FiscalYearName { get; set; }
            public TaxSlabMaster TaxSlab { get; set; }
            public TaxSurcharge TaxSurcharge { get; set; }
            public string ErrorMessage { get; set; }
        }
    }
}

