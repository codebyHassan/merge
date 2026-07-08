using System.ComponentModel.DataAnnotations;

namespace AlignHR.Models
{
    // ── Rounds ──────────────────────────────────────────────────
    public class HrInterviewRoundFormViewModel
    {
        public decimal Id { get; set; }
        public decimal JobPostingId { get; set; }
        public string? JobCode { get; set; }
        public string? JobTitle { get; set; }

        [Required(ErrorMessage = "Round name is required.")]
        [StringLength(100)]
        [Display(Name = "Round Name")]
        public string RoundName { get; set; } = string.Empty;

        [Range(1, 99)]
        [Display(Name = "Order")]
        public int RoundOrder { get; set; } = 1;

        [Display(Name = "Mandatory")]
        public bool IsMandatory { get; set; } = true;
    }

    public class HrInterviewRoundItemViewModel
    {
        public decimal Id { get; set; }
        public string? RoundName { get; set; }
        public int RoundOrder { get; set; }
        public bool IsMandatory { get; set; }
        public bool IsActive { get; set; }
        public int ScheduleCount { get; set; }
    }

    // ── Evaluation Criteria ──────────────────────────────────────
    public class HrEvaluationCriteriaFormViewModel
    {
        public decimal Id { get; set; }
        public decimal JobPostingId { get; set; }

        [Required(ErrorMessage = "Criteria name is required.")]
        [StringLength(200)]
        [Display(Name = "Criteria")]
        public string CriteriaName { get; set; } = string.Empty;

        [Required]
        [Range(1, 100)]
        [Display(Name = "Max Score")]
        public decimal MaxScore { get; set; } = 10;
    }

    public class HrEvaluationCriteriaItemViewModel
    {
        public decimal Id { get; set; }
        public string? CriteriaName { get; set; }
        public decimal MaxScore { get; set; }
    }

    // ── Schedule ─────────────────────────────────────────────────
    public class HrInterviewEditViewModel
    {
        public decimal Id { get; set; }
        public decimal ApplicationId { get; set; }
        public decimal? JobPostingId { get; set; }
        public string? CandidateName { get; set; }
        public string? JobTitle { get; set; }

        [Required(ErrorMessage = "Please select a round.")]
        [Display(Name = "Interview Round")]
        public decimal? InterviewRoundId { get; set; }

        [Required(ErrorMessage = "Scheduled date/time is required.")]
        [Display(Name = "Scheduled Date & Time")]
        public DateTime ScheduledDateTime { get; set; }

        [StringLength(1000)]
        [Display(Name = "Meeting Link")]
        public string? MeetingLink { get; set; }

        [StringLength(500)]
        public string? Location { get; set; }

        public List<decimal> PanelEmployeeIds { get; set; } = new();
    }

    public class HrScheduleFormViewModel
    {
        public decimal Id { get; set; }

        public decimal ApplicationId { get; set; }
        public decimal? JobPostingId { get; set; }
        public string? CandidateName { get; set; }
        public string? JobTitle { get; set; }

        [Required(ErrorMessage = "Please select a round.")]
        [Display(Name = "Interview Round")]
        public decimal? InterviewRoundId { get; set; }

        [Required(ErrorMessage = "Scheduled date/time is required.")]
        [Display(Name = "Scheduled Date & Time")]
        public DateTime ScheduledDateTime { get; set; } = DateTime.Now.AddDays(1);

        [StringLength(1000)]
        [Display(Name = "Meeting Link")]
        public string? MeetingLink { get; set; }

        [StringLength(500)]
        public string? Location { get; set; }

        public List<decimal> PanelEmployeeIds { get; set; } = new();
    }

    public class HrInterviewListItemViewModel
    {
        public decimal Id { get; set; }
        public string? RoundName { get; set; }
        public int RoundOrder { get; set; }
        public DateTime ScheduledDateTime { get; set; }
        public string? Status { get; set; }
        public int PanelCount { get; set; }
        public int FeedbackCount { get; set; }
        public bool HasSubmittedFeedback { get; set; }
        public decimal? AverageScore { get; set; }
        public bool IsCurrentUserOnPanel { get; set; }
    }

    public class HrApplicationInterviewsViewModel
    {
        public decimal ApplicationId { get; set; }
        public string? CandidateName { get; set; }
        public string? JobCode { get; set; }
        public string? JobTitle { get; set; }
        public decimal? JobPostingId { get; set; }
        public bool IsRecruiter { get; set; }
        public List<HrInterviewListItemViewModel> Interviews { get; set; } = new();
    }

    public class HrInterviewDetailsViewModel
    {
        public decimal Id { get; set; }
        public decimal ApplicationId { get; set; }
        public string? CandidateName { get; set; }
        public string? JobCode { get; set; }
        public string? JobTitle { get; set; }
        public decimal? JobPostingId { get; set; }
        public string? RoundName { get; set; }
        public int RoundOrder { get; set; }
        public DateTime ScheduledDateTime { get; set; }
        public string? MeetingLink { get; set; }
        public string? Location { get; set; }
        public string? Status { get; set; }
        public decimal? AverageScore { get; set; }
        public List<HrPanelMemberViewModel> Panel { get; set; } = new();
        public List<HrFeedbackSummaryViewModel> Feedback { get; set; } = new();
        public List<HrEvaluationCriteriaItemViewModel> Criteria { get; set; } = new();
    }

    public class HrPanelMemberViewModel
    {
        public decimal PanelId { get; set; }
        public decimal EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public bool HasSubmittedFeedback { get; set; }
        public decimal? FeedbackId { get; set; }
        public decimal? OverallScore { get; set; }
        public string? Recommendation { get; set; }
        public bool IsSubmitted { get; set; }
    }

    public class HrFeedbackSummaryViewModel
    {
        public decimal Id { get; set; }
        public string? InterviewerName { get; set; }
        public decimal? OverallScore { get; set; }
        public string? Recommendation { get; set; }
        public bool IsSubmitted { get; set; }
        public DateTime SubmittedDate { get; set; }
    }

    // ── Feedback / Scorecard ─────────────────────────────────────
    public class HrFeedbackFormViewModel
    {
        public decimal Id { get; set; }
        public decimal ScheduleId { get; set; }
        public decimal ApplicationId { get; set; }
        public string? CandidateName { get; set; }
        public string? RoundName { get; set; }
        public bool IsSubmitted { get; set; }

        [Required(ErrorMessage = "Recommendation is required.")]
        [Display(Name = "Recommendation")]
        public string Recommendation { get; set; } = string.Empty;

        [Display(Name = "Overall Score")]
        public decimal? OverallScore { get; set; }

        [Display(Name = "Strengths")]
        public string? Strengths { get; set; }

        [Display(Name = "Concerns")]
        public string? Concerns { get; set; }

        [Display(Name = "Additional Comments")]
        public string? Comments { get; set; }

        public List<HrCriteriaScoreInputViewModel> CriteriaScores { get; set; } = new();
    }

    public class HrCriteriaScoreInputViewModel
    {
        public decimal CriteriaId { get; set; }
        public string? CriteriaName { get; set; }
        public decimal MaxScore { get; set; }
        public decimal Score { get; set; }
    }

    public class HrFeedbackDetailsViewModel : HrFeedbackFormViewModel
    {
        public string? InterviewerName { get; set; }
        public DateTime SubmittedDate { get; set; }
        public decimal? TotalScore { get; set; }
        public decimal? MaxTotalScore { get; set; }
    }

    public class HrLMFeedbackViewModel
    {
        // Requisition context (for breadcrumb / back navigation)
        public decimal RequisitionId { get; set; }
        public string? PositionTitle { get; set; }

        // Interview context
        public decimal ScheduleId { get; set; }
        public decimal ApplicationId { get; set; }
        public decimal? JobPostingId { get; set; }
        public string? CandidateName { get; set; }
        public string? RoundName { get; set; }
        public DateTime ScheduledDateTime { get; set; }
        public string? InterviewStatus { get; set; }
        public List<string> PanelMembers { get; set; } = new();

        // Feedback form
        public decimal FeedbackId { get; set; }
        public bool IsSubmitted { get; set; }

        [Required(ErrorMessage = "Recommendation is required.")]
        public string? Recommendation { get; set; }

        [Range(0, 100)]
        public decimal? OverallScore { get; set; }
        public string? Strengths { get; set; }
        public string? Concerns { get; set; }
        public string? Comments { get; set; }
        public List<HrCriteriaScoreInputViewModel> CriteriaScores { get; set; } = new();
    }
}
