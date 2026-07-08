using AlignHR.Data;
using AlignHR.Models;
using AlignHR.Security;
using AlignHR.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AlignHR.Controllers
{
    public class HrRequisitionsController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IHrRequisitionService _requisitionService;
        private readonly IHrRequisitionWorkflowService _workflowService;

        public HrRequisitionsController(
            ApplicationDbContext context,
            IHrRequisitionService requisitionService,
            IHrRequisitionWorkflowService workflowService)
        {
            _context = context;
            _requisitionService = requisitionService;
            _workflowService = workflowService;
        }

        [RequireUrlPermission]
        public async Task<IActionResult> Index(string? search, int? page)
        {
            const int pageSize = 15;
            var pageNumber = page ?? 1;

            ViewData["CurrentFilter"] = search;
            var model = await _requisitionService.GetPagedAsync(search, pageNumber, pageSize);
            return View(model);
        }

        [RequireUrlPermission]
        public IActionResult Create()
        {
            LoadDropdowns();
            return View(new HrRequisitionFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HrRequisitionFormViewModel model, string command)
        {
            var submit = command == "Submit";

            model.EmployeeFK = GetActionByEmployeeId();

            if (ModelState.IsValid)
            {
                var username = PermissionHelper.GetCurrentUsername(HttpContext);
                try
                {
                    await _requisitionService.CreateAsync(model, username, GetActionByEmployeeId(), submit);
                    TempData["Success"] = submit
                        ? "Requisition submitted successfully."
                        : "Requisition saved as draft.";

                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            LoadDropdowns(model.DepartmentFK, model.RequisitionType, model.Nature,
                model.ReplacementEmployeeId, model.TransferFromDepartmentId);
            return View(model);
        }

        [RequireUrlPermission]
        public async Task<IActionResult> Edit(decimal id)
        {
            var model = await _requisitionService.GetForEditAsync(id);
            if (model == null)
                return NotFound();

            if (model.Status == "Submitted")
            {
                TempData["Error"] = "Submitted requisitions cannot be edited.";
                return RedirectToAction(nameof(Index));
            }

            LoadDropdowns(model.DepartmentFK, model.RequisitionType, model.Nature,
                model.ReplacementEmployeeId, model.TransferFromDepartmentId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, HrRequisitionFormViewModel model, string command)
        {
            if (id != model.Id)
                return NotFound();

            var submit = command == "Submit";

            if (ModelState.IsValid)
            {
                try
                {
                    var username = PermissionHelper.GetCurrentUsername(HttpContext);
                    await _requisitionService.UpdateAsync(id, model, username, GetActionByEmployeeId(), submit);
                    TempData["Success"] = submit
                        ? "Requisition submitted successfully."
                        : "Requisition updated successfully.";

                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException ex)
                {
                    TempData["Error"] = ex.Message;
                    return RedirectToAction(nameof(Index));
                }
            }

            LoadDropdowns(model.DepartmentFK, model.RequisitionType, model.Nature,
                model.ReplacementEmployeeId, model.TransferFromDepartmentId);
            return View(model);
        }

        [RequireUrlPermission]
        public async Task<IActionResult> Inbox()
        {
            var employeeId = GetActionByEmployeeId();
            if (!employeeId.HasValue || employeeId.Value <= 0)
            {
                TempData["Error"] = "Your user account is not linked to an employee record.";
                return RedirectToAction(nameof(Index));
            }

            var model = await _workflowService.GetInboxAsync(employeeId.Value);
            return View(model);
        }

        [RequireUrlPermission]
        public async Task<IActionResult> RecruiterInbox()
        {
            var employeeId = GetActionByEmployeeId();
            if (!employeeId.HasValue || employeeId.Value <= 0)
            {
                TempData["Error"] = "Your user account is not linked to an employee record.";
                return RedirectToAction(nameof(Index));
            }

            // Departments where this employee is the active RecruitmentHead approver
            var unitDeptIds = await _context.HrDepartmentApprovers
                .Where(a => a.ApproverType == "RecruitmentHead"
                    && a.EmployeeId == employeeId.Value
                    && a.IsActive)
                .Select(a => a.DepartmentId)
                .Distinct()
                .ToListAsync();

            if (!unitDeptIds.Any())
            {
                ViewBag.NoUnit = true;
                return View(new List<HrRecruiterInboxItemViewModel>());
            }

            var requisitions = await _context.HrRequisitions
                .Where(r => r.DepartmentFK.HasValue
                    && unitDeptIds.Contains(r.DepartmentFK.Value)
                    && r.Status == "Approved")
                .OrderByDescending(r => r.CreatedOn)
                .ToListAsync();

            var requisitionIds = requisitions.Select(r => r.Id).ToList();
            var assignedIds = await _context.HrRequisitionAssignments
                .Where(a => a.RequisitionFk.HasValue && requisitionIds.Contains(a.RequisitionFk.Value))
                .Select(a => a.RequisitionFk!.Value)
                .ToListAsync();

            var deptIds = requisitions.Where(r => r.DepartmentFK.HasValue)
                .Select(r => (int)r.DepartmentFK!.Value).Distinct().ToList();
            var deptNames = await _context.department
                .Where(d => deptIds.Contains(d.Id))
                .ToDictionaryAsync(d => d.Id, d => d.Name);

            var items = requisitions.Select(r => new HrRecruiterInboxItemViewModel
            {
                RequisitionId = r.Id,
                RequisitionNo = r.RequisitionNo,
                PositionTitle = r.PositionTitle,
                DepartmentName = r.DepartmentFK.HasValue && deptNames.TryGetValue((int)r.DepartmentFK.Value, out var dn) ? dn : null,
                IsAssigned = assignedIds.Contains(r.Id),
                CreatedOn = r.CreatedOn
            }).ToList();

            return View(items);
        }

        [RequireUrlPermission]
        public async Task<IActionResult> MyAssignments()
        {
            var employeeId = GetActionByEmployeeId();
            if (!employeeId.HasValue || employeeId.Value <= 0)
            {
                TempData["Error"] = "Your user account is not linked to an employee record.";
                return RedirectToAction(nameof(Index));
            }

            var assignments = await _context.HrRequisitionAssignments
                .Where(a => a.RecruiterEmployeeFK == employeeId.Value)
                .OrderByDescending(a => a.AssignedDate)
                .ToListAsync();

            var requisitionIds = assignments
                .Where(a => a.RequisitionFk.HasValue)
                .Select(a => a.RequisitionFk!.Value)
                .Distinct()
                .ToList();

            var requisitions = await _context.HrRequisitions
                .Where(r => requisitionIds.Contains(r.Id))
                .ToListAsync();

            var deptIds = requisitions
                .Where(r => r.DepartmentFK.HasValue)
                .Select(r => (int)r.DepartmentFK!.Value)
                .Distinct().ToList();
            var deptNames = await _context.department
                .Where(d => deptIds.Contains(d.Id))
                .ToDictionaryAsync(d => d.Id, d => d.Name);

            var assignedByIds = assignments
                .Where(a => a.AssignedByEmployeeId.HasValue)
                .Select(a => (int)a.AssignedByEmployeeId!.Value)
                .Distinct().ToList();
            var assignedByNames = await _context.emp
                .Where(e => assignedByIds.Contains(e.Id))
                .ToDictionaryAsync(e => e.Id, e => e.Code + " - " + e.FirstName + " " + e.LastName);

            var skills = await _context.HrRequisitionSkills
                .Where(s => requisitionIds.Contains(s.RequisitionId))
                .ToListAsync();

            var offerings = await _context.HrRequisitionOfferings
                .Where(o => requisitionIds.Contains(o.RequisitionId))
                .ToListAsync();

            var model = assignments.Select(a =>
            {
                var req = a.RequisitionFk.HasValue
                    ? requisitions.FirstOrDefault(r => r.Id == a.RequisitionFk.Value)
                    : null;
                var reqId = a.RequisitionFk ?? 0;
                return new HrMyAssignmentViewModel
                {
                    AssignmentId          = a.Id,
                    RequisitionId         = reqId,
                    RequisitionNo         = req?.RequisitionNo,
                    PositionTitle         = req?.PositionTitle,
                    RequisitionType       = req?.RequisitionType,
                    Nature                = req?.Nature,
                    BudgetAmountPerMonth  = req?.BudgetAmountPerMonth,
                    DepartmentName        = req?.DepartmentFK.HasValue == true
                        && deptNames.TryGetValue((int)req.DepartmentFK.Value, out var dn) ? dn : null,
                    AssignedDate          = a.AssignedDate,
                    AssignedByName        = a.AssignedByEmployeeId.HasValue
                        && assignedByNames.TryGetValue((int)a.AssignedByEmployeeId.Value, out var abn) ? abn : null,
                    Notes                 = a.Notes,
                    RequisitionStatus     = req?.Status,
                    Skills                = skills.Where(s => s.RequisitionId == reqId)
                        .Select(s => new SkillItem { SkillName = s.SkillName, YearsExperience = s.YearsExperience, IsMandatory = s.IsMandatory })
                        .ToList(),
                    Offerings             = offerings.Where(o => o.RequisitionId == reqId)
                        .Select(o => new OfferingItem { OfferingName = o.OfferingName, Description = o.Description })
                        .ToList()
                };
            }).ToList();

            return View(model);
        }

        [RequireUrlPermission]
        public async Task<IActionResult> MyAssignmentDetail(decimal id)
        {
            var assignment = await _context.HrRequisitionAssignments.FindAsync(id);
            if (assignment == null) return NotFound();

            var req = assignment.RequisitionFk.HasValue
                ? await _context.HrRequisitions.FindAsync(assignment.RequisitionFk.Value)
                : null;

            var deptName = req?.DepartmentFK.HasValue == true
                ? (await _context.department.FindAsync((int)req.DepartmentFK.Value))?.Name
                : null;

            var assignedByName = assignment.AssignedByEmployeeId.HasValue
                ? (await _context.emp.FindAsync((int)assignment.AssignedByEmployeeId.Value)) is { } e
                    ? e.Code + " - " + e.FirstName + " " + e.LastName : null
                : null;

            var reqId = assignment.RequisitionFk ?? 0;

            var skills = await _context.HrRequisitionSkills
                .Where(s => s.RequisitionId == reqId)
                .ToListAsync();

            var offerings = await _context.HrRequisitionOfferings
                .Where(o => o.RequisitionId == reqId)
                .ToListAsync();

            var model = new HrMyAssignmentViewModel
            {
                AssignmentId         = assignment.Id,
                RequisitionId        = reqId,
                RequisitionNo        = req?.RequisitionNo,
                PositionTitle        = req?.PositionTitle,
                RequisitionType      = req?.RequisitionType,
                Nature               = req?.Nature,
                BudgetAmountPerMonth = req?.BudgetAmountPerMonth,
                DepartmentName       = deptName,
                AssignedDate         = assignment.AssignedDate,
                AssignedByName       = assignedByName,
                Notes                = assignment.Notes,
                RequisitionStatus    = req?.Status,
                Skills               = skills.Select(s => new SkillItem { SkillName = s.SkillName, YearsExperience = s.YearsExperience, IsMandatory = s.IsMandatory }).ToList(),
                Offerings            = offerings.Select(o => new OfferingItem { OfferingName = o.OfferingName, Description = o.Description }).ToList()
            };

            return View(model);
        }

        [RequireUrlPermission]
        public async Task<IActionResult> Action(decimal id)
        {
            var model = await _workflowService.GetActionViewAsync(id);
            if (model == null)
                return NotFound();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(decimal workflowInstanceId, string? comments)
        {
            return await RunWorkflowActionAsync(
                workflowInstanceId,
                comments,
                (id, actionBy, notes) => _workflowService.ApproveAsync(id, actionBy, notes),
                "Requisition approved.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(decimal workflowInstanceId, string? comments)
        {
            return await RunWorkflowActionAsync(
                workflowInstanceId,
                comments,
                (id, actionBy, notes) => _workflowService.RejectAsync(id, actionBy, notes),
                "Requisition rejected.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendBack(decimal workflowInstanceId, string? comments)
        {
            return await RunWorkflowActionAsync(
                workflowInstanceId,
                comments,
                (id, actionBy, notes) => _workflowService.SendBackAsync(id, actionBy, notes),
                "Requisition sent back.");
        }

        [RequireUrlPermission]
        public async Task<IActionResult> Details(decimal id)
        {
            var model = await _requisitionService.GetDetailsAsync(id);
            if (model == null)
                return NotFound();

            ViewBag.DepartmentName = model.DepartmentFK.HasValue
                ? _context.department.Find((int)model.DepartmentFK.Value)?.Name
                : null;

            ViewBag.EmployeeName = model.EmployeeFK.HasValue
                ? _context.emp.Where(e => e.Id == (int)model.EmployeeFK.Value)
                    .Select(e => e.Code + " - " + e.FirstName + " " + e.LastName)
                    .FirstOrDefault()
                : null;

            ViewBag.IsAssigned = await _context.HrRequisitionAssignments
                .AnyAsync(a => a.RequisitionFk == id);

            ViewBag.ReplacementEmployeeName = model.ReplacementEmployeeId.HasValue
                ? _context.emp.Where(e => e.Id == (int)model.ReplacementEmployeeId.Value)
                    .Select(e => e.Code + " - " + e.FirstName + " " + e.LastName)
                    .FirstOrDefault()
                : null;

            ViewBag.TransferFromDepartmentName = model.TransferFromDepartmentId.HasValue
                ? _context.department.Find((int)model.TransferFromDepartmentId.Value)?.Name
                : null;

            return View(model);
        }

        [RequireUrlPermission]
        public async Task<IActionResult> MyInterviews()
        {
            var currentEmpId = GetActionByEmployeeId();

            // Load only schedules where the current user is on the panel
            var schedules = await _context.HrInterviewSchedules
                .Include(s => s.InterviewRound)
                .Include(s => s.Feedback)
                .Include(s => s.Panel)
                .Include(s => s.JobApplication)
                    .ThenInclude(a => a!.Candidate)
                .Include(s => s.JobApplication)
                    .ThenInclude(a => a!.JobPosting)
                .Where(s => currentEmpId.HasValue && s.Panel.Any(p => p.EmployeeFk == currentEmpId.Value))
                .OrderByDescending(s => s.ScheduledDateTime)
                .ToListAsync();

            // Collect all requisition IDs from postings
            var requisitionIds = schedules
                .Select(s => s.JobApplication?.JobPosting?.RequisitionFK)
                .Where(id => id.HasValue).Select(id => id!.Value)
                .Distinct().ToList();

            var requisitions = requisitionIds.Any()
                ? await _context.HrRequisitions
                    .Where(r => requisitionIds.Contains(r.Id))
                    .ToListAsync()
                : new List<HrRequisition>();

            var deptIds = requisitions.Where(r => r.DepartmentFK.HasValue)
                .Select(r => (int)r.DepartmentFK!.Value).Distinct().ToList();
            var deptNames = deptIds.Any()
                ? await _context.department
                    .Where(d => deptIds.Contains(d.Id))
                    .ToDictionaryAsync(d => d.Id, d => d.Name)
                : new Dictionary<int, string>();

            var model = schedules.Select(s =>
            {
                var app     = s.JobApplication;
                var posting = app?.JobPosting;
                var req     = posting?.RequisitionFK.HasValue == true
                    ? requisitions.FirstOrDefault(r => r.Id == posting.RequisitionFK.Value)
                    : null;

                return new HrMyInterviewItemViewModel
                {
                    ScheduleId        = s.Id,
                    RequisitionId     = req?.Id ?? 0,
                    PositionTitle     = req?.PositionTitle ?? posting?.JobTitle,
                    DepartmentName    = req?.DepartmentFK.HasValue == true
                        && deptNames.TryGetValue((int)req.DepartmentFK.Value, out var dn) ? dn : null,
                    CandidateName     = app?.Candidate != null
                        ? $"{app.Candidate.FirstName} {app.Candidate.LastName}".Trim()
                        : "—",
                    RoundName         = s.InterviewRound?.RoundName,
                    RoundOrder        = s.InterviewRound?.RoundOrder ?? 0,
                    ScheduledDateTime = s.ScheduledDateTime,
                    InterviewStatus   = s.Status,
                    HasFeedback       = s.Feedback.Any(),
                    IsSubmitted       = s.Feedback.FirstOrDefault()?.IsSubmitted ?? false,
                    OverallScore      = s.Feedback.FirstOrDefault()?.OverallScore,
                    IsOnPanel         = currentEmpId.HasValue && s.Panel.Any(p => p.EmployeeFk == currentEmpId.Value)
                };
            }).ToList();

            return View(model);
        }

        [RequireUrlPermission]
        public async Task<IActionResult> LineManagerFeedback(decimal id)
        {
            var req = await _context.HrRequisitions.FirstOrDefaultAsync(r => r.Id == id);
            if (req == null) return NotFound();

            var deptName = req.DepartmentFK.HasValue
                ? (await _context.department.FindAsync((int)req.DepartmentFK.Value))?.Name
                : null;

            var initiatedByName = req.EmployeeFK.HasValue
                ? await _context.emp.Where(e => e.Id == (int)req.EmployeeFK.Value)
                    .Select(e => e.Code + " - " + e.FirstName + " " + e.LastName)
                    .FirstOrDefaultAsync()
                : null;

            // Job posting linked to this requisition
            var posting = await _context.HrJobPostings
                .FirstOrDefaultAsync(p => p.RequisitionFK == id && !p.IsDeleted);

            var candidates = new List<HrLMCandidateViewModel>();

            if (posting != null)
            {
                var applications = await _context.HrJobApplications
                    .Include(a => a.Candidate)
                    .Include(a => a.CurrentStage)
                    .Where(a => a.JobPostingFK == posting.Id)
                    .OrderBy(a => a.Candidate!.FirstName)
                    .ToListAsync();

                var appIds = applications.Select(a => a.Id).ToList();

                var schedules = await _context.HrInterviewSchedules
                    .Include(s => s.InterviewRound)
                    .Include(s => s.Feedback)
                    .Where(s => appIds.Contains(s.JobApplicationId))
                    .ToListAsync();

                candidates = applications.Select(app =>
                {
                    var appSchedules = schedules
                        .Where(s => s.JobApplicationId == app.Id)
                        .OrderBy(s => s.InterviewRound?.RoundOrder ?? 0)
                        .Select(s => new HrLMInterviewItemViewModel
                        {
                            ScheduleId        = s.Id,
                            RoundName         = s.InterviewRound?.RoundName,
                            RoundOrder        = s.InterviewRound?.RoundOrder ?? 0,
                            ScheduledDateTime = s.ScheduledDateTime,
                            Status            = s.Status,
                            HasFeedback       = s.Feedback.Any(),
                            FeedbackId        = s.Feedback.FirstOrDefault()?.Id
                        }).ToList();

                    return new HrLMCandidateViewModel
                    {
                        ApplicationId     = app.Id,
                        CandidateName     = app.Candidate != null
                            ? $"{app.Candidate.FirstName} {app.Candidate.LastName}".Trim()
                            : "—",
                        ApplicationStatus = app.CurrentStage?.StageName,
                        Interviews        = appSchedules
                    };
                }).ToList();
            }

            var model = new HrLineManagerFeedbackViewModel
            {
                RequisitionId   = req.Id,
                RequisitionNo   = req.RequisitionNo,
                PositionTitle   = req.PositionTitle,
                DepartmentName  = deptName,
                InitiatedByName = initiatedByName,
                RequisitionType = req.RequisitionType,
                Nature          = req.Nature,
                Reason          = req.Reason,
                Status          = req.Status,
                Candidates      = candidates
            };

            ViewData["BreadcrumbGrandparent"] = "Requisitions";
            ViewData["BreadcrumbGrandparentUrl"] = Url.Action(nameof(Index));
            ViewData["BreadcrumbParent"] = model.RequisitionNo ?? "Requisition";
            ViewData["BreadcrumbParentUrl"] = Url.Action(nameof(Details), new { id });
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(decimal id)
        {
            try
            {
                var username = PermissionHelper.GetCurrentUsername(HttpContext);
                await _requisitionService.SubmitAsync(id, username, GetActionByEmployeeId());
                TempData["Success"] = "Requisition submitted successfully.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Details), new { id });
        }

        private void LoadDropdowns(decimal? selectedDept = null,
            string? selectedType = null, string? selectedNature = null,
            decimal? selectedReplacementEmp = null,
            decimal? selectedTransferFrom = null)
        {
            var deptList = _context.department.OrderBy(d => d.Name).ToList();
            var empList = _context.emp.OrderBy(e => e.FirstName)
                .Select(e => new { e.Id, Display = e.Code + " - " + e.FirstName + " " + e.LastName })
                .ToList();

            ViewBag.Departments = new SelectList(deptList, "Id", "Name",
                selectedDept.HasValue ? (int)selectedDept.Value : null);

            ViewBag.RequisitionTypes = new SelectList(
                new[] {
                    new { Code = "Replacement",  Name = "Replacement" },
                    new { Code = "Transfer",     Name = "Transfer" },
                    new { Code = "NewVacancy",   Name = "New Vacancy" }
                }, "Code", "Name", selectedType);

            ViewBag.RequisitionNatures = new SelectList(
                new[] {
                    new { Code = "Budget",    Name = "Budget" },
                    new { Code = "NonBudget", Name = "Non-Budget" }
                }, "Code", "Name", selectedNature);

            ViewBag.ReplacementEmployees = new SelectList(empList, "Id", "Display",
                selectedReplacementEmp.HasValue ? (int)selectedReplacementEmp.Value : null);

            ViewBag.TransferFromDepts = new SelectList(deptList, "Id", "Name",
                selectedTransferFrom.HasValue ? (int)selectedTransferFrom.Value : null);
        }

        private decimal? GetActionByEmployeeId()
        {
            var session = PermissionHelper.GetUserSession(HttpContext);
            return session?.EmployeeId;
        }

        private async Task<IActionResult> RunWorkflowActionAsync(
            decimal workflowInstanceId,
            string? comments,
            Func<decimal, decimal, string?, Task> action,
            string successMessage)
        {
            var actionBy = GetActionByEmployeeId();
            if (!actionBy.HasValue || actionBy.Value <= 0)
            {
                TempData["Error"] = "Your user account is not linked to an employee record.";
                return RedirectToAction(nameof(Inbox));
            }

            try
            {
                await action(workflowInstanceId, actionBy.Value, comments);
                TempData["Success"] = successMessage;
                return RedirectToAction(nameof(Inbox));
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Action), new { id = workflowInstanceId });
            }
        }
    }
}
