using AlignHR.Models;

namespace AlignHR.Services
{
    /// <summary>
    /// Contract for the leave approval workflow engine.
    /// All workflow routing logic is encapsulated behind this interface.
    /// Controllers MUST NOT contain workflow rules — they delegate to this service.
    /// </summary>
    public interface ILeaveWorkflowService
    {
        /// <summary>
        /// Initializes the workflow for a new leave request.
        /// Sets the first approver based on employee hierarchy.
        /// If submittedByEmployeeId is provided and different from request.EmployeeId,
        /// the workflow will follow the submitter's hierarchy.
        /// </summary>
        Task<LeaveRequest> StartWorkflowAsync(LeaveRequest request, int? submittedByEmployeeId = null);

        /// <summary>
        /// Processes an approval action. Determines the next approver dynamically
        /// or finalizes the request if the chain is complete.
        /// </summary>
        Task ApproveAsync(decimal leaveRequestId, int approverEmployeeId, string? remarks);

        /// <summary>
        /// Rejects a leave request at any point in the workflow.
        /// Terminates the workflow immediately.
        /// </summary>
        Task RejectAsync(decimal leaveRequestId, int approverEmployeeId, string reason);

        /// <summary>
        /// Routing engine method — determines the next approver in the hierarchy.
        /// Designed for extensibility: future support for policy-based, 
        /// department-based, delegation, and parallel approval routing.
        /// </summary>
        Task<Employee?> GetNextApproverAsync(int currentApproverId);

        /// <summary>
        /// Returns all pending leave requests assigned to a specific employee for approval.
        /// </summary>
        Task<List<LeaveRequest>> GetApprovalInboxAsync(int employeeId);

        /// <summary>
        /// Returns the complete approval audit trail for a leave request.
        /// </summary>
        Task<List<LeaveApproval>> GetApprovalHistoryAsync(decimal leaveRequestId);

        /// <summary>
        /// Looks up the configured external approver based on active LeaveConfiguration
        /// and the employee's department/grade/division assignment.
        /// </summary>
        Task<Employee?> GetConfiguredExternalApproverAsync(int employeeId);
    }
}
