using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class HrInterviewSchedule
    {
        [Key]
        [Column(TypeName = "numeric(18,0)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal JobApplicationId { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal InterviewRoundId { get; set; }

        public DateTime ScheduledDateTime { get; set; }

        [StringLength(1000)]
        public string? MeetingLink { get; set; }

        [StringLength(500)]
        public string? Location { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Scheduled";

        [Column(TypeName = "numeric(18,0)")]
        public decimal CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public HrJobApplication? JobApplication { get; set; }
        public HrInterviewRound? InterviewRound { get; set; }
        public ICollection<HrInterviewPanel> Panel { get; set; } = new List<HrInterviewPanel>();
        public ICollection<HrInterviewFeedback> Feedback { get; set; } = new List<HrInterviewFeedback>();
    }
}
