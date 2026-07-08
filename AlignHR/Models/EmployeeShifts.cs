using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class EmployeeShifts
    {

        [Key]
        public int id { get; set; }

        public DateOnly EffiectiveForm  { get; set; }
        public DateOnly? EffiectiveTo { get; set; }
        public DayOfWeek RestDay { get; set; }


        public int createdby { get; set; }
        public DateTime createat { get; set; } = DateTime.Now;
        public int updatedby { get; set; }
        public DateTime updateat { get; set; }


        // Employee FK
        public int EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public Employee? Employee { get; set; }


        // Shifts FK
        [ForeignKey("ShiftId")]
        public int ShiftId { get; set; }
        public virtual Shift? Shift { get; set; }

    }
}
