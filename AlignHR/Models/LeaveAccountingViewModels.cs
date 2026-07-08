using System.Collections.Generic;

namespace AlignHR.Models
{
    public class CompanyLeaveSummaryViewModel
    {
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public int Year { get; set; }
        public List<LeaveTypeSummary> Details { get; set; } = new List<LeaveTypeSummary>();

        // Helpers for totals
        public decimal TotalOpening => Details.Sum(d => d.Opening);
        public decimal TotalAvailed => Details.Sum(d => d.Availed);
        public decimal TotalBalance => Details.Sum(d => d.Balance);
    }

    public class LeaveTypeSummary
    {
        public string LeaveTypeName { get; set; }
        public decimal Opening { get; set; }
        public decimal Availed { get; set; }
        public decimal Balance => Opening - Availed;
    }
}
