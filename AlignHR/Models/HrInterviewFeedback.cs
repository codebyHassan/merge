using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class HrInterviewFeedback
    {
        [Key]
        [Column(TypeName = "numeric(18,0)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal InterviewScheduleFk { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal InterviewerEmployeeFk { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? OverallScore { get; set; }

        [Required]
        [StringLength(50)]
        public string Recommendation { get; set; } = string.Empty;

        public string? Strengths { get; set; }

        public string? Concerns { get; set; }

        public string? Comments { get; set; }

        public DateTime SubmittedDate { get; set; } = DateTime.Now;

        // Audit
        public bool IsSubmitted { get; set; } = false;

        [Column(TypeName = "numeric(18,0)")]
        public decimal? UpdatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal? SubmittedBy { get; set; }

        public DateTime? SubmittedAt { get; set; }

        public HrInterviewSchedule? InterviewSchedule { get; set; }
        public ICollection<HrEvaluationScore> Scores { get; set; } = new List<HrEvaluationScore>();
    }
}
