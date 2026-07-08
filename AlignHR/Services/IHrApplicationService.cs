using AlignHR.Models;
using X.PagedList;

namespace AlignHR.Services
{
    public interface IHrApplicationService
    {
        Task<IPagedList<HrApplicationListItemViewModel>> GetByJobPostingAsync(decimal jobPostingId, string? search, int pageNumber, int pageSize);
        Task<HrApplicationDetailsViewModel?> GetDetailsAsync(decimal id);
        Task<HrPipelineViewModel?> GetPipelineAsync(decimal jobPostingId);
        Task<HrAtsDashboardViewModel> GetDashboardAsync();
        Task<HrMoveStageViewModel?> GetForMoveStageAsync(decimal applicationId);
        Task<decimal> ApplyAsync(HrApplicationFormViewModel model, decimal? actionByEmployeeId);
        Task MoveStageAsync(decimal applicationId, decimal toStageId, string? comments, decimal? changedBy);
        Task ShortlistAsync(decimal applicationId, decimal? changedBy);
        Task RejectAsync(decimal applicationId, string? reason, decimal? changedBy);
        Task WithdrawAsync(decimal applicationId, decimal? changedBy);
        Task UpdateNotesAsync(decimal applicationId, string? notes);
        Task DeactivateAsync(decimal applicationId);
    }
}
