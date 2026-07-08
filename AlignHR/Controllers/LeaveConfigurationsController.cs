using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AlignHR.Data;
using AlignHR.Models;
using AlignHR.Security;

namespace AlignHR.Controllers
{
    public class LeaveConfigurationsController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public LeaveConfigurationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ═══════════════════════════════════════════════════════════════
        //  INDEX
        // ═══════════════════════════════════════════════════════════════

        public async Task<IActionResult> Index(string searchQuery)
        {
            ViewData["CurrentFilter"] = searchQuery;

            var query = _context.LeaveConfigurations
                .Include(c => c.Details)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                query = query.Where(c => c.GroupingType.Contains(searchQuery));
            }

            var configs = await query
                .OrderByDescending(c => c.CreatedAt)
                .AsNoTracking()
                .ToListAsync();

            ViewBag.HasConfiguration = await _context.LeaveConfigurations.AsNoTracking().AnyAsync();
            return View(configs);
        }

        // ═══════════════════════════════════════════════════════════════
        //  CREATE
        // ═══════════════════════════════════════════════════════════════

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var configurationExists = await _context.LeaveConfigurations.AsNoTracking().AnyAsync();
            if (configurationExists)
            {
                TempData["Error"] = "A Leave Configuration already exists. Please edit the existing configuration.";
                return RedirectToAction(nameof(Index));
            }

            return View(new LeaveConfiguration());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LeaveConfiguration model, int[] referenceIds, string[] referenceNames, string[] approverEmpNos, string[] approverNames)
        {
            var configurationExists = await _context.LeaveConfigurations.AnyAsync();
            if (configurationExists)
            {
                ModelState.AddModelError(string.Empty, "A Leave Configuration already exists. Please edit the existing configuration instead of creating a new one.");
                TempData["Error"] = "A Leave Configuration already exists. Please edit the existing configuration.";
                return View(model);
            }

            if (ModelState.IsValid)
            {
                // Enforce single active configuration
                if (model.IsActive)
                {
                    var existingActive = await _context.LeaveConfigurations
                        .Where(c => c.IsActive)
                        .ToListAsync();
                    foreach (var c in existingActive)
                        c.IsActive = false;
                }

                _context.LeaveConfigurations.Add(model);
                await _context.SaveChangesAsync();

                // Save detail rows
                await SaveDetailRows(model.Id, referenceIds, referenceNames, approverEmpNos, approverNames);

                TempData["Success"] = "Leave Configuration created successfully.";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // ═══════════════════════════════════════════════════════════════
        //  EDIT
        // ═══════════════════════════════════════════════════════════════

        [HttpGet]
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null) return NotFound();

            var config = await _context.LeaveConfigurations
                .Include(c => c.Details)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (config == null) return NotFound();

            return View(config);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, LeaveConfiguration model, int[] referenceIds, string[] referenceNames, string[] approverEmpNos, string[] approverNames)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.LeaveConfigurations
                        .Include(c => c.Details)
                        .FirstOrDefaultAsync(c => c.Id == id);

                    if (existing == null) return NotFound();

                    // Enforce single active configuration
                    if (model.IsActive)
                    {
                        var otherActive = await _context.LeaveConfigurations
                            .Where(c => c.IsActive && c.Id != id)
                            .ToListAsync();
                        foreach (var c in otherActive)
                            c.IsActive = false;
                    }

                    existing.IsDepartment = model.IsDepartment;
                    existing.IsSubDepartment = model.IsSubDepartment;
                    existing.IsGrade = model.IsGrade;
                    existing.IsDivision = model.IsDivision;
                    existing.IsActive = model.IsActive;

                    // Remove old details
                    _context.LeaveConfigurationDetails.RemoveRange(existing.Details);
                    await _context.SaveChangesAsync();

                    // Save new detail rows
                    await SaveDetailRows(existing.Id, referenceIds, referenceNames, approverEmpNos, approverNames);

                    TempData["Success"] = "Leave Configuration updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    TempData["Error"] = "Concurrency conflict. Please try again.";
                }
            }

            var reload = await _context.LeaveConfigurations
                .Include(c => c.Details)
                .FirstOrDefaultAsync(c => c.Id == id);
            return View(reload);
        }

        // ═══════════════════════════════════════════════════════════════
        //  DETAILS
        // ═══════════════════════════════════════════════════════════════

        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null) return NotFound();

            var config = await _context.LeaveConfigurations
                .Include(c => c.Details)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (config == null) return NotFound();

            return View(config);
        }

        // ═══════════════════════════════════════════════════════════════
        //  DELETE
        // ═══════════════════════════════════════════════════════════════

        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null) return NotFound();

            var config = await _context.LeaveConfigurations
                .Include(c => c.Details)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (config == null) return NotFound();

            return View(config);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            var config = await _context.LeaveConfigurations
                .Include(c => c.Details)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (config != null)
            {
                _context.LeaveConfigurationDetails.RemoveRange(config.Details);
                _context.LeaveConfigurations.Remove(config);
                await _context.SaveChangesAsync();
            }

            TempData["Success"] = "Leave Configuration deleted.";
            return RedirectToAction(nameof(Index));
        }

        // ═══════════════════════════════════════════════════════════════
        //  AJAX — Load Group Values
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// Returns JSON array of { id, name } for the selected grouping type.
        /// Department mode loads from Department table; others from Valueset.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetGroupValues(string groupType)
        {
            object result;

            if (groupType == "Department")
            {
                result = await _context.department
                    .OrderBy(d => d.Name)
                    .Select(d => new { id = d.Id, name = d.Name })
                    .AsNoTracking()
                    .ToListAsync();
            }
            else
            {
                // SubDepartment, Grade, Division — all from Valueset
                result = await _context.valuesets
                    .Where(v => v.GroupName == groupType && v.IsActive)
                    .OrderBy(v => v.Name)
                    .Select(v => new { id = v.Id, name = v.Name })
                    .AsNoTracking()
                    .ToListAsync();
            }

            return Json(result);
        }

        // ═══════════════════════════════════════════════════════════════
        //  AJAX — Employee Search (HTMX)
        // ═══════════════════════════════════════════════════════════════

        [HttpGet]
        public async Task<IActionResult> SearchEmployees(string term, int rowIndex)
        {
            if (string.IsNullOrEmpty(term) || term.Length < 2)
            {
                return Content("");
            }

            var employees = await _context.emp
                .Include(e => e.Department)
                .Where(e => e.FirstName.Contains(term) ||
                            e.LastName.Contains(term) ||
                            e.Code.Contains(term))
                .OrderBy(e => e.Code == term ? 0 : 1) // Exact code match first
                .ThenBy(e => e.Code.Length)           // Shorter codes next
                .ThenBy(e => e.FirstName)
                .Take(20)
                .AsNoTracking()
                .ToListAsync();

            return PartialView("_ApproverSearchResults", new ApproverSearchViewModel
            {
                Employees = employees,
                RowIndex = rowIndex
            });
        }

        // ═══════════════════════════════════════════════════════════════
        //  PRIVATE HELPERS
        // ═══════════════════════════════════════════════════════════════

        private async Task SaveDetailRows(decimal configId, int[] referenceIds, string[] referenceNames, string[] approverEmpNos, string[] approverNames)
        {
            if (referenceIds == null) return;

            for (int i = 0; i < referenceIds.Length; i++)
            {
                var empNo = (approverEmpNos != null && i < approverEmpNos.Length) ? approverEmpNos[i] : null;
                if (string.IsNullOrWhiteSpace(empNo)) continue;

                var detail = new LeaveConfigurationDetail
                {
                    LeaveConfigurationId = configId,
                    ReferenceId = referenceIds[i],
                    ReferenceName = (referenceNames != null && i < referenceNames.Length) ? referenceNames[i] : "",
                    ApproverEmpNo = empNo,
                    ApproverName = (approverNames != null && i < approverNames.Length) ? approverNames[i] : ""
                };

                _context.LeaveConfigurationDetails.Add(detail);
            }

            await _context.SaveChangesAsync();
        }
    }

    // ═══════════════════════════════════════════════════════════════
    //  VIEW MODEL for Employee Search Partial
    // ═══════════════════════════════════════════════════════════════

    public class ApproverSearchViewModel
    {
        public List<Employee> Employees { get; set; } = new();
        public int RowIndex { get; set; }
    }
}
