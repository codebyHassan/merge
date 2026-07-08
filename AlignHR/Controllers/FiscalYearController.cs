using AlignHR.Data;
using AlignHR.Models;
using AlignHR.Security;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace AlignHR.Controllers
{
    public class FiscalYearController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public FiscalYearController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string search, int? page)
        {
            int pageSize = 15;
            int pageNumber = page ?? 1;

            var obj = _context.FiscalYear.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                obj = obj.Where(f => f.title.Contains(search));
            }

            obj = obj.OrderByDescending(d => d.id);
            var model = obj.ToPagedList(pageNumber, pageSize);
            
            ViewBag.ActivePage = "FiscalYear";
            return View(model);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(FiscalYear model)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;
                model.createat = DateTime.Now;
                model.createdby = currentUserId;

                _context.FiscalYear.Add(model);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public IActionResult Edit(int id)
        {
            var obj = _context.FiscalYear.Find(id);
            if (obj == null) return NotFound();
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(FiscalYear model)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;
                model.updateat = DateTime.Now;
                model.updatedby = currentUserId;

                _context.FiscalYear.Update(model);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();
            var obj = _context.FiscalYear.FirstOrDefault(m => m.id == id);
            if (obj == null) return NotFound();
            return View(obj);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var obj = _context.FiscalYear.Find(id);
            if (obj != null) { try { _context.FiscalYear.Remove(obj); _context.SaveChanges(); TempData["Success"] = "Deleted successfully."; } catch (Microsoft.EntityFrameworkCore.DbUpdateException) { TempData["Error"] = "Cannot delete. This record is linked with other data."; } }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Export(string search, string columns, string downloadToken)
        {
            if (!string.IsNullOrEmpty(downloadToken))
            {
                Response.Cookies.Append("fileDownloadToken", downloadToken, new Microsoft.AspNetCore.Http.CookieOptions { Path = "/", HttpOnly = false, Secure = false });
            }

            var query = _context.FiscalYear.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(d => d.title.Contains(search));
            }

            var list = query.OrderByDescending(d => d.id).ToList();

            var selectedCols = !string.IsNullOrEmpty(columns)
                ? columns.Split(',').Select(c => c.Trim().ToLowerInvariant()).ToList()
                : new List<string> { "title", "start date", "end date", "status" };

            var headersList = new List<string>();
            if (selectedCols.Contains("title")) headersList.Add("Title");
            if (selectedCols.Contains("start date")) headersList.Add("Start Date");
            if (selectedCols.Contains("end date")) headersList.Add("End Date");
            if (selectedCols.Contains("status")) headersList.Add("Status");

            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (var package = new OfficeOpenXml.ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("FiscalYears");
                for (int col = 0; col < headersList.Count; col++)
                {
                    worksheet.Cells[1, col + 1].Value = headersList[col];
                    worksheet.Cells[1, col + 1].Style.Font.Bold = true;
                }
                int rowIdx = 2;
                foreach (var item in list)
                {
                    int colIdx = 1;
                    if (selectedCols.Contains("title")) worksheet.Cells[rowIdx, colIdx++].Value = item.title;
                    if (selectedCols.Contains("start date")) worksheet.Cells[rowIdx, colIdx++].Value = item.startdate.ToString("yyyy-MM-dd");
                    if (selectedCols.Contains("end date")) worksheet.Cells[rowIdx, colIdx++].Value = item.enddate.ToString("yyyy-MM-dd");
                    if (selectedCols.Contains("status")) worksheet.Cells[rowIdx, colIdx++].Value = "N/A";
                    rowIdx++;
                }
                worksheet.Cells.AutoFitColumns();
                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "fiscal_years.xlsx");
            }
        }
    }
}
