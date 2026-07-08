using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class WorkflowCondition
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int WorkflowId { get; set; }

        [ForeignKey("WorkflowId")]
        public ApprovalTemplate? Workflow { get; set; }

        [Required]
        [StringLength(100)]
        public string FieldName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Operator { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Value { get; set; } = string.Empty;

        public int? ThenWorkflowId { get; set; }

        [ForeignKey("ThenWorkflowId")]
        public ApprovalTemplate? ThenWorkflow { get; set; }

        public int createdby { get; set; }
        public DateTime createat { get; set; } = DateTime.Now;
    }
}