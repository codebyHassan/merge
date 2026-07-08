using System.Globalization;
using AlignHR.Data;
using AlignHR.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlignHR.Controllers
{
    public class PayrollController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public PayrollController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = new PayrollDashboardViewModel();

            model.TotalEmployees = await _context.emp.CountAsync();

            var activeEmployees = await _context.emp
                .Include(e => e.Department)
                .Where(e => e.SalaryStatus == "Active")
                .ToListAsync();

            model.ActiveEmployees = activeEmployees.Count;
            model.EobiMemberCount = activeEmployees.Count(e => e.isEobi);
            model.PfMemberCount = activeEmployees.Count(e => e.isPfMember);
            model.TaxFilerCount = activeEmployees.Count(e => e.IsFiler);
            model.TaxExemptCount = activeEmployees.Count(e => e.TaxExempted);

            var activeEmployeeIds = activeEmployees.Select(e => e.Id).ToList();

            var activePeriod = await _context.SalaryPeriod
                .OrderByDescending(p => p.IsActive)
                .ThenByDescending(p => p.StartDate)
                .FirstOrDefaultAsync();

            model.ActivePeriod = activePeriod;
            model.LoanMonthKey = activePeriod?.StartDate.ToString("yyyy/MM") ?? string.Empty;

            var latestPayrolls = await _context.PayRollGenrate
                .Include(p => p.PayRollDefinationFile)
                .Where(p => activeEmployeeIds.Contains(p.EmployeeFK))
                .OrderByDescending(p => p.id)
                .ToListAsync();

            var payrollByEmployee = latestPayrolls
                .GroupBy(p => p.EmployeeFK)
                .ToDictionary(g => g.Key, g => g.First());

            model.PayrollConfiguredEmployees = payrollByEmployee.Count;

            var definitionNames = payrollByEmployee.Values
                .Select(p => p.PayRollDefinationFile?.DefinitionName)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Distinct()
                .ToList();

            var definitions = definitionNames.Count == 0
                ? new List<PayRollDefinationFile>()
                : await _context.PayRollDefinationFile
                    .Where(d => d.DefinitionName != null && definitionNames.Contains(d.DefinitionName))
                    .ToListAsync();

            var definitionMap = definitions
                .GroupBy(d => d.DefinitionName ?? string.Empty)
                .ToDictionary(g => g.Key, g => g.ToList());

            var estimatedEarningsByEmployee = new Dictionary<int, decimal>();
            var estimatedEarningComponents = new Dictionary<string, PayrollComponentSummary>(StringComparer.OrdinalIgnoreCase);

            foreach (var payroll in payrollByEmployee.Values)
            {
                var componentTotal = 0m;
                var definitionName = payroll.PayRollDefinationFile?.DefinitionName ?? string.Empty;

                if (!string.IsNullOrWhiteSpace(definitionName) &&
                    definitionMap.TryGetValue(definitionName, out var components))
                {
                    foreach (var component in components)
                    {
                        var componentAmount = CalculateComponentAmount(payroll.salery, component.Percentage);
                        if (componentAmount <= 0)
                        {
                            continue;
                        }

                        componentTotal += componentAmount;
                        AddComponent(estimatedEarningComponents, component.label ?? component.PayRollCom?.Name ?? "Payroll Component", payroll.EmployeeFK, componentAmount);
                    }
                }

                if (componentTotal <= 0)
                {
                    componentTotal = payroll.salery;
                    AddComponent(estimatedEarningComponents, "Base Salary", payroll.EmployeeFK, payroll.salery);
                }

                estimatedEarningsByEmployee[payroll.EmployeeFK] = componentTotal;
                model.GrossPayroll += payroll.salery;
            }

            List<IncomeTaxDetuction> taxRecords = new();
            List<EOBIDetuction> eobiRecords = new();
            List<PFMembers> pfRecords = new();
            List<WithoutPayDays> withoutPayRecords = new();
            List<Bonus> bonuses = new();
            List<SalaryAdjustments> adjustments = new();
            List<MasterLoanAdvanceDetail> loanInstallments = new();
            List<SalarySlipMaster> salarySlips = new();
            List<SalarySlipDetail> salarySlipDetails = new();

            if (activePeriod != null)
            {
                var periodId = activePeriod.SalaryPeriodID;

                taxRecords = await _context.IncomeTaxDetuction
                    .Where(t => t.PeriodSalery == periodId)
                    .ToListAsync();

                eobiRecords = await _context.EOBIDetuction
                    .Where(e => e.PeriodSalery == periodId)
                    .ToListAsync();

                pfRecords = await _context.PFMembers
                    .Where(p => p.SalaryPeriodFK == periodId)
                    .ToListAsync();

                withoutPayRecords = await _context.WithoutPayDays
                    .Where(w => w.PeriodSalery == periodId)
                    .ToListAsync();

                bonuses = await _context.Bonuses
                    .Where(b => b.SalaryPeriodID == periodId && b.ApprovalStatus == "Approved")
                    .ToListAsync();

                adjustments = await _context.SalaryAdjustments
                    .Where(a => a.SalaryPeriodId == periodId && a.IsApproved)
                    .ToListAsync();

                if (!string.IsNullOrWhiteSpace(model.LoanMonthKey))
                {
                    loanInstallments = await _context.MasterLoanAdvanceDetail
                        .Include(l => l.MasterLoanAdvance)
                        .Where(l => l.DeductionMonth == model.LoanMonthKey)
                        .ToListAsync();
                }

                salarySlips = await _context.SalarySlipMasters
                    .Include(s => s.Employee)
                    .Where(s => s.SalaryPeriodID == periodId)
                    .ToListAsync();

                if (salarySlips.Count > 0)
                {
                    var slipIds = salarySlips.Select(s => s.Id).ToList();
                    salarySlipDetails = await _context.SalarySlipDetails
                        .Where(d => slipIds.Contains(d.SalarySlipMasterID))
                        .ToListAsync();
                }
            }

            model.HasGeneratedSalarySlips = salarySlips.Count > 0;
            model.SalarySlipCount = salarySlips.Count;
            model.EmployeesInCurrentCycle = model.HasGeneratedSalarySlips
                ? salarySlips.Select(s => s.EmployeeID).Distinct().Count()
                : model.PayrollConfiguredEmployees;

            var taxByEmployee = taxRecords
                .GroupBy(t => t.EmployeeId)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.incometaxdetection));

            var eobiByEmployee = eobiRecords
                .GroupBy(e => e.EmployeeId)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Amount ?? activePeriod?.EOBIAmount ?? 0m));

            var bonusByEmployee = bonuses
                .GroupBy(b => b.EmployeeID)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.NetBonus));

            var earningAdjustmentsByEmployee = adjustments
                .Where(a => a.Type == PayrollComponentTypes.Earning)
                .GroupBy(a => a.EmployeeId)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Amount));

            var deductionAdjustmentsByEmployee = adjustments
                .Where(a => a.Type == PayrollComponentTypes.Deduction)
                .GroupBy(a => a.EmployeeId)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Amount));

            var loanByEmployee = loanInstallments
                .Where(l => l.MasterLoanAdvance != null)
                .GroupBy(l => l.MasterLoanAdvance!.EmployeeID)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.DeductedAmount));

            var pfByEmployee = BuildPfAmounts(activeEmployees, payrollByEmployee, pfRecords, activePeriod);

            model.EmployeesWithIncomeTax = taxByEmployee.Count(kvp => kvp.Value > 0);
            model.EmployeesWithEobi = eobiByEmployee.Count(kvp => kvp.Value > 0);
            model.EmployeesWithPf = pfByEmployee.Count(kvp => kvp.Value > 0);
            model.EmployeesWithLoans = loanByEmployee.Count(kvp => kvp.Value > 0);
            model.EmployeesWithWithoutPay = withoutPayRecords.Select(w => w.EmployeeId).Distinct().Count();
            model.WithoutPayDaysTotal = withoutPayRecords.Sum(w => w.withoutpaydays);

            model.BonusTotal = bonuses.Sum(b => b.NetBonus);
            model.EarningAdjustmentsTotal = adjustments
                .Where(a => a.Type == PayrollComponentTypes.Earning)
                .Sum(a => a.Amount);
            model.DeductionAdjustmentsTotal = adjustments
                .Where(a => a.Type == PayrollComponentTypes.Deduction)
                .Sum(a => a.Amount);

            if (model.HasGeneratedSalarySlips)
            {
                model.GrossPayroll = salarySlips.Sum(s => s.GrossSalarySnapshot);
                model.TotalAllowances = salarySlips.Sum(s => s.TotalAllowances);
                model.TotalDeductions = salarySlips.Sum(s => s.TotalDeductions);
                model.NetPayable = salarySlips.Sum(s => s.PayableAmount);
                model.IncomeTaxTotal = SumDetailAmount(salarySlipDetails, "Income Tax");
                model.EobiTotal = SumDetailAmount(salarySlipDetails, "EOBI");
                model.PfTotal = SumDetailAmount(salarySlipDetails, "Provident Fund");
                model.LoanDeductionTotal = SumDetailAmount(salarySlipDetails, "Loan Installment");
                model.EarningComponents = salarySlipDetails
                    .Where(d => d.Type == PayrollComponentType.Earning)
                    .GroupBy(d => d.ComponentName)
                    .Select(g => new PayrollComponentSummary
                    {
                        Name = g.Key,
                        Employees = g.Select(x => x.SalarySlipMasterID).Distinct().Count(),
                        Amount = g.Sum(x => x.Amount)
                    })
                    .OrderByDescending(x => x.Amount)
                    .Take(6)
                    .ToList();

                model.DeductionComponents = salarySlipDetails
                    .Where(d => d.Type == PayrollComponentType.Deduction)
                    .GroupBy(d => d.ComponentName)
                    .Select(g => new PayrollComponentSummary
                    {
                        Name = g.Key,
                        Employees = g.Select(x => x.SalarySlipMasterID).Distinct().Count(),
                        Amount = g.Sum(x => x.Amount)
                    })
                    .OrderByDescending(x => x.Amount)
                    .Take(6)
                    .ToList();

                model.DepartmentSummaries = salarySlips
                    .GroupBy(s => string.IsNullOrWhiteSpace(s.DepartmentSnapshot) ? "Unassigned" : s.DepartmentSnapshot)
                    .Select(g => new PayrollDepartmentSummary
                    {
                        DepartmentName = g.Key,
                        Employees = g.Select(x => x.EmployeeID).Distinct().Count(),
                        GrossAmount = g.Sum(x => x.GrossSalarySnapshot),
                        NetAmount = g.Sum(x => x.PayableAmount),
                        DeductionAmount = g.Sum(x => x.TotalDeductions)
                    })
                    .OrderByDescending(x => x.NetAmount)
                    .Take(6)
                    .ToList();

                model.EmployeeSnapshots = salarySlips
                    .OrderByDescending(s => s.TotalDeductions)
                    .ThenByDescending(s => s.PayableAmount)
                    .Take(8)
                    .Select(s => new PayrollEmployeeSnapshot
                    {
                        EmployeeCode = s.Employee?.Code ?? string.Empty,
                        EmployeeName = s.EmployeeNameSnapshot,
                        DepartmentName = string.IsNullOrWhiteSpace(s.DepartmentSnapshot) ? "Unassigned" : s.DepartmentSnapshot,
                        GrossAmount = s.GrossSalarySnapshot,
                        EarningsAmount = s.TotalAllowances,
                        DeductionAmount = s.TotalDeductions,
                        NetAmount = s.PayableAmount,
                        IsPfMember = s.Employee?.isPfMember ?? false,
                        IsEobiMember = s.Employee?.isEobi ?? false,
                        HasIncomeTax = taxByEmployee.TryGetValue(s.EmployeeID, out var employeeTax) && employeeTax > 0
                    })
                    .ToList();
            }
            else
            {
                model.TotalAllowances = estimatedEarningsByEmployee.Values.Sum() + model.BonusTotal + model.EarningAdjustmentsTotal;
                model.IncomeTaxTotal = taxByEmployee.Values.Sum();
                model.EobiTotal = eobiByEmployee.Values.Sum();
                model.PfTotal = pfByEmployee.Values.Sum();
                model.LoanDeductionTotal = loanByEmployee.Values.Sum();
                model.TotalDeductions = model.IncomeTaxTotal + model.EobiTotal + model.PfTotal + model.LoanDeductionTotal + model.DeductionAdjustmentsTotal;
                model.NetPayable = model.TotalAllowances - model.TotalDeductions;

                model.EarningComponents = estimatedEarningComponents.Values
                    .OrderByDescending(x => x.Amount)
                    .Take(6)
                    .ToList();

                if (model.BonusTotal > 0)
                {
                    model.EarningComponents.Add(new PayrollComponentSummary
                    {
                        Name = "Bonus",
                        Employees = bonuses.Select(b => b.EmployeeID).Distinct().Count(),
                        Amount = model.BonusTotal
                    });
                }

                if (model.EarningAdjustmentsTotal > 0)
                {
                    model.EarningComponents.Add(new PayrollComponentSummary
                    {
                        Name = "Approved Earning Adjustments",
                        Employees = earningAdjustmentsByEmployee.Count,
                        Amount = model.EarningAdjustmentsTotal
                    });
                }

                model.DeductionComponents = BuildFallbackDeductionComponents(
                    model,
                    taxByEmployee,
                    eobiByEmployee,
                    pfByEmployee,
                    loanByEmployee,
                    deductionAdjustmentsByEmployee);

                model.DepartmentSummaries = activeEmployees
                    .Where(e => payrollByEmployee.ContainsKey(e.Id))
                    .GroupBy(e => e.Department?.Name ?? "Unassigned")
                    .Select(g => new PayrollDepartmentSummary
                    {
                        DepartmentName = g.Key,
                        Employees = g.Select(x => x.Id).Distinct().Count(),
                        GrossAmount = g.Sum(x => (decimal)payrollByEmployee[x.Id].salery),
                        NetAmount = g.Sum(x => EstimateEmployeeNet(x.Id, estimatedEarningsByEmployee, bonusByEmployee, earningAdjustmentsByEmployee, taxByEmployee, eobiByEmployee, pfByEmployee, loanByEmployee, deductionAdjustmentsByEmployee)),
                        DeductionAmount = g.Sum(x => EstimateEmployeeDeductions(x.Id, taxByEmployee, eobiByEmployee, pfByEmployee, loanByEmployee, deductionAdjustmentsByEmployee))
                    })
                    .OrderByDescending(x => x.NetAmount)
                    .Take(6)
                    .ToList();

                model.EmployeeSnapshots = activeEmployees
                    .Where(e => payrollByEmployee.ContainsKey(e.Id))
                    .Select(e => new PayrollEmployeeSnapshot
                    {
                        EmployeeCode = e.Code ?? string.Empty,
                        EmployeeName = $"{e.FirstName} {e.LastName}".Trim(),
                        DepartmentName = e.Department?.Name ?? "Unassigned",
                        GrossAmount = payrollByEmployee[e.Id].salery,
                        EarningsAmount = EstimateEmployeeEarnings(e.Id, estimatedEarningsByEmployee, bonusByEmployee, earningAdjustmentsByEmployee),
                        DeductionAmount = EstimateEmployeeDeductions(e.Id, taxByEmployee, eobiByEmployee, pfByEmployee, loanByEmployee, deductionAdjustmentsByEmployee),
                        NetAmount = EstimateEmployeeNet(e.Id, estimatedEarningsByEmployee, bonusByEmployee, earningAdjustmentsByEmployee, taxByEmployee, eobiByEmployee, pfByEmployee, loanByEmployee, deductionAdjustmentsByEmployee),
                        IsPfMember = e.isPfMember,
                        IsEobiMember = e.isEobi,
                        HasIncomeTax = taxByEmployee.TryGetValue(e.Id, out var employeeTax) && employeeTax > 0
                    })
                    .OrderByDescending(x => x.DeductionAmount)
                    .ThenByDescending(x => x.NetAmount)
                    .Take(8)
                    .ToList();
            }

            model.ExecutionStates = BuildExecutionStates(model, taxRecords, eobiRecords, pfRecords, loanInstallments, bonuses, adjustments, salarySlips);

            return View(model);
        }

        private static decimal CalculateComponentAmount(int salary, string? percentageValue)
        {
            if (!decimal.TryParse(percentageValue, NumberStyles.Number, CultureInfo.InvariantCulture, out var percentage) &&
                !decimal.TryParse(percentageValue, out percentage))
            {
                return 0;
            }

            return Math.Round(salary * (percentage / 100m), 2);
        }

        private static void AddComponent(Dictionary<string, PayrollComponentSummary> bucket, string name, int employeeId, decimal amount)
        {
            if (!bucket.TryGetValue(name, out var summary))
            {
                summary = new PayrollComponentSummary { Name = name };
                bucket[name] = summary;
            }

            summary.Amount += amount;
            summary.Employees += 1;
        }

        private static Dictionary<int, decimal> BuildPfAmounts(
            List<Employee> activeEmployees,
            Dictionary<int, PayRollGenrate> payrollByEmployee,
            List<PFMembers> pfRecords,
            SalaryPeriod? activePeriod)
        {
            var employeeLookup = activeEmployees.ToDictionary(e => e.Id);
            var pfEmployeeIds = pfRecords.Select(p => p.EmployeeId).Distinct().ToList();
            var amounts = new Dictionary<int, decimal>();

            foreach (var employeeId in pfEmployeeIds)
            {
                if (!employeeLookup.TryGetValue(employeeId, out var employee))
                {
                    continue;
                }

                decimal amount;
                if (employee.IsFixedPFAmount)
                {
                    amount = employee.PFAmount.GetValueOrDefault();
                }
                else if (payrollByEmployee.TryGetValue(employeeId, out var payroll))
                {
                    amount = Math.Round(payroll.salery * ((activePeriod?.PFPercentage ?? 0m) / 100m), 2);
                }
                else
                {
                    amount = 0;
                }

                amounts[employeeId] = amount;
            }

            return amounts;
        }

        private static decimal SumDetailAmount(IEnumerable<SalarySlipDetail> details, string componentName)
        {
            return details
                .Where(d => d.ComponentName == componentName)
                .Sum(d => d.Amount);
        }

        private static List<PayrollComponentSummary> BuildFallbackDeductionComponents(
            PayrollDashboardViewModel model,
            Dictionary<int, decimal> taxByEmployee,
            Dictionary<int, decimal> eobiByEmployee,
            Dictionary<int, decimal> pfByEmployee,
            Dictionary<int, decimal> loanByEmployee,
            Dictionary<int, decimal> deductionAdjustmentsByEmployee)
        {
            var items = new List<PayrollComponentSummary>();

            if (model.IncomeTaxTotal > 0)
            {
                items.Add(new PayrollComponentSummary
                {
                    Name = "Income Tax",
                    Employees = taxByEmployee.Count,
                    Amount = model.IncomeTaxTotal
                });
            }

            if (model.EobiTotal > 0)
            {
                items.Add(new PayrollComponentSummary
                {
                    Name = "EOBI",
                    Employees = eobiByEmployee.Count,
                    Amount = model.EobiTotal
                });
            }

            if (model.PfTotal > 0)
            {
                items.Add(new PayrollComponentSummary
                {
                    Name = "Provident Fund",
                    Employees = pfByEmployee.Count,
                    Amount = model.PfTotal
                });
            }

            if (model.LoanDeductionTotal > 0)
            {
                items.Add(new PayrollComponentSummary
                {
                    Name = "Loan Installment",
                    Employees = loanByEmployee.Count,
                    Amount = model.LoanDeductionTotal
                });
            }

            if (model.DeductionAdjustmentsTotal > 0)
            {
                items.Add(new PayrollComponentSummary
                {
                    Name = "Approved Deduction Adjustments",
                    Employees = deductionAdjustmentsByEmployee.Count,
                    Amount = model.DeductionAdjustmentsTotal
                });
            }

            return items
                .OrderByDescending(x => x.Amount)
                .Take(6)
                .ToList();
        }

        private static decimal EstimateEmployeeEarnings(
            int employeeId,
            Dictionary<int, decimal> estimatedEarningsByEmployee,
            Dictionary<int, decimal> bonusByEmployee,
            Dictionary<int, decimal> earningAdjustmentsByEmployee)
        {
            var baseEarnings = estimatedEarningsByEmployee.TryGetValue(employeeId, out var estimated) ? estimated : 0m;
            var bonus = bonusByEmployee.TryGetValue(employeeId, out var employeeBonus) ? employeeBonus : 0m;
            var adjustments = earningAdjustmentsByEmployee.TryGetValue(employeeId, out var employeeAdjustments) ? employeeAdjustments : 0m;
            return baseEarnings + bonus + adjustments;
        }

        private static decimal EstimateEmployeeDeductions(
            int employeeId,
            Dictionary<int, decimal> taxByEmployee,
            Dictionary<int, decimal> eobiByEmployee,
            Dictionary<int, decimal> pfByEmployee,
            Dictionary<int, decimal> loanByEmployee,
            Dictionary<int, decimal> deductionAdjustmentsByEmployee)
        {
            var tax = taxByEmployee.TryGetValue(employeeId, out var employeeTax) ? employeeTax : 0m;
            var eobi = eobiByEmployee.TryGetValue(employeeId, out var employeeEobi) ? employeeEobi : 0m;
            var pf = pfByEmployee.TryGetValue(employeeId, out var employeePf) ? employeePf : 0m;
            var loan = loanByEmployee.TryGetValue(employeeId, out var employeeLoan) ? employeeLoan : 0m;
            var adjustments = deductionAdjustmentsByEmployee.TryGetValue(employeeId, out var employeeAdjustments) ? employeeAdjustments : 0m;
            return tax + eobi + pf + loan + adjustments;
        }

        private static decimal EstimateEmployeeNet(
            int employeeId,
            Dictionary<int, decimal> estimatedEarningsByEmployee,
            Dictionary<int, decimal> bonusByEmployee,
            Dictionary<int, decimal> earningAdjustmentsByEmployee,
            Dictionary<int, decimal> taxByEmployee,
            Dictionary<int, decimal> eobiByEmployee,
            Dictionary<int, decimal> pfByEmployee,
            Dictionary<int, decimal> loanByEmployee,
            Dictionary<int, decimal> deductionAdjustmentsByEmployee)
        {
            return EstimateEmployeeEarnings(employeeId, estimatedEarningsByEmployee, bonusByEmployee, earningAdjustmentsByEmployee)
                - EstimateEmployeeDeductions(employeeId, taxByEmployee, eobiByEmployee, pfByEmployee, loanByEmployee, deductionAdjustmentsByEmployee);
        }

        private static List<PayrollExecutionState> BuildExecutionStates(
            PayrollDashboardViewModel model,
            List<IncomeTaxDetuction> taxRecords,
            List<EOBIDetuction> eobiRecords,
            List<PFMembers> pfRecords,
            List<MasterLoanAdvanceDetail> loanInstallments,
            List<Bonus> bonuses,
            List<SalaryAdjustments> adjustments,
            List<SalarySlipMaster> salarySlips)
        {
            return new List<PayrollExecutionState>
            {
                new()
                {
                    Label = "Income tax prepared",
                    IsComplete = taxRecords.Count > 0,
                    LinkController = "IncomeTaxDetuction",
                    HelperText = $"{taxRecords.Count} record(s)"
                },
                new()
                {
                    Label = "EOBI prepared",
                    IsComplete = eobiRecords.Count > 0,
                    LinkController = "EOBIDetuction",
                    HelperText = $"{eobiRecords.Count} record(s)"
                },
                new()
                {
                    Label = "PF prepared",
                    IsComplete = pfRecords.Count > 0,
                    LinkController = "PFMembers",
                    HelperText = $"{pfRecords.Count} member record(s)"
                },
                new()
                {
                    Label = "Loan deductions prepared",
                    IsComplete = loanInstallments.Count > 0,
                    LinkController = "MasterLoanAdvanceDetail",
                    HelperText = $"{loanInstallments.Count} installment(s)"
                },
                new()
                {
                    Label = "Bonuses approved",
                    IsComplete = bonuses.Count > 0,
                    LinkController = "Bonus",
                    HelperText = $"{bonuses.Count} record(s)"
                },
                new()
                {
                    Label = "Salary adjustments approved",
                    IsComplete = adjustments.Count > 0,
                    LinkController = "SalaryAdjustment",
                    HelperText = $"{adjustments.Count} record(s)"
                },
                new()
                {
                    Label = "Salary slips generated",
                    IsComplete = salarySlips.Count > 0,
                    LinkController = "PayrollRegister",
                    HelperText = model.ActivePeriod == null ? "No active period" : $"{salarySlips.Count} slip(s)"
                }
            };
        }
    }
}
