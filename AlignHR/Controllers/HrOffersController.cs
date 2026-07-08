using AlignHR.Data;
using AlignHR.Models;
using AlignHR.Security;
using AlignHR.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AlignHR.Controllers
{
    public class HrOffersController : BaseController
    {
        private readonly IHrOfferService _service;
        private readonly IFileStorageService _fileStorage;
        private readonly ApplicationDbContext _context;

        public HrOffersController(IHrOfferService service, IFileStorageService fileStorage, ApplicationDbContext context)
        {
            _service = service;
            _fileStorage = fileStorage;
            _context = context;
        }

        [RequireUrlPermission]
        public async Task<IActionResult> MyApprovals()
        {
            var empId = GetEmployeeId();
            var items = empId.HasValue
                ? await _service.GetMyApprovalsAsync((int)empId.Value)
                : new List<HrOfferApprovalInboxItemViewModel>();
            return View(items);
        }

        [RequireUrlPermission]
        public async Task<IActionResult> Index(string? search, int? page)
        {
            ViewData["CurrentFilter"] = search;
            var model = await _service.GetPagedAsync(search, page ?? 1, 15);
            return View(model);
        }

        [RequireUrlPermission]
        public async Task<IActionResult> Create(decimal applicationId)
        {
            try
            {
                var model = await _service.GetForCreateAsync(applicationId);
                if (model == null) return NotFound();
                return View(model);
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HrOfferFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var username = PermissionHelper.GetCurrentUsername(HttpContext);
                    var empId = GetEmployeeId();
                    var id = await _service.CreateAsync(model, username, empId);
                    TempData["Success"] = "Offer created successfully.";
                    return RedirectToAction(nameof(Details), new { id });
                }
                catch (InvalidOperationException ex) { ModelState.AddModelError("", ex.Message); }
            }
            return View(model);
        }

        [RequireUrlPermission]
        public async Task<IActionResult> Edit(decimal id)
        {
            try
            {
                var model = await _service.GetForEditAsync(id);
                if (model == null) return NotFound();
                return View(model);
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(HrOfferFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _service.UpdateAsync(model, PermissionHelper.GetCurrentUsername(HttpContext), GetEmployeeId());
                    TempData["Success"] = "Offer updated.";
                    return RedirectToAction(nameof(Details), new { id = model.Id });
                }
                catch (InvalidOperationException ex) { ModelState.AddModelError("", ex.Message); }
            }
            return View(model);
        }

        [RequireUrlPermission]
        public async Task<IActionResult> Details(decimal id)
        {
            var model = await _service.GetDetailsAsync(id);
            if (model == null) return NotFound();
            await LoadApproverDropdownAsync(id);
            return View(model);
        }

        // ── Revision ──────────────────────────────────────────────

        [RequireUrlPermission]
        public async Task<IActionResult> AddVersion(decimal offerId)
        {
            var model = await _service.GetForRevisionAsync(offerId);
            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddVersion(HrOfferVersionFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _service.AddVersionAsync(model, GetEmployeeId());
                    TempData["Success"] = "Revision added.";
                    return RedirectToAction(nameof(Details), new { id = model.OfferId });
                }
                catch (InvalidOperationException ex) { ModelState.AddModelError("", ex.Message); }
            }
            return View(model);
        }

        // ── Workflow ──────────────────────────────────────────────

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitForApproval(decimal id)
        {
            try
            {
                await _service.SubmitForApprovalAsync(id, PermissionHelper.GetCurrentUsername(HttpContext));
                TempData["Success"] = "Offer submitted for approval.";
            }
            catch (InvalidOperationException ex) { TempData["Error"] = ex.Message; }
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddApprover(HrAddApproverViewModel model)
        {
            await _service.AddApproverAsync(model, PermissionHelper.GetCurrentUsername(HttpContext));
            TempData["Success"] = "Approver added.";
            return RedirectToAction(nameof(Details), new { id = model.OfferId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteApprover(decimal approvalId, decimal offerId)
        {
            try
            {
                await _service.RemoveApproverAsync(approvalId);
                TempData["Success"] = "Approver removed.";
            }
            catch (InvalidOperationException ex) { TempData["Error"] = ex.Message; }
            return RedirectToAction(nameof(Details), new { id = offerId });
        }

        [RequireUrlPermission]
        public async Task<IActionResult> ApproveOffer(decimal approvalId, string? from)
        {
            var model = await _service.GetApprovalActionAsync(approvalId);
            if (model == null) return NotFound();
            ViewBag.From = from;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveOffer(HrApprovalActionViewModel model, string? from)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _service.ProcessApprovalAsync(model, GetEmployeeId());
                    TempData["Success"] = model.Action == "Approve" ? "Offer approved." : "Offer rejected.";
                    if (from == "inbox")
                        return RedirectToAction(nameof(MyApprovals));
                    return RedirectToAction(nameof(Details), new { id = model.OfferId });
                }
                catch (InvalidOperationException ex) { ModelState.AddModelError("", ex.Message); }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send(decimal id)
        {
            try
            {
                await _service.SendOfferAsync(id, PermissionHelper.GetCurrentUsername(HttpContext));
                TempData["Success"] = "Offer sent to candidate.";
            }
            catch (InvalidOperationException ex) { TempData["Error"] = ex.Message; }
            return RedirectToAction(nameof(Details), new { id });
        }

        // ── Candidate Response ────────────────────────────────────

        [RequireUrlPermission]
        public async Task<IActionResult> RecordResponse(decimal offerId)
        {
            var model = await _service.GetForResponseAsync(offerId);
            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecordResponse(HrOfferResponseFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _service.RecordResponseAsync(model, PermissionHelper.GetCurrentUsername(HttpContext));
                TempData["Success"] = "Candidate response recorded.";
                return RedirectToAction(nameof(Details), new { id = model.OfferId });
            }
            return View(model);
        }

        // ── Documents ─────────────────────────────────────────────

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadDocument(decimal offerId, Microsoft.AspNetCore.Http.IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Please select a file.";
                return RedirectToAction(nameof(Details), new { id = offerId });
            }
            await _service.UploadDocumentAsync(offerId, file, PermissionHelper.GetCurrentUsername(HttpContext));
            TempData["Success"] = "Document uploaded.";
            return RedirectToAction(nameof(Details), new { id = offerId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDocument(decimal documentId, decimal offerId)
        {
            await _service.DeleteDocumentAsync(documentId);
            TempData["Success"] = "Document deleted.";
            return RedirectToAction(nameof(Details), new { id = offerId });
        }

        public IActionResult DownloadDocument(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return NotFound();
            var absolute = _fileStorage.GetAbsolutePath(path);
            if (!System.IO.File.Exists(absolute)) return NotFound();
            return PhysicalFile(absolute, "application/octet-stream", System.IO.Path.GetFileName(absolute));
        }

        // ── Cancel ────────────────────────────────────────────────

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(decimal id)
        {
            await _service.CancelOfferAsync(id, PermissionHelper.GetCurrentUsername(HttpContext));
            TempData["Success"] = "Offer cancelled.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Withdraw(decimal id)
        {
            try
            {
                await _service.WithdrawOfferAsync(id, PermissionHelper.GetCurrentUsername(HttpContext));
                TempData["Success"] = "Offer withdrawn. You can now create a new offer.";
            }
            catch (InvalidOperationException ex) { TempData["Error"] = ex.Message; }
            return RedirectToAction(nameof(Details), new { id });
        }

        // ── Hiring Decision ───────────────────────────────────────

        [RequireUrlPermission]
        public async Task<IActionResult> HiringDecision(decimal applicationId)
        {
            var model = await _service.GetForHiringDecisionAsync(applicationId);
            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HiringDecision(HrHiringDecisionFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _service.RecordHiringDecisionAsync(model, GetEmployeeId());
                TempData["Success"] = $"Hiring decision recorded: {model.Decision}.";
                var offer = await _context.HrOffers
                    .FirstOrDefaultAsync(o => o.JobApplicationFk == model.ApplicationFk);
                if (offer != null)
                    return RedirectToAction(nameof(Details), new { id = offer.Id });
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        private decimal? GetEmployeeId()
        {
            var session = PermissionHelper.GetUserSession(HttpContext);
            return session?.EmployeeId.HasValue == true ? (decimal?)session.EmployeeId.Value : null;
        }

        private async Task LoadApproverDropdownAsync(decimal offerId)
        {
            // Employees configured as department approvers
            var approverIds = _context.HrDepartmentApprovers
                .Where(a => a.IsActive)
                .Select(a => (int)a.EmployeeId)
                .Distinct()
                .ToList();

            // Line manager who initiated the requisition: offer → application → job posting → requisition
            var lineManagerDecimalId = await (
                from offer in _context.HrOffers
                join app   in _context.HrJobApplications on offer.JobApplicationFk equals app.Id
                join post  in _context.HrJobPostings     on app.JobPostingFK        equals post.Id
                join req   in _context.HrRequisitions    on post.RequisitionFK      equals req.Id
                where offer.Id == offerId && req.EmployeeFK.HasValue
                select req.EmployeeFK
            ).FirstOrDefaultAsync();

            if (lineManagerDecimalId.HasValue)
            {
                var lineManagerId = (int)lineManagerDecimalId.Value;
                if (!approverIds.Contains(lineManagerId))
                    approverIds.Add(lineManagerId);
            }

            // Exclude employees already added as approvers for this offer
            var alreadyAdded = await _context.HrOfferApprovals
                .Where(a => a.OfferFk == offerId && a.ApproverEmployeeFk.HasValue)
                .Select(a => (int)a.ApproverEmployeeFk!.Value)
                .ToListAsync();

            ViewBag.Approvers = new SelectList(
                _context.emp
                    .Where(e => approverIds.Contains(e.Id) && !alreadyAdded.Contains(e.Id))
                    .OrderBy(e => e.FirstName)
                    .Select(e => new { e.Id, Display = e.Code + " - " + e.FirstName + " " + e.LastName }),
                "Id", "Display");
        }
    }
}
