using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class HrWorkflowDefinition
    {
        [Key]
        [Column(TypeName = "numeric(18,0)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [Required]
        [StringLength(100)]
        public string WorkflowName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        [StringLength(50)]
        public string? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        [StringLength(50)]
        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public ICollection<HrWorkflowStep> Steps { get; set; } = new List<HrWorkflowStep>();
    }
}
