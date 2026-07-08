using AlignHR.Data;
using AlignHR.Models;
using Microsoft.EntityFrameworkCore;

namespace AlignHR.Repositories
{
    public class HrApplicationRepository : IHrApplicationRepository
    {
        private readonly ApplicationDbContext _context;

        public HrApplicationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<HrJobApplication> Query()
            => _context.HrJobApplications
                .Include(a => a.Candidate)
                .Include(a => a.CurrentStage)
                .AsQueryable();

        public async Task<HrJobApplication?> GetByIdAsync(decimal id)
            => await _context.HrJobApplications
                .Include(a => a.Candidate)
                .Include(a => a.CurrentStage)
                .FirstOrDefaultAsync(a => a.Id == id);

        public async Task<HrJobApplication?> GetWithDetailsAsync(decimal id)
            => await _context.HrJobApplications
                .Include(a => a.Candidate)
                    .ThenInclude(c => c!.Documents)
                .Include(a => a.JobPosting)
                .Include(a => a.CurrentStage)
                .Include(a => a.StageHistory.OrderBy(h => h.ChangedDate))
                    .ThenInclude(h => h.FromStage)
                .Include(a => a.StageHistory)
                    .ThenInclude(h => h.ToStage)
                .FirstOrDefaultAsync(a => a.Id == id);

        public async Task<bool> ActiveApplicationExistsAsync(decimal candidateFk, decimal jobPostingFk)
            => await _context.HrJobApplications.AnyAsync(a =>
                a.CandidateFK == candidateFk &&
                a.JobPostingFK == jobPostingFk &&
                a.IsActive);

        public async Task<List<HrApplicationStage>> GetActiveStagesAsync()
            => await _context.HrApplicationStages
                .Where(s => s.IsActive)
                .OrderBy(s => s.StageOrder)
                .ToListAsync();

        public async Task<HrApplicationStage?> GetStageByNameAsync(string name)
            => await _context.HrApplicationStages
                .FirstOrDefaultAsync(s => s.StageName == name && s.IsActive);

        public async Task AddAsync(HrJobApplication application)
            => await _context.HrJobApplications.AddAsync(application);

        public void Update(HrJobApplication application)
            => _context.HrJobApplications.Update(application);

        public async Task AddHistoryAsync(HrApplicationStageHistory history)
            => await _context.HrApplicationStageHistories.AddAsync(history);

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}
