using AlignHR.Data;
using AlignHR.Models;
using AlignHR.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList;

namespace AlignHR.Controllers
{
    public class DepartmentsController : BaseController
    {
        private readonly ApplicationDbContext _context;
        public DepartmentsController(ApplicationDbContext context)
        {
            _context = context;
        }
        [RequireUrlPermission]
        public IActionResult Index(string search, int? page)
        {
            int pageSize = 15;               // Number of records per page
            int pageNumber = page ?? 1;      // Current page, default to 1

            // Start with IQueryable
            var obj = _context.department.AsQueryable();

            // Apply search filter
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

            var departments = _context.department
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                departments = departments.Where(d => d.Name.Contains(search));
            }

            var data = departments.OrderBy(d => d.Id).ToList();

            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using var package = new OfficeOpenXml.ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Departments");

            ws.Cells[1, 1].Value = "Code";
            ws.Cells[1, 2].Value = "Name";
            ws.Cells[1, 3].Value = "Owner";
            ws.Cells[1, 4].Value = "Role";
            ws.Cells[1, 5].Value = "Description";

            for (int i = 0; i < data.Count; i++)
            {
                var row = i + 2;
                ws.Cells[row, 1].Value = data[i].Code;
                ws.Cells[row, 2].Value = data[i].Name;
                ws.Cells[row, 3].Value = data[i].Owner;
                ws.Cells[row, 4].Value = data[i].Role;
                ws.Cells[row, 5].Value = data[i].Description;
            }

            ws.Cells[ws.Dimension.Address].AutoFitColumns();
            var bytes = package.GetAsByteArray();
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Departments.xlsx");
        }
        [RequireUrlPermission]
        public IActionResult create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult create(Department d)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;

                d.createat = DateTime.Now;
                d.createdby = currentUserId;

                _context.department.Add(d);
                _context.SaveChanges();
                return RedirectToAction("index");

            }
            return View(d);
        }
        [RequireUrlPermission]
        public IActionResult Details(int id)
        {
            var department = _context.department.Find(id);
            if (department == null)
                return NotFound();

            return View(department);
        }

        // GET: Departments/Delete/5

        [RequireUrlPermission]

        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var department = _context.department.FirstOrDefault(d => d.Id == id);
            if (department == null)
                return NotFound();

            return View(department);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var department = _context.department.Find(id);
            if (department != null)
            {
                try
                {
                    _context.department.Remove(department);
                    _context.SaveChanges();
                    TempData["Success"] = "Department deleted successfully.";
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException)
                {
                    TempData["Error"] = "Cannot delete. This department is linked with other records.";
                }
            }

            return RedirectToAction(nameof(Index));
        }

        ////EDIT

        [RequireUrlPermission]

        public IActionResult Edit(int id)
        {
            var dep = _context.department.Find(id);
            if (dep == null)
                return NotFound();



            return View(dep);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Department d)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;

                d.updateat = DateTime.Now;
                d.updatedby = currentUserId;


                _context.department.Update(d);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }



            return View(d);
        }
    }
}


