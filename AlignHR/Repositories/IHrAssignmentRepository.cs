using AlignHR.Models;

namespace AlignHR.Repositories
{
    public interface IHrAssignmentRepository
    {
        Task<HrRequisitionAssignment?> GetByIdAsync(decimal id);
        Task<HrRequisitionAssignment?> GetByRequisitionAsync(decimal requisitionFk);
        Task AddAsync(HrRequisitionAssignment assignment);
        void Update(HrRequisitionAssignment assignment);
        Task SaveChangesAsync();
    }
}
