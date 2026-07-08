using AlignHR.Models;

namespace AlignHR.Repositories
{
    public interface IHrJobPostingRepository
    {
        IQueryable<HrJobPosting> Query();
        Task<HrJobPosting?> GetByIdAsync(decimal id);
        Task<HrJobPosting?> GetWithChannelsAsync(decimal id);
        Task<bool> JobCodeExistsAsync(string jobCode);
        Task<int> CountForYearAsync(int year);
        Task<List<HrPostingChannel>> GetActiveChannelsAsync();
        Task AddAsync(HrJobPosting posting);
        void Update(HrJobPosting posting);
        Task SaveChangesAsync();
    }
}
