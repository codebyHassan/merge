using AlignHR.Data;
using AlignHR.Models;
using AlignHR.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AlignHR.Services
{
    public class HrAssignmentService : IHrAssignmentService
    {
        private readonly IHrAssignmentRepository _repository;
        private readonly ApplicationDbContext _context;

        public HrAssignmentService(IHrAssignmentRepository repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<HrAssignmentFormViewModel?> GetForAssignAsync(decimal requisitionFk)
        {
            var requisition = await _context.HrRequisitions.FirstOrDefaultAsync(r => r.Id == requisitionFk);
            if (requisition == null) return null;

            if (requisition.Status != "Approved")
                throw new InvalidOperationException("Only approved requisitions can be assigned a recruiter.");

            var existing = await _repository.GetByRequisitionAsync(requisitionFk);
            if (existing != null)
                throw new InvalidOperationException("This requisition already has a recruiter assigned.");

            return new HrAssignmentFormViewModel
            {
                RequisitionFk = requisitionFk,
                RequisitionNo = requisition.RequisitionNo,
                PositionTitle = requisition.PositionTitle,
                AssignedDate = DateTime.Today
            };
        }

        public async Task<HrAssignmentDetailsViewModel?> GetByRequisitionAsync(decimal requisitionFk)
        {
            var assignment = await _repository.GetByRequisitionAsync(requisitionFk);
            if (assignment == null) return null;

            var recruiterName = assignment.RecruiterEmployeeFK.HasValue
                ? await _context.emp
                    .Where(e => e.Id == (int)assignment.RecruiterEmployeeFK.Value)
                    .Select(e => e.Code + " - " + e.FirstName + " " + e.LastName)
                    .FirstOrDefaultAsync()
                : null;

            var requisition = await _context.HrRequisitions.FirstOrDefaultAsync(r => r.Id == requisitionFk);

            return new HrAssignmentDetailsViewModel
            {
                Id = assignment.Id,
                RequisitionFk = requisitionFk,
                RequisitionNo = requisition?.RequisitionNo,
                PositionTitle = requisition?.PositionTitle,
                RecruiterName = recruiterName,
                AssignedDate = assignment.AssignedDate,
                Notes = assignment.Notes,
                CreatedBy = assignment.CreatedBy,
                CreatedOn = assignment.CreatedOn
            };
        }

        public async Task AssignAsync(HrAssignmentFormViewModel model, string? username)
        {
            var requisition = await _context.HrRequisitions.FirstOrDefaultAsync(r => r.Id == model.RequisitionFk);
            if (requisition == null)
                throw new InvalidOperationException("Requisition not found.");

            if (requisition.Status != "Approved")
                throw new InvalidOperationException("Only approved requisitions can be assigned a recruiter.");

            var existing = await _repository.GetByRequisitionAsync(model.RequisitionFk);
            if (existing != null)
                throw new InvalidOperationException("This requisition already has a recruiter assigned.");

            var assignment = new HrRequisitionAssignment
            {
                RequisitionFk = model.RequisitionFk,
                RecruiterEmployeeFK = model.RecruiterEmployeeFK,
                AssignedDate = model.AssignedDate ?? DateTime.Today,
                Notes = model.Notes,
                CreatedBy = username,
                CreatedOn = DateTime.Now
            };

            await _repository.AddAsync(assignment);
            await _repository.SaveChangesAsync();
        }
    }
}
