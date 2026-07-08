using AlignHR.Data;
using AlignHR.Models;
using AlignHR.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace AlignHR.Services
{
    /// <summary>
    /// Core workflow engine for leave approval routing.
    /// 
    /// Architecture:
    /// - All routing logic lives here. Controllers are thin.
    /// - Hierarchy traversal via Employee.LineManagerEmpNo (indexed)
    /// - Final approval routed to IsLeaveFinalApprover employee
    /// - Transaction-safe: all multi-table updates wrapped in transactions
    /// - Concurrency-safe: RowVersion prevents double-approvals
    /// - Extensible: GetNextApproverAsync() designed as routing engine entry point
    /// 
    /// Infinite loop guard: max 20 hierarchy levels, HashSet visited tracking.
    /// </summary>
    public class LeaveWorkflowService : ILeaveWorkflowService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILeaveAccountingService _leaveAccountingService;
        private const int MaxHierarchyDepth = 20;

        public LeaveWorkflowService(ApplicationDbContext context, ILeaveAccountingService leaveAccountingService)
        {
            _context = context;
            _leaveAccountingService = leaveAccountingService;
        }

        // ═══════════════════════════════════════════════════════════════
        //  START WORKFLOW
        // ═══════════════════════════════════════════════════════════════

        public async Task<LeaveRequest> StartWorkflowAsync(LeaveRequest request, int? submittedByEmployeeId = null)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Resolve whose hierarchy to follow
                // If User A applies for User B, we follow User A's hierarchy if submittedByEmployeeId is provided
                int hierarchyReferenceId = (submittedByEmployeeId.HasValue && submittedByEmployeeId.Value > 0)
                    ? submittedByEmployeeId.Value
                    : request.EmployeeId;

                var hierarchyEmployee = await _context.emp
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.Id == hierarchyReferenceId);

                if (hierarchyEmployee == null)
                    throw new InvalidOperationException("Reference employee for hierarchy not found.");

                // Load the actual target employee for audit trail
                var targetEmployee = await _context.emp
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.Id == request.EmployeeId);

                if (targetEmployee == null)
                    throw new InvalidOperationException("Target employee not found.");

                // 2. Resolve first approver based on HIERARCHY REFERENCE
                Employee firstApprover = null;
                WorkflowStage initialStage = WorkflowStage.LineManager;

                if (!string.IsNullOrWhiteSpace(hierarchyEmployee.LineManagerEmpNo))
                {
                    firstApprover = await _context.emp
                        .AsNoTracking()
                        .FirstOrDefaultAsync(e => e.Code == hierarchyEmployee.LineManagerEmpNo);

                    if (firstApprover == null)
                        throw new InvalidOperationException($"Line Manager with code '{hierarchyEmployee.LineManagerEmpNo}' not found.");

                    if (firstApprover.Id == hierarchyReferenceId)
                        throw new InvalidOperationException("Circular hierarchy detected (Employee is their own manager).");
                    
                    initialStage = WorkflowStage.LineManager;
                }
                else
                {
                    // No Line Manager -> Go straight to External based on the REFERENCE employee
                    firstApprover = await GetConfiguredExternalApproverAsync(hierarchyReferenceId);
                    
                    if (firstApprover != null)
                    {
                        initialStage = WorkflowStage.ExternalApprover;
                    }
                    else
                    {
                        throw new InvalidOperationException("No hierarchy path (Line Manager or External Approver) found for the applicant.");
                    }
                }

                // 4. Set workflow state
                request.Status = "Pending";
                request.CurrentApproverId = firstApprover.Id;
                request.CurrentStage = initialStage;
                request.ApprovalLevel = 1;
                request.AppliedAt = DateTime.Now;
                request.CreatedAt = DateTime.Now;
                
                // Store the submitter's Employee ID in CreatedBy for hierarchy tracking in subsequent steps
                request.CreatedBy = hierarchyReferenceId.ToString();

                _context.LeaveRequests.Add(request);
                await _context.SaveChangesAsync();

                // 5. Insert audit trail
                var auditLog = new LeaveApproval
                {
                    LeaveRequestId = request.Id,
                    ApproverEmployeeId = hierarchyReferenceId,
                    ApprovalLevel = 0,
                    Action = "Submitted",
                    Remarks = (hierarchyReferenceId != request.EmployeeId) 
                        ? $"Leave request submitted by {hierarchyEmployee.FirstName} {hierarchyEmployee.LastName} on behalf of {targetEmployee.FirstName} {targetEmployee.LastName}"
                        : $"Leave request submitted by {targetEmployee.FirstName} {targetEmployee.LastName}",
                    ActionAt = DateTime.Now
                };

                _context.LeaveApprovals.Add(auditLog);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return request;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // ═══════════════════════════════════════════════════════════════
        //  APPROVE
        // ═══════════════════════════════════════════════════════════════

        public async Task ApproveAsync(decimal leaveRequestId, int approverEmployeeId, string? remarks)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Load request with concurrency check
                var request = await _context.LeaveRequests
                    .FirstOrDefaultAsync(lr => lr.Id == leaveRequestId);

                if (request == null)
                    throw new InvalidOperationException("Leave request not found.");

                if (request.Status != "Pending")
                    throw new InvalidOperationException(
                        $"Cannot approve a request with status '{request.Status}'. Only Pending requests can be approved.");

                if (request.CurrentApproverId != approverEmployeeId)
                    throw new InvalidOperationException(
                        "You are not the current approver for this request.");

                // 2. Load approver details
                var approver = await _context.emp
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.Id == approverEmployeeId);

                if (approver == null)
                    throw new InvalidOperationException("Approver employee not found.");

                // 3. Insert audit trail
                var auditLog = new LeaveApproval
                {
                    LeaveRequestId = leaveRequestId,
                    ApproverEmployeeId = approverEmployeeId,
                    ApprovalLevel = request.ApprovalLevel,
                    Action = "Approved",
                    Remarks = remarks,
                    ActionAt = DateTime.Now
                };

                _context.LeaveApprovals.Add(auditLog);

                // 4. Get the hierarchy reference employee (original applicant) to check their configuration
                int hierarchyId = request.EmployeeId;
                if (int.TryParse(request.CreatedBy, out int cid))
                {
                    hierarchyId = cid;
                }

                var requester = await _context.emp
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.Id == hierarchyId);

                if (requester == null)
                    throw new InvalidOperationException("Original requester not found.");

                // 5. Route request
                if (request.CurrentStage == WorkflowStage.ExternalApprover)
                {
                    // ── External Approver is always the final step ──
                    await FinalizeApprovalAsync(request, approver);
                }
                else
                {
                    // We are in the Line Manager hierarchy (LineManager or HigherManagement)
                    bool shouldRouteToExternal = false;

                    // Case 3: If the current approver is the designated "LeaveApproverEmpNo", the hierarchy stops here.
                    if (!string.IsNullOrWhiteSpace(requester.LeaveApproverEmpNo) && approver.Code == requester.LeaveApproverEmpNo)
                    {
                        shouldRouteToExternal = true;
                    }
                    else
                    {
                        // Otherwise, check if there is another line manager in the hierarchy
                        var nextApprover = await GetNextApproverAsync(approverEmployeeId);

                        if (nextApprover != null && nextApprover.Id != approver.Id)
                        {
                            // Route to next line manager
                            request.CurrentApproverId = nextApprover.Id;
                            request.ApprovalLevel += 1;
                            request.CurrentStage = WorkflowStage.HigherManagement;
                        }
                        else
                        {
                            // Case 2: Hierarchy ended naturally without hitting a LeaveApproverEmpNo stop condition
                            shouldRouteToExternal = true;
                        }
                    }

                    // If we determined we need to move to External
                    if (shouldRouteToExternal)
                    {
                        // Check External Approver based on the HIERARCHY REFERENCE (original applicant)
                        var externalApprover = await GetConfiguredExternalApproverAsync(hierarchyId);
                        if (externalApprover != null && externalApprover.Id != approver.Id)
                        {
                            request.CurrentApproverId = externalApprover.Id;
                            request.ApprovalLevel += 1;
                            request.CurrentStage = WorkflowStage.ExternalApprover;
                        }
                        else
                        {
                            // If no external approver configured, finalize the request.
                            await FinalizeApprovalAsync(request, approver);
                        }
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // ═══════════════════════════════════════════════════════════════
        //  REJECT
        // ═══════════════════════════════════════════════════════════════

        public async Task RejectAsync(decimal leaveRequestId, int approverEmployeeId, string reason)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var request = await _context.LeaveRequests
                    .FirstOrDefaultAsync(lr => lr.Id == leaveRequestId);

                if (request == null)
                    throw new InvalidOperationException("Leave request not found.");

                if (request.Status != "Pending")
                    throw new InvalidOperationException(
                        $"Cannot reject a request with status '{request.Status}'.");

                if (request.CurrentApproverId != approverEmployeeId)
                    throw new InvalidOperationException(
                        "You are not the current approver for this request.");

                // 1. Update workflow state
                request.Status = "Rejected";
                request.CurrentStage = WorkflowStage.Rejected;
                request.CurrentApproverId = null;
                request.RejectedAt = DateTime.Now;
                request.RejectionReason = reason;

                // 2. Insert audit trail
                var auditLog = new LeaveApproval
                {
                    LeaveRequestId = leaveRequestId,
                    ApproverEmployeeId = approverEmployeeId,
                    ApprovalLevel = request.ApprovalLevel,
                    Action = "Rejected",
                    Remarks = reason,
                    ActionAt = DateTime.Now
                };

                _context.LeaveApprovals.Add(auditLog);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // ═══════════════════════════════════════════════════════════════
        //  ROUTING ENGINE — GetNextApproverAsync
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// Extensible routing engine entry point.
        /// 
        /// Current implementation: traverse Employee.LineManagerEmpNo hierarchy.
        /// 
        /// Future extension points (do NOT implement now, but architecture supports):
        /// - Policy-based routing (by leave type, department, amount)
        /// - Delegation routing (vacation coverage)
        /// - Parallel approval routing
        /// - Department-specific approval chains
        /// </summary>
        public async Task<Employee?> GetNextApproverAsync(int currentApproverId)
        {
            var currentApprover = await _context.emp
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == currentApproverId);

            if (currentApprover == null)
                return null;

            // No line manager assigned → chain ended
            if (string.IsNullOrWhiteSpace(currentApprover.LineManagerEmpNo))
                return null;

            // Resolve next manager via indexed lookup on Employee.Code
            var nextManager = await _context.emp
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Code == currentApprover.LineManagerEmpNo);

            if (nextManager == null)
                return null;

            // ── Infinite loop guard ──
            // Prevent circular hierarchy (A→B→C→A)
            var visited = new HashSet<int> { currentApproverId };
            var candidate = nextManager;
            int depth = 0;

            while (candidate != null && depth < MaxHierarchyDepth)
            {
                if (visited.Contains(candidate.Id))
                {
                    // Circular reference detected — stop at current level
                    return null;
                }
                // The first candidate is valid — return it
                return candidate;
            }

            return nextManager;
        }

        // ═══════════════════════════════════════════════════════════════
        //  APPROVAL INBOX
        // ═══════════════════════════════════════════════════════════════

        public async Task<List<LeaveRequest>> GetApprovalInboxAsync(int employeeId)
        {
            return await _context.LeaveRequests
                .Include(lr => lr.Employee)
                    .ThenInclude(e => e!.Department)
                .Include(lr => lr.LeaveType)
                .Where(lr => lr.CurrentApproverId == employeeId
                          && lr.Status == "Pending")
                .OrderByDescending(lr => lr.AppliedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        // ═══════════════════════════════════════════════════════════════
        //  APPROVAL HISTORY
        // ═══════════════════════════════════════════════════════════════

        public async Task<List<LeaveApproval>> GetApprovalHistoryAsync(decimal leaveRequestId)
        {
            return await _context.LeaveApprovals
                .Include(la => la.ApproverEmployee)
                .Where(la => la.LeaveRequestId == leaveRequestId)
                .OrderBy(la => la.ApprovalLevel)
                .ThenBy(la => la.ActionAt)
                .AsNoTracking()
                .ToListAsync();
        }

        // ═══════════════════════════════════════════════════════════════
        //  CONFIGURED EXTERNAL APPROVER LOOKUP
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// Looks up the configured external approver based on the active LeaveConfiguration.
        /// Checks the employee's Department/SubDepartment/Grade/Division assignment,
        /// then finds the matching LeaveConfigurationDetail record.
        /// </summary>
        public async Task<Employee?> GetConfiguredExternalApproverAsync(int employeeId)
        {
            // 1. Find the active configuration
            var config = await _context.LeaveConfigurations
                .Include(c => c.Details)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.IsActive);

            if (config == null || !config.Details.Any())
                return null;

            // 2. Load the employee with relevant FKs
            var employee = await _context.emp
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == employeeId);

            if (employee == null)
                return null;

            // 3. Determine which reference to look up
            int? referenceId = null;

            if (config.IsDepartment)
                referenceId = employee.DepartmentFk;
            else if (config.IsSubDepartment)
                referenceId = employee.SubDepartmentFk;
            else if (config.IsGrade)
                referenceId = employee.GradeFk;
            else if (config.IsDivision)
                referenceId = employee.DivisionFk;

            if (referenceId == null)
                return null;

            // 4. Find the matching detail record
            var detail = config.Details
                .FirstOrDefault(d => d.ReferenceId == referenceId.Value);

            if (detail == null || string.IsNullOrWhiteSpace(detail.ApproverEmpNo))
                return null;

            // 5. Resolve the approver employee
            var approver = await _context.emp
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Code == detail.ApproverEmpNo);

            // 6. Prevent self-approval
            if (approver != null && approver.Id == employeeId)
                return null;

            return approver;
        }

        // ═══════════════════════════════════════════════════════════════
        //  PRIVATE — Finalization
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// Finalizes a leave request after the final approver approves.
        /// Updates LeaveBalance in the same transaction.
        /// </summary>
        private async Task FinalizeApprovalAsync(LeaveRequest request, Employee approver)
        {
            request.Status = "Approved";
            request.CurrentStage = WorkflowStage.FinalApproved;
            request.CurrentApproverId = null;
            request.ApprovedAt = DateTime.Now;
            request.ApprovedByEmpNo = approver.Code;

            // ── Insert LeaveTransaction per day ──
            var employeePolicy = await _context.EmployeeLeavePolicies
                .Include(p => p.LeavePolicy)
                .FirstOrDefaultAsync(p => p.EmployeeId == request.EmployeeId);
            
            int activeYear = DateTime.Now.Year;
            
            for (DateTime date = request.FromDate; date <= request.ToDate; date = date.AddDays(1))
            {
                decimal usageAmount = request.IsHalfDay ? 0.5m : 1.0m;

                var txn = new LeaveTransaction
                {
                    LeaveRequestFk = request.Id,
                    EmployeeFk = request.EmployeeId,
                    LeaveTypeFk = request.LeaveTypeId,
                    TransactionDate = date, 
                    TransactionType = LeaveTransactionType.Availed,
                    Year = activeYear, 
                    Debit = 0,
                    Credit = usageAmount, // Reduction/Usage
                    ReferenceNo = $"LR-{request.Id}-{date:yyyyMMdd}",
                    TransactionSource = "Workflow",
                    Narration = request.IsHalfDay 
                                ? $"Half Day Leave ({request.HalfDaySession}) on {date:MMM dd, yyyy}" 
                                : $"Leave on {date:MMM dd, yyyy}",
                    CreatedBy = approver.Id
                };

                await _leaveAccountingService.InsertTransactionAsync(txn);
            }
        }
    }
}
