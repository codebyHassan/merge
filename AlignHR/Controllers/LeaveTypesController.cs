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
    public class LeaveTypesController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public LeaveTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: LeaveTypes
        public async Task<IActionResult> Index(int? page, string searchQuery)
        {
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            
            ViewData["CurrentFilter"] = searchQuery;

            var leaveTypesQuery = _context.LeaveTypes.AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                leaveTypesQuery = leaveTypesQuery.Where(l => l.Name.Contains(searchQuery) || l.Code.Contains(searchQuery));
            }

            var leaveTypes = leaveTypesQuery.OrderByDescending(l => l.CreatedAt).ToPagedList(pageNumber, pageSize);
            
            return View(leaveTypes);
        }

        // GET: LeaveTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LeaveTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LeaveType leaveType)
        {
            if (ModelState.IsValid)
            {
                leaveType.CreatedBy = PermissionHelper.GetCurrentUsername(HttpContext);
                leaveType.CreatedAt = DateTime.Now;
                _context.Add(leaveType);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Leave type created successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(leaveType);
        }

        // GET: LeaveTypes/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveType = await _context.LeaveTypes.FindAsync(id);
            if (leaveType == null)
            {
                return NotFound();
            }
            return View(leaveType);
        }

        // POST: LeaveTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, LeaveType leaveType)
        {
            if (id != leaveType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.LeaveTypes.AsNoTracking().FirstOrDefaultAsync(l => l.Id == id);
                    if (existing == null) return NotFound();

                    leaveType.CreatedBy = existing.CreatedBy;
                    leaveType.CreatedAt = existing.CreatedAt;
                    leaveType.UpdatedBy = PermissionHelper.GetCurrentUsername(HttpContext);
                    leaveType.UpdatedAt = DateTime.Now;

                    _context.Update(leaveType);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Leave type updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LeaveTypeExists(leaveType.Id))
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
            return View(leaveType);
        }

        // POST: LeaveTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            var leaveType = await _context.LeaveTypes.FindAsync(id);
            if (leaveType == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.LeaveTypes.Remove(leaveType);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Leave type deleted successfully.";
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = "Cannot delete. This leave type is linked with other records.";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool LeaveTypeExists(decimal id)
        {
            return _context.LeaveTypes.Any(e => e.Id == id);
        }
    }
}
