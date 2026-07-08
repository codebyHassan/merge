using AlignHR.Data;
using AlignHR.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlignHR.Controllers
{
    public class RolesController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public RolesController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View(_context.roles.ToList());
        }
        public IActionResult create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult create(Role r)
        {
            if (ModelState.IsValid)
            {

                _context.roles.Add(r);
                _context.SaveChanges();
                return RedirectToAction("index");

            }

            return View(r);
        }

        // GET: Roles/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var role = _context.roles.FirstOrDefault(d => d.Id == id);
            if (role == null)
                return NotFound();

            return View(role);
        }

        // POST: Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var roles = _context.roles.Find(id);
            if (roles != null) { try { _context.roles.Remove(roles); _context.SaveChanges(); TempData["Success"] = "Deleted successfully."; } catch (Microsoft.EntityFrameworkCore.DbUpdateException) { TempData["Error"] = "Cannot delete. This record is linked with other data."; } }

            return RedirectToAction(nameof(Index));
        }

        // GET: Roles/Edit/5
        public IActionResult Edit(int id)
        {
            var r = _context.roles.Find(id);
            if (r == null)
                return NotFound();

            return View(r);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Role r)
        {
            if (ModelState.IsValid)
            {
                r.updateat = DateTime.Now;

                _context.roles.Update(r);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(r);
        }

        // GET: Roles/Manage/5 — Checkbox-based permission management
        public async Task<IActionResult> Manage(int id)
        {
            var role = await _context.roles.FindAsync(id);
            if (role == null) return NotFound();

            var allFunctions = await _context.functions
                .OrderBy(f => f.Name)
                .Select(f => new FunctionItem
                {
                    Id = f.Id,
                    Name = f.Name ?? "",
                    Route = f.route ?? ""
                })
                .ToListAsync();

            var selectedIds = await _context.FunctionRoles
                .Where(fr => fr.RoleId == id)
                .Select(fr => fr.FunctionId)
                .ToListAsync();

            var model = new RoleFunctionsViewModel
            {
                RoleId = role.Id,
                RoleName = role.Name ?? "",
                AllFunctions = allFunctions,
                SelectedFunctionIds = selectedIds
            };

            return View(model);
        }

        // POST: Roles/Manage — Save checkbox selections
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Manage(RoleFunctionsViewModel model)
        {
            var role = await _context.roles.FindAsync(model.RoleId);
            if (role == null) return NotFound();

            // Remove all existing assignments for this role
            var existing = _context.FunctionRoles.Where(fr => fr.RoleId == model.RoleId);
            _context.FunctionRoles.RemoveRange(existing);

            // Add the newly selected ones
            if (model.SelectedFunctionIds != null)
            {
                foreach (var funcId in model.SelectedFunctionIds)
                {
                    _context.FunctionRoles.Add(new FunctionRole
                    {
                        RoleId = model.RoleId,
                        FunctionId = funcId,
                        createdby = 1,
                        createat = DateTime.Now
                    });
                }
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Permissions updated for role '{role.Name}'.";
            return RedirectToAction("Index");
        }
    }
}


