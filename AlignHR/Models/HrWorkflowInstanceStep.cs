using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class HrWorkflowInstanceStep
    {
        [Key]
        [Column(TypeName = "numeric(18,0)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal WorkflowInstanceId { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal WorkflowStepId { get; set; }

        public int StepOrder { get; set; }

        [Required]
        [StringLength(50)]
        public string ApproverType { get; set; } = string.Empty;

        [Column(TypeName = "numeric(18,0)")]
        public decimal EmployeeId { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Waiting";

        public DateTime AssignedDate { get; set; } = DateTime.Now;

        public DateTime? ActionDate { get; set; }

        [StringLength(1000)]
        public string? Comments { get; set; }

        public HrWorkflowInstance? WorkflowInstance { get; set; }
        public HrWorkflowStep? WorkflowStep { get; set; }
        public ICollection<HrWorkflowAction> Actions { get; set; } = new List<HrWorkflowAction>();
    }
}
