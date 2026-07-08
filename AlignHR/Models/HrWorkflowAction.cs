using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class HrWorkflowAction
    {
        [Key]
        [Column(TypeName = "numeric(18,0)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal WorkflowInstanceStepId { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal ActionByEmployeeId { get; set; }

        [Required]
        [StringLength(50)]
        public string ActionType { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Comments { get; set; }

        public DateTime ActionDate { get; set; } = DateTime.Now;

        public HrWorkflowInstanceStep? WorkflowInstanceStep { get; set; }
    }
}
