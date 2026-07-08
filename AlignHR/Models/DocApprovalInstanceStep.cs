using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class DocApprovalInstanceStep
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public long InstanceId { get; set; }

        [ForeignKey("InstanceId")]
        public DocApprovalInstance? Instance { get; set; }

        public int StepNo { get; set; }

        [Required]
        public int? TemplateStepId { get; set; }

        [ForeignKey("TemplateStepId")]
        public ApprovalTemplateStep? TemplateStep { get; set; }

        [StringLength(20)]
        public string Action { get; set; } = "Pending";

        [StringLength(1000)]
        public string? Comments { get; set; }

        public DateTime? ActionAt { get; set; }

        public int? ApproverId { get; set; }
        [ForeignKey("ApproverId")]
        public User? Approver { get; set; }

        public int? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public Department? Department { get; set; }
    }
}
