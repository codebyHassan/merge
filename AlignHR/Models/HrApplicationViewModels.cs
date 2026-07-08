using System.ComponentModel.DataAnnotations;

namespace AlignHR.Models
{
    public class HrApplicationFormViewModel
    {
        public decimal Id { get; set; }

        [Required(ErrorMessage = "Please select a candidate.")]
        [Display(Name = "Candidate")]
        public decimal? CandidateFK { get; set; }

        public decimal? JobPostingFK { get; set; }
        public string? JobCode { get; set; }
        public string? JobTitle { get; set; }

        [Display(Name = "Applied Date")]
        [DataType(DataType.Date)]
        public DateTime AppliedDate { get; set; } = DateTime.Today;

        [Display(Name = "Recruiter Notes")]
        public string? RecruiterNotes { get; set; }
    }

    public class HrApplicationListItemViewModel
    {
        public decimal Id { get; set; }
        public decimal? CandidateFK { get; set; }
        public string? CandidateName { get; set; }
        public string? CandidateEmail { get; set; }
        public string? CandidatePhone { get; set; }
        public decimal? TotalExperienceYears { get; set; }
        public string? CurrentStage { get; set; }
        public decimal? CurrentStageFK { get; set; }
        public decimal? JobPostingFK { get; set; }
        public string? JobCode { get; set; }
        public string? JobTitle { get; set; }
        public DateTime? AppliedDate { get; set; }
        public bool IsActive { get; set; }
    }

    public class HrApplicationDetailsViewModel
    {
        public decimal Id { get; set; }
        public decimal? CandidateFK { get; set; }
        public string? CandidateName { get; set; }
        public string? CandidateEmail { get; set; }
        public string? CandidatePhone { get; set; }
        public decimal? TotalExperienceYears { get; set; }
        public string? CurrentEmployer { get; set; }
        public decimal? JobPostingFK { get; set; }
        public string? JobCode { get; set; }
        public string? JobTitle { get; set; }
        public decimal? CurrentStageFK { get; set; }
        public string? CurrentStage { get; set; }
        public DateTime? AppliedDate { get; set; }
        public string? RecruiterNotes { get; set; }
        public bool IsActive { get; set; }
        public bool HasInterviewRounds { get; set; }
        public bool AllRoundsScheduled { get; set; }
        public bool AllFeedbackSubmitted { get; set; }
        public List<HrInterviewListItemViewModel> Interviews { get; set; } = new();
        public List<HrApplicationStageHistoryItemViewModel> History { get; set; } = new();
        public List<HrCandidateDocumentViewModel> CandidateDocuments { get; set; } = new();
        public List<HrApplicationOfferSummaryViewModel> Offers { get; set; } = new();
        public HrApplicationOnboardingSummaryViewModel? Onboarding { get; set; }
    }

    public class HrApplicationOfferSummaryViewModel
    {
        public decimal Id { get; set; }
        public string? OfferNumber { get; set; }
        public decimal ProposedSalary { get; set; }
        public DateTime? CandidateJoiningDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? Status { get; set; }
        public int TotalApprovals { get; set; }
        public int ApprovedCount { get; set; }
        public string? LastResponseType { get; set; }
        public string? Decision { get; set; }
        public List<HrApplicationOfferApprovalSummary> Approvals { get; set; } = new();
    }

    public class HrApplicationOfferApprovalSummary
    {
        public string? ApproverName { get; set; }
        public string? Status { get; set; }
        public string? Comments { get; set; }
        public DateTime? ActionDate { get; set; }
    }

    public class HrApplicationOnboardingSummaryViewModel
    {
        public decimal Id { get; set; }
        public string? Status { get; set; }
        public DateTime PlannedJoiningDate { get; set; }
        public DateTime? ActualJoiningDate { get; set; }
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public List<HrOnboardingTaskItemViewModel> Tasks { get; set; } = new();
    }

    public class HrApplicationStageHistoryItemViewModel
    {
        public string? FromStage { get; set; }
        public string? ToStage { get; set; }
        public string? ChangedByName { get; set; }
        public string? Comments { get; set; }
        public DateTime? ChangedDate { get; set; }
    }

    public class HrMoveStageViewModel
    {
        public decimal ApplicationId { get; set; }
        public string? CandidateName { get; set; }
        public string? CurrentStage { get; set; }
        public decimal? CurrentStageFK { get; set; }

        [Required(ErrorMessage = "Please select a stage.")]
        [Display(Name = "Move To Stage")]
        public decimal? ToStageFK { get; set; }

        [StringLength(1000)]
        public string? Comments { get; set; }
    }

    public class HrAtsDashboardViewModel
    {
        public int OpenJobPostings { get; set; }
        public int ActiveApplications { get; set; }
        public int CandidatesInPipeline { get; set; }
        public List<HrStageCountViewModel> ApplicationsByStage { get; set; } = new();
        public List<HrApplicationListItemViewModel> RecentApplications { get; set; } = new();
        public List<HrJobPostingListItemViewModel> ActivePostings { get; set; } = new();
    }

    public class HrStageCountViewModel
    {
        public string? StageName { get; set; }
        public int Count { get; set; }
    }

    public class HrPipelineViewModel
    {
        public decimal JobPostingId { get; set; }
        public string? JobCode { get; set; }
        public string? JobTitle { get; set; }
        public List<HrPipelineStageViewModel> Stages { get; set; } = new();
    }

    public class HrPipelineStageViewModel
    {
        public decimal StageId { get; set; }
        public string? StageName { get; set; }
        public int Order { get; set; }
        public List<HrApplicationListItemViewModel> Applications { get; set; } = new();
    }
}
