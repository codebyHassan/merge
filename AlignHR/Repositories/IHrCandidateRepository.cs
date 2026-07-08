using AlignHR.Models;

namespace AlignHR.Repositories
{
    public interface IHrCandidateRepository
    {
        IQueryable<HrCandidate> Query();
        Task<HrCandidate?> GetByIdAsync(decimal id);
        Task<HrCandidate?> GetWithDetailsAsync(decimal id);
        Task<bool> EmailExistsAsync(string email, decimal? excludeId = null);
        Task AddAsync(HrCandidate candidate);
        void Update(HrCandidate candidate);
        Task AddDocumentAsync(HrCandidateDocument document);
        Task<HrCandidateDocument?> GetDocumentAsync(decimal id);
        void RemoveDocument(HrCandidateDocument document);
        Task SaveChangesAsync();
    }
}
