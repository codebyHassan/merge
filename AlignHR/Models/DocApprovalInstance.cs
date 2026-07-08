using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class DocApprovalInstance
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public long DocumentID { get; set; }

        [ForeignKey("DocumentID")]
        public Document? Document { get; set; }

        [Required]
        public int WorkflowId { get; set; }

        [ForeignKey("WorkflowId")]
        public ApprovalTemplate? Workflow { get; set; }

        public int CurrentStep { get; set; } = 1;

        [StringLength(20)]
        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? CompletedAt { get; set; }

        public int? CreatedById { get; set; }
        [ForeignKey("CreatedById")]
        public User? CreatedBy { get; set; }

        public ICollection<DocApprovalInstanceStep> InstanceSteps { get; set; } = new List<DocApprovalInstanceStep>();
    }
}
