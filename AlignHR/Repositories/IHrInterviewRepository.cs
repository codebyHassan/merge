using AlignHR.Models;

namespace AlignHR.Repositories
{
    public interface IHrInterviewRepository
    {
        // Rounds
        Task<List<HrInterviewRound>> GetRoundsAsync(decimal jobPostingId);
        Task<HrInterviewRound?> GetRoundByIdAsync(decimal id);
        Task AddRoundAsync(HrInterviewRound round);
        void RemoveRound(HrInterviewRound round);

        // Criteria
        Task<List<HrEvaluationCriteria>> GetCriteriaAsync(decimal roundId);
        Task<HrEvaluationCriteria?> GetCriteriaByIdAsync(decimal id);
        Task AddCriteriaAsync(HrEvaluationCriteria criteria);
        void RemoveCriteria(HrEvaluationCriteria criteria);

        // Schedule
        Task<List<HrInterviewSchedule>> GetSchedulesByApplicationAsync(decimal applicationId);
        Task<HrInterviewSchedule?> GetScheduleByIdAsync(decimal id);
        Task<HrInterviewSchedule?> GetScheduleWithDetailsAsync(decimal id);
        Task AddScheduleAsync(HrInterviewSchedule schedule);
        void UpdateSchedule(HrInterviewSchedule schedule);

        // Panel
        Task<HrInterviewPanel?> GetPanelEntryByIdAsync(decimal id);
        Task<bool> PanelMemberExistsAsync(decimal scheduleId, decimal employeeId);
        Task AddPanelEntryAsync(HrInterviewPanel panel);
        void RemovePanelEntry(HrInterviewPanel panel);

        // Feedback
        Task<HrInterviewFeedback?> GetFeedbackAsync(decimal scheduleId, decimal interviewerEmpFk);
        Task<HrInterviewFeedback?> GetFeedbackByScheduleAsync(decimal scheduleId);
        Task<HrInterviewFeedback?> GetFeedbackByIdAsync(decimal id);
        Task AddFeedbackAsync(HrInterviewFeedback feedback);
        Task AddScoreAsync(HrEvaluationScore score);

        Task SaveChangesAsync();
    }
}
