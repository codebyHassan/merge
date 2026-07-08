using AlignHR.Data;
using AlignHR.Helpers;
using AlignHR.Models;
using AlignHR.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AlignHR.Controllers
{
    public class UserController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: User
        public async Task<IActionResult> Index()
        {
            var users = await _context.users.AsNoTracking().ToListAsync();

            // Load role assignments separately to avoid correlated subquery translation issues
            // and anonymous-type dynamic binding problems in Razor views.
            var roleRows = await _context.UserRoles
                .AsNoTracking()
                .Join(_context.roles.AsNoTracking(),
                      ur => ur.RoleId,
                      r  => r.Id,
                      (ur, r) => new { ur.UserId, RoleName = r.Name ?? "No Role" })
                .ToListAsync();

            var roleMap = roleRows
                .GroupBy(x => x.UserId)
                .ToDictionary(g => g.Key, g => g.First().RoleName);

            ViewBag.UserRoles = roleMap;
            return View(users);
        }

        // ─── LOGIN ───────────────────────────────────────────────────────────

        // GET: User/Login
        [SkipPermissionCheck]
        public IActionResult Login()
        {
            if (PermissionHelper.IsLoggedIn(HttpContext))
                return RedirectToAction("Index", "Dashboard");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SkipPermissionCheck]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Email and password are required.");
                return View();
            }

            // Attempt to find user with linked Employee
            var user = await _context.users
                .Include(u => u.Employee)
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

            if (user != null)
            {
                // Verify password using BCrypt
                if (!PasswordHelper.VerifyPassword(password, user.Password))
                {
                    ModelState.AddModelError("", "Invalid email or password. Please try again.");
                    return View();
                }


                // Verify status
                if (user.UsrIsActive?.Trim().ToLower() != "yes")
                {
                    ModelState.AddModelError("", "Your account is currently inactive. Please contact support.");
                    return View();
                }

                // Load roles for this user
                var roleIds = await _context.UserRoles
                    .Where(ur => ur.UserId == user.Id)
                    .Select(ur => ur.RoleId)
                    .ToListAsync();

                var roleNames = await _context.roles
                    .Where(r => roleIds.Contains(r.Id))
                    .Select(r => r.Name ?? "")
                    .ToListAsync();

                // Load allowed functions for this user's roles in 2 steps to avoid complex SQL generation (CTE/WITH)
                var functionIds = await _context.FunctionRoles
                    .Where(fr => roleIds.Contains(fr.RoleId))
                    .Select(fr => fr.FunctionId)
                    .Distinct()
                    .ToListAsync();

                var allowedFunctions = await _context.functions
                    .Where(f => functionIds.Contains(f.Id))
                    .Select(f => new
                    {
                        f.Name,
                        f.route
                    })
                    .ToListAsync();

                var functionNames = allowedFunctions
                    .Select(f => f.Name ?? "")
                    .ToList();

                var allowedUrls = allowedFunctions
                    .Where(f => !string.IsNullOrEmpty(f.route))
                    .Select(f => f.route!)
                    .ToList();

                // Build menu items from allowed functions
                var menuItems = allowedFunctions
                    .Where(f => !string.IsNullOrEmpty(f.route))
                    .Select(f => new MenuItem
                    {
                        Title = f.Name ?? "",
                        Url = f.route ?? "",
                        FunctionName = f.Name ?? "",
                        Icon = "fas fa-circle"
                    })
                    .ToList();

                // Create and store the session
                var session = new UserSession
                {
                    UserId = user.Id,
                    Username = user.Email ?? "",
                    FullName = (user.FirstName ?? "") + " " + (user.LastName ?? ""),
                    Roles = roleNames,
                    Functions = functionNames,
                    AllowedUrls = allowedUrls,
                    MenuItems = menuItems,
                    EmployeeId = user.Employee?.Id,
                    DepartmentId = user.Employee?.DepartmentFk,
                    CanApplyForOthers = user.Employee?.CanApplyForOthers ?? false
                };

                PermissionHelper.SetUserSession(HttpContext, session);

                return RedirectToAction("Index", "Dashboard");
            }

            ModelState.AddModelError("", "Invalid email or password. Please try again.");
            return View();
        }

        // GET/POST: Logout
        [SkipPermissionCheck]
        public IActionResult Logout()
        {
            PermissionHelper.ClearSession(HttpContext);
            return RedirectToAction("Login");
        }

        // GET: User/AccessDenied
        [SkipPermissionCheck]
        public IActionResult AccessDenied()
        {
            return View();
        }

        // ─── CRUD ─────────────────────────────────────────────────────────────

        // GET: User/Create
        public IActionResult Create()
        {
            ViewBag.Roles = new SelectList(_context.roles.ToList(), "Id", "Name");
            ViewBag.Employees = new SelectList(_context.emp.Select(e => new { e.Id, FullName = e.FirstName + " " + e.LastName }).ToList(), "Id", "FullName");
            var dmsList = _context.DmsDefinationFiles
                .Where(d => d.DefinitionName != null && !d.DefinitionName.StartsWith("Custom_"))
                .GroupBy(d => d.DefinitionName)
                .Select(g => new { Id = g.First().Id, DefinitionName = g.Key })
                .ToList();
            ViewBag.DmsDefinitions = new SelectList(dmsList, "Id", "DefinitionName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user, int? RoleId)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;
                user.createdby = currentUserId;
                user.createat = DateTime.Now;
                user.updatedby = currentUserId;
                user.updateat = DateTime.Now;
                user.UsrIsActive = "Yes";

                // Hash password
                user.Password = PasswordHelper.HashPassword(user.Password);

                _context.Add(user);
                await _context.SaveChangesAsync();

                // Assign role if selected
                if (RoleId.HasValue && RoleId.Value > 0)
                {
                    var userRole = new UserRole
                    {
                        UserId = user.Id,
                        RoleId = RoleId.Value,
                        createdby = currentUserId,
                        createat = DateTime.Now
                    };
                    _context.UserRoles.Add(userRole);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("Index");
            }

            ViewBag.Roles = new SelectList(_context.roles.ToList(), "Id", "Name");
            ViewBag.Employees = new SelectList(_context.emp.Select(e => new { e.Id, FullName = e.FirstName + " " + e.LastName }).ToList(), "Id", "FullName", user.EmployeeId);
            var dmsList = _context.DmsDefinationFiles
                .Where(d => d.DefinitionName != null && !d.DefinitionName.StartsWith("Custom_"))
                .GroupBy(d => d.DefinitionName)
                .Select(g => new { Id = g.First().Id, DefinitionName = g.Key })
                .ToList();
            ViewBag.DmsDefinitions = new SelectList(dmsList, "Id", "DefinitionName", user.DmsDefinationFK);
            return View(user);
        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.users.FindAsync(id);
            if (user == null) return NotFound();

            var currentRole = await _context.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == id);
            ViewBag.Roles = new SelectList(_context.roles.ToList(), "Id", "Name", currentRole?.RoleId);
            ViewBag.CurrentRoleId = currentRole?.RoleId;

            ViewBag.Employees = new SelectList(_context.emp.Select(e => new { e.Id, FullName = e.FirstName + " " + e.LastName }).ToList(), "Id", "FullName", user.EmployeeId);
            var dmsList = _context.DmsDefinationFiles
                .Where(d => d.DefinitionName != null && !d.DefinitionName.StartsWith("Custom_"))
                .GroupBy(d => d.DefinitionName)
                .Select(g => new { Id = g.First().Id, DefinitionName = g.Key })
                .ToList();
            ViewBag.DmsDefinitions = new SelectList(dmsList, "Id", "DefinitionName", user.DmsDefinationFK);

            return View(user);
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User user, int? RoleId)
        {
            if (id != user.Id) return NotFound();

            // If password is blank, remove it from validation as we'll keep the existing one
            if (string.IsNullOrWhiteSpace(user.Password))
            {
                ModelState.Remove("Password");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;
                    user.updatedby = currentUserId;
                    user.updateat = DateTime.Now;

                    // Don't overwrite password if blank
                    if (!string.IsNullOrWhiteSpace(user.Password))
                        user.Password = PasswordHelper.HashPassword(user.Password);
                    else
                    {
                        var existing = await _context.users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
                        user.Password = existing?.Password;
                    }

                    _context.Update(user);
                    await _context.SaveChangesAsync();

                    // Update role assignment
                    var existingRole = await _context.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == id);
                    if (RoleId.HasValue && RoleId.Value > 0)
                    {
                        if (existingRole != null)
                        {
                            existingRole.RoleId = RoleId.Value;
                            _context.UserRoles.Update(existingRole);
                        }
                        else
                        {
                            _context.UserRoles.Add(new UserRole
                            {
                                UserId = id,
                                RoleId = RoleId.Value,
                                createdby = currentUserId,
                                createat = DateTime.Now
                            });
                        }
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.users.Any(e => e.Id == id)) return NotFound();
                    else throw;
                }
                return RedirectToAction("Index");
            }

            ViewBag.Roles = new SelectList(_context.roles.ToList(), "Id", "Name");
            ViewBag.Employees = new SelectList(_context.emp.Select(e => new { e.Id, FullName = e.FirstName + " " + e.LastName }).ToList(), "Id", "FullName", user.EmployeeId);
            var dmsList = _context.DmsDefinationFiles
                .Where(d => d.DefinitionName != null && !d.DefinitionName.StartsWith("Custom_"))
                .GroupBy(d => d.DefinitionName)
                .Select(g => new { Id = g.First().Id, DefinitionName = g.Key })
                .ToList();
            ViewBag.DmsDefinitions = new SelectList(dmsList, "Id", "DefinitionName", user.DmsDefinationFK);
            return View(user);
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.users.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null) return NotFound();

            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.users.FindAsync(id);
            if (user != null) { try { var userRoles = _context.UserRoles.Where(ur => ur.UserId == id); _context.UserRoles.RemoveRange(userRoles); _context.users.Remove(user); await _context.SaveChangesAsync(); TempData["Success"] = "User deleted successfully."; } catch (Microsoft.EntityFrameworkCore.DbUpdateException) { TempData["Error"] = "Cannot delete. This user is linked with other records."; } }
            return RedirectToAction("Index");
        }
    }
}