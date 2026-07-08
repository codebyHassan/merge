using System.ComponentModel.DataAnnotations;

namespace AlignHR.Models
{
    public class HrJobPostingCriteriaDraft
    {
        public decimal Id { get; set; }
        public string CriteriaName { get; set; } = string.Empty;
        public decimal MaxScore { get; set; } = 10;
    }

    public class HrJobPostingRoundDraft
    {
        public decimal Id { get; set; }
        public string RoundName { get; set; } = string.Empty;
        public int RoundOrder { get; set; } = 1;
        public bool IsMandatory { get; set; } = true;
        public List<HrJobPostingCriteriaDraft> Criteria { get; set; } = new();
    }

    public class HrJobPostingFormViewModel
    {
        public decimal Id { get; set; }

        public decimal? RequisitionFK { get; set; }
        public string? RequisitionNo { get; set; }
        public string? JobCode { get; set; }

        public List<HrJobPostingRoundDraft> Rounds { get; set; } = new();

        [Required(ErrorMessage = "Job title is required.")]
        [StringLength(200)]
        [Display(Name = "Job Title")]
        public string? JobTitle { get; set; }

        [StringLength(1000)]
        [Display(Name = "Job Description")]
        public string? JobDescription { get; set; }

        [StringLength(50)]
        [Display(Name = "Employment Type")]
        public string? EmploymentType { get; set; }

        [StringLength(200)]
        public string? Location { get; set; }

        public string PostingStatus { get; set; } = "Draft";

        [Required(ErrorMessage = "Open date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Open Date")]
        public DateTime OpenDate { get; set; } = DateTime.Today;

        [DataType(DataType.Date)]
        [Display(Name = "Close Date")]
        public DateTime? CloseDate { get; set; }
    }

    public class HrJobPostingListItemViewModel
    {
        public decimal Id { get; set; }
        public decimal? RequisitionFK { get; set; }
        public string? RequisitionNo { get; set; }
        public string? JobCode { get; set; }
        public string? JobTitle { get; set; }
        public string? PostingStatus { get; set; }
        public DateTime OpenDate { get; set; }
        public DateTime? CloseDate { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? AssignedRecruiterName { get; set; }
        public bool AssignedToMe { get; set; }
        public int ApplicantCount { get; set; }
        public int RoundCount { get; set; }
    }

    public class HrJobPostingDetailsViewModel
    {
        public decimal Id { get; set; }
        public decimal? RequisitionFK { get; set; }
        public string? RequisitionNo { get; set; }
        public string? PositionTitle { get; set; }
        public string? JobCode { get; set; }
        public string? JobTitle { get; set; }
        public string? JobDescription { get; set; }
        public string? EmploymentType { get; set; }
        public string? Location { get; set; }
        public string? PostingStatus { get; set; }
        public DateTime OpenDate { get; set; }
        public DateTime? CloseDate { get; set; }

        // Requisition fields
        public string? DepartmentName { get; set; }
        public string? RequisitionType { get; set; }
        public string? Nature { get; set; }
        public decimal? BudgetAmountPerMonth { get; set; }
        public DateTime? InitialDate { get; set; }
        public DateTime? PromisedDate { get; set; }
        public string? Reason { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public List<HrRequisitionSkillViewModel> Skills { get; set; } = new();
        public List<HrRequisitionOfferingViewModel> Offerings { get; set; } = new();
        public List<HrJobPostingAssignmentViewModel> Assignments { get; set; } = new();
    }

    public class HrJobPostingAssignmentViewModel
    {
        public string? RecruiterName { get; set; }
        public string? RecruiterCode { get; set; }
        public DateTime? AssignedDate { get; set; }
        public string? Notes { get; set; }
    }

    public class HrPendingPostingRequisitionViewModel
    {
        public decimal RequisitionId { get; set; }
        public string? RequisitionNo { get; set; }
        public string? PositionTitle { get; set; }
        public string? DepartmentName { get; set; }
        public DateTime? AssignedDate { get; set; }
    }
}
