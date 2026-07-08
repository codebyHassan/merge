using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class LeavePolicyRule
    {
        [Key]
        [Column(TypeName = "numeric(18,0)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [Required]
        [Column(TypeName = "numeric(18,0)")]
        public decimal LeavePolicyId { get; set; }

        [Required]
        [Column(TypeName = "numeric(18,0)")]
        public decimal LeaveTypeId { get; set; }

        // ===== Leave Allocation =====
        [Column(TypeName = "decimal(5,2)")]
        public decimal AnnualQuota { get; set; } = 0;

        [Column(TypeName = "decimal(5,2)")]
        public decimal MonthlyAccrual { get; set; } = 0;

        [Column(TypeName = "decimal(5,2)")]
        public decimal CarryForwardLimit { get; set; } = 0;

        public bool IsEncashable { get; set; } = false;

        // ===== Rules =====
        public bool AllowNegativeBalance { get; set; } = false;

        public bool AllowHalfDay { get; set; } = true;

        public bool AllowDuringProbation { get; set; } = false;

        [Column(TypeName = "decimal(5,2)")]
        public decimal MaxConsecutiveDays { get; set; } = 0;

        [Column(TypeName = "decimal(5,2)")]
        public decimal MinNoticeDays { get; set; } = 0;

        public bool ApplySandwichRule { get; set; } = false;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [StringLength(50)]
        public string? CreatedBy { get; set; }

        [StringLength(50)]
        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        // ===== Navigation =====
        [ForeignKey("LeavePolicyId")]
        public LeavePolicy? LeavePolicy { get; set; }

        [ForeignKey("LeaveTypeId")]
        public LeaveType? LeaveType { get; set; }
    }
}
