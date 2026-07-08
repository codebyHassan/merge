using System.ComponentModel.DataAnnotations;

namespace AlignHR.Models
{
    public class AttendencePolicies
    {
        [Key]
        public int id { get; set; }


        public int LateGraceTime { get; set; }
        public int OtAfterMintues { get; set; }
        public int MaxOtPerDay { get; set; }
        public int WeeklyHourLimit { get; set; }
        public DateOnly EffictiveTime { get; set; }
        public bool IsFlexiable { get; set; }

        public int createdby { get; set; }
        public DateTime createat { get; set; } = DateTime.Now;
        public int updatedby { get; set; }
        public DateTime updateat { get; set; }

        

    }
}
