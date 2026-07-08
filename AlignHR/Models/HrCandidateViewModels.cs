using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace AlignHR.Models
{
    public class HrCandidateFormViewModel
    {
        public decimal Id { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(100)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        [StringLength(200)]
        public string Email { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Phone { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(50)]
        public string? Gender { get; set; }

        [StringLength(200)]
        [Display(Name = "Current Location")]
        public string? CurrentLocation { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(100)]
        public string? Country { get; set; }

        [StringLength(100)]
        [Display(Name = "Highest Degree")]
        public string? HighestDegree { get; set; }

        [StringLength(200)]
        [Display(Name = "Field of Study")]
        public string? FieldOfStudy { get; set; }

        [StringLength(200)]
        public string? University { get; set; }

        [Range(0, 50)]
        [Display(Name = "Total Experience (Years)")]
        public decimal? TotalExperienceYears { get; set; }

        [Display(Name = "Currently Employed")]
        public bool? IsCurrentlyEmployed { get; set; }

        [StringLength(200)]
        [Display(Name = "Current Employer")]
        public string? CurrentEmployer { get; set; }

        [StringLength(200)]
        [Display(Name = "Current Designation")]
        public string? CurrentDesignation { get; set; }

        [StringLength(50)]
        [Display(Name = "Notice Period")]
        public string? NoticePeriod { get; set; }

        [StringLength(500)]
        [Display(Name = "LinkedIn Profile")]
        public string? LinkedInProfile { get; set; }

        [Display(Name = "Resume / Document")]
        public IFormFile? DocumentFile { get; set; }

        [Display(Name = "Document Type")]
        public string DocumentType { get; set; } = "Resume";
    }

    public class HrCandidateListItemViewModel
    {
        public decimal Id { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public decimal? TotalExperienceYears { get; set; }
        public string? CurrentEmployer { get; set; }
        public string? CurrentDesignation { get; set; }
        public int ApplicationCount { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    public class HrCandidateDetailsViewModel
    {
        public decimal Id { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? CurrentLocation { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? HighestDegree { get; set; }
        public string? FieldOfStudy { get; set; }
        public string? University { get; set; }
        public decimal? TotalExperienceYears { get; set; }
        public bool? IsCurrentlyEmployed { get; set; }
        public string? CurrentEmployer { get; set; }
        public string? CurrentDesignation { get; set; }
        public string? NoticePeriod { get; set; }
        public string? LinkedInProfile { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public List<HrCandidateDocumentViewModel> Documents { get; set; } = new();
        public List<HrCandidateApplicationSummaryViewModel> Applications { get; set; } = new();
    }

    public class HrCandidateDocumentViewModel
    {
        public decimal Id { get; set; }
        public string? DocumentType { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public DateTime? UploadedDate { get; set; }
    }

    public class HrCandidateApplicationSummaryViewModel
    {
        public decimal ApplicationId { get; set; }
        public string? JobCode { get; set; }
        public string? JobTitle { get; set; }
        public string? CurrentStage { get; set; }
        public DateTime? AppliedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
