using AlignHR.Data;
using AlignHR.Models;
using AlignHR.Security;
using AlignHR.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AlignHR.Controllers
{
    public class HrAssignmentsController : BaseController
    {
        private readonly IHrAssignmentService _service;
        private readonly ApplicationDbContext _context;

        public HrAssignmentsController(IHrAssignmentService service, ApplicationDbContext context)
        {
            _service = service;
            _context = context;
        }

        [RequireUrlPermission]
        public async Task<IActionResult> Assign(decimal requisitionId)
        {
            var employeeId = GetCurrentEmployeeId();
            if (employeeId.HasValue && !await IsHeadForRequisitionAsync(requisitionId, employeeId.Value))
            {
                TempData["Error"] = "Only the Recruitment Head for this department may assign recruiters.";
                return RedirectToAction("Details", "HrRequisitions", new { id = requisitionId });
            }

            try
            {
                var model = await _service.GetForAssignAsync(requisitionId);
                if (model == null) return NotFound();
                await LoadRecruiterDropdown(requisitionId);
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
        public async Task<IActionResult> Assign(HrAssignmentFormViewModel model)
        {
            var employeeId = GetCurrentEmployeeId();
            if (!employeeId.HasValue || !await IsHeadForRequisitionAsync(model.RequisitionFk, employeeId.Value))
            {
                TempData["Error"] = "Only the Recruitment Head for this department may assign recruiters.";
                return RedirectToAction("RecruiterInbox", "HrRequisitions");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var username = PermissionHelper.GetCurrentUsername(HttpContext);
                    await _service.AssignAsync(model, username);
                    TempData["Success"] = "Recruiter assigned successfully.";
                    return RedirectToAction("RecruiterInbox", "HrRequisitions");
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            await LoadRecruiterDropdown(model.RequisitionFk, model.RecruiterEmployeeFK);
            return View(model);
        }

        private decimal? GetCurrentEmployeeId()
        {
            var session = PermissionHelper.GetUserSession(HttpContext);
            return session?.EmployeeId;
        }

        private async Task<bool> IsHeadForRequisitionAsync(decimal requisitionId, decimal employeeId)
        {
            var deptId = await _context.HrRequisitions
                .Where(r => r.Id == requisitionId)
                .Select(r => r.DepartmentFK)
                .FirstOrDefaultAsync();

            if (!deptId.HasValue) return false;

            return await _context.HrDepartmentApprovers
                .AnyAsync(a => a.DepartmentId == deptId.Value
                    && a.ApproverType == "RecruitmentHead"
                    && a.EmployeeId == employeeId
                    && a.IsActive);
        }

        private async Task LoadRecruiterDropdown(decimal requisitionId, decimal? selected = null)
        {
            var deptId = await _context.HrRequisitions
                .Where(r => r.Id == requisitionId)
                .Select(r => r.DepartmentFK)
                .FirstOrDefaultAsync();

            if (deptId.HasValue)
            {
                var recruitDeptId = await _context.HrRecruitmentUnits
                    .Where(u => u.BusinessDepartmentId == (int)deptId.Value)
                    .Select(u => (int?)u.RecruitmentDepartmentId)
                    .FirstOrDefaultAsync();

                if (recruitDeptId.HasValue)
                {
                    var recruiters = await _context.emp
                        .Where(e => e.DepartmentFk == recruitDeptId.Value)
                        .OrderBy(e => e.FirstName)
                        .Select(e => new { e.Id, Display = e.Code + " - " + e.FirstName + " " + e.LastName })
                        .ToListAsync();

                    ViewBag.Recruiters = new SelectList(recruiters, "Id", "Display",
                        selected.HasValue ? (int)selected.Value : null);
                    return;
                }
            }

            ViewBag.Recruiters = new SelectList(Enumerable.Empty<object>(), "Id", "Display");
            ViewBag.RecruiterWarning = "No recruitment department is configured for this requisition's department.";
        }
    }
}
