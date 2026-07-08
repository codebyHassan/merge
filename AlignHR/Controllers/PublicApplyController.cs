using AlignHR.Data;
using AlignHR.Models;
using AlignHR.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlignHR.Controllers
{
    public class PublicApplyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHrJobPostingService _jobPostingService;
        private readonly IFileStorageService _fileStorage;

        public PublicApplyController(
            ApplicationDbContext context,
            IHrJobPostingService jobPostingService,
            IFileStorageService fileStorage)
        {
            _context = context;
            _jobPostingService = jobPostingService;
            _fileStorage = fileStorage;
        }

        [HttpGet("/jobs")]
        public async Task<IActionResult> Jobs([FromQuery] string? search, [FromQuery] string? department)
        {
            var postings = await _context.HrJobPostings
                .Where(p => !p.IsDeleted && p.PostingStatus == "Published")
                .OrderByDescending(p => p.OpenDate)
                .ToListAsync();

            // Gather requisition + department data
            var reqIds = postings
                .Where(p => p.RequisitionFK.HasValue)
                .Select(p => p.RequisitionFK!.Value)
                .Distinct()
                .ToList();

            var requisitions = await _context.HrRequisitions
                .Where(r => reqIds.Contains(r.Id))
                .ToListAsync();

            var deptIds = requisitions
                .Where(r => r.DepartmentFK.HasValue)
                .Select(r => (int)r.DepartmentFK!.Value)
                .Distinct()
                .ToList();

            var departments = await _context.department
                .Where(d => deptIds.Contains(d.Id))
                .ToDictionaryAsync(d => d.Id, d => d.Name);

            var items = postings.Select(p =>
            {
                var req = p.RequisitionFK.HasValue
                    ? requisitions.FirstOrDefault(r => r.Id == p.RequisitionFK.Value)
                    : null;

                string? deptName = null;
                if (req?.DepartmentFK.HasValue == true)
                    departments.TryGetValue((int)req.DepartmentFK.Value, out deptName);

                return new HrJobPortalItemViewModel
                {
                    Id             = p.Id,
                    JobCode        = p.JobCode,
                    JobTitle       = p.JobTitle,
                    DepartmentName = deptName,
                    EmploymentType = p.EmploymentType,
                    Location       = p.Location,
                    Nature         = req?.Nature,
                    JobDescription = p.JobDescription,
                    OpenDate       = p.OpenDate,
                    CloseDate      = p.CloseDate
                };
            }).ToList();

            // Optional filters
            if (!string.IsNullOrWhiteSpace(search))
                items = items.Where(i =>
                    (i.JobTitle?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (i.DepartmentName?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (i.Location?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false)).ToList();

            if (!string.IsNullOrWhiteSpace(department))
                items = items.Where(i =>
                    i.DepartmentName?.Equals(department, StringComparison.OrdinalIgnoreCase) ?? false).ToList();

            var allDepartments = postings
                .Select(p =>
                {
                    var req = p.RequisitionFK.HasValue
                        ? requisitions.FirstOrDefault(r => r.Id == p.RequisitionFK.Value)
                        : null;
                    if (req?.DepartmentFK.HasValue != true) return null;
                    departments.TryGetValue((int)req.DepartmentFK.Value, out var dn);
                    return dn;
                })
                .Where(d => !string.IsNullOrWhiteSpace(d))
                .Distinct()
                .OrderBy(d => d)
                .ToList();

            ViewData["Title"]          = "Job Openings – AlignHR";
            ViewData["Search"]         = search;
            ViewData["Department"]     = department;
            ViewData["AllDepartments"] = allDepartments;
            ViewData["Layout"]         = "~/Views/Shared/_PublicLayout.cshtml";
            return View(items);
        }

        [HttpGet("/apply/{id:decimal}")]
        public async Task<IActionResult> Apply(decimal id)
        {
            var posting = await _context.HrJobPostings
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted && p.PostingStatus == "Published");

            if (posting == null)
                return View("NotFound");

            string? departmentName = null;
            if (posting.RequisitionFK.HasValue)
            {
                var req = await _context.HrRequisitions.FirstOrDefaultAsync(r => r.Id == posting.RequisitionFK.Value);
                if (req?.DepartmentFK.HasValue == true)
                {
                    var dept = await _context.department.FirstOrDefaultAsync(d => d.Id == (int)req.DepartmentFK.Value);
                    departmentName = dept?.Name;
                }
            }

            var vm = new HrPublicApplyViewModel
            {
                JobPostingId = posting.Id,
                JobTitle = posting.JobTitle,
                JobCode = posting.JobCode,
                DepartmentName = departmentName,
                EmploymentType = posting.EmploymentType,
                Location = posting.Location,
                Country = "Pakistan"
            };

            ViewData["Title"] = $"Apply – {posting.JobTitle}";
            ViewData["Layout"] = "~/Views/Shared/_PublicLayout.cshtml";
            return View(vm);
        }

        [HttpPost("/apply/{id:decimal}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(decimal id, HrPublicApplyViewModel model)
        {
            var posting = await _context.HrJobPostings
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted && p.PostingStatus == "Published");

            if (posting == null)
                return View("NotFound");

            if (model.IsCurrentlyEmployed == true)
            {
                if (string.IsNullOrWhiteSpace(model.CurrentEmployer))
                    ModelState.AddModelError(nameof(model.CurrentEmployer), "Current organization is required when employed.");
                if (string.IsNullOrWhiteSpace(model.NoticePeriod))
                    ModelState.AddModelError(nameof(model.NoticePeriod), "Notice period is required when employed.");
            }

            if (!ModelState.IsValid)
            {
                model.JobTitle = posting.JobTitle;
                model.JobCode = posting.JobCode;
                model.EmploymentType = posting.EmploymentType;
                model.Location = posting.Location;
                ViewData["Title"] = $"Apply – {posting.JobTitle}";
                ViewData["Layout"] = "~/Views/Shared/_PublicLayout.cshtml";
                return View(model);
            }

            string? resumeFileName = null;
            if (model.ResumeFile != null && model.ResumeFile.Length > 0)
            {
                try
                {
                    var (_, relativePath) = await _fileStorage.SaveAsync(model.ResumeFile, "resumes");
                    resumeFileName = relativePath;
                }
                catch (Exception ex)
                {
                    // Surface the specific reason (unsupported type / too large) for validation
                    // failures; fall back to a generic message for unexpected I/O errors.
                    var message = ex is InvalidOperationException
                        ? ex.Message
                        : "Resume upload failed. Please try again.";
                    ModelState.AddModelError(nameof(model.ResumeFile), message);
                    model.JobTitle = posting.JobTitle;
                    model.JobCode = posting.JobCode;
                    ViewData["Title"] = $"Apply – {posting.JobTitle}";
                    ViewData["Layout"] = "~/Views/Shared/_PublicLayout.cshtml";
                    return View(model);
                }
            }

            var email = model.Email.Trim();
            var candidate = await _context.HrCandidates
                .FirstOrDefaultAsync(c => c.Email == email);

            if (candidate == null)
            {
                candidate = new HrCandidate
                {
                    FirstName = model.FirstName.Trim(),
                    LastName = model.LastName.Trim(),
                    Email = email,
                    Phone = model.Phone?.Trim(),
                    City = model.City,
                    Country = model.Country,
                    DateOfBirth = model.DateOfBirth,
                    Gender = model.Gender,
                    HighestDegree = model.HighestDegree,
                    FieldOfStudy = model.FieldOfStudy,
                    University = model.University,
                    IsCurrentlyEmployed = model.IsCurrentlyEmployed,
                    CurrentEmployer = model.IsCurrentlyEmployed == true ? model.CurrentEmployer?.Trim() : null,
                    CurrentDesignation = model.IsCurrentlyEmployed == true ? model.CurrentDesignation?.Trim() : null,
                    NoticePeriod = model.IsCurrentlyEmployed == true ? model.NoticePeriod : null,
                    ResumeFileName = resumeFileName,
                    CreatedDate = DateTime.Now
                };
                _context.HrCandidates.Add(candidate);
                await _context.SaveChangesAsync();
            }
            else if (resumeFileName != null)
            {
                // Update resume if a new one was uploaded on re-application
                candidate.ResumeFileName = resumeFileName;
                await _context.SaveChangesAsync();
            }

            // Prevent duplicate application to the same posting
            var alreadyApplied = await _context.HrJobApplications
                .AnyAsync(a => a.CandidateFK == candidate.Id && a.JobPostingFK == posting.Id);

            if (alreadyApplied)
            {
                TempData["AppliedName"] = candidate.FirstName;
                return RedirectToAction(nameof(Confirmation));
            }

            var firstStage = await _context.HrApplicationStages
                .Where(s => s.IsActive)
                .OrderBy(s => s.StageOrder)
                .FirstOrDefaultAsync();

            var application = new HrJobApplication
            {
                CandidateFK = candidate.Id,
                JobPostingFK = posting.Id,
                CurrentStageFK = firstStage?.Id,
                AppliedDate = DateTime.Now,
                IsActive = true
            };

            _context.HrJobApplications.Add(application);
            await _context.SaveChangesAsync();

            TempData["AppliedName"] = candidate.FirstName;
            return RedirectToAction(nameof(Confirmation));
        }

        [HttpGet("/apply/confirmation")]
        public IActionResult Confirmation()
        {
            ViewData["Title"] = "Application Submitted – AlignHR";
            ViewData["Layout"] = "~/Views/Shared/_PublicLayout.cshtml";
            return View();
        }
    }
}
