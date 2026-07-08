using AlignHR.Data;
using AlignHR.Models;
using AlignHR.Security;
using AlignHR.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlignHR.Controllers
{
    public class ApprovalInboxController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IWorkflowEngine _workflowEngine;

        public ApprovalInboxController(ApplicationDbContext context, IWorkflowEngine workflowEngine)
        {
            _context = context;
            _workflowEngine = workflowEngine;
        }

        [SkipPermissionCheck]
        public async Task<IActionResult> Index()
        {
            if (!PermissionHelper.IsLoggedIn(HttpContext))
                return RedirectToAction("Login", "User");

            int currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;

            var pendingSteps = await _workflowEngine.GetPendingStepsAsync(currentUserId);

            return View(pendingSteps);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SkipPermissionCheck]
        public async Task<IActionResult> Approve(long stepId, string? comments)
        {
            if (!PermissionHelper.IsLoggedIn(HttpContext))
                return RedirectToAction("Login", "User");

            int currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;

            var ok = await _workflowEngine.ProcessApprovalAsync(stepId, currentUserId, "Approved", comments);

            TempData[ok ? "SuccessMessage" : "ErrorMessage"] = ok
                ? "Document approved and routed to next step."
                : "Approval step not found or already processed.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SkipPermissionCheck]
        public async Task<IActionResult> Reject(long stepId, string? comments)
        {
            if (!PermissionHelper.IsLoggedIn(HttpContext))
                return RedirectToAction("Login", "User");

            if (string.IsNullOrWhiteSpace(comments))
            {
                TempData["ErrorMessage"] = "Comments are required when rejecting a document.";
                return RedirectToAction(nameof(Index));
            }

            int currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;

            var ok = await _workflowEngine.ProcessApprovalAsync(stepId, currentUserId, "Rejected", comments);

            TempData[ok ? "SuccessMessage" : "ErrorMessage"] = ok
                ? "Document rejected."
                : "Approval step not found or already processed.";

            return RedirectToAction(nameof(Index));
        }

        [SkipPermissionCheck]
        public async Task<IActionResult> History()
        {
            if (!PermissionHelper.IsLoggedIn(HttpContext))
                return RedirectToAction("Login", "User");

            int currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;

            var history = await _context.DocApprovalInstanceSteps
                .Include(s => s.Instance)
                    .ThenInclude(i => i!.Document)
                        .ThenInclude(d => d!.DocumentType)
                .Include(s => s.Instance)
                    .ThenInclude(i => i!.CreatedBy)
                .Include(s => s.TemplateStep)
                    .ThenInclude(ts => ts!.Role)
                .Where(s => s.ApproverId == currentUserId && s.Action != "Pending")
                .OrderByDescending(s => s.ActionAt)
                .Take(50)
                .ToListAsync();

            return View(history);
        }
    }
}
