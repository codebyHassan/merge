using AlignHR.Data;
using AlignHR.Models;
using AlignHR.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AlignHR.Controllers
{
    public class AttendenceLogsController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public AttendenceLogsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string search)
        {
            var obj = _context.AttendenceLogs.Include(d => d.Employee).Include(d => d.Shift).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                obj = obj.Where(d => d.Employee.FirstName.Contains(search) ||
                d.Employee.LastName.Contains(search) ||
                d.Source.Contains(search));
            }

            return View(obj.ToList());
        }

        public IActionResult create()
        {
            ViewBag.Employees = new SelectList(_context.emp.Select(e => new { Id = e.Id, Name = e.FirstName + " " + e.LastName, e.Location, e.Department, e.Code, e.EmploymentStatus }).ToList(), "Id", "Name");
            ViewBag.PunchTypes = new SelectList(Enum.GetValues(typeof(PunchType)).Cast<PunchType>().Select(v => new { Id = v, Name = v.ToString() }), "Id", "Name");
            ViewBag.Shifts = new SelectList(_context.shifts.Select(s => new { Id = s.Id, Name = s.ShiftName + " (" + s.GroupName + ")" }).ToList(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult create(AttendenceLogs d)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;
                d.createdby = currentUserId;
                d.createat = DateTime.Now;
                d.updatedby = currentUserId;
                d.updateat = DateTime.Now;

                _context.AttendenceLogs.Add(d);
                _context.SaveChanges();
                return RedirectToAction("index");
            }
            ViewBag.Employees = new SelectList(_context.emp.Select(e => new { Id = e.Id, Name = e.FirstName + " " + e.LastName, e.Location, e.Department, e.Code, e.EmploymentStatus }).ToList(), "Id", "Name");
            ViewBag.PunchTypes = new SelectList(Enum.GetValues(typeof(PunchType)).Cast<PunchType>().Select(v => new { Id = v, Name = v.ToString() }), "Id", "Name", d.PunchType);
            ViewBag.Shifts = new SelectList(_context.shifts.Select(s => new { Id = s.Id, Name = s.ShiftName + " (" + s.GroupName + ")" }).ToList(), "Id", "Name", d.shiftidFk);
            return View(d);
        }

        [RequireUrlPermission]
        public IActionResult Edit(int id)
        {
            var obj = _context.AttendenceLogs.Include(e => e.Employee).FirstOrDefault(x => x.id == id);
            if (obj == null)
                return NotFound();

            ViewBag.Employees = new SelectList(_context.emp.Select(e => new { Id = e.Id, Name = e.FirstName + " " + e.LastName }).ToList(), "Id", "Name", obj.EmployeeID);
            ViewBag.PunchTypes = new SelectList(Enum.GetValues(typeof(PunchType)).Cast<PunchType>().Select(v => new { Id = v, Name = v.ToString() }), "Id", "Name", obj.PunchType);
            ViewBag.Shifts = new SelectList(_context.shifts.Select(s => new { Id = s.Id, Name = s.ShiftName + " (" + s.GroupName + ")" }).ToList(), "Id", "Name", obj.shiftidFk);
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(AttendenceLogs d)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;

                var existing = _context.AttendenceLogs.AsNoTracking().FirstOrDefault(x => x.id == d.id);
                if (existing == null) return NotFound();

                d.createdby = existing.createdby;
                d.createat = existing.createat;
                d.updatedby = currentUserId;
                d.updateat = DateTime.Now;

                _context.AttendenceLogs.Update(d);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }
            ViewBag.Employees = new SelectList(_context.emp.Select(e => new { Id = e.Id, Name = e.FirstName + " " + e.LastName }).ToList(), "Id", "Name", d.EmployeeID);
            ViewBag.PunchTypes = new SelectList(Enum.GetValues(typeof(PunchType)).Cast<PunchType>().Select(v => new { Id = v, Name = v.ToString() }), "Id", "Name", d.PunchType);
            ViewBag.Shifts = new SelectList(_context.shifts.Select(s => new { Id = s.Id, Name = s.ShiftName + " (" + s.GroupName + ")" }).ToList(), "Id", "Name", d.shiftidFk);
            return View(d);
        }

        [RequireUrlPermission]
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var obj = _context.AttendenceLogs.Include(d => d.Employee).FirstOrDefault(d => d.id == id);
            if (obj == null)
                return NotFound();

            return View(obj);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var obj = _context.AttendenceLogs.Find(id);
            if (obj != null) { try { _context.AttendenceLogs.Remove(obj); _context.SaveChanges(); TempData["Success"] = "Deleted successfully."; } catch (Microsoft.EntityFrameworkCore.DbUpdateException) { TempData["Error"] = "Cannot delete. This record is linked with other data."; } }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult GetEmployeeDetails(int? EmployeeID)
        {
            if (EmployeeID == null)
            {
                return Content("");
            }

            var employee = _context.emp
                .Include(e => e.Department)
                .Include(e => e.Location)
                .Include(e => e.EmploymentStatus)
                .FirstOrDefault(e => e.Id == EmployeeID);

            if (employee == null)
            {
                return Content("");
            }

            return PartialView("_EmployeeDetailCard", employee);
        }
    }
}

