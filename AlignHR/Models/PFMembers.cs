using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class PFMembers
    {
        [Key]
        public int id { get; set; }

        [Required]
        public int EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]
        public Employee? Employee { get; set; }

        [Required]
        public int SalaryPeriodFK { get; set; }
        [ForeignKey("SalaryPeriodFK")]
        public SalaryPeriod? SalaryPeriod { get; set; }


        [Display(Name = "Amount")]
        public decimal? Amount { get; set; }
        // Stamps 
        public int createdby { get; set; }
        public DateTime createat { get; set; } = DateTime.Now;
        public int updatedby { get; set; }
        public DateTime updateat { get; set; }
    }
}
