using AlignHR.Models;
using AlignHR.Security;
using AlignHR.Services;
using Microsoft.AspNetCore.Mvc;

namespace AlignHR.Controllers
{
    public class HrJobPostingsController : BaseController
    {
        private readonly IHrJobPostingService _service;

        public HrJobPostingsController(IHrJobPostingService service)
        {
            _service = service;
        }

        [RequireUrlPermission]
        public async Task<IActionResult> Index(decimal? requisitionId, string? search, int? page)
        {
            const int pageSize = 15;
            ViewData["CurrentFilter"] = search;
            ViewData["RequisitionId"] = requisitionId;

            // Show only postings whose requisition is assigned to the logged-in recruiter.
            // (When viewing postings for a specific requisition, don't restrict by recruiter.)
            decimal? assignedToEmployeeId = null;
            if (!requisitionId.HasValue)
            {
                var session = PermissionHelper.GetUserSession(HttpContext);
                assignedToEmployeeId = session?.EmployeeId;
                if (!assignedToEmployeeId.HasValue || assignedToEmployeeId.Value <= 0)
                {
                    TempData["Error"] = "Your user account is not linked to an employee record.";
                    assignedToEmployeeId = -1; // forces empty result
                }
            }

            var model = await _service.GetPagedAsync(requisitionId, search, page ?? 1, pageSize, assignedToEmployeeId);

            if (!requisitionId.HasValue && assignedToEmployeeId.HasValue && assignedToEmployeeId.Value > 0)
                ViewBag.PendingRequisitions = await _service.GetPendingRequisitionsAsync(assignedToEmployeeId.Value);

            return View(model);
        }

        [RequireUrlPermission]
        public async Task<IActionResult> Create(decimal requisitionId)
        {
            try
            {
                var model = await _service.GetForCreateAsync(requisitionId);
                if (model == null) return NotFound();
                return View(model);
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Details", "HrRequisitions", new { id = requisitionId });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HrJobPostingFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var username = PermissionHelper.GetCurrentUsername(HttpContext);
                    var id = await _service.CreateAsync(model, username);
                    return RedirectToAction(nameof(Details), new { id });
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(model);
        }

        [RequireUrlPermission]
        public async Task<IActionResult> Edit(decimal id)
        {
            var model = await _service.GetForEditAsync(id);
            if (model == null) return NotFound();
            if (model.PostingStatus == "Closed")
            {
                TempData["Error"] = "Closed postings cannot be edited.";
                return RedirectToAction(nameof(Details), new { id });
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, HrJobPostingFormViewModel model)
        {
            if (id != model.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    var username = PermissionHelper.GetCurrentUsername(HttpContext);
                    await _service.UpdateAsync(id, model, username);
                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(model);
        }

        [RequireUrlPermission]
        public async Task<IActionResult> Details(decimal id)
        {
            var model = await _service.GetDetailsAsync(id);
            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Publish(decimal id)
            => await RunStatusChangeAsync(id, "Published");

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pause(decimal id)
            => await RunStatusChangeAsync(id, "Paused");

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Close(decimal id)
            => await RunStatusChangeAsync(id, "Closed");

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(decimal id)
        {
            try
            {
                var username = PermissionHelper.GetCurrentUsername(HttpContext);
                await _service.DeleteAsync(id, username);
                TempData["Success"] = "Job posting deleted.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        private async Task<IActionResult> RunStatusChangeAsync(decimal id, string newStatus)
        {
            try
            {
                var username = PermissionHelper.GetCurrentUsername(HttpContext);
                await _service.ChangeStatusAsync(id, newStatus, username);
                TempData["Success"] = $"Job posting {newStatus.ToLower()}.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
