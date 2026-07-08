using AlignHR.Data;
using AlignHR.Models;
using Microsoft.EntityFrameworkCore;

namespace AlignHR.Repositories
{
    public class HrInterviewRepository : IHrInterviewRepository
    {
        private readonly ApplicationDbContext _context;

        public HrInterviewRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Rounds
        public async Task<List<HrInterviewRound>> GetRoundsAsync(decimal jobPostingId)
            => await _context.HrInterviewRounds
                .Where(r => r.JobPostingId == jobPostingId && r.IsActive)
                .OrderBy(r => r.RoundOrder)
                .ToListAsync();

        public async Task<HrInterviewRound?> GetRoundByIdAsync(decimal id)
            => await _context.HrInterviewRounds.FindAsync(id);

        public async Task AddRoundAsync(HrInterviewRound round)
            => await _context.HrInterviewRounds.AddAsync(round);

        public void RemoveRound(HrInterviewRound round)
        {
            round.IsActive = false;
            _context.HrInterviewRounds.Update(round);
        }

        // Criteria
        public async Task<List<HrEvaluationCriteria>> GetCriteriaAsync(decimal roundId)
            => await _context.HrEvaluationCriterias
                .Where(c => c.InterviewRoundId == roundId)
                .OrderBy(c => c.CriteriaName)
                .ToListAsync();

        public async Task<HrEvaluationCriteria?> GetCriteriaByIdAsync(decimal id)
            => await _context.HrEvaluationCriterias.FindAsync(id);

        public async Task AddCriteriaAsync(HrEvaluationCriteria criteria)
            => await _context.HrEvaluationCriterias.AddAsync(criteria);

        public void RemoveCriteria(HrEvaluationCriteria criteria)
            => _context.HrEvaluationCriterias.Remove(criteria);

        // Schedule
        public async Task<List<HrInterviewSchedule>> GetSchedulesByApplicationAsync(decimal applicationId)
            => await _context.HrInterviewSchedules
                .Include(s => s.InterviewRound)
                .Include(s => s.Panel)
                .Include(s => s.Feedback)
                .Where(s => s.JobApplicationId == applicationId)
                .OrderBy(s => s.InterviewRound!.RoundOrder)
                .ThenBy(s => s.ScheduledDateTime)
                .ToListAsync();

        public async Task<HrInterviewSchedule?> GetScheduleByIdAsync(decimal id)
            => await _context.HrInterviewSchedules
                .Include(s => s.InterviewRound)
                .Include(s => s.JobApplication)
                .FirstOrDefaultAsync(s => s.Id == id);

        public async Task<HrInterviewSchedule?> GetScheduleWithDetailsAsync(decimal id)
            => await _context.HrInterviewSchedules
                .Include(s => s.InterviewRound)
                .Include(s => s.JobApplication)
                    .ThenInclude(a => a!.Candidate)
                .Include(s => s.JobApplication)
                    .ThenInclude(a => a!.JobPosting)
                .Include(s => s.Panel)
                .Include(s => s.Feedback)
                    .ThenInclude(f => f.Scores)
                        .ThenInclude(sc => sc.EvaluationCriteria)
                .FirstOrDefaultAsync(s => s.Id == id);

        public async Task AddScheduleAsync(HrInterviewSchedule schedule)
            => await _context.HrInterviewSchedules.AddAsync(schedule);

        public void UpdateSchedule(HrInterviewSchedule schedule)
            => _context.HrInterviewSchedules.Update(schedule);

        // Panel
        public async Task<HrInterviewPanel?> GetPanelEntryByIdAsync(decimal id)
            => await _context.HrInterviewPanels.FindAsync(id);

        public async Task<bool> PanelMemberExistsAsync(decimal scheduleId, decimal employeeId)
            => await _context.HrInterviewPanels.AnyAsync(p =>
                p.InterviewScheduleId == scheduleId && p.EmployeeFk == employeeId);

        public async Task AddPanelEntryAsync(HrInterviewPanel panel)
            => await _context.HrInterviewPanels.AddAsync(panel);

        public void RemovePanelEntry(HrInterviewPanel panel)
            => _context.HrInterviewPanels.Remove(panel);

        // Feedback
        public async Task<HrInterviewFeedback?> GetFeedbackAsync(decimal scheduleId, decimal interviewerEmpFk)
            => await _context.HrInterviewFeedbacks
                .Include(f => f.Scores)
                    .ThenInclude(s => s.EvaluationCriteria)
                .FirstOrDefaultAsync(f =>
                    f.InterviewScheduleFk == scheduleId &&
                    f.InterviewerEmployeeFk == interviewerEmpFk);

        public async Task<HrInterviewFeedback?> GetFeedbackByScheduleAsync(decimal scheduleId)
            => await _context.HrInterviewFeedbacks
                .Include(f => f.Scores)
                    .ThenInclude(s => s.EvaluationCriteria)
                .FirstOrDefaultAsync(f => f.InterviewScheduleFk == scheduleId);

        public async Task<HrInterviewFeedback?> GetFeedbackByIdAsync(decimal id)
            => await _context.HrInterviewFeedbacks
                .Include(f => f.Scores)
                    .ThenInclude(s => s.EvaluationCriteria)
                .FirstOrDefaultAsync(f => f.Id == id);

        public async Task AddFeedbackAsync(HrInterviewFeedback feedback)
            => await _context.HrInterviewFeedbacks.AddAsync(feedback);

        public async Task AddScoreAsync(HrEvaluationScore score)
            => await _context.HrEvaluationScores.AddAsync(score);

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}
