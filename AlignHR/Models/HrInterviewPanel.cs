using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class HrInterviewPanel
    {
        [Key]
        [Column(TypeName = "numeric(18,0)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal InterviewScheduleId { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal EmployeeFk { get; set; }

        public HrInterviewSchedule? InterviewSchedule { get; set; }
    }
}
