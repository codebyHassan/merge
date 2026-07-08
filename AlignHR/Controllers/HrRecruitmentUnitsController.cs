using AlignHR.Data;
using AlignHR.Models;
using AlignHR.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AlignHR.Controllers
{
    public class HrRecruitmentUnitsController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public HrRecruitmentUnitsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [RequireUrlPermission]
        public async Task<IActionResult> Index()
        {
            var mappings = await _context.HrRecruitmentUnits.ToListAsync();

            var deptIds = mappings.Select(m => m.BusinessDepartmentId)
                .Concat(mappings.Select(m => m.RecruitmentDepartmentId))
                .Distinct().ToList();

            var deptNames = await _context.department
                .Where(d => deptIds.Contains(d.Id))
                .ToDictionaryAsync(d => d.Id, d => d.Name);

            var recruitDeptIds = mappings.Select(m => m.RecruitmentDepartmentId).Distinct().ToList();
            var recruiterCounts = recruitDeptIds.Any()
                ? await _context.emp
                    .Where(e => recruitDeptIds.Contains(e.DepartmentFk))
                    .GroupBy(e => e.DepartmentFk)
                    .ToDictionaryAsync(g => g.Key, g => g.Count())
                : new Dictionary<int, int>();

            var model = mappings.Select(m => new HrRecruitmentUnitListItemViewModel
            {
                Id                      = m.Id,
                BusinessDepartmentName  = deptNames.TryGetValue(m.BusinessDepartmentId, out var bn) ? bn : m.BusinessDepartmentId.ToString(),
                RecruitmentDepartmentName = deptNames.TryGetValue(m.RecruitmentDepartmentId, out var rn) ? rn : m.RecruitmentDepartmentId.ToString(),
                RecruiterCount          = recruiterCounts.TryGetValue(m.RecruitmentDepartmentId, out var rc) ? rc : 0
            })
            .OrderBy(m => m.BusinessDepartmentName)
            .ToList();

            return View(model);
        }

        [RequireUrlPermission]
        public IActionResult Create()
        {
            LoadDropdowns();
            return View(new HrRecruitmentUnitFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HrRecruitmentUnitFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                var duplicate = await _context.HrRecruitmentUnits
                    .AnyAsync(u => u.BusinessDepartmentId == model.BusinessDepartmentId!.Value);
                if (duplicate)
                    ModelState.AddModelError(nameof(model.BusinessDepartmentId),
                        "This business department already has a recruitment mapping.");
            }

            if (ModelState.IsValid)
            {
                _context.HrRecruitmentUnits.Add(new HrRecruitmentUnit
                {
                    BusinessDepartmentId    = model.BusinessDepartmentId!.Value,
                    RecruitmentDepartmentId = model.RecruitmentDepartmentId!.Value
                });
                await _context.SaveChangesAsync();
                TempData["Success"] = "Mapping saved.";
                return RedirectToAction(nameof(Index));
            }

            LoadDropdowns(model.BusinessDepartmentId, model.RecruitmentDepartmentId);
            return View(model);
        }

        [RequireUrlPermission]
        public async Task<IActionResult> Edit(decimal id)
        {
            var unit = await _context.HrRecruitmentUnits.FindAsync(id);
            if (unit == null) return NotFound();

            LoadDropdowns(unit.BusinessDepartmentId, unit.RecruitmentDepartmentId);
            return View(new HrRecruitmentUnitFormViewModel
            {
                Id                      = unit.Id,
                BusinessDepartmentId    = unit.BusinessDepartmentId,
                RecruitmentDepartmentId = unit.RecruitmentDepartmentId
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(HrRecruitmentUnitFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                var duplicate = await _context.HrRecruitmentUnits
                    .AnyAsync(u => u.BusinessDepartmentId == model.BusinessDepartmentId!.Value
                                   && u.Id != model.Id);
                if (duplicate)
                    ModelState.AddModelError(nameof(model.BusinessDepartmentId),
                        "This business department already has a recruitment mapping.");
            }

            if (ModelState.IsValid)
            {
                var unit = await _context.HrRecruitmentUnits.FindAsync(model.Id);
                if (unit == null) return NotFound();

                unit.BusinessDepartmentId    = model.BusinessDepartmentId!.Value;
                unit.RecruitmentDepartmentId = model.RecruitmentDepartmentId!.Value;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Mapping updated.";
                return RedirectToAction(nameof(Index));
            }

            LoadDropdowns(model.BusinessDepartmentId, model.RecruitmentDepartmentId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(decimal id)
        {
            var unit = await _context.HrRecruitmentUnits.FindAsync(id);
            if (unit != null)
            {
                _context.HrRecruitmentUnits.Remove(unit);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Mapping removed.";
            }
            return RedirectToAction(nameof(Index));
        }

        private void LoadDropdowns(int? selectedBusiness = null, int? selectedRecruitment = null)
        {
            var depts = _context.department.OrderBy(d => d.Name).ToList();
            ViewBag.BusinessDepartments    = new SelectList(depts, "Id", "Name", selectedBusiness);
            ViewBag.RecruitmentDepartments = new SelectList(depts, "Id", "Name", selectedRecruitment);
        }
    }
}
