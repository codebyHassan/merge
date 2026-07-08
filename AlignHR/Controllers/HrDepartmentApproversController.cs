using AlignHR.Data;
using AlignHR.Models;
using AlignHR.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AlignHR.Controllers
{
    public class HrDepartmentApproversController : BaseController
    {
        private static readonly string[] ApproverTypes = { "HRBP", "HOD", "RecruitmentHead" };

        private readonly ApplicationDbContext _context;

        public HrDepartmentApproversController(ApplicationDbContext context)
        {
            _context = context;
        }

        [RequireUrlPermission]
        public async Task<IActionResult> Index()
        {
            var approvers = await _context.HrDepartmentApprovers
                .OrderBy(a => a.DepartmentId)
                .ThenBy(a => a.ApproverType)
                .ToListAsync();

            var departmentIds = approvers.Select(a => (int)a.DepartmentId).Distinct().ToList();
            var employeeIds = approvers.Select(a => (int)a.EmployeeId).Distinct().ToList();

            var departments = await _context.department
                .Where(d => departmentIds.Contains(d.Id))
                .ToDictionaryAsync(d => d.Id, d => d.Name);

            var employees = await _context.emp
                .Where(e => employeeIds.Contains(e.Id))
                .ToDictionaryAsync(e => e.Id, e => e.Code + " - " + e.FirstName + " " + e.LastName);

            var model = approvers.Select(a => new HrDepartmentApproverListItemViewModel
            {
                Id = a.Id,
                DepartmentName = departments.TryGetValue((int)a.DepartmentId, out var departmentName) ? departmentName : a.DepartmentId.ToString("0"),
                ApproverType = a.ApproverType,
                EmployeeName = employees.TryGetValue((int)a.EmployeeId, out var employeeName) ? employeeName : a.EmployeeId.ToString("0"),
                IsActive = a.IsActive
            }).ToList();

            return View(model);
        }

        [RequireUrlPermission]
        public IActionResult Create()
        {
            LoadDropdowns();
            return View(new HrDepartmentApproverFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HrDepartmentApproverFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                await ValidateNoDuplicateActiveAsync(model);

                if (ModelState.IsValid)
                {
                    _context.HrDepartmentApprovers.Add(new HrDepartmentApprover
                    {
                        DepartmentId = model.DepartmentId!.Value,
                        ApproverType = model.ApproverType!.Trim(),
                        EmployeeId = model.EmployeeId!.Value,
                        IsActive = model.IsActive
                    });

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Department approver saved.";
                    return RedirectToAction(nameof(Index));
                }
            }

            LoadDropdowns(model.DepartmentId, model.EmployeeId, model.ApproverType);
            return View(model);
        }

        [RequireUrlPermission]
        public async Task<IActionResult> Edit(decimal id)
        {
            var approver = await _context.HrDepartmentApprovers.FindAsync(id);
            if (approver == null)
                return NotFound();

            var model = new HrDepartmentApproverFormViewModel
            {
                Id = approver.Id,
                DepartmentId = approver.DepartmentId,
                ApproverType = approver.ApproverType,
                EmployeeId = approver.EmployeeId,
                IsActive = approver.IsActive
            };

            LoadDropdowns(model.DepartmentId, model.EmployeeId, model.ApproverType);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(HrDepartmentApproverFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                await ValidateNoDuplicateActiveAsync(model);

                if (ModelState.IsValid)
                {
                    var approver = await _context.HrDepartmentApprovers.FindAsync(model.Id);
                    if (approver == null)
                        return NotFound();

                    approver.DepartmentId = model.DepartmentId!.Value;
                    approver.ApproverType = model.ApproverType!.Trim();
                    approver.EmployeeId = model.EmployeeId!.Value;
                    approver.IsActive = model.IsActive;

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Department approver updated.";
                    return RedirectToAction(nameof(Index));
                }
            }

            LoadDropdowns(model.DepartmentId, model.EmployeeId, model.ApproverType);
            return View(model);
        }

        [HttpGet]
        public IActionResult GetEmployees(int departmentId, string? approverType)
        {
            var empQuery = _context.emp.OrderBy(e => e.FirstName).AsQueryable();

            if (departmentId > 0)
            {
                if (approverType == "RecruitmentHead" || approverType == "HRBP")
                {
                    var recruitDeptId = _context.HrRecruitmentUnits
                        .Where(u => u.BusinessDepartmentId == departmentId)
                        .Select(u => (int?)u.RecruitmentDepartmentId)
                        .FirstOrDefault();

                    empQuery = recruitDeptId.HasValue
                        ? empQuery.Where(e => e.DepartmentFk == recruitDeptId.Value)
                        : empQuery.Where(_ => false);
                }
                else
                {
                    empQuery = empQuery.Where(e => e.DepartmentFk == departmentId);
                }
            }

            var result = empQuery
                .Select(e => new { id = e.Id, display = e.Code + " - " + e.FirstName + " " + e.LastName })
                .ToList();

            return Json(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(decimal id)
        {
            var approver = await _context.HrDepartmentApprovers.FindAsync(id);
            if (approver != null)
            {
                approver.IsActive = false;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Department approver deactivated.";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task ValidateNoDuplicateActiveAsync(HrDepartmentApproverFormViewModel model)
        {
            if (!model.IsActive || !model.DepartmentId.HasValue || string.IsNullOrWhiteSpace(model.ApproverType))
                return;

            var approverType = model.ApproverType.Trim();
            var exists = await _context.HrDepartmentApprovers.AnyAsync(a =>
                a.Id != model.Id
                && a.DepartmentId == model.DepartmentId.Value
                && a.ApproverType == approverType
                && a.IsActive);

            if (exists)
                ModelState.AddModelError(nameof(model.ApproverType), "This department already has an active approver for this type.");
        }

        private void LoadDropdowns(decimal? selectedDepartment = null, decimal? selectedEmployee = null, string? selectedApproverType = null)
        {
            ViewBag.Departments = new SelectList(
                _context.department.OrderBy(d => d.Name),
                "Id",
                "Name",
                selectedDepartment.HasValue ? (int)selectedDepartment.Value : null);

            var empQuery = _context.emp.OrderBy(e => e.FirstName).AsQueryable();
            if (selectedDepartment.HasValue)
            {
                var deptId = (int)selectedDepartment.Value;
                if (selectedApproverType == "RecruitmentHead" || selectedApproverType == "HRBP")
                {
                    // Filter to employees of the mapped recruitment department
                    var recruitDeptId = _context.HrRecruitmentUnits
                        .Where(u => u.BusinessDepartmentId == deptId)
                        .Select(u => (int?)u.RecruitmentDepartmentId)
                        .FirstOrDefault();
                    if (recruitDeptId.HasValue)
                        empQuery = empQuery.Where(e => e.DepartmentFk == recruitDeptId.Value);
                }
                else
                {
                    // HOD: filter to employees of the selected department itself
                    empQuery = empQuery.Where(e => e.DepartmentFk == deptId);
                }
            }

            ViewBag.Employees = new SelectList(
                empQuery.Select(e => new { e.Id, Display = e.Code + " - " + e.FirstName + " " + e.LastName }),
                "Id",
                "Display",
                selectedEmployee.HasValue ? (int)selectedEmployee.Value : null);

            ViewBag.ApproverTypes = new SelectList(ApproverTypes, selectedApproverType);
        }
    }
}
