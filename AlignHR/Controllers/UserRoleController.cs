using AlignHR.Data;
using AlignHR.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AlignHR.Controllers
{
    public class UserRoleController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public UserRoleController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: UserRole
        public async Task<IActionResult> Index()
        {
            var userRoles = await _context.UserRoles
                .Include(ur => ur.User)
                .Include(ur => ur.Role)
                .ToListAsync();
            return View(userRoles);
        }

        // GET: UserRole/Create
        public IActionResult Create()
        {
            ViewBag.Users = new SelectList(
                _context.users.Select(u => new { u.Id, FullName = u.FirstName + " " + u.LastName }),
                "Id", "FullName");
            ViewBag.Roles = new SelectList(_context.roles.ToList(), "Id", "Name");
            return View();
        }

        // POST: UserRole/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserRole userRole)
        {
            // Check if assignment already exists
            bool exists = await _context.UserRoles
                .AnyAsync(ur => ur.UserId == userRole.UserId && ur.RoleId == userRole.RoleId);

            if (exists)
            {
                ModelState.AddModelError("", "This role is already assigned to the selected user.");
            }

            if (ModelState.IsValid)
            {
                userRole.createdby = Convert.ToInt32(HttpContext.Session.GetInt32("UserId"));
                userRole.createat = DateTime.Now;
                _context.UserRoles.Add(userRole);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.Users = new SelectList(
                _context.users.Select(u => new { u.Id, FullName = u.FirstName + " " + u.LastName }),
                "Id", "FullName");
            ViewBag.Roles = new SelectList(_context.roles.ToList(), "Id", "Name");
            return View(userRole);
        }

        // POST: UserRole/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userRole = await _context.UserRoles.FindAsync(id);
            if (userRole != null)
            {
                _context.UserRoles.Remove(userRole);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}

