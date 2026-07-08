using AlignHR.Data;
using AlignHR.Models;
using AlignHR.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace AlignHR.Controllers
{
    public class TaxSurchargeController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public TaxSurchargeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string search, int? page)
        {
            int pageSize = 15;
            int pageNumber = page ?? 1;

            var obj = _context.TaxSurcharge.Include(t => t.FiscalYear).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                // Optionally add more search logic if needed
            }

            obj = obj.OrderByDescending(d => d.id);
            var model = obj.ToPagedList(pageNumber, pageSize);
            
            ViewBag.ActivePage = "TaxSurcharge";
            return View(model);
        }

        public IActionResult Create()
        {
            ViewBag.FiscalYears = _context.FiscalYear.OrderByDescending(f => f.id).ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TaxSurcharge model)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;
                model.createat = DateTime.Now;
                model.createdby = currentUserId;

                _context.TaxSurcharge.Add(model);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FiscalYears = _context.FiscalYear.OrderByDescending(f => f.id).ToList();
            return View(model);
        }

        public IActionResult Edit(int id)
        {
            ViewBag.FiscalYears = _context.FiscalYear.OrderByDescending(f => f.id).ToList();
            var obj = _context.TaxSurcharge.Find(id);
            if (obj == null) return NotFound();
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(TaxSurcharge model)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;
                model.updateat = DateTime.Now;
                model.updatedby = currentUserId;

                _context.TaxSurcharge.Update(model);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FiscalYears = _context.FiscalYear.OrderByDescending(f => f.id).ToList();
            return View(model);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();
            var obj = _context.TaxSurcharge.FirstOrDefault(m => m.id == id);
            if (obj == null) return NotFound();
            return View(obj);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var obj = _context.TaxSurcharge.Find(id);
            if (obj != null) { try { _context.TaxSurcharge.Remove(obj); _context.SaveChanges(); TempData["Success"] = "Deleted successfully."; } catch (Microsoft.EntityFrameworkCore.DbUpdateException) { TempData["Error"] = "Cannot delete. This record is linked with other data."; } }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Export(string search, string columns, string downloadToken)
        {
            if (!string.IsNullOrEmpty(downloadToken))
            {
                Response.Cookies.Append("fileDownloadToken", downloadToken, new Microsoft.AspNetCore.Http.CookieOptions { Path = "/", HttpOnly = false, Secure = false });
            }

            var query = _context.TaxSurcharge.Include(t => t.FiscalYear).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                // Optional search
            }

            var list = query.OrderByDescending(d => d.id).ToList();

            var selectedCols = !string.IsNullOrEmpty(columns)
                ? columns.Split(',').Select(c => c.Trim().ToLowerInvariant()).ToList()
                : new List<string> { "fiscal year", "rate %", "income range", "status" };

            var headersList = new List<string>();
            if (selectedCols.Contains("fiscal year")) headersList.Add("Fiscal Year");
            if (selectedCols.Contains("rate %")) headersList.Add("Rate %");
            if (selectedCols.Contains("income range")) headersList.Add("Income Range");
            if (selectedCols.Contains("status")) headersList.Add("Status");

            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (var package = new OfficeOpenXml.ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("TaxSurcharges");
                for (int col = 0; col < headersList.Count; col++)
                {
                    worksheet.Cells[1, col + 1].Value = headersList[col];
                    worksheet.Cells[1, col + 1].Style.Font.Bold = true;
                }
                int rowIdx = 2;
                foreach (var item in list)
                {
                    int colIdx = 1;
                    if (selectedCols.Contains("fiscal year")) worksheet.Cells[rowIdx, colIdx++].Value = item.FiscalYear?.title;
                    if (selectedCols.Contains("rate %")) worksheet.Cells[rowIdx, colIdx++].Value = item.RatePercent;
                    if (selectedCols.Contains("income range")) worksheet.Cells[rowIdx, colIdx++].Value = $"{item.IncomeFrom:N0} - {item.IncomeTo?.ToString("N0") ?? "Above"}";
                    if (selectedCols.Contains("status")) worksheet.Cells[rowIdx, colIdx++].Value = item.IsActive ? "Active" : "Inactive";
                    rowIdx++;
                }
                worksheet.Cells.AutoFitColumns();
                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "tax_surcharges.xlsx");
            }
        }
    }
}
