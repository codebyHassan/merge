using AlignHR.Data;
using AlignHR.Models;
using Microsoft.EntityFrameworkCore;

namespace AlignHR.Repositories
{
    public class HrJobPostingRepository : IHrJobPostingRepository
    {
        private readonly ApplicationDbContext _context;

        public HrJobPostingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<HrJobPosting> Query()
            => _context.HrJobPostings.Where(p => !p.IsDeleted).AsQueryable();

        public async Task<HrJobPosting?> GetByIdAsync(decimal id)
            => await _context.HrJobPostings.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

        public async Task<HrJobPosting?> GetWithChannelsAsync(decimal id)
            => await _context.HrJobPostings
                .Include(p => p.PostingChannels)
                    .ThenInclude(jpc => jpc.PostingChannel)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

        public async Task<bool> JobCodeExistsAsync(string jobCode)
            => await _context.HrJobPostings.AnyAsync(p => p.JobCode == jobCode && !p.IsDeleted);

        public async Task<int> CountForYearAsync(int year)
        {
            var prefix = $"JP-{year}-";
            return await _context.HrJobPostings.CountAsync(p => p.JobCode != null && p.JobCode.StartsWith(prefix));
        }

        public async Task<List<HrPostingChannel>> GetActiveChannelsAsync()
            => await _context.HrPostingChannels
                .Where(c => c.IsActive)
                .OrderBy(c => c.IsInternal ? 0 : 1)
                .ThenBy(c => c.ChannelName)
                .ToListAsync();

        public async Task AddAsync(HrJobPosting posting)
            => await _context.HrJobPostings.AddAsync(posting);

        public void Update(HrJobPosting posting)
            => _context.HrJobPostings.Update(posting);

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}
