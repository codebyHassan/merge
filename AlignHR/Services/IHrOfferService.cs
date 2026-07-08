using AlignHR.Models;
using Microsoft.AspNetCore.Http;
using X.PagedList;

namespace AlignHR.Services
{
    public interface IHrOfferService
    {
        Task<IPagedList<HrOfferListItemViewModel>> GetPagedAsync(string? search, int pageNumber, int pageSize);
        Task<HrOfferFormViewModel?> GetForCreateAsync(decimal applicationFk);
        Task<HrOfferFormViewModel?> GetForEditAsync(decimal offerId);
        Task<HrOfferDetailsViewModel?> GetDetailsAsync(decimal offerId);
        Task<HrOfferVersionFormViewModel?> GetForRevisionAsync(decimal offerId);
        Task<HrApprovalActionViewModel?> GetApprovalActionAsync(decimal approvalId);
        Task<HrHiringDecisionFormViewModel?> GetForHiringDecisionAsync(decimal applicationFk);

        Task<decimal> CreateAsync(HrOfferFormViewModel model, string? username, decimal? createdByEmpId);
        Task UpdateAsync(HrOfferFormViewModel model, string? username, decimal? empId);
        Task AddVersionAsync(HrOfferVersionFormViewModel model, decimal? empId);
        Task SubmitForApprovalAsync(decimal offerId, string? username);
        Task AddApproverAsync(HrAddApproverViewModel model, string? username);
        Task RemoveApproverAsync(decimal approvalId);
        Task ProcessApprovalAsync(HrApprovalActionViewModel model, decimal? empId);
        Task SendOfferAsync(decimal offerId, string? username);
        Task UploadDocumentAsync(decimal offerId, IFormFile file, string? username);
        Task DeleteDocumentAsync(decimal documentId);
        Task CancelOfferAsync(decimal offerId, string? username);
        Task WithdrawOfferAsync(decimal offerId, string? username);
        Task RecordHiringDecisionAsync(HrHiringDecisionFormViewModel model, decimal? empId);
        Task<List<HrOfferApprovalInboxItemViewModel>> GetMyApprovalsAsync(int employeeId);
        Task<int> GetMyApprovalCountAsync(int employeeId);
    }
}
