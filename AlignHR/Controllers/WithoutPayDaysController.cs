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
    public class WithoutPayDaysController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IPayrollLockService _lockService;

        public WithoutPayDaysController(ApplicationDbContext context, IPayrollLockService lockService)
        {
            _context = context;
            _lockService = lockService;
        }

        [RequireUrlPermission]
        public IActionResult Index(string search, int? page)
        {
            int pageSize = 15;
            int pageNumber = page ?? 1;

            var obj = _context.WithoutPayDays
                .Include(i => i.Employee)
                .Include(i => i.SalaryPeriod)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                obj = obj.Where(d => d.Employee.Code.Contains(search) || d.SalaryPeriod.PeriodName.Contains(search));
            }

            obj = obj.OrderByDescending(d => d.id);

            var model = obj.ToPagedList(pageNumber, pageSize);
            ViewBag.Search = search;

            return View(model);
        }

        [RequireUrlPermission]
        public IActionResult Create()
        {
            ViewBag.EmployeeId = new SelectList(_context.emp, "Id", "Code");
            ViewBag.PeriodSalery = new SelectList(_context.SalaryPeriod.Where(s => !s.IsPostedToGL), "SalaryPeriodID", "PeriodName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WithoutPayDays d)
        {
            if (ModelState.IsValid)
            {
                if (await _lockService.IsRecordLocked(d.EmployeeId, d.PeriodSalery))
                {
                    ModelState.AddModelError("", "Salary Slip generated or Period Posted to GL - so this record cannot be added, edited or deleted");
                    ViewBag.EmployeeId = new SelectList(_context.emp, "Id", "Code", d.EmployeeId);
                    ViewBag.PeriodSalery = new SelectList(_context.SalaryPeriod.Where(s => !s.IsPostedToGL), "SalaryPeriodID", "PeriodName", d.PeriodSalery);
                    return View(d);
                }
                var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;

                d.createat = DateTime.Now;
                d.createdby = currentUserId;

                _context.WithoutPayDays.Add(d);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EmployeeId = new SelectList(_context.emp, "Id", "Code", d.EmployeeId);
            ViewBag.PeriodSalery = new SelectList(_context.SalaryPeriod.Where(s => !s.IsPostedToGL), "SalaryPeriodID", "PeriodName", d.PeriodSalery);
            return View(d);
        }

        [RequireUrlPermission]
        public async Task<IActionResult> Details(int id)
        {
            var record = await _context.WithoutPayDays
                .Include(r => r.Employee)
                .Include(r => r.SalaryPeriod)
                .FirstOrDefaultAsync(m => m.id == id);

            if (record == null)
                return NotFound();

            return View(record);
        }

        [RequireUrlPermission]
        public IActionResult Edit(int id)
        {
            var record = _context.WithoutPayDays
                .Include(r => r.Employee)
                .FirstOrDefault(r => r.id == id);

            if (record == null)
                return NotFound();

            ViewBag.PeriodSalery = new SelectList(_context.SalaryPeriod.Where(s => !s.IsPostedToGL), "SalaryPeriodID", "PeriodName", record.PeriodSalery);
            return View(record);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(WithoutPayDays d)
        {
            if (ModelState.IsValid)
            {
                if (await _lockService.IsRecordLocked(d.EmployeeId, d.PeriodSalery))
                {
                    ModelState.AddModelError("", "Salary Slip generated or Period Posted to GL - so this record cannot be added, edited or deleted");
                    ViewBag.EmployeeId = new SelectList(_context.emp, "Id", "Code", d.EmployeeId);
                    ViewBag.PeriodSalery = new SelectList(_context.SalaryPeriod.Where(s => !s.IsPostedToGL), "SalaryPeriodID", "PeriodName", d.PeriodSalery);
                    return View(d);
                }
                var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;

                d.updateat = DateTime.Now;
                d.updatedby = currentUserId;

                _context.WithoutPayDays.Update(d);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }
            ViewBag.EmployeeId = new SelectList(_context.emp, "Id", "Code", d.EmployeeId);
            ViewBag.PeriodSalery = new SelectList(_context.SalaryPeriod.Where(s => !s.IsPostedToGL), "SalaryPeriodID", "PeriodName", d.PeriodSalery);
            return View(d);
        }

        [RequireUrlPermission]
        public IActionResult BulkUpload()
        {
            ViewBag.PeriodSalery = new SelectList(_context.SalaryPeriod.Where(s => !s.IsPostedToGL), "SalaryPeriodID", "PeriodName");
            return View();
        }

        [HttpGet]
        public IActionResult DownloadSampleCsv()
        {
            var builder = new System.Text.StringBuilder();
            builder.AppendLine("EmployeeCode,Days");
            builder.AppendLine("EMP001,2.0"); // Sample row
            
            var csvBytes = System.Text.Encoding.UTF8.GetBytes(builder.ToString());
            return File(csvBytes, "text/csv", "WithoutPayDays_Sample.csv");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkUpload(IFormFile file, int PeriodSalery)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("", "Please upload a valid CSV file.");
                ViewBag.PeriodSalery = new SelectList(_context.SalaryPeriod.Where(s => !s.IsPostedToGL), "SalaryPeriodID", "PeriodName", PeriodSalery);
                return View();
            }

            var errors = new List<string>();
            var recordsToSave = new List<WithoutPayDays>();
            var codesInFile = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;

            // Block upload if period is locked
            if (await _lockService.IsPeriodLocked(PeriodSalery))
            {
                TempData["Error"] = "This Salary Period is already Posted to GL and cannot be modified.";
                return RedirectToAction("Index");
            }

            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                int lineCount = 0;
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    lineCount++;

                    // Row 1: Header
                    if (lineCount == 1) continue;

                    // Row 2: Sample/Demonstration (Skipped as requested)
                    if (lineCount == 2) continue;

                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var parts = line.Split(',');
                    if (parts.Length < 2)
                    {
                        errors.Add($"Line {lineCount}: Invalid format. Expected 'EmployeeCode,Days'.");
                        continue;
                    }

                    var code = parts[0].Trim();
                    var daysStr = parts[1].Trim();

                    // Validation: Duplicate Code in File
                    if (codesInFile.Contains(code))
                    {
                        errors.Add($"Line {lineCount}: Duplicate Employee Code '{code}' found in the file.");
                        continue;
                    }
                    codesInFile.Add(code);

                    // Validation: Employee Existence
                    var employee = await _context.emp.FirstOrDefaultAsync(e => e.Code == code);
                    if (employee == null)
                    {
                        errors.Add($"Line {lineCount}: Employee with code '{code}' not found.");
                        continue;
                    }

                    // Validation: Numeric Days
                    if (!decimal.TryParse(daysStr, out decimal days))
                    {
                        errors.Add($"Line {lineCount}: Invalid days value '{daysStr}'.");
                        continue;
                    }

                    recordsToSave.Add(new WithoutPayDays
                    {
                        EmployeeId = employee.Id,
                        PeriodSalery = PeriodSalery,
                        withoutpaydays = days,
                        createdby = currentUserId,
                        createat = DateTime.Now
                    });
                }
            }

            if (errors.Any())
            {
                ViewBag.Errors = errors;
                ViewBag.PeriodSalery = new SelectList(_context.SalaryPeriod.Where(s => !s.IsPostedToGL), "SalaryPeriodID", "PeriodName", PeriodSalery);
                return View();
            }

            if (recordsToSave.Any())
            {
                _context.WithoutPayDays.AddRange(recordsToSave);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Successfully uploaded {recordsToSave.Count} records.";
            }
            else
            {
                TempData["Error"] = "No valid records found after skipping sample row.";
            }

            return RedirectToAction("Index");
        }

        [RequireUrlPermission]
        public IActionResult Delete(int id)
        {
            var record = _context.WithoutPayDays
                .Include(r => r.Employee)
                .Include(r => r.SalaryPeriod)
                .FirstOrDefault(r => r.id == id);

            if (record == null)
                return NotFound();

            return View(record);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var record = await _context.WithoutPayDays.FindAsync(id);
            if (record != null)
            {
                if (await _lockService.IsRecordLocked(record.EmployeeId, record.PeriodSalery))
                {
                    TempData["Error"] = "Salary Slip generated or Period Posted to GL - so this record cannot be added, edited or deleted";
                    return RedirectToAction(nameof(Index));
                }
                try
                {
                    _context.WithoutPayDays.Remove(record);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Deleted successfully.";
                }
                catch (DbUpdateException)
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

            var query = _context.WithoutPayDays.Include(i => i.Employee).Include(i => i.SalaryPeriod).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(d => d.Employee.Code.Contains(search) || d.SalaryPeriod.PeriodName.Contains(search));
            }

            var list = query.OrderByDescending(d => d.id).ToList();

            var selectedCols = !string.IsNullOrEmpty(columns)
                ? columns.Split(',').Select(c => c.Trim().ToLowerInvariant()).ToList()
                : new List<string> { "employee code", "salary period", "days", "created at" };

            var headersList = new List<string>();
            if (selectedCols.Contains("employee code")) headersList.Add("Employee Code");
            if (selectedCols.Contains("salary period")) headersList.Add("Salary Period");
            if (selectedCols.Contains("days")) headersList.Add("Days");
            if (selectedCols.Contains("created at")) headersList.Add("Created At");

            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (var package = new OfficeOpenXml.ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("WithoutPayDays");
                for (int col = 0; col < headersList.Count; col++)
                {
                    worksheet.Cells[1, col + 1].Value = headersList[col];
                    worksheet.Cells[1, col + 1].Style.Font.Bold = true;
                }
                int rowIdx = 2;
                foreach (var item in list)
                {
                    int colIdx = 1;
                    if (selectedCols.Contains("employee code")) worksheet.Cells[rowIdx, colIdx++].Value = item.Employee?.Code;
                    if (selectedCols.Contains("salary period")) worksheet.Cells[rowIdx, colIdx++].Value = item.SalaryPeriod?.PeriodName;
                    if (selectedCols.Contains("days")) worksheet.Cells[rowIdx, colIdx++].Value = item.withoutpaydays;
                    if (selectedCols.Contains("created at")) worksheet.Cells[rowIdx, colIdx++].Value = item.createat.ToString("yyyy-MM-dd");
                    rowIdx++;
                }
                worksheet.Cells.AutoFitColumns();
                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "without_pay_days.xlsx");
            }
        }
    }
}
