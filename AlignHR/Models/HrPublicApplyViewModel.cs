using System.ComponentModel.DataAnnotations;

namespace AlignHR.Models
{
    public class HrJobPortalItemViewModel
    {
        public decimal Id { get; set; }
        public string? JobCode { get; set; }
        public string? JobTitle { get; set; }
        public string? DepartmentName { get; set; }
        public string? EmploymentType { get; set; }
        public string? Location { get; set; }
        public string? Nature { get; set; }
        public string? JobDescription { get; set; }
        public DateTime OpenDate { get; set; }
        public DateTime? CloseDate { get; set; }
    }

    public class HrPublicApplyViewModel
    {
        public decimal JobPostingId { get; set; }
        public string? JobTitle { get; set; }
        public string? JobCode { get; set; }
        public string? DepartmentName { get; set; }
        public string? EmploymentType { get; set; }
        public string? Location { get; set; }
        public string? Nature { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(100)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(100)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [StringLength(200)]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Contact number is required.")]
        [StringLength(50)]
        [Display(Name = "Contact Number")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required.")]
        [StringLength(100)]
        [Display(Name = "Current City")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "Country is required.")]
        [StringLength(100)]
        [Display(Name = "Country")]
        public string Country { get; set; } = "Pakistan";

        [Required(ErrorMessage = "Date of birth is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        [StringLength(50)]
        [Display(Name = "Gender")]
        public string Gender { get; set; } = string.Empty;

        [Required(ErrorMessage = "Highest degree is required.")]
        [StringLength(100)]
        [Display(Name = "Highest Degree")]
        public string HighestDegree { get; set; } = string.Empty;

        [Required(ErrorMessage = "Field of study is required.")]
        [StringLength(200)]
        [Display(Name = "Field of Study / Specialization")]
        public string FieldOfStudy { get; set; } = string.Empty;

        [Required(ErrorMessage = "University/Institution is required.")]
        [StringLength(200)]
        [Display(Name = "University / Institution")]
        public string University { get; set; } = string.Empty;

        [Required(ErrorMessage = "Experience is required.")]
        [StringLength(50)]
        [Display(Name = "Total Years of Experience")]
        public string ExperienceRange { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please indicate employment status.")]
        [Display(Name = "Currently Employed?")]
        public bool? IsCurrentlyEmployed { get; set; }

        [StringLength(200)]
        [Display(Name = "Current Organization")]
        public string? CurrentEmployer { get; set; }

        [StringLength(200)]
        [Display(Name = "Current Designation")]
        public string? CurrentDesignation { get; set; }

        [StringLength(50)]
        [Display(Name = "Notice Period")]
        public string? NoticePeriod { get; set; }

        [Display(Name = "Resume / CV")]
        public IFormFile? ResumeFile { get; set; }
    }
}
