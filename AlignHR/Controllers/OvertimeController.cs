using AlignHR.Data;
using AlignHR.Models;
using AlignHR.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace AlignHR.Controllers
{
    public class OvertimeController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public OvertimeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [RequireUrlPermission]
        public async Task<IActionResult> Index(string search, string dayType, bool? isActive, int? page)
        {
            var query = _context.Overtimes.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(o => o.PolicyName.Contains(search));
            }

            if (!string.IsNullOrEmpty(dayType) && dayType != "All")
            {
                query = query.Where(o => o.DayType == dayType);
            }

            if (isActive.HasValue)
            {
                query = query.Where(o => o.IsActive == isActive.Value);
            }

            // Calculation summaries
            ViewBag.TotalActive = await _context.Overtimes.CountAsync(o => o.IsActive);
            ViewBag.TotalPolicies = await _context.Overtimes.CountAsync();

            var activeMultipliers = await _context.Overtimes
                .Where(o => o.IsActive && o.RateType == "MULTIPLIER")
                .Select(o => o.RateValue)
                .ToListAsync();
            ViewBag.AvgMultiplier = activeMultipliers.Any() ? activeMultipliers.Average() : 0;

            ViewBag.CurrentDayType = string.IsNullOrEmpty(dayType) ? "All" : dayType;
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentIsActive = isActive;

            int pageSize = 15;
            int pageNumber = page ?? 1;

            var pagedPolicies = await query
                .OrderByDescending(o => o.OvertimeId)
                .ToPagedListAsync(pageNumber, pageSize);

            return View(pagedPolicies);
        }

        [RequireUrlPermission]
        public IActionResult Create()
        {
            return View(new Overtime { IsActive = true, EffectiveFrom = DateOnly.FromDateTime(DateTime.Now) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Overtime model)
        {
            if (ModelState.IsValid)
            {
                if (model.IsDefault)
                {
                    bool defaultExists = await _context.Overtimes.AnyAsync(o => o.IsDefault);
                    if (defaultExists)
                    {
                        ModelState.AddModelError("IsDefault", "Another policy is already default. Kindly uncheck or off it first, then you can enable this one.");
                        return View(model);
                    }
                }

                model.CreatedBy = PermissionHelper.GetCurrentUserId(HttpContext);
                model.CreatedOn = DateTime.Now;

                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [RequireUrlPermission]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var overtime = await _context.Overtimes.FindAsync(id);
            if (overtime == null) return NotFound();

            return View(overtime);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Overtime model)
        {
            if (id != model.OvertimeId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.Overtimes.AsNoTracking().FirstOrDefaultAsync(o => o.OvertimeId == id);
                    if (existing == null) return NotFound();

                    if (model.IsDefault)
                    {
                        bool defaultExists = await _context.Overtimes.AnyAsync(o => o.IsDefault && o.OvertimeId != id);
                        if (defaultExists)
                        {
                            ModelState.AddModelError("IsDefault", "Another policy is already default. Kindly uncheck or off it first, then you can enable this one.");
                            return View(model);
                        }
                    }

                    model.CreatedBy = existing.CreatedBy;
                    model.CreatedOn = existing.CreatedOn;
                    model.UpdatedBy = PermissionHelper.GetCurrentUserId(HttpContext);
                    model.UpdatedOn = DateTime.Now;

                    _context.Update(model);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OvertimeExists(model.OvertimeId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [RequireUrlPermission]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var overtime = await _context.Overtimes.FirstOrDefaultAsync(m => m.OvertimeId == id);
            if (overtime == null) return NotFound();

            return View(overtime);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var overtime = await _context.Overtimes.FindAsync(id);
            if (overtime != null)
            {
                _context.Overtimes.Remove(overtime);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool OvertimeExists(int id)
        {
            return _context.Overtimes.Any(e => e.OvertimeId == id);
        }
    }
}

