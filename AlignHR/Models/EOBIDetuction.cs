using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class EOBIDetuction
    {
        [Key]
        public int id { get; set; }
        
        // Fk with Period Salery 
        [Required]
        [Display(Name = "Salary Period")]
        public int PeriodSalery { get; set; }
        [ForeignKey("PeriodSalery")]
        public SalaryPeriod? SalaryPeriod { get; set; }

        // FK with Employee 
        [Required]
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]
        public Employee? Employee { get; set; }

        [Display(Name = "Amount")]
        public decimal? Amount { get; set; }

        // Stamps 
        public int createdby { get; set; }
        public DateTime createat { get; set; } = DateTime.Now;
        public int updatedby { get; set; }
        public DateTime updateat { get; set; }
    }
}
