using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    /// <summary>
    /// Audit trail for leave workflow actions.
    /// Every approval, rejection, or submission is logged here.
    /// </summary>
    public class LeaveApproval
    {
        [Key]
        [Column(TypeName = "numeric(18,0)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [Required]
        [Column(TypeName = "numeric(18,0)")]
        public decimal LeaveRequestId { get; set; }

        [Required]
        public int ApproverEmployeeId { get; set; }

        /// <summary>
        /// The approval depth at which this action was taken (1 = first approver, etc.)
        /// </summary>
        public int ApprovalLevel { get; set; }

        /// <summary>
        /// Action taken: "Submitted", "Approved", "Rejected"
        /// </summary>
        [Required]
        [StringLength(20)]
        public string Action { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Remarks { get; set; }

        public DateTime ActionAt { get; set; } = DateTime.Now;

        // ===== Navigation =====

        [ForeignKey("LeaveRequestId")]
        public LeaveRequest? LeaveRequest { get; set; }

        [ForeignKey("ApproverEmployeeId")]
        public Employee? ApproverEmployee { get; set; }
    }
}
