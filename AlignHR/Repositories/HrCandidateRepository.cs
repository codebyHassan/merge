using AlignHR.Data;
using AlignHR.Models;
using Microsoft.EntityFrameworkCore;

namespace AlignHR.Repositories
{
    public class HrCandidateRepository : IHrCandidateRepository
    {
        private readonly ApplicationDbContext _context;

        public HrCandidateRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<HrCandidate> Query()
            => _context.HrCandidates.Where(c => !c.IsDeleted).AsQueryable();

        public async Task<HrCandidate?> GetByIdAsync(decimal id)
            => await _context.HrCandidates.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

        public async Task<HrCandidate?> GetWithDetailsAsync(decimal id)
            => await _context.HrCandidates
                .Include(c => c.Documents)
                .Include(c => c.Applications)
                    .ThenInclude(a => a.CurrentStage)
                .Include(c => c.Applications)
                    .ThenInclude(a => a.JobPosting)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

        public async Task<bool> EmailExistsAsync(string email, decimal? excludeId = null)
            => await _context.HrCandidates.AnyAsync(c =>
                c.Email == email && !c.IsDeleted &&
                (excludeId == null || c.Id != excludeId.Value));

        public async Task AddAsync(HrCandidate candidate)
            => await _context.HrCandidates.AddAsync(candidate);

        public void Update(HrCandidate candidate)
            => _context.HrCandidates.Update(candidate);

        public async Task AddDocumentAsync(HrCandidateDocument document)
            => await _context.HrCandidateDocuments.AddAsync(document);

        public async Task<HrCandidateDocument?> GetDocumentAsync(decimal id)
            => await _context.HrCandidateDocuments.FindAsync(id);

        public void RemoveDocument(HrCandidateDocument document)
            => _context.HrCandidateDocuments.Remove(document);

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}
