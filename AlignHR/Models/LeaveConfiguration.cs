using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    /// <summary>
    /// Defines which grouping logic is used for external approval routing.
    /// Only ONE bit field (IsDepartment / IsSubDepartment / IsGrade / IsDivision) can be true.
    /// </summary>
    public class LeaveConfiguration : IValidatableObject
    {
        [Key]
        [Column(TypeName = "numeric(18,0)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        /// <summary>Only one of these four can be true at a time.</summary>
        [Display(Name = "Department")]
        public bool IsDepartment { get; set; }

        [Display(Name = "Sub-Department")]
        public bool IsSubDepartment { get; set; }

        [Display(Name = "Grade")]
        public bool IsGrade { get; set; }

        [Display(Name = "Division")]
        public bool IsDivision { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Created")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // ===== Navigation =====
        public ICollection<LeaveConfigurationDetail> Details { get; set; } = new List<LeaveConfigurationDetail>();

        // ===== Helpers =====

        /// <summary>Returns the human-readable name of the selected grouping type.</summary>
        [NotMapped]
        public string GroupingType =>
            IsDepartment ? "Department" :
            IsSubDepartment ? "Sub-Department" :
            IsGrade ? "Grade" :
            IsDivision ? "Division" :
            "None";

        // ===== Validation =====
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            int count = (IsDepartment ? 1 : 0)
                      + (IsSubDepartment ? 1 : 0)
                      + (IsGrade ? 1 : 0)
                      + (IsDivision ? 1 : 0);

            if (count == 0)
            {
                yield return new ValidationResult(
                    "At least one grouping type must be selected.",
                    new[] { nameof(IsDepartment) });
            }
            else if (count > 1)
            {
                yield return new ValidationResult(
                    "Only ONE grouping type can be selected at a time.",
                    new[] { nameof(IsDepartment) });
            }
        }
    }
}
