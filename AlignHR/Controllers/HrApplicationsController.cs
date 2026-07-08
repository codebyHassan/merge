using AlignHR.Data;
using AlignHR.Models;
using AlignHR.Security;
using AlignHR.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AlignHR.Controllers
{
    public class HrApplicationsController : BaseController
    {
        private readonly IHrApplicationService _service;
        private readonly ApplicationDbContext _context;

        public HrApplicationsController(IHrApplicationService service, ApplicationDbContext context)
        {
            _service = service;
            _context = context;
        }

        [RequireUrlPermission]
        public async Task<IActionResult> Index(decimal jobPostingId, string? search, int? page)
        {
            ViewData["CurrentFilter"] = search;
            ViewData["JobPostingId"] = jobPostingId;

            var posting = await _context.HrJobPostings.FirstOrDefaultAsync(p => p.Id == jobPostingId);
            ViewBag.JobCode = posting?.JobCode;
            ViewBag.JobTitle = posting?.JobTitle;

            ViewData["BreadcrumbIsRoot"] = false;
            ViewData["BreadcrumbParent"] = "Job Postings";
            ViewData["BreadcrumbParentUrl"] = Url.Action("Index", "HrJobPostings");

            var model = await _service.GetByJobPostingAsync(jobPostingId, search, page ?? 1, 15);
            return View(model);
        }

        [RequireUrlPermission]
        public async Task<IActionResult> Pipeline(decimal jobPostingId)
        {
            var model = await _service.GetPipelineAsync(jobPostingId);
            if (model == null) return NotFound();
            ViewData["BreadcrumbParent"] = "Candidates";
            ViewData["BreadcrumbParentUrl"] = Url.Action("Index", new { jobPostingId });
            return View(model);
        }

        [RequireUrlPermission]
        public async Task<IActionResult> Create(decimal jobPostingId)
        {
            var posting = await _context.HrJobPostings.FirstOrDefaultAsync(p => p.Id == jobPostingId);
            if (posting == null) return NotFound();

            ViewData["BreadcrumbParent"] = "Candidates";
            ViewData["BreadcrumbParentUrl"] = Url.Action("Index", new { jobPostingId });
            LoadCandidateDropdown();
            return View(new HrApplicationFormViewModel
            {
                JobPostingFK = jobPostingId,
                JobCode = posting.JobCode,
                JobTitle = posting.JobTitle,
                AppliedDate = DateTime.Today
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HrApplicationFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var actionBy = PermissionHelper.GetUserSession(HttpContext)?.EmployeeId;
                    var id = await _service.ApplyAsync(model, actionBy.HasValue ? (decimal?)actionBy.Value : null);
                    TempData["Success"] = "Application added successfully.";
                    return RedirectToAction(nameof(Details), new { id });
                }
                catch (InvalidOperationException ex) { ModelState.AddModelError("", ex.Message); }
            }
            LoadCandidateDropdown(model.CandidateFK);
            return View(model);
        }

        [RequireUrlPermission]
        public async Task<IActionResult> Details(decimal id)
        {
            var model = await _service.GetDetailsAsync(id);
            if (model == null) return NotFound();
            ViewData["BreadcrumbGrandparent"] = "Job Postings";
            ViewData["BreadcrumbGrandparentUrl"] = Url.Action("Index", "HrJobPostings");
            ViewData["BreadcrumbParent"] = "Applications";
            ViewData["BreadcrumbParentUrl"] = Url.Action("Index", new { jobPostingId = model.JobPostingFK });
            ViewData["Title"] = model.CandidateName;
            return View(model);
        }

        [RequireUrlPermission]
        public async Task<IActionResult> MoveStage(decimal id)
        {
            var model = await _service.GetForMoveStageAsync(id);
            if (model == null) return NotFound();
            ViewData["BreadcrumbGrandparent"] = "Applications";
            ViewData["BreadcrumbGrandparentUrl"] = Url.Action("Index", "HrApplications");
            ViewData["BreadcrumbParent"] = model.CandidateName;
            ViewData["BreadcrumbParentUrl"] = Url.Action("Details", new { id });
            await LoadStageDropdown(model.CurrentStageFK);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MoveStage(HrMoveStageViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var changedBy = PermissionHelper.GetUserSession(HttpContext)?.EmployeeId;
                    await _service.MoveStageAsync(model.ApplicationId, model.ToStageFK!.Value,
                        model.Comments, changedBy.HasValue ? (decimal?)changedBy.Value : null);
                    TempData["Success"] = "Application stage updated.";
                    return RedirectToAction(nameof(Details), new { id = model.ApplicationId });
                }
                catch (InvalidOperationException ex) { ModelState.AddModelError("", ex.Message); }
            }
            await LoadStageDropdown(model.CurrentStageFK);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Shortlist(decimal id)
        {
            try
            {
                var changedBy = PermissionHelper.GetUserSession(HttpContext)?.EmployeeId;
                await _service.ShortlistAsync(id, changedBy.HasValue ? (decimal?)changedBy.Value : null);
            }
            catch (InvalidOperationException ex) { TempData["Error"] = ex.Message; }
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(decimal id, string? rejectionReason)
        {
            try
            {
                var changedBy = PermissionHelper.GetUserSession(HttpContext)?.EmployeeId;
                await _service.RejectAsync(id, rejectionReason, changedBy.HasValue ? (decimal?)changedBy.Value : null);
            }
            catch (InvalidOperationException ex) { TempData["Error"] = ex.Message; }
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Withdraw(decimal id)
        {
            try
            {
                var changedBy = PermissionHelper.GetUserSession(HttpContext)?.EmployeeId;
                await _service.WithdrawAsync(id, changedBy.HasValue ? (decimal?)changedBy.Value : null);
                TempData["Success"] = "Application marked as withdrawn.";
            }
            catch (InvalidOperationException ex) { TempData["Error"] = ex.Message; }
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateNotes(decimal id, string? notes)
        {
            await _service.UpdateNotesAsync(id, notes);
            TempData["Success"] = "Notes saved.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(decimal id, decimal jobPostingId)
        {
            await _service.DeactivateAsync(id);
            TempData["Success"] = "Application deactivated.";
            return RedirectToAction(nameof(Index), new { jobPostingId });
        }

        private void LoadCandidateDropdown(decimal? selected = null)
        {
            ViewBag.Candidates = new SelectList(
                _context.HrCandidates
                    .Where(c => !c.IsDeleted)
                    .OrderBy(c => c.FirstName)
                    .Select(c => new { c.Id, Display = c.FirstName + " " + c.LastName + " — " + c.Email }),
                "Id", "Display",
                selected);
        }

        private async Task LoadStageDropdown(decimal? excludeId = null)
        {
            var stages = await _context.HrApplicationStages
                .Where(s => s.IsActive && s.Id != excludeId)
                .OrderBy(s => s.StageOrder)
                .ToListAsync();
            ViewBag.Stages = new SelectList(stages, "Id", "StageName");
        }
    }
}
