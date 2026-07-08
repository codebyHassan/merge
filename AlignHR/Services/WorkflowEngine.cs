using AlignHR.Data;
using AlignHR.Models;
using Microsoft.EntityFrameworkCore;

namespace AlignHR.Services
{
    public interface IWorkflowEngine
    {
        Task<DocApprovalInstance?> StartWorkflowAsync(Document document, ApprovalTemplate workflow);
        Task<bool> ProcessApprovalAsync(long instanceStepId, int approverId, string action, string? comments);
        Task<DocApprovalInstance?> GetPendingWorkflowAsync(long documentId);
        Task<List<DocApprovalInstanceStep>> GetPendingStepsAsync(int userId);
    }

    public class WorkflowEngine : IWorkflowEngine
    {
        private readonly ApplicationDbContext _context;
        private readonly IFlowResolver _flowResolver;

        public WorkflowEngine(ApplicationDbContext context, IFlowResolver flowResolver)
        {
            _context = context;
            _flowResolver = flowResolver;
        }

        public async Task<DocApprovalInstance?> StartWorkflowAsync(Document document, ApprovalTemplate workflow)
        {
            var steps = await _context.ApprovalTemplateSteps
                .Include(s => s.Role)
                .Where(s => s.TemplateId == workflow.Id)
                .OrderBy(s => s.StepNo)
                .ToListAsync();

            if (!steps.Any()) return null;

            var instance = new DocApprovalInstance
            {
                DocumentID  = document.DocumentID,
                WorkflowId  = workflow.Id,
                Status      = "Pending",
                CurrentStep = steps.First().StepNo,
                CreatedAt   = DateTime.Now,
                CreatedById = document.UploadedBy
            };

            _context.DocApprovalInstances.Add(instance);
            await _context.SaveChangesAsync();

            // Get uploader's department for context storage
            var uploader = await _context.users
                .Include(u => u.Employee)
                .FirstOrDefaultAsync(u => u.Id == document.UploadedBy);

            foreach (var step in steps)
            {
                if (step.ApprovalType == ApprovalType.Parallel)
                {
                    // Resolve all approvers for parallel step
                    var approverIds = await _flowResolver.ResolveParallelApproversAsync(step, workflow.FlowType, document.UploadedBy);

                    if (!approverIds.Any() && !step.IsOptional)
                    {
                        // Fallback: single resolution
                        var single = await _flowResolver.ResolveApproverAsync(step, workflow.FlowType, document.UploadedBy);
                        if (single.HasValue) approverIds.Add(single.Value);
                    }

                    foreach (var approverId in approverIds)
                    {
                        _context.DocApprovalInstanceSteps.Add(new DocApprovalInstanceStep
                        {
                            InstanceId     = instance.Id,
                            StepNo         = step.StepNo,
                            TemplateStepId = step.Id,
                            ApproverId     = approverId,
                            Action         = "Pending",
                            DepartmentId   = uploader?.Employee?.DepartmentFk
                        });
                    }
                }
                else
                {
                    // Single or Any — one approver per step
                    var approverId = await _flowResolver.ResolveApproverAsync(step, workflow.FlowType, document.UploadedBy);

                    _context.DocApprovalInstanceSteps.Add(new DocApprovalInstanceStep
                    {
                        InstanceId     = instance.Id,
                        StepNo         = step.StepNo,
                        TemplateStepId = step.Id,
                        ApproverId     = approverId,
                        Action         = "Pending",
                        DepartmentId   = uploader?.Employee?.DepartmentFk
                    });
                }
            }

            await _context.SaveChangesAsync();

            // Mark document as pending approval
            document.Status = "Pending";
            await _context.SaveChangesAsync();

            return instance;
        }

        public async Task<bool> ProcessApprovalAsync(long instanceStepId, int approverId, string action, string? comments)
        {
            var step = await _context.DocApprovalInstanceSteps
                .Include(s => s.Instance)
                    .ThenInclude(i => i!.Document)
                .Include(s => s.TemplateStep)
                .FirstOrDefaultAsync(s => s.Id == instanceStepId &&
                                          s.ApproverId == approverId &&
                                          s.Action == "Pending");

            if (step?.Instance == null) return false;

            var instance = step.Instance;

            if (instance.Status != "Pending" || instance.CurrentStep != step.StepNo)
                return false;

            step.Action    = action;
            step.ActionAt  = DateTime.Now;
            step.Comments  = comments;

            if (action == "Rejected")
            {
                instance.Status      = "Rejected";
                instance.CompletedAt = DateTime.Now;
                if (instance.Document != null)
                {
                    instance.Document.Status  = "Rejected";
                    instance.Document.Remarks = comments;
                }
                await _context.SaveChangesAsync();
                return true;
            }

            // Approved — check if step is fully satisfied
            var templateStep = step.TemplateStep;
            bool stepSatisfied = false;

            if (templateStep?.ApprovalType == ApprovalType.Any)
            {
                // Any one approval is enough
                stepSatisfied = true;

                // Auto-approve remaining parallel sibling steps
                var siblings = await _context.DocApprovalInstanceSteps
                    .Where(s => s.InstanceId == instance.Id &&
                                s.StepNo == step.StepNo &&
                                s.Id != step.Id &&
                                s.Action == "Pending")
                    .ToListAsync();

                foreach (var sibling in siblings)
                {
                    sibling.Action   = "Skipped";
                    sibling.ActionAt = DateTime.Now;
                    sibling.Comments = "Auto-skipped (Any approval satisfied)";
                }
            }
            else if (templateStep?.ApprovalType == ApprovalType.Parallel)
            {
                // All must approve — check if all siblings are approved
                var pendingSiblings = await _context.DocApprovalInstanceSteps
                    .Where(s => s.InstanceId == instance.Id &&
                                s.StepNo == step.StepNo &&
                                s.Id != step.Id &&
                                s.Action == "Pending")
                    .CountAsync();

                stepSatisfied = pendingSiblings == 0;
            }
            else
            {
                // Single — one approval satisfies the step
                stepSatisfied = true;
            }

            if (stepSatisfied)
            {
                // Find next step
                var nextStepNo = await _context.DocApprovalInstanceSteps
                    .Where(s => s.InstanceId == instance.Id && s.StepNo > step.StepNo)
                    .Select(s => (int?)s.StepNo)
                    .MinAsync();

                if (nextStepNo == null)
                {
                    // Final step — document fully approved
                    instance.Status      = "Approved";
                    instance.CompletedAt = DateTime.Now;
                    if (instance.Document != null)
                        instance.Document.Status = "Approved";
                }
                else
                {
                    instance.CurrentStep = nextStepNo.Value;
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<DocApprovalInstance?> GetPendingWorkflowAsync(long documentId)
        {
            return await _context.DocApprovalInstances
                .Include(i => i.Document)
                .Include(i => i.Workflow)
                    .ThenInclude(w => w!.Steps)
                .Where(i => i.DocumentID == documentId && i.Status == "Pending")
                .OrderByDescending(i => i.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<List<DocApprovalInstanceStep>> GetPendingStepsAsync(int userId)
        {
            return await _context.DocApprovalInstanceSteps
                .Include(s => s.Instance)
                    .ThenInclude(i => i!.Document)
                        .ThenInclude(d => d!.DocumentType)
                .Include(s => s.Instance)
                    .ThenInclude(i => i!.CreatedBy)
                .Include(s => s.TemplateStep)
                    .ThenInclude(ts => ts!.Role)
                .Where(s => s.ApproverId == userId &&
                            s.Action == "Pending" &&
                            s.Instance!.Status == "Pending" &&
                            s.Instance.CurrentStep == s.StepNo)
                .OrderByDescending(s => s.Instance!.CreatedAt)
                .ToListAsync();
        }
    }
}
