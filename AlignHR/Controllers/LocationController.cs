using AlignHR.Data;
using AlignHR.Models;
using AlignHR.Security;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace AlignHR.Controllers
{
    public class LocationController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public LocationController(ApplicationDbContext context)
        {
            _context = context;

        }
     

        [RequireUrlPermission]
        public IActionResult Index(string search, int?page)
        {
            int pageSize = 15;
            int pageNumber = page ?? 1;

            var obj = _context.location.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                obj = obj.Where(d => d.Name.Contains(search));
            }

            // Order by ID (or any other field)
            obj = obj.OrderBy(d => d.Id);

            // Convert to paged list
            var model = obj.ToPagedList(pageNumber, pageSize);

            return View(model);
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

            var locations = _context.location.AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                locations = locations.Where(l => l.Name.Contains(search));
            }

            var data = locations.OrderBy(l => l.Id).ToList();

            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using var package = new OfficeOpenXml.ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Locations");

            ws.Cells[1, 1].Value = "Name";
            ws.Cells[1, 2].Value = "Address";
            ws.Cells[1, 3].Value = "Latitude";
            ws.Cells[1, 4].Value = "Longitude";

            for (int i = 0; i < data.Count; i++)
            {
                var row = i + 2;
                ws.Cells[row, 1].Value = data[i].Name;
                ws.Cells[row, 2].Value = data[i].Adress;
                ws.Cells[row, 3].Value = data[i].Latitude;
                ws.Cells[row, 4].Value = data[i].Longitude;
            }

            ws.Cells[ws.Dimension.Address].AutoFitColumns();
            var bytes = package.GetAsByteArray();
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Locations.xlsx");
        }




        [RequireUrlPermission]
        public IActionResult create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult create(Location l)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;
                l.createdby = currentUserId;
                l.createat = DateTime.Now;
                _context.Add(l);
                _context.SaveChanges();
                return RedirectToAction("Index");

            }

            return View(l);
        }
        [RequireUrlPermission]
        public IActionResult Details(int id)
        {
            var location = _context.location.Find(id);
            if (location == null)
                return NotFound();

            return View(location);
        }

        // GET: locations/Delete/5
        [RequireUrlPermission]
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var location = _context.location.FirstOrDefault(d => d.Id == id);
            if (location == null)
                return NotFound();

            return View(location);
        }

        // POST: locations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var location = _context.location.Find(id);
            if (location != null) { try { _context.location.Remove(location); _context.SaveChanges(); TempData["Success"] = "Deleted successfully."; } catch (Microsoft.EntityFrameworkCore.DbUpdateException) { TempData["Error"] = "Cannot delete. This record is linked with other data."; } }

            return RedirectToAction(nameof(Index));
        }



        ////EDIT

        [RequireUrlPermission]
        public IActionResult Edit(int id)
        {
            var l = _context.location.Find(id);
            if (l == null)
                return NotFound();



            return View(l);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Location l)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;
                l.updateat = DateTime.Now;
                l.updatedby = currentUserId;

                _context.location.Update(l);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }



            return View(l);
        }
    }
}


