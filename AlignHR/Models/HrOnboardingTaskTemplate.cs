using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class HrOnboardingTaskTemplate
    {
        [Key]
        [Column(TypeName = "numeric(18,0)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [Required]
        [StringLength(200)]
        public string TaskName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string ResponsibleDepartment { get; set; } = string.Empty;

        public bool IsMandatory { get; set; } = true;

        public bool IsActive { get; set; } = true;
    }
}
