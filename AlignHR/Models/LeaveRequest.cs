using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AlignHR.Models.Enums;

namespace AlignHR.Models
{
    public class LeaveRequest
    {
        [Key]
        [Column(TypeName = "numeric(18,0)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        [Column(TypeName = "numeric(18,0)")]
        public decimal LeaveTypeId { get; set; }

        // ===== Leave Dates =====
        [Column(TypeName = "date")]
        public DateTime FromDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime ToDate { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal DaysCount { get; set; }

        // ===== Half Day Support =====
        public bool IsHalfDay { get; set; } = false;

        [StringLength(20)]
        public string? HalfDaySession { get; set; } // "FirstHalf" / "SecondHalf"

        // ===== Details =====
        [StringLength(500)]
        public string? Reason { get; set; }

        public string? AttachmentUrl { get; set; }

        // ===== Workflow State =====

        /// <summary>
        /// Current workflow status: Pending, Approved, Rejected, Cancelled
        /// </summary>
        [StringLength(20)]
        public string Status { get; set; } = "Pending";

        /// <summary>
        /// FK to the Employee currently responsible for approval action.
        /// Null when workflow is finalized (approved or rejected).
        /// </summary>
        public int? CurrentApproverId { get; set; }

        /// <summary>
        /// Type-safe workflow stage — replaces magic strings.
        /// </summary>
        public WorkflowStage CurrentStage { get; set; } = WorkflowStage.LineManager;

        /// <summary>
        /// Current approval depth (1 = first approver, 2 = second, etc.)
        /// </summary>
        public int ApprovalLevel { get; set; } = 0;

        /// <summary>
        /// When the leave was submitted by the employee.
        /// </summary>
        public DateTime? AppliedAt { get; set; }

        /// <summary>
        /// Timestamp of final approval (by HR / final approver).
        /// </summary>
        public DateTime? ApprovedAt { get; set; }

        /// <summary>
        /// Immutable employee code of the final approver. ERP-safe audit field.
        /// </summary>
        [StringLength(50)]
        public string? ApprovedByEmpNo { get; set; }

        /// <summary>
        /// Timestamp when the request was rejected.
        /// </summary>
        public DateTime? RejectedAt { get; set; }

        /// <summary>
        /// Reason provided by the rejecting approver.
        /// </summary>
        [StringLength(500)]
        public string? RejectionReason { get; set; }

        // ===== Concurrency =====

        /// <summary>
        /// Concurrency token to prevent double-approvals and stale updates.
        /// </summary>
        [Timestamp]
        public byte[]? RowVersion { get; set; }

        // ===== Audit =====
        public DateTime? CreatedAt { get; set; }
        public string? CreatedBy { get; set; }

        // ===== Navigation =====
        [ForeignKey("EmployeeId")]
        public Employee? Employee { get; set; }

        [ForeignKey("LeaveTypeId")]
        public LeaveType? LeaveType { get; set; }

        [ForeignKey("CurrentApproverId")]
        public Employee? CurrentApprover { get; set; }

        /// <summary>
        /// Approval history audit trail for this request.
        /// </summary>
        public ICollection<LeaveApproval> LeaveApprovals { get; set; } = new List<LeaveApproval>();
    }
}
