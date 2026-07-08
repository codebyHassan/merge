using System.ComponentModel.DataAnnotations;

namespace AlignHR.Models
{
    public class HrRequisitionSkillViewModel
    {
        public decimal Id { get; set; }

        [Required(ErrorMessage = "Skill name is required.")]
        [StringLength(200)]
        [Display(Name = "Skill")]
        public string SkillName { get; set; } = string.Empty;

        [Display(Name = "Years Exp.")]
        public decimal? YearsExperience { get; set; }

        [Display(Name = "Mandatory")]
        public bool IsMandatory { get; set; } = true;
    }

    public class HrRequisitionOfferingViewModel
    {
        public decimal Id { get; set; }

        [Required(ErrorMessage = "Offering name is required.")]
        [StringLength(200)]
        [Display(Name = "Offering")]
        public string OfferingName { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Description")]
        public string? Description { get; set; }
    }

    public class HrRequisitionFormViewModel
    {
        public decimal Id { get; set; }

        [Display(Name = "Requisition No")]
        public string? RequisitionNo { get; set; }

        [Display(Name = "Requesting Employee")]
        public decimal? EmployeeFK { get; set; }

        [Required(ErrorMessage = "Department is required.")]
        [Display(Name = "Department")]
        public decimal? DepartmentFK { get; set; }

        [Display(Name = "Initial Date")]
        [DataType(DataType.Date)]
        public DateTime? InitialDate { get; set; }

        [Display(Name = "Promised Date")]
        [DataType(DataType.Date)]
        public DateTime? PromisedDate { get; set; }

        [Display(Name = "Requisition Type")]
        public string? RequisitionType { get; set; }

        [Display(Name = "Nature")]
        public string? Nature { get; set; }

        [Required(ErrorMessage = "Position title is required.")]
        [StringLength(200)]
        [Display(Name = "Position Title")]
        public string? PositionTitle { get; set; }

        [Display(Name = "Budget / Month")]
        public decimal? BudgetAmountPerMonth { get; set; }

        [Display(Name = "Replacement Employee")]
        public decimal? ReplacementEmployeeId { get; set; }

        [Display(Name = "Transfer From")]
        public decimal? TransferFromDepartmentId { get; set; }

        [StringLength(1000)]
        public string? Reason { get; set; }

        public string Status { get; set; } = "Draft";

        public List<HrRequisitionSkillViewModel> Skills { get; set; } = new();
        public List<HrRequisitionOfferingViewModel> Offerings { get; set; } = new();
    }

    public class HrMyInterviewItemViewModel
    {
        public decimal ScheduleId { get; set; }
        public decimal RequisitionId { get; set; }
        public string? PositionTitle { get; set; }
        public string? DepartmentName { get; set; }
        public string? CandidateName { get; set; }
        public string? RoundName { get; set; }
        public int RoundOrder { get; set; }
        public DateTime ScheduledDateTime { get; set; }
        public string? InterviewStatus { get; set; }
        public bool HasFeedback { get; set; }
        public bool IsSubmitted { get; set; }
        public decimal? OverallScore { get; set; }
        public bool IsOnPanel { get; set; }
    }

    public class HrLMInterviewItemViewModel
    {
        public decimal ScheduleId { get; set; }
        public string? RoundName { get; set; }
        public int RoundOrder { get; set; }
        public DateTime ScheduledDateTime { get; set; }
        public string? Status { get; set; }
        public bool HasFeedback { get; set; }
        public decimal? FeedbackId { get; set; }
    }

    public class HrLMCandidateViewModel
    {
        public decimal ApplicationId { get; set; }
        public string? CandidateName { get; set; }
        public string? ApplicationStatus { get; set; }
        public List<HrLMInterviewItemViewModel> Interviews { get; set; } = new();
    }

    public class HrLineManagerFeedbackViewModel
    {
        public decimal RequisitionId { get; set; }
        public string? RequisitionNo { get; set; }
        public string? PositionTitle { get; set; }
        public string? DepartmentName { get; set; }
        public string? InitiatedByName { get; set; }
        public string? RequisitionType { get; set; }
        public string? Nature { get; set; }
        public string? Reason { get; set; }
        public string? Status { get; set; }
        public List<HrLMCandidateViewModel> Candidates { get; set; } = new();
    }

    public class HrRequisitionListItemViewModel
    {
        public decimal Id { get; set; }
        public string? RequisitionNo { get; set; }
        public string? PositionTitle { get; set; }
        public string? DepartmentName { get; set; }
        public string? EmployeeName { get; set; }
        public string? RequisitionType { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
