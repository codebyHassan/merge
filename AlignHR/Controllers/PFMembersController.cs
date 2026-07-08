using AlignHR.Data;
using AlignHR.Models;
using AlignHR.Security;
using AlignHR.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using System.Threading.Tasks;

namespace AlignHR.Controllers
{
    public class PFMembersController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IPayrollLockService _lockService;

        public PFMembersController(ApplicationDbContext context, IPayrollLockService lockService)
        {
            _context = context;
            _lockService = lockService;
        }

        [RequireUrlPermission]
        public IActionResult Index(string search, int? page)
        {
            int pageSize = 15;               
            int pageNumber = page ?? 1;      

            var query = _context.PFMembers
                .Include(o => o.Employee)
                .Include(o => o.SalaryPeriod)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(d => d.Employee.FullName.Contains(search) || d.Employee.FirstName.Contains(search) || d.Employee.LastName.Contains(search) || d.SalaryPeriod.PeriodName.Contains(search));
            }

            var projected = query.Select(o => new PFMemberDisplayViewModel
            {
                id = o.id,
                EmployeeName = o.Employee.FullName,
                EmployeeCode = o.Employee.Code,
                PeriodName = o.SalaryPeriod.PeriodName,
                PFPercentage = o.SalaryPeriod.PFPercentage,
                PFAmount = o.Employee.PFAmount.GetValueOrDefault(),
                IsFixedPFAmount = o.Employee.IsFixedPFAmount,
                IsPostedToGL = o.SalaryPeriod != null && o.SalaryPeriod.IsPostedToGL,
                LatestSalary = _context.PayRollGenrate
                    .Where(p => p.EmployeeFK == o.EmployeeId)
                    .OrderByDescending(p => p.id)
                    .Select(p => (decimal?)p.salery)
                    .FirstOrDefault() ?? 0
            });

            var model = projected.OrderByDescending(d => d.id).ToPagedList(pageNumber, pageSize);

            if (Request.Headers["HX-Request"] == "true")
            {
                ViewBag.Search = search;
                return PartialView("_PFMembersTable", model);
            }

            return View(model);
        }

        [RequireUrlPermission]
        public IActionResult Create()
        {
            ViewBag.EmployeeId = new SelectList(_context.emp.Where(e => e.isPfMember && e.SalaryStatus == "Active").Select(e => new { e.Id, Name = e.FirstName + " " + e.LastName }), "Id", "Name");
            ViewBag.SalaryPeriodFK = new SelectList(_context.SalaryPeriod.Where(s => !s.IsPostedToGL), "SalaryPeriodID", "PeriodName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PFMembers d)
        {
            if (ModelState.IsValid)
            {
                if (await _lockService.IsRecordLocked(d.EmployeeId, d.SalaryPeriodFK))
                {
                    TempData["Error"] = "Salary Slip generated - so this record cannot be deleted or edited";
                    ViewBag.EmployeeId = new SelectList(_context.emp.Where(e => e.isPfMember && e.SalaryStatus == "Active").Select(e => new { e.Id, Name = e.FirstName + " " + e.LastName }), "Id", "Name", d.EmployeeId);
                    ViewBag.SalaryPeriodFK = new SelectList(_context.SalaryPeriod.Where(s => !s.IsPostedToGL), "SalaryPeriodID", "PeriodName", d.SalaryPeriodFK);
                    return View(d);
                }
                var exists = await _context.PFMembers.AnyAsync(o => o.EmployeeId == d.EmployeeId && o.SalaryPeriodFK == d.SalaryPeriodFK);
                if (exists)
                {
                    TempData["Error"] = "PF Member record already exists for this employee in the selected period.";
                    ViewBag.EmployeeId = new SelectList(_context.emp.Where(e => e.isPfMember && e.SalaryStatus == "Active").Select(e => new { e.Id, Name = e.FirstName + " " + e.LastName }), "Id", "Name", d.EmployeeId);
                    ViewBag.SalaryPeriodFK = new SelectList(_context.SalaryPeriod.Where(s => !s.IsPostedToGL), "SalaryPeriodID", "PeriodName", d.SalaryPeriodFK);
                    return View(d);
                }

                var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;
                d.createat = DateTime.Now;
                d.createdby = currentUserId;

                _context.PFMembers.Add(d);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.EmployeeId = new SelectList(_context.emp.Where(e => e.isPfMember && e.SalaryStatus == "Active").Select(e => new { e.Id, Name = e.FirstName + " " + e.LastName }), "Id", "Name", d.EmployeeId);
            ViewBag.SalaryPeriodFK = new SelectList(_context.SalaryPeriod.Where(s => !s.IsPostedToGL), "SalaryPeriodID", "PeriodName", d.SalaryPeriodFK);
            return View(d);
        }

        [RequireUrlPermission]
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var ot = _context.PFMembers
                .Include(o => o.Employee)
                .Include(o => o.SalaryPeriod)
                .FirstOrDefault(d => d.id == id);
            
            if (ot == null)
                return NotFound();

            return View(ot);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ot = await _context.PFMembers.FindAsync(id);
            if (ot != null)
            {
                if (await _lockService.IsRecordLocked(ot.EmployeeId, ot.SalaryPeriodFK))
                {
                    TempData["Error"] = "Salary Slip generated - so this record cannot be deleted or edited";
                    return RedirectToAction(nameof(Index));
                }
                try
                {
                    _context.PFMembers.Remove(ot);
                    _context.SaveChanges();
                    TempData["Success"] = "PF Member record deleted successfully.";
                }
                catch (DbUpdateException)
                {
                    TempData["Error"] = "Cannot delete this record.";
                }
            }

            return RedirectToAction(nameof(Index));
        }

        [RequireUrlPermission]
        public async Task<IActionResult> Details(int id)
        {
            var ot = await _context.PFMembers
                .Include(o => o.Employee)
                .Include(o => o.SalaryPeriod)
                .FirstOrDefaultAsync(m => m.id == id);

            if (ot == null)
                return NotFound();

            return View(ot);
        }

        [RequireUrlPermission]
        public IActionResult Edit(int id)
        {
            var ot = _context.PFMembers.Find(id);
            if (ot == null)
                return NotFound();

            ViewBag.EmployeeId = new SelectList(_context.emp.Where(e => e.isPfMember && e.SalaryStatus == "Active").Select(e => new { e.Id, Name = e.FirstName + " " + e.LastName }), "Id", "Name", ot.EmployeeId);
            ViewBag.SalaryPeriodFK = new SelectList(_context.SalaryPeriod.Where(s => !s.IsPostedToGL), "SalaryPeriodID", "PeriodName", ot.SalaryPeriodFK);

            return View(ot);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PFMembers d)
        {
            if (ModelState.IsValid)
            {
                if (await _lockService.IsRecordLocked(d.EmployeeId, d.SalaryPeriodFK))
                {
                    TempData["Error"] = "Salary Slip generated - so this record cannot be deleted or edited";
                    ViewBag.EmployeeId = new SelectList(_context.emp.Where(e => e.isPfMember && e.SalaryStatus == "Active").Select(e => new { e.Id, Name = e.FirstName + " " + e.LastName }), "Id", "Name", d.EmployeeId);
                    ViewBag.SalaryPeriodFK = new SelectList(_context.SalaryPeriod.Where(s => !s.IsPostedToGL), "SalaryPeriodID", "PeriodName", d.SalaryPeriodFK);
                    return View(d);
                }
                var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;

                d.updateat = DateTime.Now;
                d.updatedby = currentUserId;

                _context.PFMembers.Update(d);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            ViewBag.EmployeeId = new SelectList(_context.emp.Where(e => e.isPfMember && e.SalaryStatus == "Active").Select(e => new { e.Id, Name = e.FirstName + " " + e.LastName }), "Id", "Name", d.EmployeeId);
            ViewBag.SalaryPeriodFK = new SelectList(_context.SalaryPeriod.Where(s => !s.IsPostedToGL), "SalaryPeriodID", "PeriodName", d.SalaryPeriodFK);

            return View(d);
        }

        [RequireUrlPermission]
        public IActionResult BulkUpload()
        {
            ViewBag.SalaryPeriodFK = new SelectList(_context.SalaryPeriod.Where(s => !s.IsPostedToGL).OrderByDescending(s => s.SalaryPeriodID), "SalaryPeriodID", "PeriodName");
            return View();
        }

        [HttpGet]
        public IActionResult DownloadSampleCsv()
        {
            var builder = new System.Text.StringBuilder();
            builder.AppendLine("EmployeeCode,Amount");
            builder.AppendLine("EMP001,5000"); 
            
            var csvBytes = System.Text.Encoding.UTF8.GetBytes(builder.ToString());
            return File(csvBytes, "text/csv", "PFMembers_Sample.csv");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkUpload(IFormFile file, int SalaryPeriodFK)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("", "Please upload a valid CSV file.");
                ViewBag.SalaryPeriodFK = new SelectList(_context.SalaryPeriod.Where(s => !s.IsPostedToGL).OrderByDescending(s => s.SalaryPeriodID), "SalaryPeriodID", "PeriodName", SalaryPeriodFK);
                return View();
            }

            var errors = new List<string>();
            var recordsToSave = new List<PFMembers>();
            var codesInFile = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;

            // Block upload if period is locked
            if (await _lockService.IsPeriodLocked(SalaryPeriodFK))
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

                    if (lineCount == 1) continue;
                    if (lineCount == 2) continue;
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var parts = line.Split(',');
                    if (parts.Length < 2)
                    {
                        errors.Add($"Line {lineCount}: Invalid format. Expected 'EmployeeCode,Amount'.");
                        continue;
                    }

                    var code = parts[0].Trim();
                    var amountStr = parts[1].Trim();

                    if (codesInFile.Contains(code))
                    {
                        errors.Add($"Line {lineCount}: Duplicate Employee Code '{code}' found in the file.");
                        continue;
                    }
                    codesInFile.Add(code);

                    var employee = await _context.emp.FirstOrDefaultAsync(e => e.Code == code && e.SalaryStatus == "Active" && e.isPfMember);
                    if (employee == null)
                    {
                        errors.Add($"Line {lineCount}: Employee with code '{code}' not found, not active, or not a PF member.");
                        continue;
                    }

                    var alreadyExists = await _context.PFMembers.AnyAsync(o => o.EmployeeId == employee.Id && o.SalaryPeriodFK == SalaryPeriodFK);
                    if (alreadyExists)
                    {
                        errors.Add($"Line {lineCount}: PF record already exists for employee '{code}' in this period.");
                        continue;
                    }

                    if (!decimal.TryParse(amountStr, out decimal amount))
                    {
                        errors.Add($"Line {lineCount}: Invalid amount '{amountStr}'. Must be a number.");
                        continue;
                    }

                    recordsToSave.Add(new PFMembers
                    {
                        EmployeeId = employee.Id,
                        SalaryPeriodFK = SalaryPeriodFK,
                        createdby = currentUserId,
                        createat = DateTime.Now
                    });
                }
            }

            if (errors.Any())
            {
                ViewBag.Errors = errors;
                ViewBag.SalaryPeriodFK = new SelectList(_context.SalaryPeriod.Where(s => !s.IsPostedToGL).OrderByDescending(s => s.SalaryPeriodID), "SalaryPeriodID", "PeriodName", SalaryPeriodFK);
                return View();
            }

            if (recordsToSave.Any())
            {
                _context.PFMembers.AddRange(recordsToSave);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Successfully uploaded {recordsToSave.Count} records.";
            }
            else
            {
                TempData["Error"] = "No valid records found after skipping sample row.";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        [SkipPermissionCheck]
        public IActionResult GetEmployeeCard(int employeeId)
        {
            var employee = _context.emp
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .Include(e => e.Location)
                .Include(e => e.EmploymentType)
                .Include(e => e.EmploymentStatus)
                .FirstOrDefault(e => e.Id == employeeId);

            if (employee == null) return Content("");

            return PartialView("~/Views/SalaryAdjustment/_EmployeeCard.cshtml", employee);
        }

        [HttpGet]
        [SkipPermissionCheck]
        public async Task<IActionResult> CalculatePFDetails(int? EmployeeId, int? SalaryPeriodFK)
        {
            if (!EmployeeId.HasValue || EmployeeId.Value <= 0 || !SalaryPeriodFK.HasValue || SalaryPeriodFK.Value <= 0)
            {
                return Content("");
            }

            var employee = await _context.emp.FindAsync(EmployeeId.Value);
            var period = await _context.SalaryPeriod.FindAsync(SalaryPeriodFK.Value);

            if (employee == null || period == null)
            {
                return Content("");
            }

            // Fetch Latest Gross Salary
            var latestGross = await _context.PayRollGenrate
                .Where(p => p.EmployeeFK == EmployeeId.Value)
                .OrderByDescending(p => p.id)
                .Select(p => (decimal?)p.salery)
                .FirstOrDefaultAsync() ?? 0;

            decimal pfAmount = 0;
            if (employee.IsFixedPFAmount)
            {
                pfAmount = employee.PFAmount.GetValueOrDefault();
            }
            else
            {
                pfAmount = (latestGross * period.PFPercentage / 100);
            }

            return Json(new { amount = pfAmount });
        }

        [HttpGet]
        public IActionResult Export(string search, string columns, string downloadToken)
        {
            if (!string.IsNullOrEmpty(downloadToken))
            {
                Response.Cookies.Append("fileDownloadToken", downloadToken, new Microsoft.AspNetCore.Http.CookieOptions { Path = "/", HttpOnly = false, Secure = false });
            }

            var query = _context.PFMembers.Include(o => o.Employee).Include(o => o.SalaryPeriod).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(d => d.Employee.FullName.Contains(search) || d.Employee.FirstName.Contains(search) || d.Employee.LastName.Contains(search) || d.SalaryPeriod.PeriodName.Contains(search));
            }

            var list = query.Select(o => new PFMemberDisplayViewModel
            {
                id = o.id,
                EmployeeName = o.Employee.FullName,
                EmployeeCode = o.Employee.Code,
                PeriodName = o.SalaryPeriod.PeriodName,
                PFPercentage = o.SalaryPeriod.PFPercentage,
                PFAmount = o.Employee.PFAmount.GetValueOrDefault(),
                IsFixedPFAmount = o.Employee.IsFixedPFAmount,
                LatestSalary = _context.PayRollGenrate.Where(p => p.EmployeeFK == o.EmployeeId).OrderByDescending(p => p.id).Select(p => (decimal?)p.salery).FirstOrDefault() ?? 0
            }).OrderByDescending(d => d.id).ToList();

            var selectedCols = !string.IsNullOrEmpty(columns)
                ? columns.Split(',').Select(c => c.Trim().ToLowerInvariant()).ToList()
                : new List<string> { "employee", "salary period", "pf configuration" };

            var headersList = new List<string>();
            if (selectedCols.Contains("employee")) headersList.Add("Employee");
            if (selectedCols.Contains("salary period")) headersList.Add("Salary Period");
            if (selectedCols.Contains("pf configuration")) headersList.Add("PF Configuration");

            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (var package = new OfficeOpenXml.ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("PFMembers");
                for (int col = 0; col < headersList.Count; col++)
                {
                    worksheet.Cells[1, col + 1].Value = headersList[col];
                    worksheet.Cells[1, col + 1].Style.Font.Bold = true;
                }
                int rowIdx = 2;
                foreach (var item in list)
                {
                    int colIdx = 1;
                    if (selectedCols.Contains("employee")) worksheet.Cells[rowIdx, colIdx++].Value = $"{item.EmployeeName} ({item.EmployeeCode})";
                    if (selectedCols.Contains("salary period")) worksheet.Cells[rowIdx, colIdx++].Value = item.PeriodName;
                    if (selectedCols.Contains("pf configuration")) worksheet.Cells[rowIdx, colIdx++].Value = item.CalculatedAmount;
                    rowIdx++;
                }
                worksheet.Cells.AutoFitColumns();
                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "pf_members.xlsx");
            }
        }
    }

    public class PFMemberDisplayViewModel
    {
        public int id { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string PeriodName { get; set; }
        public decimal PFPercentage { get; set; }
        public decimal PFAmount { get; set; }
        public bool IsFixedPFAmount { get; set; }
        public decimal LatestSalary { get; set; }
        public bool IsPostedToGL { get; set; }
        public decimal CalculatedAmount => IsFixedPFAmount ? PFAmount : (LatestSalary * PFPercentage / 100);
    }
}

