using System.ComponentModel.DataAnnotations;

namespace AlignHR.Models
{
    public class HrOnboardingListItemViewModel
    {
        public decimal Id { get; set; }
        public string? CandidateName { get; set; }
        public string? JobTitle { get; set; }
        public string? OfferNumber { get; set; }
        public DateTime PlannedJoiningDate { get; set; }
        public DateTime? ActualJoiningDate { get; set; }
        public string? Status { get; set; }
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public DateTime? CreatedOn { get; set; }
    }

    public class HrOnboardingDetailsViewModel
    {
        public decimal Id { get; set; }
        public string? CandidateName { get; set; }
        public string? CandidateEmail { get; set; }
        public string? JobTitle { get; set; }
        public string? OfferNumber { get; set; }
        public decimal? OfferId { get; set; }
        public DateTime PlannedJoiningDate { get; set; }
        public DateTime? ActualJoiningDate { get; set; }
        public string? Status { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public List<HrOnboardingTaskItemViewModel> Tasks { get; set; } = new();
    }

    public class HrOnboardingTaskItemViewModel
    {
        public decimal Id { get; set; }
        public string? TaskName { get; set; }
        public string? ResponsibleDepartment { get; set; }
        public bool IsMandatory { get; set; }
        public string? AssignedToName { get; set; }
        public DateTime? DueDate { get; set; }
        public string? Status { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string? Remarks { get; set; }
    }

    public class HrTaskUpdateViewModel
    {
        public decimal TaskId { get; set; }
        public decimal OnboardingId { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        public string Status { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Remarks { get; set; }
    }

    public class HrTaskTemplateFormViewModel
    {
        public decimal Id { get; set; }

        [Required(ErrorMessage = "Task name is required.")]
        [StringLength(200)]
        [Display(Name = "Task Name")]
        public string TaskName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Responsible department is required.")]
        [StringLength(100)]
        [Display(Name = "Responsible Department")]
        public string ResponsibleDepartment { get; set; } = string.Empty;

        [Display(Name = "Mandatory")]
        public bool IsMandatory { get; set; } = true;

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;
    }

    public class HrTaskTemplateListItemViewModel
    {
        public decimal Id { get; set; }
        public string? TaskName { get; set; }
        public string? ResponsibleDepartment { get; set; }
        public bool IsMandatory { get; set; }
        public bool IsActive { get; set; }
    }
}
