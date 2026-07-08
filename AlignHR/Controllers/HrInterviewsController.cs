using AlignHR.Data;
using AlignHR.Models;
using AlignHR.Security;
using AlignHR.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AlignHR.Controllers
{
    public class HrInterviewsController : BaseController
    {
        private readonly IHrInterviewService _service;
        private readonly ApplicationDbContext _context;

        public HrInterviewsController(IHrInterviewService service, ApplicationDbContext context)
        {
            _service = service;
            _context = context;
        }

        [RequireUrlPermission]
        public async Task<IActionResult> Schedule(decimal applicationId)
        {
            var model = await _service.GetForScheduleAsync(applicationId);
            if (model == null) return NotFound();
            ViewData["BreadcrumbGrandparent"] = "Applications";
            ViewData["BreadcrumbGrandparentUrl"] = Url.Action("Index", "HrApplications", new { jobPostingId = model.JobPostingId });
            ViewData["BreadcrumbParent"] = model.CandidateName;
            ViewData["BreadcrumbParentUrl"] = Url.Action("Details", "HrApplications", new { id = applicationId });
            await LoadRoundDropdown(applicationId);
            await LoadSchedulePanelAsync(applicationId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Schedule(HrScheduleFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var empId = GetCurrentEmployeeId();
                    if (!empId.HasValue)
                    {
                        ModelState.AddModelError("", "Your account is not linked to an employee record.");
                    }
                    else
                    {
                        await _service.ScheduleInterviewAsync(model, empId.Value);
                        TempData["Success"] = "Interview scheduled successfully.";
                        return RedirectToAction("Details", "HrApplications", new { id = model.ApplicationId });
                    }
                }
                catch (InvalidOperationException ex) { ModelState.AddModelError("", ex.Message); }
            }
            await LoadRoundDropdown(model.ApplicationId);
            await LoadSchedulePanelAsync(model.ApplicationId);
            return View(model);
        }

        // ── Edit Interview ───────────────────────────────────────

        [RequireUrlPermission]
        public async Task<IActionResult> Edit(decimal id)
        {
            var model = await _service.GetForEditAsync(id);
            if (model == null) return NotFound();
            ViewData["BreadcrumbGrandparent"] = "Applications";
            ViewData["BreadcrumbGrandparentUrl"] = Url.Action("Index", "HrApplications", new { jobPostingId = model.JobPostingId });
            ViewData["BreadcrumbParent"] = model.CandidateName;
            ViewData["BreadcrumbParentUrl"] = Url.Action("Details", "HrApplications", new { id = model.ApplicationId });
            await LoadRoundDropdown(model.ApplicationId, model.Id);
            await LoadSchedulePanelAsync(model.ApplicationId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(HrInterviewEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _service.UpdateScheduleAsync(model);
                    TempData["Success"] = "Interview updated successfully.";
                    return RedirectToAction("Details", "HrApplications", new { id = model.ApplicationId });
                }
                catch (InvalidOperationException ex) { ModelState.AddModelError("", ex.Message); }
            }
            await LoadRoundDropdown(model.ApplicationId, model.Id);
            await LoadSchedulePanelAsync(model.ApplicationId);
            return View(model);
        }

        // ── Interview Details ────────────────────────────────────

        [RequireUrlPermission]
        public async Task<IActionResult> Details(decimal id)
        {
            var model = await _service.GetDetailsAsync(id);
            if (model == null) return NotFound();
            ViewData["BreadcrumbGrandparent"] = "Applications";
            ViewData["BreadcrumbGrandparentUrl"] = Url.Action("Index", "HrApplications", new { jobPostingId = model.JobPostingId });
            ViewData["BreadcrumbParent"] = model.CandidateName;
            ViewData["BreadcrumbParentUrl"] = Url.Action("Details", "HrApplications", new { id = model.ApplicationId });
            LoadInterviewerDropdown();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPanelist(decimal scheduleId, decimal employeeId)
        {
            try
            {
                await _service.AddPanelistAsync(scheduleId, employeeId);
                TempData["Success"] = "Panelist added.";
            }
            catch (InvalidOperationException ex) { TempData["Error"] = ex.Message; }
            return RedirectToAction(nameof(Details), new { id = scheduleId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemovePanelist(decimal panelId, decimal scheduleId)
        {
            await _service.RemovePanelistAsync(panelId);
            TempData["Success"] = "Panelist removed.";
            return RedirectToAction(nameof(Details), new { id = scheduleId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(decimal id)
            => await ChangeStatus(id, "Completed");

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(decimal id)
            => await ChangeStatus(id, "Cancelled");

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NoShow(decimal id)
            => await ChangeStatus(id, "NoShow");

        // ── Feedback ─────────────────────────────────────────────

        [RequireUrlPermission]
        public async Task<IActionResult> SubmitFeedback(decimal scheduleId)
        {
            var model = await _service.GetFeedbackFormAsync(scheduleId);
            if (model == null) return NotFound();
            ViewData["BreadcrumbGrandparent"] = model.CandidateName;
            ViewData["BreadcrumbGrandparentUrl"] = Url.Action("Details", "HrApplications", new { id = model.ApplicationId });
            ViewData["BreadcrumbParent"] = "Interview";
            ViewData["BreadcrumbParentUrl"] = Url.Action(nameof(Details), new { id = scheduleId });
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitFeedback(HrFeedbackFormViewModel model)
        {
            var empId = GetCurrentEmployeeId();
            if (!empId.HasValue)
            {
                TempData["Error"] = "Your account is not linked to an employee record.";
                return RedirectToAction("Details", "HrApplications", new { id = model.ApplicationId });
            }

            foreach (var cs in model.CriteriaScores)
            {
                if (cs.Score > cs.MaxScore)
                    ModelState.AddModelError("",
                        $"Score for \"{cs.CriteriaName}\" ({cs.Score}) cannot exceed the maximum ({cs.MaxScore}).");
                if (cs.Score < 0)
                    ModelState.AddModelError("",
                        $"Score for \"{cs.CriteriaName}\" cannot be negative.");
            }

            if (ModelState.IsValid)
            {
                var isSubmit = Request.Form["btnAction"].ToString() == "submit";
                await _service.SubmitFeedbackAsync(model, empId.Value, isSubmit);
                TempData["Success"] = isSubmit ? "Feedback submitted successfully." : "Draft saved.";
                return RedirectToAction("Details", "HrApplications", new { id = model.ApplicationId });
            }
            return View(model);
        }

        [RequireUrlPermission]
        public async Task<IActionResult> FeedbackDetails(decimal feedbackId)
        {
            var model = await _service.GetFeedbackDetailsAsync(feedbackId);
            if (model == null) return NotFound();
            return View(model);
        }

        // ── Line Manager Feedback ────────────────────────────────

        [RequireUrlPermission]
        public async Task<IActionResult> LineManagerFeedback(decimal scheduleId)
        {
            var model = await BuildLMFeedbackAsync(scheduleId);
            if (model == null) return NotFound();

            ViewData["BreadcrumbGrandparent"] = "Requisitions";
            ViewData["BreadcrumbGrandparentUrl"] = Url.Action("Index", "HrRequisitions");
            ViewData["BreadcrumbParent"] = model.PositionTitle ?? "Requisition";
            ViewData["BreadcrumbParentUrl"] = Url.Action("Details", "HrRequisitions", new { id = model.RequisitionId });
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LineManagerFeedback(HrLMFeedbackViewModel model)
        {
            if (!ModelState.IsValid)
                return View(await RebuildLMFeedbackAsync(model));

            var empId = GetCurrentEmployeeId();
            if (!empId.HasValue)
            {
                ModelState.AddModelError("", "Your account is not linked to an employee record.");
                return View(await RebuildLMFeedbackAsync(model));
            }

            var feedbackForm = new HrFeedbackFormViewModel
            {
                ScheduleId      = model.ScheduleId,
                Recommendation  = model.Recommendation ?? string.Empty,
                OverallScore    = model.OverallScore,
                Strengths       = model.Strengths,
                Concerns        = model.Concerns,
                Comments        = model.Comments,
                CriteriaScores  = model.CriteriaScores
            };

            var isSubmit = Request.Form["btnAction"].ToString() == "submit";

            await _service.SubmitFeedbackAsync(feedbackForm, empId.Value, isSubmit);
            TempData["Success"] = isSubmit ? "Feedback submitted successfully." : "Feedback saved as draft.";
            return RedirectToAction(nameof(LineManagerFeedback), new { scheduleId = model.ScheduleId });
        }

        private async Task<HrLMFeedbackViewModel?> BuildLMFeedbackAsync(decimal scheduleId)
        {
            var schedule = await _context.HrInterviewSchedules
                .Include(s => s.InterviewRound)
                .Include(s => s.JobApplication)
                    .ThenInclude(a => a!.Candidate)
                .Include(s => s.JobApplication)
                    .ThenInclude(a => a!.JobPosting)
                .Include(s => s.Panel)
                .Include(s => s.Feedback)
                    .ThenInclude(f => f.Scores)
                        .ThenInclude(sc => sc.EvaluationCriteria)
                .FirstOrDefaultAsync(s => s.Id == scheduleId);

            if (schedule == null) return null;

            var app     = schedule.JobApplication;
            var posting = app?.JobPosting;

            // Trace back to requisition (for breadcrumb only)
            HrRequisition? req = null;
            if (posting?.RequisitionFK.HasValue == true)
                req = await _context.HrRequisitions
                    .FirstOrDefaultAsync(r => r.Id == posting.RequisitionFK.Value);

            // Panel member names
            var panelEmpIds = schedule.Panel.Select(p => (int)p.EmployeeFk).ToList();
            var panelEmps   = await _context.emp.Where(e => panelEmpIds.Contains(e.Id))
                .ToDictionaryAsync(e => e.Id, e => $"{e.Code} - {e.FirstName} {e.LastName}");
            var panelNames  = schedule.Panel
                .Select(p => panelEmps.TryGetValue((int)p.EmployeeFk, out var n) ? n : null)
                .Where(n => n != null).ToList()!;

            // Existing feedback (one per interview)
            var existing = schedule.Feedback.FirstOrDefault();

            // Criteria for scorecard — per interview round
            var criteria = schedule.InterviewRoundId > 0
                ? await _context.HrEvaluationCriterias
                    .Where(c => c.InterviewRoundId == schedule.InterviewRoundId)
                    .OrderBy(c => c.CriteriaName)
                    .ToListAsync()
                : new List<HrEvaluationCriteria>();

            var candidate = app?.Candidate;

            return new HrLMFeedbackViewModel
            {
                RequisitionId     = req?.Id ?? 0,
                PositionTitle     = req?.PositionTitle ?? posting?.JobTitle,

                ScheduleId        = schedule.Id,
                ApplicationId     = schedule.JobApplicationId,
                JobPostingId      = posting?.Id,
                CandidateName     = candidate != null
                    ? $"{candidate.FirstName} {candidate.LastName}".Trim()
                    : null,
                RoundName         = schedule.InterviewRound?.RoundName,
                ScheduledDateTime = schedule.ScheduledDateTime,
                InterviewStatus   = schedule.Status,
                PanelMembers      = panelNames!,

                FeedbackId        = existing?.Id ?? 0,
                IsSubmitted       = existing?.IsSubmitted ?? false,
                Recommendation    = existing?.Recommendation,
                OverallScore      = existing?.OverallScore,
                Strengths         = existing?.Strengths,
                Concerns          = existing?.Concerns,
                Comments          = existing?.Comments,
                CriteriaScores    = criteria.Select(c => new HrCriteriaScoreInputViewModel
                {
                    CriteriaId   = c.Id,
                    CriteriaName = c.CriteriaName,
                    MaxScore     = c.MaxScore,
                    Score        = existing?.Scores.FirstOrDefault(s => s.EvaluationCriteriaFk == c.Id)?.Score ?? 0
                }).ToList()
            };
        }

        private async Task<HrLMFeedbackViewModel> RebuildLMFeedbackAsync(HrLMFeedbackViewModel posted)
        {
            var rebuilt = await BuildLMFeedbackAsync(posted.ScheduleId) ?? posted;
            rebuilt.Recommendation = posted.Recommendation;
            rebuilt.OverallScore   = posted.OverallScore;
            rebuilt.Strengths      = posted.Strengths;
            rebuilt.Concerns       = posted.Concerns;
            rebuilt.Comments       = posted.Comments;
            rebuilt.CriteriaScores = posted.CriteriaScores;
            return rebuilt;
        }

        // ── Helpers ──────────────────────────────────────────────

        private async Task<IActionResult> ChangeStatus(decimal id, string status)
        {
            try
            {
                await _service.ChangeStatusAsync(id, status);
                TempData["Success"] = $"Interview marked as {status}.";
            }
            catch (InvalidOperationException ex) { TempData["Error"] = ex.Message; }
            return RedirectToAction(nameof(Details), new { id });
        }

        private decimal? GetCurrentEmployeeId()
        {
            var session = PermissionHelper.GetUserSession(HttpContext);
            return session?.EmployeeId.HasValue == true ? (decimal?)session.EmployeeId.Value : null;
        }

        private async Task LoadRoundDropdown(decimal applicationId, decimal? editingScheduleId = null)
        {
            var app = await _context.HrJobApplications.FirstOrDefaultAsync(a => a.Id == applicationId);
            if (app?.JobPostingFK.HasValue == true)
            {
                // Round IDs already in use for this application (Scheduled or Rescheduled),
                // excluding the schedule currently being edited so its own round stays selectable.
                var usedRoundIds = await _context.HrInterviewSchedules
                    .Where(s => s.JobApplicationId == applicationId
                             && (s.Status == "Scheduled" || s.Status == "Rescheduled")
                             && s.Id != (editingScheduleId ?? 0))
                    .Select(s => s.InterviewRoundId)
                    .ToListAsync();

                var rounds = await _context.HrInterviewRounds
                    .Where(r => r.JobPostingId == app.JobPostingFK.Value
                             && r.IsActive
                             && !usedRoundIds.Contains(r.Id))
                    .OrderBy(r => r.RoundOrder)
                    .ToListAsync();

                ViewBag.Rounds = new SelectList(rounds, "Id", "RoundName");
            }
            else
            {
                ViewBag.Rounds = new SelectList(Enumerable.Empty<object>(), "Id", "RoundName");
            }
        }

        private void LoadInterviewerDropdown(decimal? selected = null)
        {
            ViewBag.Interviewers = new SelectList(
                _context.emp.OrderBy(e => e.FirstName)
                    .Select(e => new { e.Id, Display = e.Code + " - " + e.FirstName + " " + e.LastName }),
                "Id", "Display", selected);
        }

        private async Task LoadSchedulePanelAsync(decimal applicationId)
        {
            // Walk: application → posting → requisition
            decimal? requisitionId = null;
            int? hiringDeptId = null;

            var app = await _context.HrJobApplications
                .Include(a => a.JobPosting)
                .FirstOrDefaultAsync(a => a.Id == applicationId);

            if (app?.JobPosting?.RequisitionFK.HasValue == true)
            {
                requisitionId = app.JobPosting.RequisitionFK.Value;
                var req = await _context.HrRequisitions
                    .FirstOrDefaultAsync(r => r.Id == requisitionId.Value);
                if (req?.DepartmentFK.HasValue == true)
                    hiringDeptId = (int)req.DepartmentFK.Value;
            }

            // Get the recruiter assigned to this requisition (latest assignment)
            int? assignedRecruiterId = null;
            if (requisitionId.HasValue)
            {
                var assignment = await _context.HrRequisitionAssignments
                    .Where(a => a.RequisitionFk == requisitionId.Value && a.RecruiterEmployeeFK.HasValue)
                    .OrderByDescending(a => a.AssignedDate)
                    .FirstOrDefaultAsync();
                if (assignment?.RecruiterEmployeeFK.HasValue == true)
                    assignedRecruiterId = (int)assignment.RecruiterEmployeeFK.Value;
            }

            // Fetch hiring dept employees
            var hiringEmps = hiringDeptId.HasValue
                ? await _context.emp.Include(e => e.Department)
                    .Where(e => e.DepartmentFk == hiringDeptId.Value)
                    .OrderBy(e => e.FirstName)
                    .ToListAsync()
                : new List<Employee>();

            // Fetch the assigned recruiter (if not already in hiring dept)
            Employee? recruiter = null;
            if (assignedRecruiterId.HasValue && !hiringEmps.Any(e => e.Id == assignedRecruiterId.Value))
            {
                recruiter = await _context.emp.Include(e => e.Department)
                    .FirstOrDefaultAsync(e => e.Id == assignedRecruiterId.Value);
            }

            // Build grouped dictionary: hiring dept first, then recruiter's dept
            var groups = new Dictionary<string, List<SelectListItem>>();

            if (hiringEmps.Any())
            {
                var deptName = hiringEmps.First().Department?.Name ?? "Hiring Department";
                groups[deptName] = hiringEmps.Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = $"{e.Code} - {e.FirstName} {e.LastName}"
                }).ToList();
            }

            if (recruiter != null)
            {
                var recruitDeptName = recruiter.Department?.Name ?? "Recruitment";
                if (!groups.ContainsKey(recruitDeptName))
                    groups[recruitDeptName] = new List<SelectListItem>();
                groups[recruitDeptName].Add(new SelectListItem
                {
                    Value = recruiter.Id.ToString(),
                    Text = $"{recruiter.Code} - {recruiter.FirstName} {recruiter.LastName}"
                });
            }

            // Fallback: show all employees if nothing resolved
            if (!groups.Any())
            {
                var all = await _context.emp.Include(e => e.Department)
                    .OrderBy(e => e.Department!.Name).ThenBy(e => e.FirstName)
                    .ToListAsync();
                foreach (var grp in all.GroupBy(e => e.Department?.Name ?? "All Employees"))
                {
                    groups[grp.Key] = grp.Select(e => new SelectListItem
                    {
                        Value = e.Id.ToString(),
                        Text = $"{e.Code} - {e.FirstName} {e.LastName}"
                    }).ToList();
                }
            }

            ViewBag.PanelDeptGroups = groups;
        }
    }
}
