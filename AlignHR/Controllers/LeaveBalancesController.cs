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

namespace AlignHR.Controllers
{
    public class LeaveBalancesController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public LeaveBalancesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: LeaveBalances
        public async Task<IActionResult> Index(int? page, string searchQuery)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            
            ViewData["CurrentFilter"] = searchQuery;

            var query = _context.LeaveBalances
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(l => l.Employee.FirstName.Contains(searchQuery) || 
                                         l.Employee.LastName.Contains(searchQuery) || 
                                         l.LeaveType.Code.Contains(searchQuery));
            }

            var pagedList = await query.OrderByDescending(l => l.UpdatedAt).ToPagedListAsync(pageNumber, pageSize);
            return View(pagedList);
        }

        // GET: LeaveBalances/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveBalance = await _context.LeaveBalances
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (leaveBalance == null)
            {
                return NotFound();
            }

            return View(leaveBalance);
        }

        // GET: LeaveBalances/Create
        public IActionResult Create()
        {
            ViewData["EmployeeId"] = new SelectList(_context.emp, "Id", "Id");
            ViewData["LeaveTypeId"] = new SelectList(_context.LeaveTypes, "Id", "Code");
            return View();
        }

        // POST: LeaveBalances/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EmployeeId,LeaveTypeId,Year,Allocated,Used,Pending,CarriedForward,Available,UpdatedAt")] LeaveBalance leaveBalance)
        {
            if (ModelState.IsValid)
            {
                _context.Add(leaveBalance);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeId"] = new SelectList(_context.emp, "Id", "Id", leaveBalance.EmployeeId);
            ViewData["LeaveTypeId"] = new SelectList(_context.LeaveTypes, "Id", "Code", leaveBalance.LeaveTypeId);
            return View(leaveBalance);
        }

        // GET: LeaveBalances/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveBalance = await _context.LeaveBalances.FindAsync(id);
            if (leaveBalance == null)
            {
                return NotFound();
            }
            ViewData["EmployeeId"] = new SelectList(_context.emp, "Id", "Id", leaveBalance.EmployeeId);
            ViewData["LeaveTypeId"] = new SelectList(_context.LeaveTypes, "Id", "Code", leaveBalance.LeaveTypeId);
            return View(leaveBalance);
        }

        // POST: LeaveBalances/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Id,EmployeeId,LeaveTypeId,Year,Allocated,Used,Pending,CarriedForward,Available,UpdatedAt")] LeaveBalance leaveBalance)
        {
            if (id != leaveBalance.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(leaveBalance);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LeaveBalanceExists(leaveBalance.Id))
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
            ViewData["EmployeeId"] = new SelectList(_context.emp, "Id", "Id", leaveBalance.EmployeeId);
            ViewData["LeaveTypeId"] = new SelectList(_context.LeaveTypes, "Id", "Code", leaveBalance.LeaveTypeId);
            return View(leaveBalance);
        }

        // GET: LeaveBalances/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveBalance = await _context.LeaveBalances
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (leaveBalance == null)
            {
                return NotFound();
            }

            return View(leaveBalance);
        }

        // POST: LeaveBalances/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            var leaveBalance = await _context.LeaveBalances.FindAsync(id);
            if (leaveBalance != null)
            {
                _context.LeaveBalances.Remove(leaveBalance);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: LeaveBalances/SearchEmployees
        [HttpGet]
        public async Task<IActionResult> SearchEmployees(string term)
        {
            if (string.IsNullOrEmpty(term) || term.Length < 2)
            {
                return PartialView("_EmployeeSearchResults", new List<Employee>());
            }

            var employees = await _context.emp
                .Where(e => e.FirstName.Contains(term) || 
                            e.LastName.Contains(term) || 
                            e.Code.Contains(term))
                .Take(10)
                .ToListAsync();

            return PartialView("_EmployeeSearchResults", employees);
        }

        private bool LeaveBalanceExists(decimal id)
        {
            return _context.LeaveBalances.Any(e => e.Id == id);
        }
    }
}
