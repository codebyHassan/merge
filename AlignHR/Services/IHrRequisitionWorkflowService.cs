using AlignHR.Models;

namespace AlignHR.Services
{
    public interface IHrRequisitionWorkflowService
    {
        Task<decimal> StartAsync(HrRequisition requisition, decimal actionBy, string? comments);
        Task<List<HrWorkflowInboxItemViewModel>> GetInboxAsync(decimal employeeId);
        Task<HrWorkflowActionViewModel?> GetActionViewAsync(decimal workflowInstanceId);
        Task ApproveAsync(decimal workflowInstanceId, decimal actionBy, string? comments);
        Task RejectAsync(decimal workflowInstanceId, decimal actionBy, string? comments);
        Task SendBackAsync(decimal workflowInstanceId, decimal actionBy, string? comments);
    }
}
