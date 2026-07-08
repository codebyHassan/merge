using AlignHR.Data;
using AlignHR.Models;
using AlignHR.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace AlignHR.Controllers
{
    public class TaxSlabMasterController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public TaxSlabMasterController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string search, int? page)
        {
            int pageSize = 15;
            int pageNumber = page ?? 1;

            var obj = _context.TaxSlabMaster.Include(t => t.FiscalYear).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                // Optionally add more search logic if needed
            }

            obj = obj.OrderByDescending(d => d.id);
            var model = obj.ToPagedList(pageNumber, pageSize);
            
            ViewBag.ActivePage = "TaxSlabMaster";
            return View(model);
        }

        public IActionResult Create()
        {
            ViewBag.FiscalYears = _context.FiscalYear.OrderByDescending(f => f.id).ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TaxSlabMaster model)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;
                model.createat = DateTime.Now;
                model.createdby = currentUserId;

                _context.TaxSlabMaster.Add(model);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FiscalYears = _context.FiscalYear.OrderByDescending(f => f.id).ToList();
            return View(model);
        }

        public IActionResult Edit(int id)
        {
            ViewBag.FiscalYears = _context.FiscalYear.OrderByDescending(f => f.id).ToList();
            var obj = _context.TaxSlabMaster.Find(id);
            if (obj == null) return NotFound();
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(TaxSlabMaster model)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;
                model.updateat = DateTime.Now;
                model.updatedby = currentUserId;

                _context.TaxSlabMaster.Update(model);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FiscalYears = _context.FiscalYear.OrderByDescending(f => f.id).ToList();
            return View(model);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();
            var obj = _context.TaxSlabMaster.FirstOrDefault(m => m.id == id);
            if (obj == null) return NotFound();
            return View(obj);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var obj = _context.TaxSlabMaster.Find(id);
            if (obj != null) { try { _context.TaxSlabMaster.Remove(obj); _context.SaveChanges(); TempData["Success"] = "Deleted successfully."; } catch (Microsoft.EntityFrameworkCore.DbUpdateException) { TempData["Error"] = "Cannot delete. This record is linked with other data."; } }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Export(string search, string columns, string downloadToken)
        {
            if (!string.IsNullOrEmpty(downloadToken))
            {
                Response.Cookies.Append("fileDownloadToken", downloadToken, new Microsoft.AspNetCore.Http.CookieOptions { Path = "/", HttpOnly = false, Secure = false });
            }

            var query = _context.TaxSlabMaster.Include(t => t.FiscalYear).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                // query = query.Where(d => d.SlabName.Contains(search));
            }

            var list = query.OrderByDescending(d => d.id).ToList();

            var selectedCols = !string.IsNullOrEmpty(columns)
                ? columns.Split(',').Select(c => c.Trim().ToLowerInvariant()).ToList()
                : new List<string> { "fiscal year", "slab name", "income range", "fixed tax", "tax rate %", "status" };

            var headersList = new List<string>();
            if (selectedCols.Contains("fiscal year")) headersList.Add("Fiscal Year");
            if (selectedCols.Contains("slab name")) headersList.Add("Slab Name");
            if (selectedCols.Contains("income range")) headersList.Add("Income Range");
            if (selectedCols.Contains("fixed tax")) headersList.Add("Fixed Tax");
            if (selectedCols.Contains("tax rate %")) headersList.Add("Tax Rate %");
            if (selectedCols.Contains("status")) headersList.Add("Status");

            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (var package = new OfficeOpenXml.ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("TaxSlabs");
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
                    if (selectedCols.Contains("slab name")) worksheet.Cells[rowIdx, colIdx++].Value = "";
                    if (selectedCols.Contains("income range")) worksheet.Cells[rowIdx, colIdx++].Value = $"{item.IncomeFrom:N0} - {item.IncomeTo?.ToString("N0") ?? "Above"}";
                    if (selectedCols.Contains("fixed tax")) worksheet.Cells[rowIdx, colIdx++].Value = item.FixedTax;
                    if (selectedCols.Contains("tax rate %")) worksheet.Cells[rowIdx, colIdx++].Value = item.RatePercent;
                    if (selectedCols.Contains("status")) worksheet.Cells[rowIdx, colIdx++].Value = item.IsActive ? "Active" : "Inactive";
                    rowIdx++;
                }
                worksheet.Cells.AutoFitColumns();
                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "tax_slabs.xlsx");
            }
        }
    }
}
