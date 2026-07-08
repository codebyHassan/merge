using AlignHR.Models;

namespace AlignHR.Repositories
{
    public interface IHrJobPostingRepository
    {
        IQueryable<HrJobPosting> Query();
        Task<HrJobPosting?> GetByIdAsync(decimal id);
        Task<bool> JobCodeExistsAsync(string jobCode);
        Task<int> CountForYearAsync(int year);
        Task AddAsync(HrJobPosting posting);
        void Update(HrJobPosting posting);
        Task SaveChangesAsync();
    }
}
