using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class AttendenceException
    {
        [Key]
        public int id { get; set; }

        public DateOnly AttendeceDate { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }

        //FK with Employee 

        public int EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]
        public Employee? Employee { get; set; }


        //Fk with Valueset 
        public int? ExpectionTypeFk { get; set; }
        [ForeignKey("ExpectionTypeFk")]
        public Valueset? ExpectionType { get; set; }

        public int? ExpectionStatusFk { get; set; }
        [ForeignKey("ExpectionStatusFk")]
        public Valueset? ExpectionStatus { get; set; }


        //Stamps 
        
        public int createdby { get; set; }
        public DateTime createat { get; set; } = DateTime.Now;
        public int updatedby { get; set; }
        public DateTime updateat { get; set; }
    }
}
