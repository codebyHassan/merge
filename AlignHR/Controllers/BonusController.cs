using AlignHR.Data;
using AlignHR.Models;
using AlignHR.Security;
using AlignHR.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace AlignHR.Controllers
{
    public class BonusController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IPayrollLockService _lockService;

        public BonusController(ApplicationDbContext context, IPayrollLockService lockService)
        {
            _context = context;
            _lockService = lockService;
        }

        [RequireUrlPermission]
        public IActionResult Index(string search, int? page)
        {
            int pageSize = 15;
            int pageNumber = page ?? 1;

            var query = _context.Bonuses
                .Include(b => b.Employee)
                .Include(b => b.BonusType)
                .Include(b => b.SalaryPeriod)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(b => b.Employee.FirstName.Contains(search) || b.Employee.LastName.Contains(search) || b.Employee.Code.Contains(search));
            }

            var model = query.OrderByDescending(b => b.createat).ToPagedList(pageNumber, pageSize);
            return View(model);
        }

        [RequireUrlPermission]
        public IActionResult Create()
        {
            ViewBag.Employees = _context.emp.Where(e => e.SalaryStatus == "Active").OrderBy(e => e.FirstName).ToList();
            ViewBag.Periods = _context.SalaryPeriod.Where(p => !p.IsPostedToGL).OrderByDescending(p => p.StartDate).ToList();
            ViewBag.BonusTypes = _context.valuesets.Where(v => v.GroupName == "BonusType" && v.IsActive).ToList();
            
            return View(new Bonus());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireUrlPermission]
        public async Task<IActionResult> Create(Bonus model)
        {
            if (ModelState.IsValid)
            {
                if (await _lockService.IsRecordLocked(model.EmployeeID, model.SalaryPeriodID))
                {
                    ModelState.AddModelError("", "Salary Slip generated - so this record cannot be deleted or edited");
                    ViewBag.Employees = _context.emp.Where(e => e.SalaryStatus == "Active").OrderBy(e => e.FirstName).ToList();
                    ViewBag.Periods = _context.SalaryPeriod.Where(p => !p.IsPostedToGL).OrderByDescending(p => p.StartDate).ToList();
                    ViewBag.BonusTypes = _context.valuesets.Where(v => v.GroupName == "BonusType" && v.IsActive).ToList();
                    return View(model);
                }
                var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;
                model.createdby = currentUserId;
                model.createat = DateTime.Now;

                _context.Bonuses.Add(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Bonus added successfully.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Employees = _context.emp.Where(e => e.SalaryStatus == "Active").OrderBy(e => e.FirstName).ToList();
            ViewBag.Periods = _context.SalaryPeriod.Where(p => !p.IsPostedToGL).OrderByDescending(p => p.StartDate).ToList();
            ViewBag.BonusTypes = _context.valuesets.Where(v => v.GroupName == "BonusType" && v.IsActive).ToList();
            return View(model);
        }

        [RequireUrlPermission]
        public async Task<IActionResult> Details(int id)
        {
            var bonus = await _context.Bonuses
                .Include(b => b.Employee)
                .Include(b => b.SalaryPeriod)
                .Include(b => b.BonusType)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (bonus == null) return NotFound();

            return View(bonus);
        }

        [RequireUrlPermission]
        public IActionResult Edit(int id)
        {
            var bonus = _context.Bonuses.Find(id);
            if (bonus == null) return NotFound();

            ViewBag.Employees = _context.emp.Where(e => e.SalaryStatus == "Active").OrderBy(e => e.FirstName).ToList();
            ViewBag.Periods = _context.SalaryPeriod.Where(p => !p.IsPostedToGL).OrderByDescending(p => p.StartDate).ToList();
            ViewBag.BonusTypes = _context.valuesets.Where(v => v.GroupName == "BonusType" && v.IsActive).ToList();
            return View(bonus);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireUrlPermission]
        public async Task<IActionResult> Edit(Bonus model)
        {
            if (ModelState.IsValid)
            {
                var existing = await _context.Bonuses.FindAsync(model.Id);
                if (existing == null) return NotFound();

                if (await _lockService.IsRecordLocked(existing.EmployeeID, existing.SalaryPeriodID) || await _lockService.IsRecordLocked(model.EmployeeID, model.SalaryPeriodID))
                {
                    ModelState.AddModelError("", "Salary Slip generated - so this record cannot be deleted or edited");
                    ViewBag.Employees = _context.emp.Where(e => e.SalaryStatus == "Active").OrderBy(e => e.FirstName).ToList();
                    ViewBag.Periods = _context.SalaryPeriod.Where(p => !p.IsPostedToGL).OrderByDescending(p => p.StartDate).ToList();
                    ViewBag.BonusTypes = _context.valuesets.Where(v => v.GroupName == "BonusType" && v.IsActive).ToList();
                    return View(model);
                }
                var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;

                existing.EmployeeID = model.EmployeeID;
                existing.SalaryPeriodID = model.SalaryPeriodID;
                existing.BonusTypeID = model.BonusTypeID;
                existing.BonusAmount = model.BonusAmount;
                existing.BonusPercentage = model.BonusPercentage;
                existing.IsPercentage = model.IsPercentage;
                existing.TaxDeduction = model.TaxDeduction;
                existing.NetBonus = model.NetBonus;
                existing.PaymentDate = model.PaymentDate;
                existing.ApprovalStatus = model.ApprovalStatus;
                existing.Remarks = model.Remarks;
                
                existing.updatedby = currentUserId;
                existing.updateat = DateTime.Now;

                await _context.SaveChangesAsync();
                TempData["Success"] = "Bonus updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Employees = _context.emp.Where(e => e.SalaryStatus == "Active").OrderBy(e => e.FirstName).ToList();
            ViewBag.Periods = _context.SalaryPeriod.Where(p => !p.IsPostedToGL).OrderByDescending(p => p.StartDate).ToList();
            ViewBag.BonusTypes = _context.valuesets.Where(v => v.GroupName == "BonusType" && v.IsActive).ToList();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireUrlPermission]
        public async Task<IActionResult> Delete(int id)
        {
            var bonus = await _context.Bonuses.FindAsync(id);
            if (bonus != null)
            {
                if (await _lockService.IsRecordLocked(bonus.EmployeeID, bonus.SalaryPeriodID))
                {
                    return Json(new { success = false, message = "Salary Slip generated - so this record cannot be deleted or edited" });
                }
                _context.Bonuses.Remove(bonus);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        // --- Get Gross Salary for selected employee ---
        [HttpGet]
        public IActionResult GetGrossSalary(int EmployeeID)
        {
            try
            {
                var latestPayroll = _context.PayRollGenrate
                    .Where(p => p.EmployeeFK == EmployeeID)
                    .OrderByDescending(p => p.id)
                    .Select(p => p.salery)
                    .FirstOrDefault();

                return Json(new { success = true, gross = latestPayroll });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, gross = 0, error = ex.Message });
            }
        }
        [HttpGet]
        public IActionResult Export(string search, string columns, string downloadToken)
        {
            if (!string.IsNullOrEmpty(downloadToken))
            {
                Response.Cookies.Append("fileDownloadToken", downloadToken, new Microsoft.AspNetCore.Http.CookieOptions { Path = "/", HttpOnly = false, Secure = false });
            }

            var query = _context.Bonuses.Include(b => b.Employee).Include(b => b.BonusType).Include(b => b.SalaryPeriod).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(b => b.Employee.FirstName.Contains(search) || b.Employee.LastName.Contains(search) || b.Employee.Code.Contains(search));
            }

            var list = query.OrderByDescending(b => b.createat).ToList();

            var selectedCols = !string.IsNullOrEmpty(columns)
                ? columns.Split(',').Select(c => c.Trim().ToLowerInvariant()).ToList()
                : new List<string> { "employee", "bonus type", "period", "amount", "tax", "net bonus", "payment date", "status" };

            var headersList = new List<string>();
            if (selectedCols.Contains("employee")) headersList.Add("Employee");
            if (selectedCols.Contains("bonus type")) headersList.Add("Bonus Type");
            if (selectedCols.Contains("period")) headersList.Add("Period");
            if (selectedCols.Contains("amount")) headersList.Add("Amount");
            if (selectedCols.Contains("tax")) headersList.Add("Tax");
            if (selectedCols.Contains("net bonus")) headersList.Add("Net Bonus");
            if (selectedCols.Contains("payment date")) headersList.Add("Payment Date");
            if (selectedCols.Contains("status")) headersList.Add("Status");

            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (var package = new OfficeOpenXml.ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Bonuses");
                for (int col = 0; col < headersList.Count; col++)
                {
                    worksheet.Cells[1, col + 1].Value = headersList[col];
                    worksheet.Cells[1, col + 1].Style.Font.Bold = true;
                }
                int rowIdx = 2;
                foreach (var item in list)
                {
                    int colIdx = 1;
                    if (selectedCols.Contains("employee")) worksheet.Cells[rowIdx, colIdx++].Value = $"{item.Employee?.FirstName} {item.Employee?.LastName} ({item.Employee?.Code})";
                    if (selectedCols.Contains("bonus type")) worksheet.Cells[rowIdx, colIdx++].Value = item.BonusType?.Name;
                    if (selectedCols.Contains("period")) worksheet.Cells[rowIdx, colIdx++].Value = item.SalaryPeriod?.PeriodName;
                    if (selectedCols.Contains("amount")) worksheet.Cells[rowIdx, colIdx++].Value = item.BonusAmount;
                    if (selectedCols.Contains("tax")) worksheet.Cells[rowIdx, colIdx++].Value = item.TaxDeduction;
                    if (selectedCols.Contains("net bonus")) worksheet.Cells[rowIdx, colIdx++].Value = item.NetBonus;
                    if (selectedCols.Contains("payment date")) worksheet.Cells[rowIdx, colIdx++].Value = item.PaymentDate.ToString("yyyy-MM-dd");
                    if (selectedCols.Contains("status")) worksheet.Cells[rowIdx, colIdx++].Value = item.ApprovalStatus;
                    rowIdx++;
                }
                worksheet.Cells.AutoFitColumns();
                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "bonuses.xlsx");
            }
        }
    }
}
