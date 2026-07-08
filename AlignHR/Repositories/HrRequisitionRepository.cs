using AlignHR.Data;
using AlignHR.Models;
using Microsoft.EntityFrameworkCore;

namespace AlignHR.Repositories
{
    public class HrRequisitionRepository : IHrRequisitionRepository
    {
        private readonly ApplicationDbContext _context;

        public HrRequisitionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<HrRequisition> Query()
        {
            return _context.HrRequisitions.AsQueryable();
        }

        public async Task<HrRequisition?> GetByIdAsync(decimal id)
        {
            return await _context.HrRequisitions.FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task AddAsync(HrRequisition requisition)
        {
            await _context.HrRequisitions.AddAsync(requisition);
        }

        public void Update(HrRequisition requisition)
        {
            _context.HrRequisitions.Update(requisition);
        }

        public async Task<bool> RequisitionNoExistsAsync(string requisitionNo)
        {
            return await _context.HrRequisitions.AnyAsync(r => r.RequisitionNo == requisitionNo);
        }

        public async Task<int> CountForYearAsync(int year)
        {
            var prefix = $"REQ-{year}-";
            return await _context.HrRequisitions.CountAsync(r => r.RequisitionNo != null && r.RequisitionNo.StartsWith(prefix));
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
