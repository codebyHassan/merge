using AlignHR.Data;
using AlignHR.Models;
using AlignHR.Repositories;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace AlignHR.Services
{
    public class HrRequisitionService : IHrRequisitionService
    {
        private readonly IHrRequisitionRepository _repository;
        private readonly ApplicationDbContext _context;
        private readonly IHrRequisitionWorkflowService _workflowService;

        public HrRequisitionService(
            IHrRequisitionRepository repository,
            ApplicationDbContext context,
            IHrRequisitionWorkflowService workflowService)
        {
            _repository = repository;
            _context = context;
            _workflowService = workflowService;
        }

        public async Task<IPagedList<HrRequisitionListItemViewModel>> GetPagedAsync(string? search, int pageNumber, int pageSize)
        {
            var query = _repository.Query();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(r =>
                    (r.RequisitionNo != null && r.RequisitionNo.Contains(search)) ||
                    (r.PositionTitle != null && r.PositionTitle.Contains(search)));
            }

            var requisitions = await query.OrderByDescending(r => r.CreatedOn).ToListAsync();
            var departmentIds = requisitions
                .Where(r => r.DepartmentFK.HasValue)
                .Select(r => (int)r.DepartmentFK!.Value)
                .Distinct()
                .ToList();
            var employeeIds = requisitions
                .Where(r => r.EmployeeFK.HasValue)
                .Select(r => (int)r.EmployeeFK!.Value)
                .Distinct()
                .ToList();

            var departments = await _context.department
                .Where(d => departmentIds.Contains(d.Id))
                .ToDictionaryAsync(d => d.Id, d => d.Name);
            var employees = await _context.emp
                .Where(e => employeeIds.Contains(e.Id))
                .ToDictionaryAsync(e => e.Id, e => e.Code + " - " + e.FirstName + " " + e.LastName);

            var items = requisitions.Select(requisition => new HrRequisitionListItemViewModel
            {
                Id = requisition.Id,
                RequisitionNo = requisition.RequisitionNo,
                PositionTitle = requisition.PositionTitle,
                DepartmentName = requisition.DepartmentFK.HasValue && departments.TryGetValue((int)requisition.DepartmentFK.Value, out var departmentName)
                    ? departmentName
                    : null,
                EmployeeName = requisition.EmployeeFK.HasValue && employees.TryGetValue((int)requisition.EmployeeFK.Value, out var employeeName)
                    ? employeeName
                    : null,
                Status = requisition.Status,
                CreatedOn = requisition.CreatedOn
            });

            return items.ToPagedList(pageNumber, pageSize);
        }

        public async Task<HrRequisitionFormViewModel?> GetForEditAsync(decimal id)
        {
            var requisition = await _context.HrRequisitions
                .Include(r => r.Skills)
                .Include(r => r.Offerings)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (requisition == null)
                return null;

            return MapToViewModel(requisition);
        }

        public async Task<decimal> CreateAsync(HrRequisitionFormViewModel model, string? username, decimal? actionBy, bool submit)
        {
            var requisition = new HrRequisition
            {
                RequisitionNo = await GenerateRequisitionNoAsync(),
                EmployeeFK = actionBy,
                DepartmentFK = model.DepartmentFK,
                InitialDate = model.InitialDate,
                PromisedDate = model.PromisedDate,
                RequisitionType = model.RequisitionType,
                Nature = model.Nature,
                PositionTitle = model.PositionTitle,
                BudgetAmountPerMonth = model.BudgetAmountPerMonth,
                ReplacementEmployeeId = model.ReplacementEmployeeId,
                TransferFromDepartmentId = model.TransferFromDepartmentId,
                TransferEmployeeId = model.TransferEmployeeId,
                Reason = model.Reason,
                Status = "Draft",
                CreatedBy = username,
                CreatedOn = DateTime.Now
            };

            await _repository.AddAsync(requisition);
            await _repository.SaveChangesAsync();

            SaveSkillsAndOfferings(requisition.Id, model);
            await _repository.SaveChangesAsync();

            if (submit)
            {
                await _workflowService.StartAsync(requisition, ResolveActionBy(actionBy), "Submitted requisition.");
                requisition.UpdatedBy = username;
                requisition.UpdatedOn = DateTime.Now;
                await _repository.SaveChangesAsync();
            }

            return requisition.Id;
        }

        public async Task UpdateAsync(decimal id, HrRequisitionFormViewModel model, string? username, decimal? actionBy, bool submit)
        {
            var requisition = await _context.HrRequisitions
                .Include(r => r.Skills)
                .Include(r => r.Offerings)
                .FirstOrDefaultAsync(r => r.Id == id)
                ?? throw new InvalidOperationException("Requisition not found.");

            if (requisition.Status == "Submitted")
                throw new InvalidOperationException("Submitted requisitions cannot be edited.");

            requisition.DepartmentFK = model.DepartmentFK;
            requisition.InitialDate = model.InitialDate;
            requisition.PromisedDate = model.PromisedDate;
            requisition.RequisitionType = model.RequisitionType;
            requisition.Nature = model.Nature;
            requisition.PositionTitle = model.PositionTitle;
            requisition.BudgetAmountPerMonth = model.BudgetAmountPerMonth;
            requisition.ReplacementEmployeeId = model.ReplacementEmployeeId;
            requisition.TransferFromDepartmentId = model.TransferFromDepartmentId;
            requisition.TransferEmployeeId = model.TransferEmployeeId;
            requisition.Reason = model.Reason;
            requisition.Status = "Draft";
            requisition.UpdatedBy = username;
            requisition.UpdatedOn = DateTime.Now;

            // Replace skills and offerings
            _context.HrRequisitionSkills.RemoveRange(requisition.Skills);
            _context.HrRequisitionOfferings.RemoveRange(requisition.Offerings);
            _repository.Update(requisition);
            await _repository.SaveChangesAsync();

            SaveSkillsAndOfferings(requisition.Id, model);
            await _repository.SaveChangesAsync();

            if (submit)
            {
                await _workflowService.StartAsync(requisition, ResolveActionBy(actionBy), "Submitted requisition.");
                await _repository.SaveChangesAsync();
            }
        }

        public async Task<HrRequisitionFormViewModel?> GetDetailsAsync(decimal id)
        {
            return await GetForEditAsync(id);
        }

        private void SaveSkillsAndOfferings(decimal requisitionId, HrRequisitionFormViewModel model)
        {
            foreach (var s in model.Skills.Where(s => !string.IsNullOrWhiteSpace(s.SkillName)))
            {
                _context.HrRequisitionSkills.Add(new HrRequisitionSkill
                {
                    RequisitionId = requisitionId,
                    SkillName = s.SkillName.Trim(),
                    YearsExperience = s.YearsExperience,
                    IsMandatory = s.IsMandatory
                });
            }

            foreach (var o in model.Offerings.Where(o => !string.IsNullOrWhiteSpace(o.OfferingName)))
            {
                _context.HrRequisitionOfferings.Add(new HrRequisitionOffering
                {
                    RequisitionId = requisitionId,
                    OfferingName = o.OfferingName.Trim(),
                    Description = o.Description?.Trim()
                });
            }
        }

        private static HrRequisitionFormViewModel MapToViewModel(HrRequisition r) => new()
        {
            Id = r.Id,
            RequisitionNo = r.RequisitionNo,
            EmployeeFK = r.EmployeeFK,
            DepartmentFK = r.DepartmentFK,
            InitialDate = r.InitialDate,
            PromisedDate = r.PromisedDate,
            RequisitionType = r.RequisitionType,
            Nature = r.Nature,
            PositionTitle = r.PositionTitle,
            BudgetAmountPerMonth = r.BudgetAmountPerMonth,
            ReplacementEmployeeId = r.ReplacementEmployeeId,
            TransferFromDepartmentId = r.TransferFromDepartmentId,
            TransferEmployeeId = r.TransferEmployeeId,
            Reason = r.Reason,
            Status = r.Status,
            Skills = r.Skills.Select(s => new HrRequisitionSkillViewModel
            {
                Id = s.Id,
                SkillName = s.SkillName,
                YearsExperience = s.YearsExperience,
                IsMandatory = s.IsMandatory
            }).ToList(),
            Offerings = r.Offerings.Select(o => new HrRequisitionOfferingViewModel
            {
                Id = o.Id,
                OfferingName = o.OfferingName,
                Description = o.Description
            }).ToList()
        };

        public async Task SubmitAsync(decimal id, string? username, decimal? actionBy)
        {
            var requisition = await _repository.GetByIdAsync(id);
            if (requisition == null)
                throw new InvalidOperationException("Requisition not found.");

            if (requisition.Status != "Draft")
                throw new InvalidOperationException("Only Draft requisitions can be submitted.");

            var resolvedActionBy = actionBy.HasValue && actionBy.Value > 0
                ? actionBy.Value
                : requisition.EmployeeFK.HasValue && requisition.EmployeeFK.Value > 0
                    ? requisition.EmployeeFK.Value
                    : throw new InvalidOperationException("A linked employee is required to submit a requisition into workflow.");

            // StartAsync sets requisition.Status = "Submitted" and requisition.WorkflowInstanceFK
            await _workflowService.StartAsync(requisition, resolvedActionBy, "Submitted requisition.");
            requisition.UpdatedBy = username;
            requisition.UpdatedOn = DateTime.Now;

            _repository.Update(requisition);
            await _repository.SaveChangesAsync();
        }

        private async Task<string> GenerateRequisitionNoAsync()
        {
            var year = DateTime.Now.Year;
            var sequence = await _repository.CountForYearAsync(year) + 1;
            var requisitionNo = $"REQ-{year}-{sequence:D4}";

            while (await _repository.RequisitionNoExistsAsync(requisitionNo))
            {
                sequence++;
                requisitionNo = $"REQ-{year}-{sequence:D4}";
            }

            return requisitionNo;
        }

        private static decimal ResolveActionBy(decimal? actionBy)
        {
            if (actionBy.HasValue && actionBy.Value > 0)
                return actionBy.Value;

            throw new InvalidOperationException("Your account is not linked to an employee record. Please contact HR.");
        }
    }
}
