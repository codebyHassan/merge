using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class HrEvaluationScore
    {
        [Key]
        [Column(TypeName = "numeric(18,0)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal InterviewFeedbackFk { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal EvaluationCriteriaFk { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal Score { get; set; }

        public HrInterviewFeedback? InterviewFeedback { get; set; }
        public HrEvaluationCriteria? EvaluationCriteria { get; set; }
    }
}
