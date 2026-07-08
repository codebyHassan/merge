using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AlignHR.Data;
using AlignHR.Models;
using AlignHR.Security;
using X.PagedList;

namespace AlignHR.Controllers
{
    public class LeavePolicyRulesController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public LeavePolicyRulesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: LeavePolicyRules
        public async Task<IActionResult> Index(int? page, string searchQuery, decimal? leavePolicyId)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            ViewData["CurrentFilter"] = searchQuery;
            ViewData["CurrentPolicy"] = leavePolicyId;

            if (!leavePolicyId.HasValue)
            {
                ViewBag.CurrentPolicyName = "Leave Policy";
                var emptyResult = Enumerable.Empty<LeavePolicyRule>().ToPagedList(pageNumber, pageSize);
                return View(emptyResult);
            }

            var query = _context.LeavePolicyRules
                .Include(l => l.LeavePolicy)
                .Include(l => l.LeaveType)
                .Where(l => l.LeavePolicyId == leavePolicyId.Value)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(l => l.LeaveType!.Name.Contains(searchQuery));
            }

            var currentPolicy = await _context.LeavePolicy
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == leavePolicyId.Value);

            ViewBag.CurrentPolicyName = currentPolicy?.PolicyName ?? "Leave Policy";

            var result = await query.OrderBy(l => l.LeaveType!.Name).ToPagedListAsync(pageNumber, pageSize);

            return View(result);
        }

        // GET: LeavePolicyRules/Create
        public IActionResult Create(decimal? leavePolicyId)
        {
            var model = new LeavePolicyRule { IsActive = true, LeavePolicyId = leavePolicyId ?? 0 };
            PopulateDropdowns(model.LeavePolicyId > 0 ? model.LeavePolicyId : null);
            return View(model);
        }

        // POST: LeavePolicyRules/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LeavePolicyRule leavePolicyRule)
        {
            await ValidateUniqueLeaveTypeAsync(leavePolicyRule);

            if (ModelState.IsValid)
            {
                leavePolicyRule.CreatedBy = PermissionHelper.GetCurrentUsername(HttpContext);
                leavePolicyRule.CreatedAt = DateTime.Now;
                _context.Add(leavePolicyRule);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Leave policy rule created successfully.";
                return RedirectToAction(nameof(Index), new { leavePolicyId = leavePolicyRule.LeavePolicyId });
            }
            PopulateDropdowns(leavePolicyRule.LeavePolicyId, leavePolicyRule.LeaveTypeId);
            return View(leavePolicyRule);
        }

        // GET: LeavePolicyRules/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leavePolicyRule = await _context.LeavePolicyRules.FindAsync(id);
            if (leavePolicyRule == null)
            {
                return NotFound();
            }
            PopulateDropdowns(leavePolicyRule.LeavePolicyId, leavePolicyRule.LeaveTypeId, leavePolicyRule.Id);
            return View(leavePolicyRule);
        }

        // POST: LeavePolicyRules/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, LeavePolicyRule leavePolicyRule)
        {
            if (id != leavePolicyRule.Id)
            {
                return NotFound();
            }

            await ValidateUniqueLeaveTypeAsync(leavePolicyRule, id);

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.LeavePolicyRules.AsNoTracking().FirstOrDefaultAsync(l => l.Id == id);
                    if (existing == null) return NotFound();

                    leavePolicyRule.CreatedBy = existing.CreatedBy;
                    leavePolicyRule.CreatedAt = existing.CreatedAt;
                    leavePolicyRule.UpdatedBy = PermissionHelper.GetCurrentUsername(HttpContext);
                    leavePolicyRule.UpdatedAt = DateTime.Now;

                    _context.Update(leavePolicyRule);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LeavePolicyRuleExists(leavePolicyRule.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["Success"] = "Leave policy rule updated successfully.";
                return RedirectToAction(nameof(Index), new { leavePolicyId = leavePolicyRule.LeavePolicyId });
            }
            PopulateDropdowns(leavePolicyRule.LeavePolicyId, leavePolicyRule.LeaveTypeId, leavePolicyRule.Id);
            return View(leavePolicyRule);
        }

        // POST: LeavePolicyRules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            var leavePolicyRule = await _context.LeavePolicyRules.FindAsync(id);
            decimal? policyId = null;
            if (leavePolicyRule != null)
            {
                policyId = leavePolicyRule.LeavePolicyId;
                _context.LeavePolicyRules.Remove(leavePolicyRule);
                TempData["Success"] = "Leave policy rule deleted successfully.";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { leavePolicyId = policyId });
        }

        private bool LeavePolicyRuleExists(decimal id)
        {
            return _context.LeavePolicyRules.Any(e => e.Id == id);
        }

        private void PopulateDropdowns(decimal? leavePolicyId, decimal? selectedLeaveTypeId = null, decimal? currentRuleId = null)
        {
            ViewBag.LeavePolicyId = new SelectList(
                _context.LeavePolicy.OrderBy(p => p.PolicyName),
                "Id",
                "PolicyName",
                leavePolicyId);

            var leaveTypesQuery = _context.LeaveTypes.AsQueryable();

            if (leavePolicyId.HasValue && leavePolicyId.Value > 0)
            {
                var usedLeaveTypeIds = _context.LeavePolicyRules
                    .Where(r => r.LeavePolicyId == leavePolicyId.Value);

                if (currentRuleId.HasValue)
                {
                    usedLeaveTypeIds = usedLeaveTypeIds.Where(r => r.Id != currentRuleId.Value);
                }

                var excludedIds = usedLeaveTypeIds
                    .Select(r => r.LeaveTypeId)
                    .ToList();

                leaveTypesQuery = leaveTypesQuery.Where(t => !excludedIds.Contains(t.Id) || t.Id == selectedLeaveTypeId);
            }

            ViewBag.LeaveTypeId = new SelectList(
                leaveTypesQuery.OrderBy(t => t.Name).ToList(),
                "Id",
                "Name",
                selectedLeaveTypeId);
        }

        private async Task ValidateUniqueLeaveTypeAsync(LeavePolicyRule leavePolicyRule, decimal? currentRuleId = null)
        {
            if (leavePolicyRule.LeavePolicyId <= 0 || leavePolicyRule.LeaveTypeId <= 0)
            {
                return;
            }

            var duplicateExists = await _context.LeavePolicyRules.AnyAsync(r =>
                r.LeavePolicyId == leavePolicyRule.LeavePolicyId &&
                r.LeaveTypeId == leavePolicyRule.LeaveTypeId &&
                (!currentRuleId.HasValue || r.Id != currentRuleId.Value));

            if (duplicateExists)
            {
                ModelState.AddModelError(nameof(LeavePolicyRule.LeaveTypeId), "This leave type is already added to the selected policy.");
            }
        }
    }
}
