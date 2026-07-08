using AlignHR.Models;
using Microsoft.AspNetCore.Http;
using X.PagedList;

namespace AlignHR.Services
{
    public interface IHrCandidateService
    {
        Task<IPagedList<HrCandidateListItemViewModel>> GetPagedAsync(string? search, int pageNumber, int pageSize);
        Task<HrCandidateFormViewModel?> GetForEditAsync(decimal id);
        Task<HrCandidateDetailsViewModel?> GetDetailsAsync(decimal id);
        Task<decimal> CreateAsync(HrCandidateFormViewModel model);
        Task UpdateAsync(decimal id, HrCandidateFormViewModel model);
        Task DeleteAsync(decimal id);
        Task UploadDocumentAsync(decimal candidateId, IFormFile file, string documentType);
        Task DeleteDocumentAsync(decimal documentId);
    }
}
