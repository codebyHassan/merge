using AlignHR.Data;
using AlignHR.Models;
using Microsoft.EntityFrameworkCore;

namespace AlignHR.Repositories
{
    public class HrOfferRepository : IHrOfferRepository
    {
        private readonly ApplicationDbContext _context;

        public HrOfferRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<HrOffer> Query()
            => _context.HrOffers.Include(o => o.JobApplication).AsQueryable();

        public async Task<HrOffer?> GetByIdAsync(decimal id)
            => await _context.HrOffers.FirstOrDefaultAsync(o => o.Id == id);

        public async Task<HrOffer?> GetWithDetailsAsync(decimal id)
            => await _context.HrOffers
                .Include(o => o.JobApplication)
                    .ThenInclude(a => a!.Candidate)
                .Include(o => o.JobApplication)
                    .ThenInclude(a => a!.JobPosting)
                .Include(o => o.Versions.OrderBy(v => v.VersionNo))
                .Include(o => o.Approvals.OrderBy(a => a.ApprovalLevel))
                .Include(o => o.Responses.OrderBy(r => r.ResponseDate))
                .Include(o => o.Documents)
                .FirstOrDefaultAsync(o => o.Id == id);

        public async Task<HrOffer?> GetByApplicationAsync(decimal applicationFk)
            => await _context.HrOffers.FirstOrDefaultAsync(o => o.JobApplicationFk == applicationFk);

        public async Task<int> CountForYearAsync(int year)
        {
            var prefix = $"OFF-{year}-";
            return await _context.HrOffers.CountAsync(o => o.OfferNumber != null && o.OfferNumber.StartsWith(prefix));
        }

        public async Task<bool> OfferNumberExistsAsync(string offerNumber)
            => await _context.HrOffers.AnyAsync(o => o.OfferNumber == offerNumber);

        public async Task AddOfferAsync(HrOffer offer)
            => await _context.HrOffers.AddAsync(offer);

        public void UpdateOffer(HrOffer offer)
            => _context.HrOffers.Update(offer);

        public async Task AddVersionAsync(HrOfferVersion version)
            => await _context.HrOfferVersions.AddAsync(version);

        public async Task<int> GetNextVersionNoAsync(decimal offerFk)
        {
            var max = await _context.HrOfferVersions
                .Where(v => v.OfferFk == offerFk)
                .MaxAsync(v => (int?)v.VersionNo);
            return (max ?? 0) + 1;
        }

        public async Task AddApprovalAsync(HrOfferApproval approval)
            => await _context.HrOfferApprovals.AddAsync(approval);

        public async Task<HrOfferApproval?> GetApprovalByIdAsync(decimal id)
            => await _context.HrOfferApprovals.FindAsync(id);

        public void UpdateApproval(HrOfferApproval approval)
            => _context.HrOfferApprovals.Update(approval);

        public async Task AddDocumentAsync(HrOfferDocument document)
            => await _context.HrOfferDocuments.AddAsync(document);

        public async Task<HrOfferDocument?> GetDocumentByIdAsync(decimal id)
            => await _context.HrOfferDocuments.FindAsync(id);

        public void RemoveDocument(HrOfferDocument document)
            => _context.HrOfferDocuments.Remove(document);

        public void RemoveApproval(HrOfferApproval approval)
            => _context.HrOfferApprovals.Remove(approval);

        public async Task AddResponseAsync(HrOfferResponse response)
            => await _context.HrOfferResponses.AddAsync(response);

        public async Task AddHiringDecisionAsync(HrHiringDecision decision)
            => await _context.HrHiringDecisions.AddAsync(decision);

        public async Task<HrHiringDecision?> GetHiringDecisionAsync(decimal applicationFk)
            => await _context.HrHiringDecisions.FirstOrDefaultAsync(h => h.JobApplicationFk == applicationFk);

        public void UpdateHiringDecision(HrHiringDecision decision)
            => _context.HrHiringDecisions.Update(decision);

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}
