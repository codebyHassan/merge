using AlignHR.Data;
using AlignHR.Models;
using AlignHR.Security;
using Microsoft.AspNetCore.Mvc;

namespace AlignHR.Controllers
{
    public class FunctionController : BaseController
    {
        private readonly ApplicationDbContext _context;
        public FunctionController(ApplicationDbContext context)
        {
            _context = context;
        }

        [RequireUrlPermission]
        public IActionResult Index(string search)
        {
            var obj = _context.functions.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                obj = obj.Where(d => d.Name.Contains(search) || d.route.Contains(search)); 
            }


            return View(obj.ToList());
        }

        [RequireUrlPermission]
        public IActionResult create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult create(Function f)
        {
            if (ModelState.IsValid)
            {
                _context.functions.Add(f);
                _context.SaveChanges();
                return RedirectToAction("index");
            }
            return View(f);
        }

        // GET: Function/Delete/5
        [RequireUrlPermission]
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var f = _context.functions.FirstOrDefault(d => d.Id == id);
            if (f == null)
                return NotFound();

            return View(f);
        }

        // POST: Function/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var fun = _context.functions.Find(id);
            if (fun != null) { try { _context.functions.Remove(fun); _context.SaveChanges(); TempData["Success"] = "Deleted successfully."; } catch (Microsoft.EntityFrameworkCore.DbUpdateException) { TempData["Error"] = "Cannot delete. This record is linked with other data."; } }

            return RedirectToAction(nameof(Index));
        }



        ////EDIT

        [RequireUrlPermission]

        public IActionResult Edit(int id)
        {
            var l = _context.functions.Find(id);
            if (l == null)
                return NotFound();



            return View(l);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Function l)
        {
            if (ModelState.IsValid)
            {
                l.updateat = DateTime.Now;

                _context.functions.Update(l);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }



            return View(l);
        }
    }
}


