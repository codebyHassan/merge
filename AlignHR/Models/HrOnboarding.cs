using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class HrOnboarding
    {
        [Key]
        [Column(TypeName = "numeric(18,0)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal CandidateId { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal OfferId { get; set; }

        [Column(TypeName = "date")]
        public DateTime PlannedJoiningDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ActualJoiningDate { get; set; }

        [StringLength(50)]
        public string? Status { get; set; } = "Initiated";

        [StringLength(50)]
        public string? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        [StringLength(50)]
        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public HrCandidate? Candidate { get; set; }
        public HrOffer? Offer { get; set; }
        public ICollection<HrOnboardingTask> Tasks { get; set; } = new List<HrOnboardingTask>();
    }
}
