using AlignHR.Data;
using AlignHR.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AlignHR.Controllers
{
    public class FunctionRoleController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public FunctionRoleController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FunctionRole
        public async Task<IActionResult> Index()
        {
            var functionRoles = await _context.FunctionRoles
                .Include(fr => fr.Function)
                .Include(fr => fr.Role)
                .ToListAsync();
            return View(functionRoles);
        }

        // GET: FunctionRole/Create
        public IActionResult Create()
        {
            ViewBag.Functions = new SelectList(_context.functions.ToList(), "Id", "Name");
            ViewBag.Roles = new SelectList(_context.roles.ToList(), "Id", "Name");
            return View();
        }

        // POST: FunctionRole/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FunctionRole functionRole)
        {
            // Check if assignment already exists
            bool exists = await _context.FunctionRoles
                .AnyAsync(fr => fr.FunctionId == functionRole.FunctionId && fr.RoleId == functionRole.RoleId);

            if (exists)
            {
                ModelState.AddModelError("", "This function is already assigned to the selected role.");
            }

            if (ModelState.IsValid)
            {
                functionRole.createdby = Convert.ToInt32(HttpContext.Session.GetInt32("UserId"));
                functionRole.createat = DateTime.Now;
                _context.FunctionRoles.Add(functionRole);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.Functions = new SelectList(_context.functions.ToList(), "Id", "Name");
            ViewBag.Roles = new SelectList(_context.roles.ToList(), "Id", "Name");
            return View(functionRole);
        }

        // POST: FunctionRole/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var functionRole = await _context.FunctionRoles.FindAsync(id);
            if (functionRole != null)
            {
                _context.FunctionRoles.Remove(functionRole);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}

