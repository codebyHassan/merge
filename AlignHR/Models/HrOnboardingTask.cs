using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class HrOnboardingTask
    {
        [Key]
        [Column(TypeName = "numeric(18,0)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal OnboardingFk { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal TaskTemplateFk { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal? AssignedToEmployeeFk { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DueDate { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Pending";

        public DateTime? CompletedDate { get; set; }

        [StringLength(1000)]
        public string? Remarks { get; set; }

        public HrOnboarding? Onboarding { get; set; }
        public HrOnboardingTaskTemplate? TaskTemplate { get; set; }
    }
}
