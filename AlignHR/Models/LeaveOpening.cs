using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class LeaveOpening
    {
        [Key]
        [Column(TypeName = "numeric(18,0)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        public int EmployeeFk { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal LeaveTypeFk { get; set; }

        public int Year { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal OpeningBalance { get; set; }

        [StringLength(500)]
        public string? Narration { get; set; }


        public int CreatedBy { get; set; }
        // public int UpdatedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        // Navigation

        [ForeignKey("EmployeeFk")]
        public Employee? Employee { get; set; }

        [ForeignKey("LeaveTypeFk")]
        public LeaveType? LeaveType { get; set; }
    }
}
