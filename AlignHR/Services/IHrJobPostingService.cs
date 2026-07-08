using AlignHR.Models;
using X.PagedList;

namespace AlignHR.Services
{
    public interface IHrJobPostingService
    {
        Task<IPagedList<HrJobPostingListItemViewModel>> GetPagedAsync(decimal? requisitionFk, string? search, int pageNumber, int pageSize, decimal? assignedToEmployeeId = null);
        Task<HrJobPostingFormViewModel?> GetForCreateAsync(decimal requisitionFk);
        Task<HrJobPostingFormViewModel?> GetForEditAsync(decimal id);
        Task<HrJobPostingDetailsViewModel?> GetDetailsAsync(decimal id);
        Task<decimal> CreateAsync(HrJobPostingFormViewModel model, string? username);
        Task UpdateAsync(decimal id, HrJobPostingFormViewModel model, string? username);
        Task ChangeStatusAsync(decimal id, string newStatus, string? username);
        Task DeleteAsync(decimal id, string? username);
        Task<List<HrPendingPostingRequisitionViewModel>> GetPendingRequisitionsAsync(decimal employeeId);
    }
}
