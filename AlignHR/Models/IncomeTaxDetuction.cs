using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class IncomeTaxDetuction
    {
        // id 
        [Key]
        public int id { get; set; }

        // withoutpaydays 
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal incometaxdetection { get; set; }



        //Fk with Period Salery 
        public int PeriodSalery { get; set; }
        [ForeignKey("PeriodSalery")]
        public SalaryPeriod? SalaryPeriod { get; set; }



        //FK with Employee 

        public int EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]
        public Employee? Employee { get; set; }




        //Stamps 

        public int createdby { get; set; }
        public DateTime createat { get; set; } = DateTime.Now;
        public int updatedby { get; set; }
        public DateTime updateat { get; set; }
    }
}
