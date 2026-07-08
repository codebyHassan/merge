using AlignHR.Data;
using AlignHR.Models;
using Microsoft.EntityFrameworkCore;

namespace AlignHR.Repositories
{
    public class HrWorkflowRepository : IHrWorkflowRepository
    {
        private readonly ApplicationDbContext _context;

        public HrWorkflowRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<HrWorkflowDefinition?> GetActiveDefinitionAsync(string workflowName)
        {
            return await _context.HrWorkflowDefinitions
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.WorkflowName == workflowName && w.IsActive);
        }

        public async Task<List<HrWorkflowStep>> GetStepsAsync(decimal workflowDefinitionId)
        {
            return await _context.HrWorkflowSteps
                .Where(s => s.WorkflowDefinitionId == workflowDefinitionId)
                .OrderBy(s => s.StepOrder)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<HrWorkflowInstance?> GetInstanceAsync(decimal workflowInstanceId)
        {
            return await _context.HrWorkflowInstances
                .Include(i => i.CurrentStep)
                .FirstOrDefaultAsync(i => i.Id == workflowInstanceId);
        }

        public async Task<HrWorkflowInstance?> GetInstanceByEntityAsync(string entityType, decimal entityId)
        {
            return await _context.HrWorkflowInstances
                .Include(i => i.CurrentStep)
                .FirstOrDefaultAsync(i => i.EntityType == entityType && i.EntityId == entityId);
        }

        public async Task<List<HrWorkflowInstanceStep>> GetInstanceStepsAsync(decimal workflowInstanceId)
        {
            return await _context.HrWorkflowInstanceSteps
                .Include(s => s.WorkflowStep)
                .Where(s => s.WorkflowInstanceId == workflowInstanceId)
                .OrderBy(s => s.StepOrder)
                .ToListAsync();
        }

        public async Task<HrWorkflowInstanceStep?> GetPendingInstanceStepAsync(decimal workflowInstanceId)
        {
            return await _context.HrWorkflowInstanceSteps
                .Include(s => s.WorkflowStep)
                .FirstOrDefaultAsync(s => s.WorkflowInstanceId == workflowInstanceId && s.Status == "Pending");
        }

        public IQueryable<HrWorkflowInstance> QueryInstances()
        {
            return _context.HrWorkflowInstances
                .Include(i => i.CurrentStep)
                .AsQueryable();
        }

        public IQueryable<HrWorkflowInstanceStep> QueryInstanceSteps()
        {
            return _context.HrWorkflowInstanceSteps
                .Include(s => s.WorkflowInstance)
                .Include(s => s.WorkflowStep)
                .AsQueryable();
        }

        public async Task AddInstanceAsync(HrWorkflowInstance instance)
        {
            await _context.HrWorkflowInstances.AddAsync(instance);
        }

        public async Task AddInstanceStepAsync(HrWorkflowInstanceStep instanceStep)
        {
            await _context.HrWorkflowInstanceSteps.AddAsync(instanceStep);
        }

        public async Task AddActionAsync(HrWorkflowAction action)
        {
            await _context.HrWorkflowActions.AddAsync(action);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
