using AlignHR.Models;
using X.PagedList;

namespace AlignHR.Services
{
    public interface IHrOnboardingService
    {
        Task<IPagedList<HrOnboardingListItemViewModel>> GetPagedAsync(string? search, int pageNumber, int pageSize);
        Task<HrOnboardingDetailsViewModel?> GetDetailsAsync(decimal id);
        Task<decimal> StartOnboardingAsync(decimal offerId, string? username);
        Task UpdateTaskStatusAsync(HrTaskUpdateViewModel model);
        Task OnEmployeeCreatedAsync(decimal onboardingId, DateTime joiningDate);
    }
}
