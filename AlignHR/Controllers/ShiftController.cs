using AlignHR.Data;
using AlignHR.Models;
using AlignHR.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace AlignHR.Controllers
{
    public class ShiftController : BaseController
    {
        private readonly ApplicationDbContext _context;
        public ShiftController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(string search, string group, int? page)
        {
            int pageSize = 10;
            int pageNumber = page ?? 1;

            var query = _context.shifts.OrderBy(s => s.Id).AsQueryable();
            if (!string.IsNullOrEmpty(search))
                query = query.Where(s => s.ShiftName.Contains(search));

            if (!string.IsNullOrEmpty(group) && Enum.TryParse<GroupName>(group, out var groupEnum))
                query = query.Where(s => s.GroupName == groupEnum);

            var model = query.ToPagedList(pageNumber, pageSize);
            return View(model);
        }

        public IActionResult Export(string search, string group, string downloadToken)
        {
            if (!string.IsNullOrEmpty(downloadToken))
            {
                Response.Cookies.Append("fileDownloadToken", downloadToken, new Microsoft.AspNetCore.Http.CookieOptions
                {
                    Path = "/",
                    HttpOnly = false, // Must be readable by JavaScript
                    Secure = false
                });
            }

            var query = _context.shifts.OrderBy(s => s.Id).AsQueryable();
            if (!string.IsNullOrEmpty(search))
                query = query.Where(s => s.ShiftName.Contains(search));
            if (!string.IsNullOrEmpty(group) && Enum.TryParse<GroupName>(group, out var groupEnum))
                query = query.Where(s => s.GroupName == groupEnum);

            var shifts = query.ToList();

            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using var package = new OfficeOpenXml.ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Shifts");

            // Header
            ws.Cells[1, 1].Value = "Shift Name";
            ws.Cells[1, 2].Value = "Group";
            ws.Cells[1, 3].Value = "Start Time";
            ws.Cells[1, 4].Value = "End Time";
            ws.Cells[1, 5].Value = "Grace Time";
            ws.Cells[1, 6].Value = "Break Start";
            ws.Cells[1, 7].Value = "Break End";
            ws.Cells[1, 8].Value = "Night Shift";
            using (var hdr = ws.Cells[1, 1, 1, 8])
            {
                hdr.Style.Font.Bold = true;
                hdr.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                hdr.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));
                hdr.Style.Font.Color.SetColor(System.Drawing.Color.White);
            }

            // Rows
            for (int i = 0; i < shifts.Count; i++)
            {
                var s = shifts[i];
                int row = i + 2;
                ws.Cells[row, 1].Value = s.ShiftName;
                ws.Cells[row, 2].Value = s.GroupName?.ToString() ?? "";
                ws.Cells[row, 3].Value = s.StartTime?.ToString("HH:mm") ?? "";
                ws.Cells[row, 4].Value = s.EndTime?.ToString("HH:mm") ?? "";
                ws.Cells[row, 5].Value = s.GraceTime?.ToString("HH:mm") ?? "";
                ws.Cells[row, 6].Value = s.BreakStartTime?.ToString("HH:mm") ?? "";
                ws.Cells[row, 7].Value = s.BreakEndTime?.ToString("HH:mm") ?? "";
                ws.Cells[row, 8].Value = s.IsNightShift == true ? "Yes" : "No";
            }

            ws.Cells[ws.Dimension.Address].AutoFitColumns();
            var bytes = package.GetAsByteArray();
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Shifts.xlsx");
        }
        public IActionResult create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult create(Shift s)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;
                s.createdby = currentUserId;
                s.createat = DateTime.Now;


                _context.shifts.Add(s);
                _context.SaveChanges();
                return RedirectToAction("index");

            }
            return View(s);
        }

        public IActionResult Details(int id)
        {
            var shift = _context.shifts.Find(id);
            if (shift == null)
                return NotFound();

            return View(shift);
        }

        public IActionResult Edit(int id)
        {
            var shifts = _context.shifts.Find(id);
            if (shifts == null)
                return NotFound();

            return View(shifts);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Shift s)
        {
            if (ModelState.IsValid)
            {

                var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;

                s.updateat = DateTime.Now;
                s.updatedby = currentUserId;


                _context.shifts.Update(s);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }


            return View(s);
        }

        // GET: Departments/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var s = _context.shifts.FirstOrDefault(d => d.Id == id);
            if (s == null)
                return NotFound();

            return View(s);
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var s = _context.shifts.Find(id);
            if (s != null) { try { _context.shifts.Remove(s); _context.SaveChanges(); TempData["Success"] = "Deleted successfully."; } catch (Microsoft.EntityFrameworkCore.DbUpdateException) { TempData["Error"] = "Cannot delete. This record is linked with other data."; } }

            return RedirectToAction(nameof(Index));
        }
    }
}



