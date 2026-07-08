using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
namespace AlignHR.Models
{
    public enum GroupName 
    {
        Morning, 
        Evening, 
        Night 
    }
    public class Shift: IValidatableObject
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Shift name is required")]
        [StringLength(100, ErrorMessage = "Shift name cannot exceed 100 characters")]
        public string? ShiftName { get; set; }

        public GroupName? GroupName { get; set; }

        [Required(ErrorMessage = "Start time is required")]
        public TimeOnly? StartTime { get; set; }

        [Required(ErrorMessage = "End time is required")]
        public TimeOnly? EndTime { get; set; }

        [Required(ErrorMessage = "Grace time is required")]
        public TimeOnly? GraceTime { get; set; }

        public TimeOnly? BreakStartTime { get; set; }

        public TimeOnly? BreakEndTime { get; set; }

        // This looks like a flag but kept as-is (no rename as requested)
        public bool? IsNightShift { get; set; }

        [Required]
        public int createdby { get; set; }

        public DateTime createat { get; set; } = DateTime.Now;

        public int updatedby { get; set; }

        public DateTime updateat { get; set; }

       public ICollection<AttendenceLogs> AttendenceLogs { get; set; } = new List<AttendenceLogs>();



        // 🔥 Custom Validation Logic
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartTime.HasValue && EndTime.HasValue)
            {
                if (StartTime >= EndTime && IsNightShift != true)
                {
                    yield return new ValidationResult(
                        "End time must be greater than Start time for non-night shifts",
                        new[] { nameof(EndTime) });
                }
            }

            if (BreakStartTime.HasValue && BreakEndTime.HasValue)
            {
                if (BreakStartTime >= BreakEndTime)
                {
                    yield return new ValidationResult(
                        "Break end time must be greater than Break start time",
                        new[] { nameof(BreakEndTime) });
                }
            }

            if (BreakStartTime.HasValue && StartTime.HasValue && BreakStartTime < StartTime)
            {
                yield return new ValidationResult(
                    "Break start time must be within shift time",
                    new[] { nameof(BreakStartTime) });
            }

            if (BreakEndTime.HasValue && EndTime.HasValue && BreakEndTime > EndTime)
            {
                yield return new ValidationResult(
                    "Break end time must be within shift time",
                    new[] { nameof(BreakEndTime) });
            }
        }
    }
}