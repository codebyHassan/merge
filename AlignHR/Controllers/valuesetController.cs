using AlignHR.Data;
using AlignHR.Models;
using AlignHR.Security;
using Microsoft.AspNetCore.Mvc;

namespace AlignHR.Controllers
{
    public class valuesetController : BaseController
    {

        private readonly ApplicationDbContext _context;
        public valuesetController(ApplicationDbContext context)
        {
            _context = context;
        }
        [RequireUrlPermission]
        public IActionResult Index(string search, string groupName)
        {
            var obj = _context.valuesets.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                obj = obj.Where(d => d.Name.Contains(search) || d.GroupName.Contains(search));
            }

            if (!string.IsNullOrEmpty(groupName))
            {
                obj = obj.Where(d => d.GroupName == groupName);
            }

            ViewBag.Groups = _context.valuesets.Select(v => v.GroupName).Distinct().ToList();
            ViewBag.SelectedGroup = groupName;
            ViewBag.Search = search;

            return View(obj.ToList());
        }

        [RequireUrlPermission]
        public IActionResult create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult create(Valueset v)
        {
            if (ModelState.IsValid)
            {
                _context.valuesets.Add(v);
                _context.SaveChanges();
                return RedirectToAction("index");

            }
            return View(v);
        }

        [RequireUrlPermission]
        public IActionResult Details(int id)
        {
            var valueset = _context.valuesets.Find(id);
            if (valueset == null)
                return NotFound();

            return View(valueset);
        }

        [RequireUrlPermission]
        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();
            var valueset = _context.valuesets.Find(id);
            if (valueset == null) return NotFound();
            return View(valueset);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Valueset v)
        {
            if (ModelState.IsValid)
            {
                _context.valuesets.Update(v);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(v);
        }

        [RequireUrlPermission]
        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();
            var valueset = _context.valuesets.Find(id);
            if (valueset == null) return NotFound();
            return View(valueset);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var valueset = _context.valuesets.Find(id);
            if (valueset != null) { try { _context.valuesets.Remove(valueset); _context.SaveChanges(); TempData["Success"] = "Deleted successfully."; } catch (Microsoft.EntityFrameworkCore.DbUpdateException) { TempData["Error"] = "Cannot delete. This record is linked with other data."; } }
            return RedirectToAction(nameof(Index));
        }
    }
}

