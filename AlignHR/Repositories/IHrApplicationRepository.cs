using AlignHR.Models;

namespace AlignHR.Repositories
{
    public interface IHrApplicationRepository
    {
        IQueryable<HrJobApplication> Query();
        Task<HrJobApplication?> GetByIdAsync(decimal id);
        Task<HrJobApplication?> GetWithDetailsAsync(decimal id);
        Task<bool> ActiveApplicationExistsAsync(decimal candidateFk, decimal jobPostingFk);
        Task<List<HrApplicationStage>> GetActiveStagesAsync();
        Task<HrApplicationStage?> GetStageByNameAsync(string name);
        Task AddAsync(HrJobApplication application);
        void Update(HrJobApplication application);
        Task AddHistoryAsync(HrApplicationStageHistory history);
        Task SaveChangesAsync();
    }
}
