using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.X509Certificates;

namespace AlignHR.Models
{
    public enum PayrollComponentTypes
    {
        Earning,
        Deduction
    }

    [Table("SalaryAdjustments")]
    public class SalaryAdjustments
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]
        public Employee? employee { get; set; }

        public int SalaryPeriodId { get; set; }
        [ForeignKey("SalaryPeriodId")]
        public SalaryPeriod? SalaryPeriod { get; set; }
        public decimal Amount { get; set; }
        public PayrollComponentTypes Type { get; set; } 

        public string AdjustmentCategory { get; set; }

        public string Reason { get; set; }

        public bool IsApproved { get; set; } = false;

        public bool IsAppliedInPayroll { get; set; } = false;

        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }

        public DateTime? ApprovedAt { get; set; }
        public int? ApprovedBy { get; set; }

  
    }
}
