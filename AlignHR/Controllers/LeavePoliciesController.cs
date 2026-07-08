using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AlignHR.Data;
using AlignHR.Models;

using X.PagedList;
using AlignHR.Security;

namespace AlignHR.Controllers
{
    public class LeavePoliciesController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public LeavePoliciesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: LeavePolicies
        public async Task<IActionResult> Index(int? page, string searchQuery)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            
            ViewData["CurrentFilter"] = searchQuery;

            var leavePoliciesQuery = _context.LeavePolicy.AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                leavePoliciesQuery = leavePoliciesQuery.Where(l => l.PolicyName.Contains(searchQuery));
            }

            var leavePolicies = leavePoliciesQuery.OrderByDescending(l => l.CreatedAt).ToPagedList(pageNumber, pageSize);
            
            return View(leavePolicies);
        }

        // GET: LeavePolicies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LeavePolicies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LeavePolicy leavePolicy)
        {
            if (ModelState.IsValid)
            {
                leavePolicy.CreatedBy = PermissionHelper.GetCurrentUsername(HttpContext);
                leavePolicy.CreatedAt = DateTime.Now;
                _context.Add(leavePolicy);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Leave policy created successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(leavePolicy);
        }

        // GET: LeavePolicies/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leavePolicy = await _context.LeavePolicy.FindAsync(id);
            if (leavePolicy == null)
            {
                return NotFound();
            }
            return View(leavePolicy);
        }

        // POST: LeavePolicies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, LeavePolicy leavePolicy)
        {
            if (id != leavePolicy.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.LeavePolicy.AsNoTracking().FirstOrDefaultAsync(l => l.Id == id);
                    if (existing == null) return NotFound();

                    leavePolicy.CreatedBy = existing.CreatedBy;
                    leavePolicy.CreatedAt = existing.CreatedAt;
                    leavePolicy.UpdatedBy = PermissionHelper.GetCurrentUsername(HttpContext);
                    leavePolicy.UpdatedAt = DateTime.Now;

                    _context.Update(leavePolicy);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Leave policy updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LeavePolicyExists(leavePolicy.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(leavePolicy);
        }

        // POST: LeavePolicies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            var leavePolicy = await _context.LeavePolicy.FindAsync(id);
            if (leavePolicy == null)
            {
                return RedirectToAction(nameof(Index));
            }

            // DOUBLE CHECK: Block if employees are assigned
            var employeesAssigned = await _context.EmployeeLeavePolicies.AnyAsync(e => e.LeavePolicyId == id);
            if (employeesAssigned)
            {
                TempData["Error"] = "Cannot delete this policy because there are employees assigned to it. Please unassign all employees first.";
                return RedirectToAction(nameof(Index));
            }

            // Delete associated rules first to satisfy FK constraint
            var rules = await _context.LeavePolicyRules.Where(r => r.LeavePolicyId == id).ToListAsync();
            if (rules.Any())
            {
                _context.LeavePolicyRules.RemoveRange(rules);
            }

            _context.LeavePolicy.Remove(leavePolicy);
            await _context.SaveChangesAsync();
            
            TempData["Success"] = "Leave policy and its associated rules were deleted.";
            return RedirectToAction(nameof(Index));
        }

        private bool LeavePolicyExists(decimal id)
        {
            return _context.LeavePolicy.Any(e => e.Id == id);
        }
    }
}
