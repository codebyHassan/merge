using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class MasterLoanAdvanceDetail
    {
        [Key]
        public int id { get; set; }

        // Links to loan/advance request
        [Required]
        public int MasterId { get; set; }
        [ForeignKey("MasterId")]
        public MasterLoanAdvance? MasterLoanAdvance { get; set; }

        // Month order (1,2,3…)
        public int InstallmentNo { get; set; }

        // Which payroll month (e.g., "2024/04")
        [Required]
        [StringLength(50)]
        public string DeductionMonth { get; set; }

        // EMI / installment
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalDeductionAmount { get; set; }

        // Actually deducted
        [Column(TypeName = "decimal(18,2)")]
        public decimal DeductedAmount { get; set; }

        // Balance left
        [Column(TypeName = "decimal(18,2)")]
        public decimal RemainingAmount { get; set; }

        // Loan-only field
        [Column(TypeName = "decimal(18,2)")]
        public decimal InterestAmount { get; set; }

        // Pending / Deducted / Skipped
        public LoanInstallmentStatus Status { get; set; }

        // Stamps 
        public int createdby { get; set; }
        public DateTime createat { get; set; } = DateTime.Now;
        public int updatedby { get; set; }
        public DateTime updateat { get; set; }
    }
}
