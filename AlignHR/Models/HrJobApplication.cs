using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class HrJobApplication
    {
        [Key]
        [Column(TypeName = "numeric(18,0)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal? CandidateFK { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal? JobPostingFK { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal? CurrentStageFK { get; set; }

        public DateTime? AppliedDate { get; set; }

        public string? RecruiterNotes { get; set; }

        public bool IsActive { get; set; } = true;

        public HrCandidate? Candidate { get; set; }
        public HrJobPosting? JobPosting { get; set; }
        public HrApplicationStage? CurrentStage { get; set; }
        public ICollection<HrApplicationStageHistory> StageHistory { get; set; } = new List<HrApplicationStageHistory>();
    }
}
