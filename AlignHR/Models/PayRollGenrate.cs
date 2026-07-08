using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class PayRollGenrate
    {
        //id 
        [Key]
        public int id { get; set; }

        //Employee FK 
        public int EmployeeFK { get; set; }
        [ForeignKey("EmployeeFK")]
        public Employee? Employee { get; set; }


        //payroll defination file FK
        public int PayRollDefinationFK { get; set; }
        [ForeignKey("PayRollDefinationFK")]
        public PayRollDefinationFile? PayRollDefinationFile { get; set; }

        //Salery 
        public int salery { get; set; }

        //stamps 
        public int createdby { get; set; }
        public DateTime createat { get; set; } = DateTime.Now;
        public int updatedby { get; set; }
        public DateTime updateat { get; set; }
    }
}
