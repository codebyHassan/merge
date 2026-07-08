using AlignHR.Models;

namespace AlignHR.Repositories
{
    public interface IHrWorkflowRepository
    {
        Task<HrWorkflowDefinition?> GetActiveDefinitionAsync(string workflowName);
        Task<List<HrWorkflowStep>> GetStepsAsync(decimal workflowDefinitionId);
        Task<HrWorkflowInstance?> GetInstanceAsync(decimal workflowInstanceId);
        Task<HrWorkflowInstance?> GetInstanceByEntityAsync(string entityType, decimal entityId);
        Task<List<HrWorkflowInstanceStep>> GetInstanceStepsAsync(decimal workflowInstanceId);
        Task<HrWorkflowInstanceStep?> GetPendingInstanceStepAsync(decimal workflowInstanceId);
        IQueryable<HrWorkflowInstance> QueryInstances();
        IQueryable<HrWorkflowInstanceStep> QueryInstanceSteps();
        Task AddInstanceAsync(HrWorkflowInstance instance);
        Task AddInstanceStepAsync(HrWorkflowInstanceStep instanceStep);
        Task AddActionAsync(HrWorkflowAction action);
        Task SaveChangesAsync();
    }
}
