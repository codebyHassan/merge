using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class HrInterviewRound
    {
        [Key]
        [Column(TypeName = "numeric(18,0)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal JobPostingId { get; set; }

        [Required]
        [StringLength(100)]
        public string RoundName { get; set; } = string.Empty;

        public int RoundOrder { get; set; }

        public bool IsMandatory { get; set; } = true;

        public bool IsActive { get; set; } = true;

        public ICollection<HrEvaluationCriteria> Criteria { get; set; } = new List<HrEvaluationCriteria>();
        public ICollection<HrInterviewSchedule> Schedules { get; set; } = new List<HrInterviewSchedule>();
    }
}
