using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class HrJoiningConfirmation
    {
        [Key]
        [Column(TypeName = "numeric(18,0)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal OnboardingId { get; set; }

        [Column(TypeName = "date")]
        public DateTime JoinedDate { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal ConfirmedByEmployeeFk { get; set; }

        [StringLength(1000)]
        public string? Remarks { get; set; }

        public DateTime ConfirmedDate { get; set; } = DateTime.Now;

        public HrOnboarding? Onboarding { get; set; }
    }
}
