using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public enum BonusApprovalStatus
    {
        Pending,
        Approved,
        Rejected
    }

    [Table("Bonuses")]
    public class Bonus
    {
        [Key]
        public int Id { get; set; }

        // ==========================================
        // 1. RELATIONSHIPS
        // ==========================================
        [Required]
        public int EmployeeID { get; set; }
        [ForeignKey("EmployeeID")]
        public virtual Employee? Employee { get; set; }

        [Required]
        public int SalaryPeriodID { get; set; }
        [ForeignKey("SalaryPeriodID")]
        public virtual SalaryPeriod? SalaryPeriod { get; set; }

        [Required]
        public int BonusTypeID { get; set; }
        [ForeignKey("BonusTypeID")]
        public virtual Valueset? BonusType { get; set; }

        // ==========================================
        // 2. CALCULATIONS
        // ==========================================
        [Column(TypeName = "decimal(18,2)")]
        public decimal BonusPercentage { get; set; } // Based on Gross Salary %

        public bool IsPercentage { get; set; } = false;

        [Column(TypeName = "decimal(18,2)")]
        public decimal BonusAmount { get; set; } // Fixed or Calculated

        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxDeduction { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal NetBonus { get; set; }

        // ==========================================
        // 3. METADATA & STATUS
        // ==========================================
        public DateTime PaymentDate { get; set; } = DateTime.Now;

        [Required]
        public string ApprovalStatus { get; set; } = "Pending";

        public string? Remarks { get; set; }

        // Stamps
        public int createdby { get; set; }
        public DateTime createat { get; set; } = DateTime.Now;
        public int? updatedby { get; set; }
        public DateTime? updateat { get; set; }
    }
}
