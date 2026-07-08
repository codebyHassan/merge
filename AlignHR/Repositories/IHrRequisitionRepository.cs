using AlignHR.Models;

namespace AlignHR.Repositories
{
    public interface IHrRequisitionRepository
    {
        IQueryable<HrRequisition> Query();
        Task<HrRequisition?> GetByIdAsync(decimal id);
        Task AddAsync(HrRequisition requisition);
        void Update(HrRequisition requisition);
        Task<bool> RequisitionNoExistsAsync(string requisitionNo);
        Task<int> CountForYearAsync(int year);
        Task SaveChangesAsync();
    }
}
