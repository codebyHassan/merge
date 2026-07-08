using AlignHR.Data;
using AlignHR.Models;
using Microsoft.EntityFrameworkCore;

namespace AlignHR.Services
{
    public interface IFlowResolver
    {
        Task<int?> ResolveApproverAsync(ApprovalTemplateStep step, WorkflowFlowType flowType, int uploaderUserId);
        Task<List<int>> ResolveParallelApproversAsync(ApprovalTemplateStep step, WorkflowFlowType flowType, int uploaderUserId);
    }

    public class FlowResolver : IFlowResolver
    {
        private readonly ApplicationDbContext _context;

        public FlowResolver(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int?> ResolveApproverAsync(ApprovalTemplateStep step, WorkflowFlowType flowType, int uploaderUserId)
        {
            var uploader = await _context.users
                .Include(u => u.Employee)
                .ThenInclude(e => e!.Department)
                .Include(u => u.Employee)
                .ThenInclude(e => e!.Grade)
                .FirstOrDefaultAsync(u => u.Id == uploaderUserId);

            if (uploader?.Employee == null)
            {
                // No employee linked — fallback to any user with the required role
                return step.RoleId > 0 ? await FallbackByRole(step.RoleId) : null;
            }

            return flowType switch
            {
                WorkflowFlowType.Department => await ResolveDepartmentFlow(step.RoleId, uploader.Employee.DepartmentFk),
                WorkflowFlowType.Reporting  => await ResolveReportingFlow(uploader.Employee.LineManagerEmpNo),
                WorkflowFlowType.Position   => await ResolvePositionFlow(step.RoleId, uploader.Employee.GradeFk),
                _                           => null
            };
        }

        public async Task<List<int>> ResolveParallelApproversAsync(ApprovalTemplateStep step, WorkflowFlowType flowType, int uploaderUserId)
        {
            var uploader = await _context.users
                .Include(u => u.Employee)
                .ThenInclude(e => e!.Department)
                .Include(u => u.Employee)
                .ThenInclude(e => e!.Grade)
                .FirstOrDefaultAsync(u => u.Id == uploaderUserId);

            if (uploader?.Employee == null)
            {
                var fallback = await FallbackByRole(step.RoleId);
                return fallback.HasValue ? new List<int> { fallback.Value } : new List<int>();
            }

            var approvers = new List<int>();

            if (flowType == WorkflowFlowType.Department)
            {
                // All users in the same department with the required role
                var usersWithRole = await _context.UserRoles
                    .Where(ur => ur.RoleId == step.RoleId)
                    .Select(ur => ur.UserId)
                    .ToListAsync();

                var deptApprovers = await _context.users
                    .Include(u => u.Employee)
                    .Where(u => usersWithRole.Contains(u.Id) &&
                                u.Employee != null &&
                                u.Employee.DepartmentFk == uploader.Employee.DepartmentFk)
                    .Select(u => u.Id)
                    .ToListAsync();
                approvers.AddRange(deptApprovers);
            }
            else
            {
                var single = await ResolveApproverAsync(step, flowType, uploaderUserId);
                if (single.HasValue) approvers.Add(single.Value);
            }

            return approvers;
        }

        private async Task<int?> ResolveDepartmentFlow(int roleId, int? departmentId)
        {
            if (departmentId.HasValue)
            {
                var usersWithRole = await _context.UserRoles
                    .Where(ur => ur.RoleId == roleId)
                    .Select(ur => ur.UserId)
                    .ToListAsync();

                var result = await _context.users
                    .Where(u => usersWithRole.Contains(u.Id) &&
                                u.Employee != null &&
                                u.Employee.DepartmentFk == departmentId)
                    .Select(u => (int?)u.Id)
                    .FirstOrDefaultAsync();

                if (result.HasValue) return result;
            }

            // Fallback: any user with the required role
            return await FallbackByRole(roleId);
        }

        private async Task<int?> ResolveReportingFlow(string? managerEmpNo)
        {
            if (!string.IsNullOrEmpty(managerEmpNo))
            {
                var manager = await _context.emp.FirstOrDefaultAsync(e => e.Code == managerEmpNo);
                if (manager != null)
                {
                    var result = await _context.users
                        .Where(u => u.EmployeeId == manager.Id)
                        .Select(u => (int?)u.Id)
                        .FirstOrDefaultAsync();

                    if (result.HasValue) return result;
                }
            }

            return null; // No fallback for reporting — line manager must be configured
        }

        private async Task<int?> ResolvePositionFlow(int roleId, int? gradeId)
        {
            if (gradeId.HasValue)
            {
                var usersWithRole = await _context.UserRoles
                    .Where(ur => ur.RoleId == roleId)
                    .Select(ur => ur.UserId)
                    .ToListAsync();

                var result = await _context.users
                    .Where(u => usersWithRole.Contains(u.Id) &&
                                u.Employee != null &&
                                u.Employee.GradeFk == gradeId)
                    .Select(u => (int?)u.Id)
                    .FirstOrDefaultAsync();

                if (result.HasValue) return result;
            }

            // Fallback: any user with the required role
            return await FallbackByRole(roleId);
        }

        private async Task<int?> FallbackByRole(int roleId)
        {
            var userIds = await _context.UserRoles
                .Where(ur => ur.RoleId == roleId)
                .Select(ur => (int?)ur.UserId)
                .FirstOrDefaultAsync();

            return userIds;
        }
    }
}
