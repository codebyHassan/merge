using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AlignHR.Models.Enums;

namespace AlignHR.Models
{
    public class LeaveTransaction
    {
        [Key]
        [Column(TypeName = "numeric(18,0)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        // ===== References =====

        [Column(TypeName = "numeric(18,0)")]
        public decimal? LeaveRequestFk { get; set; }

        public int EmployeeFk { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal LeaveTypeFk { get; set; }

        // ===== Transaction =====

        [Column(TypeName = "date")]
        public DateTime TransactionDate { get; set; } = DateTime.Now;

        [Required]
        public LeaveTransactionType TransactionType { get; set; }

        public int Year { get; set; }

        // ===== Values =====

        [Column(TypeName = "decimal(5,2)")]
        public decimal Credit { get; set; } = 0;

        [Column(TypeName = "decimal(5,2)")]
        public decimal Debit { get; set; } = 0;

        // Cached running balance for UI/reporting only.
        // Real balance must always be calculated from ledger entries.
        [Column(TypeName = "decimal(5,2)")]
        public decimal Balance { get; set; } = 0;

        // ===== Description & Audit =====

        [StringLength(1000)]
        public string? Narration { get; set; }

        [StringLength(100)]
        public string? ReferenceNo { get; set; }

        [StringLength(50)]
        public string? TransactionSource { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal? ReversalOfTransactionId { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // ===== Navigation =====

        [ForeignKey("LeaveRequestFk")]
        public LeaveRequest? LeaveRequest { get; set; }

        [ForeignKey("EmployeeFk")]
        public Employee? Employee { get; set; }

        [ForeignKey("LeaveTypeFk")]
        public LeaveType? LeaveType { get; set; }
    }
}
