using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class Overtime
    {
        [Key]
        public int OvertimeId { get; set; }

        [Required]
        [StringLength(100)]
        public string PolicyName { get; set; }

        [Required]
        [StringLength(20)]
        public string DayType { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal MinHours { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal MaxHours { get; set; }

        [Required]
        [StringLength(20)]
        public string RateType { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal RateValue { get; set; }

        [Required]
        [StringLength(20)]
        public string CalculationBase { get; set; }

        public int AfterShiftMinutes { get; set; }

        [Required]
        [StringLength(20)]
        public string RoundType { get; set; }

        public int RoundValue { get; set; }

        public bool IsTaxable { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }


        public DateOnly EffectiveFrom { get; set; }
        public DateOnly? EffectiveTo { get; set; }

        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
