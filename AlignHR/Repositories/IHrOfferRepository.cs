using AlignHR.Models;

namespace AlignHR.Repositories
{
    public interface IHrOfferRepository
    {
        IQueryable<HrOffer> Query();
        Task<HrOffer?> GetByIdAsync(decimal id);
        Task<HrOffer?> GetWithDetailsAsync(decimal id);
        Task<HrOffer?> GetByApplicationAsync(decimal applicationFk);
        Task<int> CountForYearAsync(int year);
        Task<bool> OfferNumberExistsAsync(string offerNumber);
        Task AddOfferAsync(HrOffer offer);
        void UpdateOffer(HrOffer offer);
        Task AddVersionAsync(HrOfferVersion version);
        Task<int> GetNextVersionNoAsync(decimal offerFk);
        Task AddApprovalAsync(HrOfferApproval approval);
        Task<HrOfferApproval?> GetApprovalByIdAsync(decimal id);
        void UpdateApproval(HrOfferApproval approval);
        void RemoveApproval(HrOfferApproval approval);
        Task AddDocumentAsync(HrOfferDocument document);
        Task<HrOfferDocument?> GetDocumentByIdAsync(decimal id);
        void RemoveDocument(HrOfferDocument document);
        Task AddHiringDecisionAsync(HrHiringDecision decision);
        Task<HrHiringDecision?> GetHiringDecisionAsync(decimal applicationFk);
        void UpdateHiringDecision(HrHiringDecision decision);
        Task SaveChangesAsync();
    }
}
