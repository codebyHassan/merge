using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class HrWorkflowInstance
    {
        [Key]
        [Column(TypeName = "numeric(18,0)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal WorkflowDefinitionId { get; set; }

        [Required]
        [StringLength(50)]
        public string EntityType { get; set; } = string.Empty;

        [Column(TypeName = "numeric(18,0)")]
        public decimal EntityId { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal CurrentStepId { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Pending";

        public DateTime StartedDate { get; set; } = DateTime.Now;

        public DateTime? CompletedDate { get; set; }

        public HrWorkflowDefinition? WorkflowDefinition { get; set; }
        public HrWorkflowStep? CurrentStep { get; set; }
        public ICollection<HrWorkflowInstanceStep> InstanceSteps { get; set; } = new List<HrWorkflowInstanceStep>();
    }
}
