using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace AlignHR.Models
{
    public enum PunchType
    {
        In, 
        Out 
    }

    public class AttendenceLogs
    {
        [Key]
        public int id { get; set; }

        [Required]
        public int EmployeeID { get; set; }
        [ForeignKey("EmployeeID")]
        public Employee? Employee { get; set; }

        [Required]
        public DateOnly AttendenceDate { get; set; }

        [Required]
        public DateTime PunchTime { get; set; }

        public PunchType? PunchType { get; set; }

        [Required]
        public string Source { get; set; } = string.Empty;

        public string? Devicesd { get; set; }

        public decimal latitude { get; set; }

        public decimal longitude { get; set; }

        // Forign key with Shifts 
        
        public int? shiftidFk { get; set; }
        [ForeignKey("shiftidFk")]
        public Shift? Shift { get; set; }

        public int createdby { get; set; }
        public DateTime createat { get; set; } = DateTime.Now;
        public int updatedby { get; set; }
        public DateTime updateat { get; set; }
    }
}
