using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class ApprovalTemplate
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string TemplateName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public int Version { get; set; } = 1;

        [Required]
        public int TypeId { get; set; }

        [ForeignKey("TypeId")]
        public DocumentType? DocumentType { get; set; }

        public WorkflowFlowType FlowType { get; set; }

        public int createdby { get; set; }
        public DateTime createat { get; set; } = DateTime.Now;
        public int updatedby { get; set; }
        public DateTime updateat { get; set; }

        public ICollection<ApprovalTemplateStep> Steps { get; set; } = new List<ApprovalTemplateStep>();
        public ICollection<WorkflowCondition> Conditions { get; set; } = new List<WorkflowCondition>();
    }
}
