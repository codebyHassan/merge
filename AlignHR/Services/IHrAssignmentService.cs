using AlignHR.Models;

namespace AlignHR.Services
{
    public interface IHrAssignmentService
    {
        Task<HrAssignmentFormViewModel?> GetForAssignAsync(decimal requisitionFk);
        Task<HrAssignmentDetailsViewModel?> GetByRequisitionAsync(decimal requisitionFk);
        Task AssignAsync(HrAssignmentFormViewModel model, string? username);
    }
}
