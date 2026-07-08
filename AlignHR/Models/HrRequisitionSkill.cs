using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class HrRequisitionSkill
    {
        [Key]
        [Column(TypeName = "numeric(18,0)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal RequisitionId { get; set; }

        [Required]
        [StringLength(200)]
        public string SkillName { get; set; } = string.Empty;

        [Column(TypeName = "decimal(5,2)")]
        public decimal? YearsExperience { get; set; }

        public bool IsMandatory { get; set; } = true;

        public HrRequisition? Requisition { get; set; }
    }
}
