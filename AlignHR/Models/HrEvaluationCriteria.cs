using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class HrEvaluationCriteria
    {
        [Key]
        [Column(TypeName = "numeric(18,0)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal InterviewRoundId { get; set; }

        [Required]
        [StringLength(200)]
        public string CriteriaName { get; set; } = string.Empty;

        [Column(TypeName = "decimal(5,2)")]
        public decimal MaxScore { get; set; }

        public HrInterviewRound? Round { get; set; }
        public ICollection<HrEvaluationScore> Scores { get; set; } = new List<HrEvaluationScore>();
    }
}
