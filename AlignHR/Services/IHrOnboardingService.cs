using AlignHR.Models;
using Microsoft.AspNetCore.Http;
using X.PagedList;

namespace AlignHR.Services
{
    public interface IHrOnboardingService
    {
        Task<IPagedList<HrOnboardingListItemViewModel>> GetPagedAsync(string? search, int pageNumber, int pageSize);
        Task<HrOnboardingDetailsViewModel?> GetDetailsAsync(decimal id);
        Task<HrJoiningConfirmationFormViewModel?> GetForJoiningAsync(decimal onboardingId);

        Task<decimal> StartOnboardingAsync(decimal offerId, string? username);
        Task UpdateTaskStatusAsync(HrTaskUpdateViewModel model);
        Task UploadDocumentAsync(decimal onboardingId, string documentType, IFormFile file);
        Task DeleteDocumentAsync(decimal documentId);
        Task ConfirmJoiningAsync(HrJoiningConfirmationFormViewModel model, decimal confirmedByEmpId);
    }
}
