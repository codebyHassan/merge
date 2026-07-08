using AlignHR.Data;
using AlignHR.Models;
using AlignHR.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AlignHR.Services
{
    public class HrRequisitionWorkflowService : IHrRequisitionWorkflowService
    {
        private const string WorkflowName = "Recruitment Requisition Approval";
        private const string EntityType = "Requisition";

        private readonly ApplicationDbContext _context;
        private readonly IHrWorkflowRepository _workflowRepository;

        public HrRequisitionWorkflowService(ApplicationDbContext context, IHrWorkflowRepository workflowRepository)
        {
            _context = context;
            _workflowRepository = workflowRepository;
        }

        public async Task<decimal> StartAsync(HrRequisition requisition, decimal actionBy, string? comments)
        {
            var existing = await _workflowRepository.GetInstanceByEntityAsync(EntityType, requisition.Id);
            if (existing != null)
                return existing.Id;

            if (!requisition.DepartmentFK.HasValue)
                throw new InvalidOperationException("A department is required before submitting a requisition.");

            var definition = await _workflowRepository.GetActiveDefinitionAsync(WorkflowName);
            if (definition == null)
                throw new InvalidOperationException("Active recruitment requisition workflow definition was not found.");

            var steps = await _workflowRepository.GetStepsAsync(definition.Id);
            var firstStep = steps.FirstOrDefault();
            if (firstStep == null)
                throw new InvalidOperationException("Recruitment requisition workflow has no configured steps.");

            var approvers = await ResolveApproversAsync(requisition.DepartmentFK.Value, steps);

            var instance = new HrWorkflowInstance
            {
                WorkflowDefinitionId = definition.Id,
                EntityType = EntityType,
                EntityId = requisition.Id,
                CurrentStepId = firstStep.Id,
                Status = "Pending",
                StartedDate = DateTime.Now
            };

            await _workflowRepository.AddInstanceAsync(instance);
            await _workflowRepository.SaveChangesAsync();

            HrWorkflowInstanceStep? submittedStep = null;
            foreach (var step in steps)
            {
                var instanceStep = new HrWorkflowInstanceStep
                {
                    WorkflowInstanceId = instance.Id,
                    WorkflowStepId = step.Id,
                    StepOrder = step.StepOrder,
                    ApproverType = step.ApproverType,
                    EmployeeId = approvers[step.ApproverType],
                    Status = step.Id == firstStep.Id ? "Pending" : "Waiting",
                    AssignedDate = DateTime.Now
                };

                if (step.Id == firstStep.Id)
                    submittedStep = instanceStep;

                await _workflowRepository.AddInstanceStepAsync(instanceStep);
            }

            await _workflowRepository.SaveChangesAsync();

            requisition.WorkflowInstanceFK = instance.Id;
            requisition.Status = "Submitted";

            if (submittedStep != null)
            {
                await _workflowRepository.AddActionAsync(new HrWorkflowAction
                {
                    WorkflowInstanceStepId = submittedStep.Id,
                    ActionByEmployeeId = actionBy,
                    ActionType = "Submitted",
                    Comments = comments,
                    ActionDate = DateTime.Now
                });
            }

            await _workflowRepository.SaveChangesAsync();
            return instance.Id;
        }

        public async Task<List<HrWorkflowInboxItemViewModel>> GetInboxAsync(decimal employeeId)
        {
            var pendingSteps = await _workflowRepository.QueryInstanceSteps()
                .Where(s => s.EmployeeId == employeeId
                    && s.Status == "Pending"
                    && s.WorkflowInstance != null
                    && s.WorkflowInstance.EntityType == EntityType
                    && s.WorkflowInstance.Status == "Pending")
                .OrderBy(s => s.AssignedDate)
                .ToListAsync();

            if (!pendingSteps.Any())
                return new List<HrWorkflowInboxItemViewModel>();

            var requisitionIds = pendingSteps
                .Where(s => s.WorkflowInstance != null)
                .Select(s => s.WorkflowInstance!.EntityId)
                .Distinct()
                .ToList();

            var requisitions = await _context.HrRequisitions
                .Where(r => requisitionIds.Contains(r.Id))
                .ToListAsync();

            var departmentIds = requisitions.Where(r => r.DepartmentFK.HasValue).Select(r => (int)r.DepartmentFK!.Value).Distinct().ToList();
            var employeeIds = requisitions.Where(r => r.EmployeeFK.HasValue).Select(r => (int)r.EmployeeFK!.Value).Distinct().ToList();

            var departments = await _context.department
                .Where(d => departmentIds.Contains(d.Id))
                .ToDictionaryAsync(d => d.Id, d => d.Name);
            var employees = await _context.emp
                .Where(e => employeeIds.Contains(e.Id))
                .ToDictionaryAsync(e => e.Id, e => e.Code + " - " + e.FirstName + " " + e.LastName);

            return pendingSteps.Select(step =>
            {
                var instance = step.WorkflowInstance;
                var requisition = instance == null ? null : requisitions.FirstOrDefault(r => r.Id == instance.EntityId);
                return new HrWorkflowInboxItemViewModel
                {
                    WorkflowInstanceId = instance?.Id ?? 0,
                    RequisitionId = instance?.EntityId ?? 0,
                    RequisitionNo = requisition?.RequisitionNo,
                    PositionTitle = requisition?.PositionTitle,
                    DepartmentName = requisition?.DepartmentFK.HasValue == true && departments.TryGetValue((int)requisition.DepartmentFK.Value, out var departmentName)
                        ? departmentName
                        : null,
                    EmployeeName = requisition?.EmployeeFK.HasValue == true && employees.TryGetValue((int)requisition.EmployeeFK.Value, out var employeeName)
                        ? employeeName
                        : null,
                    StepName = step.WorkflowStep?.StepName,
                    ApproverType = step.ApproverType,
                    Status = step.Status,
                    StartedDate = instance?.StartedDate ?? step.AssignedDate
                };
            }).ToList();
        }

        public async Task<HrWorkflowActionViewModel?> GetActionViewAsync(decimal workflowInstanceId)
        {
            var instance = await _workflowRepository.GetInstanceAsync(workflowInstanceId);
            if (instance == null || instance.EntityType != EntityType)
                return null;

            var requisition = await _context.HrRequisitions
                .Include(r => r.Skills)
                .Include(r => r.Offerings)
                .FirstOrDefaultAsync(r => r.Id == instance.EntityId);
            if (requisition == null)
                return null;

            var departmentName = requisition.DepartmentFK.HasValue
                ? await _context.department.Where(d => d.Id == (int)requisition.DepartmentFK.Value).Select(d => d.Name).FirstOrDefaultAsync()
                : null;

            var employeeName = requisition.EmployeeFK.HasValue
                ? await _context.emp.Where(e => e.Id == (int)requisition.EmployeeFK.Value).Select(e => e.Code + " - " + e.FirstName + " " + e.LastName).FirstOrDefaultAsync()
                : null;

            var instanceSteps = await _workflowRepository.GetInstanceStepsAsync(workflowInstanceId);
            var currentInstanceStep = instanceSteps.FirstOrDefault(s => s.Status == "Pending");
            var actions = await _context.HrWorkflowActions
                .Include(a => a.WorkflowInstanceStep)
                    .ThenInclude(s => s!.WorkflowStep)
                .Where(a => a.WorkflowInstanceStep != null && a.WorkflowInstanceStep.WorkflowInstanceId == workflowInstanceId)
                .OrderBy(a => a.ActionDate)
                .ToListAsync();

            var actionByIds = actions.Select(a => (int)a.ActionByEmployeeId).Distinct().ToList();
            var assignedEmployeeIds = instanceSteps.Select(s => (int)s.EmployeeId).Distinct().ToList();
            var allEmployeeIds = actionByIds.Concat(assignedEmployeeIds).Distinct().ToList();
            var employees = await _context.emp
                .Where(e => allEmployeeIds.Contains(e.Id))
                .ToDictionaryAsync(e => e.Id, e => e.Code + " - " + e.FirstName + " " + e.LastName);

            return new HrWorkflowActionViewModel
            {
                WorkflowInstanceId    = instance.Id,
                RequisitionId         = requisition.Id,
                RequisitionNo         = requisition.RequisitionNo,
                PositionTitle         = requisition.PositionTitle,
                DepartmentName        = departmentName,
                EmployeeName          = employeeName,
                RequisitionType       = requisition.RequisitionType,
                Nature                = requisition.Nature,
                InitialDate           = requisition.InitialDate,
                PromisedDate          = requisition.PromisedDate,
                BudgetAmountPerMonth  = requisition.BudgetAmountPerMonth,
                Reason                = requisition.Reason,
                CurrentStepName       = currentInstanceStep?.WorkflowStep?.StepName ?? instance.CurrentStep?.StepName,
                CurrentApproverType   = currentInstanceStep?.ApproverType ?? instance.CurrentStep?.ApproverType,
                CurrentApproverName   = currentInstanceStep != null && employees.TryGetValue((int)currentInstanceStep.EmployeeId, out var approverName)
                    ? approverName
                    : null,
                Status = instance.Status,
                Skills = requisition.Skills.Select(s => new HrRequisitionSkillViewModel
                {
                    Id              = s.Id,
                    SkillName       = s.SkillName,
                    YearsExperience = s.YearsExperience,
                    IsMandatory     = s.IsMandatory
                }).ToList(),
                Offerings = requisition.Offerings.Select(o => new HrRequisitionOfferingViewModel
                {
                    Id           = o.Id,
                    OfferingName = o.OfferingName,
                    Description  = o.Description
                }).ToList(),
                History = actions.Select(action => new HrWorkflowHistoryItemViewModel
                {
                    StepName    = action.WorkflowInstanceStep?.WorkflowStep?.StepName,
                    ApproverType = action.WorkflowInstanceStep?.ApproverType,
                    ActionByName = employees.TryGetValue((int)action.ActionByEmployeeId, out var actionByName)
                        ? actionByName
                        : action.ActionByEmployeeId.ToString("0"),
                    ActionType  = action.ActionType,
                    Comments    = action.Comments,
                    ActionDate  = action.ActionDate
                }).ToList()
            };
        }

        public async Task ApproveAsync(decimal workflowInstanceId, decimal actionBy, string? comments)
        {
            await MoveAsync(workflowInstanceId, actionBy, "Approved", comments);
        }

        public async Task RejectAsync(decimal workflowInstanceId, decimal actionBy, string? comments)
        {
            var instance = await GetPendingInstanceAsync(workflowInstanceId);
            var currentStep = await GetAssignedPendingStepAsync(instance.Id, actionBy);
            var requisition = await _context.HrRequisitions.FirstOrDefaultAsync(r => r.Id == instance.EntityId);

            currentStep.Status = "Rejected";
            currentStep.ActionDate = DateTime.Now;
            currentStep.Comments = comments;
            instance.Status = "Rejected";
            instance.CompletedDate = DateTime.Now;
            if (requisition != null)
                requisition.Status = "Rejected";

            await AddActionAsync(currentStep, actionBy, "Rejected", comments);
            await _workflowRepository.SaveChangesAsync();
        }

        public async Task SendBackAsync(decimal workflowInstanceId, decimal actionBy, string? comments)
        {
            var instance = await GetPendingInstanceAsync(workflowInstanceId);
            var currentStep = await GetAssignedPendingStepAsync(instance.Id, actionBy);
            var previousStep = await _context.HrWorkflowInstanceSteps
                .Include(s => s.WorkflowStep)
                .Where(s => s.WorkflowInstanceId == instance.Id && s.StepOrder < currentStep.StepOrder)
                .OrderByDescending(s => s.StepOrder)
                .FirstOrDefaultAsync();

            if (previousStep == null)
                throw new InvalidOperationException("This requisition is already at the first workflow step.");

            currentStep.Status = "Waiting";
            currentStep.ActionDate = DateTime.Now;
            currentStep.Comments = comments;
            previousStep.Status = "Pending";
            previousStep.ActionDate = null;
            previousStep.Comments = null;
            instance.CurrentStepId = previousStep.WorkflowStepId;

            await AddActionAsync(currentStep, actionBy, "SentBack", comments);
            await _workflowRepository.SaveChangesAsync();
        }

        private async Task MoveAsync(decimal workflowInstanceId, decimal actionBy, string actionType, string? comments)
        {
            var instance = await GetPendingInstanceAsync(workflowInstanceId);
            var currentStep = await GetAssignedPendingStepAsync(instance.Id, actionBy);
            var requisition = await _context.HrRequisitions.FirstOrDefaultAsync(r => r.Id == instance.EntityId);

            currentStep.Status = actionType;
            currentStep.ActionDate = DateTime.Now;
            currentStep.Comments = comments;
            await AddActionAsync(currentStep, actionBy, actionType, comments);

            var isFinalStep = currentStep.WorkflowStep?.IsFinalStep == true;
            var nextStep = await _context.HrWorkflowInstanceSteps
                .Include(s => s.WorkflowStep)
                .Where(s => s.WorkflowInstanceId == instance.Id && s.StepOrder > currentStep.StepOrder)
                .OrderBy(s => s.StepOrder)
                .FirstOrDefaultAsync();

            if (isFinalStep || nextStep == null)
            {
                instance.Status = "Completed";
                instance.CompletedDate = DateTime.Now;
                if (requisition != null)
                    requisition.Status = "Approved";
            }
            else
            {
                nextStep.Status = "Pending";
                nextStep.AssignedDate = DateTime.Now;
                instance.CurrentStepId = nextStep.WorkflowStepId;
            }

            await _workflowRepository.SaveChangesAsync();
        }

        private async Task<Dictionary<string, decimal>> ResolveApproversAsync(decimal departmentId, List<HrWorkflowStep> steps)
        {
            var approverTypes = steps.Select(s => s.ApproverType).Distinct().ToList();
            if (approverTypes.Any(string.IsNullOrWhiteSpace))
                throw new InvalidOperationException("Every workflow step must have an approver type configured.");

            // All approver types (HRBP, HOD, RecruitmentHead) are resolved from HrDepartmentApprovers
            var approvers = await _context.HrDepartmentApprovers
                .Where(a => a.DepartmentId == departmentId
                    && a.IsActive
                    && approverTypes.Contains(a.ApproverType))
                .GroupBy(a => a.ApproverType)
                .Select(g => g.OrderBy(a => a.Id).First())
                .ToDictionaryAsync(a => a.ApproverType, a => a.EmployeeId);

            var missingTypes = approverTypes
                .Where(type => !approvers.ContainsKey(type))
                .ToList();

            if (missingTypes.Any())
                throw new InvalidOperationException($"No active department approver is configured for: {string.Join(", ", missingTypes)}.");

            return approvers;
        }

        private async Task<HrWorkflowInstance> GetPendingInstanceAsync(decimal workflowInstanceId)
        {
            var instance = await _workflowRepository.GetInstanceAsync(workflowInstanceId);
            if (instance == null)
                throw new InvalidOperationException("Workflow instance was not found.");

            if (instance.Status != "Pending")
                throw new InvalidOperationException("Only pending workflow instances can be actioned.");

            return instance;
        }

        private async Task<HrWorkflowInstanceStep> GetAssignedPendingStepAsync(decimal workflowInstanceId, decimal actionBy)
        {
            var step = await _workflowRepository.GetPendingInstanceStepAsync(workflowInstanceId);
            if (step == null)
                throw new InvalidOperationException("Pending workflow step was not found.");

            if (step.EmployeeId != actionBy)
                throw new InvalidOperationException("This workflow step is assigned to another employee.");

            return step;
        }

        private async Task AddActionAsync(HrWorkflowInstanceStep step, decimal actionBy, string actionType, string? comments)
        {
            await _workflowRepository.AddActionAsync(new HrWorkflowAction
            {
                WorkflowInstanceStepId = step.Id,
                ActionByEmployeeId = actionBy,
                ActionType = actionType,
                Comments = comments,
                ActionDate = DateTime.Now
            });
        }
    }
}
