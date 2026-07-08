using AlignHR.Data;
using AlignHR.Models;
using AlignHR.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace AlignHR.Controllers
{
    public class AttendenceLogsController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public AttendenceLogsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string search, int? page)
        {
            var obj = _context.AttendenceLogs.Include(d => d.Employee).Include(d => d.Shift).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                obj = obj.Where(d => d.Employee.FirstName.Contains(search) ||
                d.Employee.LastName.Contains(search) ||
                d.Source.Contains(search));
            }

            int pageSize = 15;
            int pageNumber = page ?? 1;

            var pagedLogs = await obj.OrderByDescending(d => d.id).ToPagedListAsync(pageNumber, pageSize);

            return View(pagedLogs);
        }

        [RequireUrlPermission]
        public IActionResult Export(string search, string downloadToken)
        {
            if (!string.IsNullOrEmpty(downloadToken))
            {
                Response.Cookies.Append("fileDownloadToken", downloadToken, new Microsoft.AspNetCore.Http.CookieOptions
                {
                    Path = "/",
                    HttpOnly = false,
                    Secure = false
                });
            }

            var query = _context.AttendenceLogs.Include(d => d.Employee).Include(d => d.Shift).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(d => d.Employee.FirstName.Contains(search) ||
                d.Employee.LastName.Contains(search) ||
                d.Source.Contains(search));
            }

            var data = query.OrderByDescending(d => d.id).ToList();

            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using var package = new OfficeOpenXml.ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("AttendenceLogs");

            ws.Cells[1, 1].Value = "ID";
            ws.Cells[1, 2].Value = "Employee Code";
            ws.Cells[1, 3].Value = "Employee Name";
            ws.Cells[1, 4].Value = "Attendance Date";
            ws.Cells[1, 5].Value = "Punch Time";
            ws.Cells[1, 6].Value = "Type";
            ws.Cells[1, 7].Value = "Source";
            ws.Cells[1, 8].Value = "Device";

            for (int i = 0; i < data.Count; i++)
            {
                var row = i + 2;
                var item = data[i];
                ws.Cells[row, 1].Value = item.id;
                ws.Cells[row, 2].Value = item.Employee?.Code;
                ws.Cells[row, 3].Value = item.Employee != null ? $"{item.Employee.FirstName} {item.Employee.LastName}" : "";
                ws.Cells[row, 4].Value = item.AttendenceDate.ToString("yyyy-MM-dd");
                ws.Cells[row, 5].Value = item.PunchTime.ToString("HH:mm:ss");
                ws.Cells[row, 6].Value = item.PunchType?.ToString();
                ws.Cells[row, 7].Value = item.Source;
                ws.Cells[row, 8].Value = item.Devicesd;
            }

            ws.Cells[ws.Dimension.Address].AutoFitColumns();
            var bytes = package.GetAsByteArray();
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AttendenceLogs.xlsx");
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

