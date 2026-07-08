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
        private readonly ApplicationDbContext _context;

        public HrOnboardingController(
            IHrOnboardingService service,
            ApplicationDbContext context)
        {
            _service = service;
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
            var existing = await _context.HrOnboardings
                .Include(o => o.Offer)
                    .ThenInclude(of => of!.JobApplication)
                        .ThenInclude(a => a!.JobPosting)
                .FirstOrDefaultAsync(o => o.OfferId == offerId);

            if (existing != null)
            {
                var existingCandidateId = existing.Offer?.JobApplication?.CandidateFK;
                var existingHireType = await GetHireTypeFromPostingAsync(existing.Offer?.JobApplication?.JobPosting?.RequisitionFK);
                TempData["Success"] = "Onboarding already started. Please create the employee profile.";
                return RedirectToAction("Create", "Employees", new { candidateId = existingCandidateId, onboardingId = existing.Id, hireType = existingHireType });
            }

            try
            {
                var username = PermissionHelper.GetCurrentUsername(HttpContext);
                var onboardingId = await _service.StartOnboardingAsync(offerId, username);

                var offer = await _context.HrOffers
                    .Include(o => o.JobApplication)
                        .ThenInclude(a => a!.JobPosting)
                    .FirstOrDefaultAsync(o => o.Id == offerId);
                var candidateId = offer?.JobApplication?.CandidateFK;
                var hireType = await GetHireTypeFromPostingAsync(offer?.JobApplication?.JobPosting?.RequisitionFK);

                TempData["Success"] = "Onboarding started. Please create the employee profile.";
                return RedirectToAction("Create", "Employees", new { candidateId, onboardingId, hireType });
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

        private decimal? GetEmployeeId()
        {
            var session = PermissionHelper.GetUserSession(HttpContext);
            return session?.EmployeeId.HasValue == true ? (decimal?)session.EmployeeId.Value : null;
        }

        private async Task<string> GetHireTypeFromPostingAsync(decimal? requisitionFk)
        {
            if (!requisitionFk.HasValue) return "New";
            var req = await _context.HrRequisitions
                .FirstOrDefaultAsync(r => r.Id == requisitionFk.Value);
            if (req == null) return "New";
            return req.RequisitionType?.Trim().ToLower() switch
            {
                "replacement" or "replace" => "Replacement",
                _ => "New"
            };
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
