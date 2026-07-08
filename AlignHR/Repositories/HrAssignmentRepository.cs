using AlignHR.Data;
using AlignHR.Models;
using Microsoft.EntityFrameworkCore;

namespace AlignHR.Repositories
{
    public class HrAssignmentRepository : IHrAssignmentRepository
    {
        private readonly ApplicationDbContext _context;

        public HrAssignmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<HrRequisitionAssignment?> GetByIdAsync(decimal id)
            => await _context.HrRequisitionAssignments.FirstOrDefaultAsync(a => a.Id == id);

        public async Task<HrRequisitionAssignment?> GetByRequisitionAsync(decimal requisitionFk)
            => await _context.HrRequisitionAssignments.FirstOrDefaultAsync(a => a.RequisitionFk == requisitionFk);

        public async Task AddAsync(HrRequisitionAssignment assignment)
            => await _context.HrRequisitionAssignments.AddAsync(assignment);

        public void Update(HrRequisitionAssignment assignment)
            => _context.HrRequisitionAssignments.Update(assignment);

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}
