namespace AlignHR.Models
{
    public class PayrollDashboardViewModel
    {
        public SalaryPeriod? ActivePeriod { get; set; }
        public string LoanMonthKey { get; set; } = string.Empty;
        public bool HasGeneratedSalarySlips { get; set; }
        public bool HasPostedPeriod => ActivePeriod?.IsPostedToGL ?? false;
        public bool UsesEstimatedFigures => ActivePeriod != null && !HasGeneratedSalarySlips;

        public int TotalEmployees { get; set; }
        public int ActiveEmployees { get; set; }
        public int PayrollConfiguredEmployees { get; set; }
        public int EmployeesInCurrentCycle { get; set; }
        public int EobiMemberCount { get; set; }
        public int PfMemberCount { get; set; }
        public int TaxFilerCount { get; set; }
        public int TaxExemptCount { get; set; }

        public int EmployeesWithIncomeTax { get; set; }
        public int EmployeesWithEobi { get; set; }
        public int EmployeesWithPf { get; set; }
        public int EmployeesWithLoans { get; set; }
        public int EmployeesWithWithoutPay { get; set; }
        public int SalarySlipCount { get; set; }

        public decimal GrossPayroll { get; set; }
        public decimal TotalAllowances { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal NetPayable { get; set; }
        public decimal IncomeTaxTotal { get; set; }
        public decimal EobiTotal { get; set; }
        public decimal PfTotal { get; set; }
        public decimal LoanDeductionTotal { get; set; }
        public decimal BonusTotal { get; set; }
        public decimal EarningAdjustmentsTotal { get; set; }
        public decimal DeductionAdjustmentsTotal { get; set; }
        public decimal WithoutPayDaysTotal { get; set; }

        public decimal AverageGrossPerEmployee => EmployeesInCurrentCycle > 0
            ? Math.Round(GrossPayroll / EmployeesInCurrentCycle, 2)
            : 0;

        public decimal AverageNetPerEmployee => EmployeesInCurrentCycle > 0
            ? Math.Round(NetPayable / EmployeesInCurrentCycle, 2)
            : 0;

        public decimal DeductionRate => TotalAllowances > 0
            ? Math.Round((TotalDeductions / TotalAllowances) * 100, 2)
            : 0;

        public decimal StatutoryDeductionTotal => IncomeTaxTotal + EobiTotal + PfTotal;

        public decimal OtherDeductionsTotal
        {
            get
            {
                var other = TotalDeductions - IncomeTaxTotal - EobiTotal - PfTotal - LoanDeductionTotal;
                return other > 0 ? other : 0;
            }
        }

        public string PeriodLabel => ActivePeriod?.PeriodName ?? "No active salary period";

        public string PeriodDateRange => ActivePeriod == null
            ? "Configure a salary period to unlock cycle analytics."
            : $"{ActivePeriod.StartDate:dd MMM yyyy} to {ActivePeriod.EndDate:dd MMM yyyy}";

        public string CycleStatusLabel
        {
            get
            {
                if (ActivePeriod == null) return "Not configured";
                if (HasGeneratedSalarySlips && ActivePeriod.IsPostedToGL) return "Posted to GL";
                if (HasGeneratedSalarySlips) return "Slips generated";
                if (ActivePeriod.IsProcessed) return "Processed";
                if (ActivePeriod.IsActive) return "Open period";
                return "Ready";
            }
        }

        public List<PayrollExecutionState> ExecutionStates { get; set; } = new();
        public List<PayrollDepartmentSummary> DepartmentSummaries { get; set; } = new();
        public List<PayrollComponentSummary> EarningComponents { get; set; } = new();
        public List<PayrollComponentSummary> DeductionComponents { get; set; } = new();
        public List<PayrollEmployeeSnapshot> EmployeeSnapshots { get; set; } = new();
    }

    public class PayrollExecutionState
    {
        public string Label { get; set; } = string.Empty;
        public bool IsComplete { get; set; }
        public string LinkController { get; set; } = string.Empty;
        public string LinkAction { get; set; } = "Index";
        public string? HelperText { get; set; }
    }

    public class PayrollDepartmentSummary
    {
        public string DepartmentName { get; set; } = "Unassigned";
        public int Employees { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal DeductionAmount { get; set; }
    }

    public class PayrollComponentSummary
    {
        public string Name { get; set; } = string.Empty;
        public int Employees { get; set; }
        public decimal Amount { get; set; }
    }

    public class PayrollEmployeeSnapshot
    {
        public string EmployeeCode { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = "Unassigned";
        public decimal GrossAmount { get; set; }
        public decimal EarningsAmount { get; set; }
        public decimal DeductionAmount { get; set; }
        public decimal NetAmount { get; set; }
        public bool IsPfMember { get; set; }
        public bool IsEobiMember { get; set; }
        public bool HasIncomeTax { get; set; }
    }
}
