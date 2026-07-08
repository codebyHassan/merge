using AlignHR.Data;
using AlignHR.Models;
using AlignHR.Security;
using AlignHR.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AlignHR.Controllers
{
    public class HrOnboardingController : BaseController
    {
        private readonly IHrOnboardingService _service;
        private readonly IFileStorageService _fileStorage;
        private readonly ApplicationDbContext _context;

        public HrOnboardingController(
            IHrOnboardingService service,
            IFileStorageService fileStorage,
            ApplicationDbContext context)
        {
            _service = service;
            _fileStorage = fileStorage;
            _context = context;
        }

        [RequireUrlPermission]
        public async Task<IActionResult> Index(string? search, int? page)
        {
            ViewData["CurrentFilter"] = search;
            var model = await _service.GetPagedAsync(search, page ?? 1, 15);
            return View(model);
        }

        [RequireUrlPermission]
        public async Task<IActionResult> Details(decimal id)
        {
            var model = await _service.GetDetailsAsync(id);
            if (model == null) return NotFound();
            LoadDropdowns();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Start(decimal offerId)
        {
            try
            {
                var username = PermissionHelper.GetCurrentUsername(HttpContext);
                var id = await _service.StartOnboardingAsync(offerId, username);
                TempData["Success"] = "Onboarding started successfully.";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Details", "HrOffers", new { id = offerId });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateTask(HrTaskUpdateViewModel model)
        {
            try
            {
                await _service.UpdateTaskStatusAsync(model);
                TempData["Success"] = "Task updated.";
            }
            catch (InvalidOperationException ex) { TempData["Error"] = ex.Message; }
            return RedirectToAction(nameof(Details), new { id = model.OnboardingId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadDocument(HrOnboardingDocumentUploadViewModel model)
        {
            if (model.File == null || model.File.Length == 0)
            {
                TempData["Error"] = "Please select a file.";
                return RedirectToAction(nameof(Details), new { id = model.OnboardingId });
            }
            try
            {
                await _service.UploadDocumentAsync(model.OnboardingId, model.DocumentType, model.File);
                TempData["Success"] = "Document uploaded.";
            }
            catch (Exception ex) { TempData["Error"] = ex.Message; }
            return RedirectToAction(nameof(Details), new { id = model.OnboardingId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDocument(decimal documentId, decimal onboardingId)
        {
            await _service.DeleteDocumentAsync(documentId);
            TempData["Success"] = "Document deleted.";
            return RedirectToAction(nameof(Details), new { id = onboardingId });
        }

        public IActionResult DownloadDocument(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return NotFound();
            var absolute = _fileStorage.GetAbsolutePath(path);
            if (!System.IO.File.Exists(absolute)) return NotFound();
            return PhysicalFile(absolute, "application/octet-stream", System.IO.Path.GetFileName(absolute));
        }

        [RequireUrlPermission]
        public async Task<IActionResult> ConfirmJoining(decimal id)
        {
            var model = await _service.GetForJoiningAsync(id);
            if (model == null) return NotFound();
            LoadDropdowns();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmJoining(HrJoiningConfirmationFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var empId = GetEmployeeId();
                    if (!empId.HasValue)
                    {
                        ModelState.AddModelError("", "Could not resolve your employee record. Please link your user to an employee.");
                        LoadDropdowns();
                        return View(model);
                    }

                    await _service.ConfirmJoiningAsync(model, empId.Value);
                    TempData["Success"] = "Joining confirmed and employee record created successfully.";
                    return RedirectToAction(nameof(Details), new { id = model.OnboardingId });
                }
                catch (InvalidOperationException ex) { ModelState.AddModelError("", ex.Message); }
            }
            LoadDropdowns();
            return View(model);
        }

        private decimal? GetEmployeeId()
        {
            var session = PermissionHelper.GetUserSession(HttpContext);
            return session?.EmployeeId.HasValue == true ? (decimal?)session.EmployeeId.Value : null;
        }

        private void LoadDropdowns()
        {
            ViewBag.Departments = new SelectList(
                _context.department.OrderBy(d => d.Name).Select(d => new { d.Id, d.Name }),
                "Id", "Name");

            ViewBag.Locations = new SelectList(
                _context.location.OrderBy(l => l.Name).Select(l => new { l.Id, l.Name }),
                "Id", "Name");

            ViewBag.Employees = new SelectList(
                _context.emp.OrderBy(e => e.FirstName)
                    .Select(e => new { e.Id, Display = e.Code + " - " + e.FirstName + " " + e.LastName }),
                "Id", "Display");
        }
    }
}
