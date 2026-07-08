using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class OverTimeHours
    {
        //id 
        public int id { get; set; }


        //Employee Fk 
        [Required]
        public int EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]
        public Employee? Employee { get; set; }

        //Salary Period Fk
        [Required]
        public int SalaryPeriodFK { get; set; }
        [ForeignKey("SalaryPeriodFK")]
        public SalaryPeriod? SalaryPeriod { get; set; }

        //OT hours 
        public string OTHours { get; set; }

        //OT amount
        public int Amount { get; set; }

        //Stamps 
        public int createdby { get; set; }
        public DateTime createat { get; set; } = DateTime.Now;
        public int updatedby { get; set; }
        public DateTime updateat { get; set; }
    }
}
