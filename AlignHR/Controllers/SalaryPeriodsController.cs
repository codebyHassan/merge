using AlignHR.Data;
using AlignHR.Models;
using AlignHR.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace AlignHR.Controllers
{
    public class SalaryPeriodsController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public SalaryPeriodsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [RequireUrlPermission]
        public IActionResult Index(string search, int? page)
        {
            int pageSize = 15;
            int pageNumber = page ?? 1;

            var obj = _context.SalaryPeriod.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                obj = obj.Where(d => d.PeriodName.Contains(search));
            }

            obj = obj.OrderByDescending(d => d.SalaryPeriodID);

            var model = obj.ToPagedList(pageNumber, pageSize);
            ViewBag.Search = search;

            return View(model);
        }

        [RequireUrlPermission]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SalaryPeriod d)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;

                d.createat = DateTime.Now;
                d.createdby = currentUserId;

                _context.SalaryPeriod.Add(d);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(d);
        }

        [RequireUrlPermission]
        public IActionResult Edit(int id)
        {
            var period = _context.SalaryPeriod.Find(id);
            if (period == null)
                return NotFound();

            return View(period);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(SalaryPeriod d)
        {
            var existing = _context.SalaryPeriod.AsNoTracking().FirstOrDefault(p => p.SalaryPeriodID == d.SalaryPeriodID);
            if (existing != null && existing.IsPostedToGL)
            {
                TempData["Error"] = "This Salary Period is already Posted to GL and cannot be modified.";
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;

                d.updateat = DateTime.Now;
                d.updatedby = currentUserId;

                _context.SalaryPeriod.Update(d);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(d);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var period = _context.SalaryPeriod.Find(id);
            if (period != null) 
            { 
                if (period.IsPostedToGL)
                {
                    TempData["Error"] = "This Salary Period is already Posted to GL and cannot be deleted.";
                    return RedirectToAction("Index");
                }
                try 
                { 
                    _context.SalaryPeriod.Remove(period); 
                    _context.SaveChanges(); 
                    TempData["Success"] = "Deleted successfully."; 
                } 
                catch (Microsoft.EntityFrameworkCore.DbUpdateException) 
                { 
                    TempData["Error"] = "Cannot delete. This record is linked with other data."; 
                } 
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Export(string search, string columns, string downloadToken)
        {
            if (!string.IsNullOrEmpty(downloadToken))
            {
                Response.Cookies.Append("fileDownloadToken", downloadToken, new Microsoft.AspNetCore.Http.CookieOptions { Path = "/", HttpOnly = false, Secure = false });
            }

            var query = _context.SalaryPeriod.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(d => d.PeriodName.Contains(search));
            }

            var list = query.OrderByDescending(d => d.SalaryPeriodID).ToList();

            var selectedCols = !string.IsNullOrEmpty(columns)
                ? columns.Split(',').Select(c => c.Trim().ToLowerInvariant()).ToList()
                : new List<string> { "period name", "start date", "end date", "days", "working days", "gl status" };

            var headersList = new List<string>();
            if (selectedCols.Contains("period name")) headersList.Add("Period Name");
            if (selectedCols.Contains("start date")) headersList.Add("Start Date");
            if (selectedCols.Contains("end date")) headersList.Add("End Date");
            if (selectedCols.Contains("days")) headersList.Add("Days");
            if (selectedCols.Contains("working days")) headersList.Add("Working Days");
            if (selectedCols.Contains("gl status")) headersList.Add("GL Status");

            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (var package = new OfficeOpenXml.ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("SalaryPeriods");
                for (int col = 0; col < headersList.Count; col++)
                {
                    worksheet.Cells[1, col + 1].Value = headersList[col];
                    worksheet.Cells[1, col + 1].Style.Font.Bold = true;
                }
                int rowIdx = 2;
                foreach (var item in list)
                {
                    int colIdx = 1;
                    if (selectedCols.Contains("period name")) worksheet.Cells[rowIdx, colIdx++].Value = item.PeriodName;
                    if (selectedCols.Contains("start date")) worksheet.Cells[rowIdx, colIdx++].Value = item.StartDate.ToString("yyyy-MM-dd");
                    if (selectedCols.Contains("end date")) worksheet.Cells[rowIdx, colIdx++].Value = item.EndDate.ToString("yyyy-MM-dd");
                    if (selectedCols.Contains("days")) worksheet.Cells[rowIdx, colIdx++].Value = "";
                    if (selectedCols.Contains("working days")) worksheet.Cells[rowIdx, colIdx++].Value = "";
                    if (selectedCols.Contains("gl status")) worksheet.Cells[rowIdx, colIdx++].Value = item.IsPostedToGL ? "Locked" : "Open";
                    rowIdx++;
                }
                worksheet.Cells.AutoFitColumns();
                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "salary_periods.xlsx");
            }
        }
    }
}
