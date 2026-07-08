using AlignHR.Models;
using X.PagedList;

namespace AlignHR.Services
{
    public interface IHrRequisitionService
    {
        Task<IPagedList<HrRequisitionListItemViewModel>> GetPagedAsync(string? search, int pageNumber, int pageSize);
        Task<HrRequisitionFormViewModel?> GetForEditAsync(decimal id);
        Task<decimal> CreateAsync(HrRequisitionFormViewModel model, string? username, decimal? actionBy, bool submit);
        Task UpdateAsync(decimal id, HrRequisitionFormViewModel model, string? username, decimal? actionBy, bool submit);
        Task SubmitAsync(decimal id, string? username, decimal? actionBy);
        Task<HrRequisitionFormViewModel?> GetDetailsAsync(decimal id);
    }
}
