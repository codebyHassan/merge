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
    public class EOBIDetuctionController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IPayrollLockService _lockService;

        public EOBIDetuctionController(ApplicationDbContext context, IPayrollLockService lockService)
        {
            _context = context;
            _lockService = lockService;
        }

        [RequireUrlPermission]
        public IActionResult Index(string search, int? page)
        {
            int pageSize = 15;
            int pageNumber = page ?? 1;

            var obj = _context.EOBIDetuction
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
            ViewBag.EmployeeId = new SelectList(_context.emp.Where(e => e.SalaryStatus == "Active" && e.isEobi), "Id", "Code");
            ViewBag.PeriodSalery = new SelectList(_context.SalaryPeriod.Where(s => !s.IsPostedToGL).OrderByDescending(s => s.SalaryPeriodID), "SalaryPeriodID", "PeriodName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EOBIDetuction d)
        {
            if (ModelState.IsValid)
            {
                if (await _lockService.IsRecordLocked(d.EmployeeId, d.PeriodSalery))
                {
                    ModelState.AddModelError("", "Salary Slip generated - so this record cannot be deleted or edited");
                    ViewBag.EmployeeId = new SelectList(_context.emp.Where(e => e.SalaryStatus == "Active" && e.isEobi), "Id", "Code", d.EmployeeId);
                    ViewBag.PeriodSalery = new SelectList(_context.SalaryPeriod.Where(s => !s.IsPostedToGL).OrderByDescending(s => s.SalaryPeriodID), "SalaryPeriodID", "PeriodName", d.PeriodSalery);
                    return View(d);
                }
                var employee = await _context.emp.FindAsync(d.EmployeeId);
                if (employee == null || employee.SalaryStatus != "Active" || !employee.isEobi)
                {
                    ModelState.AddModelError("", "This employee is either not active or not enrolled in EOBI.");
                    ViewBag.EmployeeId = new SelectList(_context.emp.Where(e => e.SalaryStatus == "Active" && e.isEobi), "Id", "Code", d.EmployeeId);
                    ViewBag.PeriodSalery = new SelectList(_context.SalaryPeriod.Where(s => !s.IsPostedToGL).OrderByDescending(s => s.SalaryPeriodID), "SalaryPeriodID", "PeriodName", d.PeriodSalery);
                    return View(d);
                }

                var exists = await _context.EOBIDetuction.AnyAsync(i => i.EmployeeId == d.EmployeeId && i.PeriodSalery == d.PeriodSalery);
                if (exists)
                {
                    ModelState.AddModelError("", "An EOBI deduction already exists for this employee in the selected period.");
                    ViewBag.EmployeeId = new SelectList(_context.emp.Where(e => e.SalaryStatus == "Active" && e.isEobi), "Id", "Code", d.EmployeeId);
                    ViewBag.PeriodSalery = new SelectList(_context.SalaryPeriod.Where(s => !s.IsPostedToGL).OrderByDescending(s => s.SalaryPeriodID), "SalaryPeriodID", "PeriodName", d.PeriodSalery);
                    return View(d);
                }

                var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;
                d.createat = DateTime.Now;
                d.createdby = currentUserId;

                _context.EOBIDetuction.Add(d);
                await _context.SaveChangesAsync();
                TempData["Success"] = "EOBI record created successfully.";
                return RedirectToAction("Index");
            }
            ViewBag.EmployeeId = new SelectList(_context.emp.Where(e => e.SalaryStatus == "Active" && e.isEobi), "Id", "Code", d.EmployeeId);
            ViewBag.PeriodSalery = new SelectList(_context.SalaryPeriod.Where(s => !s.IsPostedToGL).OrderByDescending(s => s.SalaryPeriodID), "SalaryPeriodID", "PeriodName", d.PeriodSalery);
            return View(d);
        }

        [RequireUrlPermission]
        public async Task<IActionResult> Details(int id)
        {
            var record = await _context.EOBIDetuction
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
            var record = _context.EOBIDetuction
                .Include(r => r.Employee)
                .Include(r => r.SalaryPeriod)
                .FirstOrDefault(r => r.id == id);

            if (record == null)
                return NotFound();

            ViewBag.PeriodSalery = new SelectList(_context.SalaryPeriod.Where(s => !s.IsPostedToGL).OrderByDescending(s => s.SalaryPeriodID), "SalaryPeriodID", "PeriodName", record.PeriodSalery);
            return View(record);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EOBIDetuction d)
        {
            if (ModelState.IsValid)
            {
                if (await _lockService.IsRecordLocked(d.EmployeeId, d.PeriodSalery))
                {
                    ModelState.AddModelError("", "Salary Slip generated - so this record cannot be deleted or edited");
                    ViewBag.PeriodSalery = new SelectList(_context.SalaryPeriod.Where(s => !s.IsPostedToGL).OrderByDescending(s => s.SalaryPeriodID), "SalaryPeriodID", "PeriodName", d.PeriodSalery);
                    return View(d);
                }
                var employee = await _context.emp.FindAsync(d.EmployeeId);
                if (employee == null || employee.SalaryStatus != "Active" || !employee.isEobi)
                {
                    ModelState.AddModelError("", "This employee is either not active or not enrolled in EOBI.");
                    ViewBag.PeriodSalery = new SelectList(_context.SalaryPeriod.Where(s => !s.IsPostedToGL).OrderByDescending(s => s.SalaryPeriodID), "SalaryPeriodID", "PeriodName", d.PeriodSalery);
                    return View(d);
                }

                var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;

                d.updateat = DateTime.Now;
                d.updatedby = currentUserId;

                _context.EOBIDetuction.Update(d);
                await _context.SaveChangesAsync();
                TempData["Success"] = "EOBI record updated successfully.";

                return RedirectToAction("Index");
            }
            ViewBag.PeriodSalery = new SelectList(_context.SalaryPeriod.Where(s => !s.IsPostedToGL).OrderByDescending(s => s.SalaryPeriodID), "SalaryPeriodID", "PeriodName", d.PeriodSalery);
            return View(d);
        }

        [RequireUrlPermission]
        public IActionResult BulkUpload()
        {
            ViewBag.PeriodSalery = new SelectList(_context.SalaryPeriod.Where(s => !s.IsPostedToGL).OrderByDescending(s => s.SalaryPeriodID), "SalaryPeriodID", "PeriodName");
            return View();
        }

        [HttpGet]
        public IActionResult DownloadSampleCsv()
        {
            var builder = new System.Text.StringBuilder();
            builder.AppendLine("EmployeeCode");
            builder.AppendLine("EMP001"); // Sample row
            
            var csvBytes = System.Text.Encoding.UTF8.GetBytes(builder.ToString());
            return File(csvBytes, "text/csv", "EOBIDeduction_Sample.csv");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkUpload(IFormFile file, int PeriodSalery)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("", "Please upload a valid CSV file.");
                ViewBag.PeriodSalery = new SelectList(_context.SalaryPeriod.Where(s => !s.IsPostedToGL).OrderByDescending(s => s.SalaryPeriodID), "SalaryPeriodID", "PeriodName", PeriodSalery);
                return View();
            }

            var errors = new List<string>();
            var recordsToSave = new List<EOBIDetuction>();
            var codesInFile = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;

            // Block upload if period is locked
            if (await _lockService.IsPeriodLocked(PeriodSalery))
            {
                TempData["Error"] = "This Salary Period is already Posted to GL and cannot be modified.";
                return RedirectToAction("Index");
            }

            var salaryPeriod = await _context.SalaryPeriod.FindAsync(PeriodSalery);
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                int lineCount = 0;
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    lineCount++;

                    if (lineCount == 1 || string.IsNullOrWhiteSpace(line)) continue;

                    var code = line.Trim();

                    if (codesInFile.Contains(code))
                    {
                        errors.Add($"Line {lineCount}: Duplicate Employee Code '{code}' found in the file.");
                        continue;
                    }
                    codesInFile.Add(code);

                    var employee = await _context.emp.FirstOrDefaultAsync(e => e.Code == code && e.SalaryStatus == "Active" && e.isEobi);
                    if (employee == null)
                    {
                        errors.Add($"Line {lineCount}: Employee with code '{code}' not found, not active, or EOBI not enabled.");
                        continue;
                    }

                    var alreadyExists = await _context.EOBIDetuction.AnyAsync(i => i.EmployeeId == employee.Id && i.PeriodSalery == PeriodSalery);
                    if (alreadyExists)
                    {
                        errors.Add($"Line {lineCount}: EOBI record already exists for employee '{code}' in this period.");
                        continue;
                    }

                    recordsToSave.Add(new EOBIDetuction
                    {
                        EmployeeId = employee.Id,
                        PeriodSalery = PeriodSalery,
                        Amount = salaryPeriod?.EOBIAmount,
                        createdby = currentUserId,
                        createat = DateTime.Now
                    });
                }
            }

            if (errors.Any())
            {
                ViewBag.Errors = errors;
                ViewBag.PeriodSalery = new SelectList(_context.SalaryPeriod.Where(s => !s.IsPostedToGL).OrderByDescending(s => s.SalaryPeriodID), "SalaryPeriodID", "PeriodName", PeriodSalery);
                return View();
            }

            if (recordsToSave.Any())
            {
                _context.EOBIDetuction.AddRange(recordsToSave);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Successfully uploaded {recordsToSave.Count} records.";
            }

            return RedirectToAction("Index");
        }

        [RequireUrlPermission]
        public IActionResult Delete(int id)
        {
            var record = _context.EOBIDetuction
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
            var record = await _context.EOBIDetuction.FindAsync(id);
            if (record != null)
            {
                if (await _lockService.IsRecordLocked(record.EmployeeId, record.PeriodSalery))
                {
                    TempData["Error"] = "Salary Slip generated - so this record cannot be deleted or edited";
                    return RedirectToAction(nameof(Index));
                }
                _context.EOBIDetuction.Remove(record);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Deleted successfully.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [SkipPermissionCheck]
        public async Task<IActionResult> CalculateEOBIDetails(int? EmployeeId, int? PeriodSalery)
        {
            if (!EmployeeId.HasValue || EmployeeId.Value <= 0 || !PeriodSalery.HasValue || PeriodSalery.Value <= 0)
            {
                return PartialView("_EOBICalculationResult");
            }

            var employee = await _context.emp.FindAsync(EmployeeId.Value);
            var period = await _context.SalaryPeriod.FindAsync(PeriodSalery.Value);

            if (employee == null || period == null)
            {
                return PartialView("_EOBICalculationResult");
            }

            // Fetch Latest Gross Salary
            var latestGross = await _context.PayRollGenrate
                .Where(p => p.EmployeeFK == EmployeeId.Value)
                .OrderByDescending(p => p.id)
                .Select(p => (decimal?)p.salery)
                .FirstOrDefaultAsync() ?? 0;

            var periodAmount = period.EOBIAmount ?? 0;
            var appliedAmount = employee.isEobi ? periodAmount : 0;

            ViewBag.GrossSalary = latestGross;
            ViewBag.PeriodName = period.PeriodName;
            ViewBag.PeriodRate = periodAmount;
            ViewBag.EOBIAmount = appliedAmount;
            ViewBag.NetSalary = latestGross - appliedAmount;
            ViewBag.IsEobiActive = employee.isEobi;
            ViewBag.CalculationSuccessful = true;

            return PartialView("_EOBICalculationResult");
        }

        [RequireUrlPermission]
        public IActionResult BulkAdd()
        {
            ViewBag.PeriodSalery = new SelectList(_context.SalaryPeriod.Where(s => !s.IsPostedToGL).OrderByDescending(s => s.SalaryPeriodID), "SalaryPeriodID", "PeriodName");
            return View();
        }

        [HttpGet]
        [SkipPermissionCheck]
        public async Task<IActionResult> GetBulkAddPreview(int periodId)
        {
            if (periodId <= 0) return Content("");

            var salaryPeriod = await _context.SalaryPeriod.FindAsync(periodId);
            if (salaryPeriod == null) return Content("<div class='alert alert-danger'>Invalid Salary Period selected.</div>");

            var employeeData = await _context.emp
                .Where(e => e.SalaryStatus == "Active" && e.isEobi)
                .Select(e => new BulkAddPreviewItem
                {
                    EmployeeId = e.Id,
                    Code = e.Code,
                    Name = $"{e.FirstName} {e.LastName}",
                    GrossSalary = _context.PayRollGenrate
                        .Where(p => p.EmployeeFK == e.Id)
                        .OrderByDescending(p => p.id)
                        .Select(p => (decimal?)p.salery)
                        .FirstOrDefault() ?? 0,
                    EOBIAmount = salaryPeriod.EOBIAmount ?? 0
                })
                .ToListAsync();

            ViewBag.PeriodId = periodId;
            return PartialView("_BulkAddPreview", employeeData);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkAdd(int periodId)
        {
            if (periodId <= 0) return RedirectToAction("BulkAdd");

            // Block if period is locked
            if (await _lockService.IsPeriodLocked(periodId))
            {
                TempData["Error"] = "This Salary Period is already Posted to GL and cannot be modified.";
                return RedirectToAction("Index");
            }

            var employees = await _context.emp
                .Where(e => e.SalaryStatus == "Active" && e.isEobi)
                .Select(e => e.Id)
                .ToListAsync();

            var existingRecords = new HashSet<int>(await _context.EOBIDetuction
                .Where(i => i.PeriodSalery == periodId)
                .Select(i => i.EmployeeId)
                .ToListAsync());

            var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;
            var recordsToSave = new List<EOBIDetuction>();

            var salaryPeriod = await _context.SalaryPeriod.FindAsync(periodId);
            foreach (var empId in employees)
            {
                if (existingRecords.Contains(empId)) continue;

                recordsToSave.Add(new EOBIDetuction
                {
                    EmployeeId = empId,
                    PeriodSalery = periodId,
                    Amount = salaryPeriod?.EOBIAmount,
                    createat = DateTime.Now,
                    createdby = currentUserId
                });
            }

            if (recordsToSave.Any())
            {
                _context.EOBIDetuction.AddRange(recordsToSave);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Successfully added {recordsToSave.Count} EOBI records.";
            }
            else
            {
                TempData["Info"] = "No new records added.";
            }

            return RedirectToAction("Index");
        }

        public class BulkAddPreviewItem
        {
            public int EmployeeId { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }
            public decimal GrossSalary { get; set; }
            public decimal EOBIAmount { get; set; }
            public decimal NetSalary => GrossSalary - EOBIAmount;
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
        public IActionResult Export(string search, string columns, string downloadToken)
        {
            if (!string.IsNullOrEmpty(downloadToken))
            {
                Response.Cookies.Append("fileDownloadToken", downloadToken, new Microsoft.AspNetCore.Http.CookieOptions { Path = "/", HttpOnly = false, Secure = false });
            }

            var query = _context.EOBIDetuction.Include(i => i.Employee).Include(i => i.SalaryPeriod).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(d => d.Employee.Code.Contains(search) || d.SalaryPeriod.PeriodName.Contains(search));
            }

            var list = query.OrderByDescending(d => d.id).ToList();

            var selectedCols = !string.IsNullOrEmpty(columns)
                ? columns.Split(',').Select(c => c.Trim().ToLowerInvariant()).ToList()
                : new List<string> { "employee code", "salary period", "amount", "created at" };

            var headersList = new List<string>();
            if (selectedCols.Contains("employee code")) headersList.Add("Employee Code");
            if (selectedCols.Contains("salary period")) headersList.Add("Salary Period");
            if (selectedCols.Contains("amount")) headersList.Add("Amount");
            if (selectedCols.Contains("created at")) headersList.Add("Created At");

            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (var package = new OfficeOpenXml.ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("EOBIDeduction");
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
                    if (selectedCols.Contains("amount")) worksheet.Cells[rowIdx, colIdx++].Value = item.Amount ?? item.SalaryPeriod?.EOBIAmount ?? 0;
                    if (selectedCols.Contains("created at")) worksheet.Cells[rowIdx, colIdx++].Value = item.createat.ToString("yyyy-MM-dd");
                    rowIdx++;
                }
                worksheet.Cells.AutoFitColumns();
                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "eobi_deduction.xlsx");
            }
        }
    }
}
