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
    public class IncomeTaxDetuctionController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IPayrollLockService _lockService;

        public IncomeTaxDetuctionController(ApplicationDbContext context, IPayrollLockService lockService)
        {
            _context = context;
            _lockService = lockService;
        }

        [RequireUrlPermission]
        public IActionResult Index(string search, int? page)
        {
            int pageSize = 15;
            int pageNumber = page ?? 1;

            var obj = _context.IncomeTaxDetuction
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
        public async Task<IActionResult> Create(IncomeTaxDetuction d)
        {
                if (ModelState.IsValid)
                {
                    if (await _lockService.IsRecordLocked(d.EmployeeId, d.PeriodSalery))
                    {
                        ModelState.AddModelError("", "Salary Slip generated - so this record cannot be deleted or edited");
                        ViewBag.EmployeeId = new SelectList(_context.emp, "Id", "Code", d.EmployeeId);
                        ViewBag.PeriodSalery = new SelectList(_context.SalaryPeriod.Where(s => !s.IsPostedToGL), "SalaryPeriodID", "PeriodName", d.PeriodSalery);
                        return View(d);
                    }
                    var exists = await _context.IncomeTaxDetuction.AnyAsync(i => i.EmployeeId == d.EmployeeId && i.PeriodSalery == d.PeriodSalery);
                    if (exists)
                    {
                        ModelState.AddModelError("", "A tax deduction already exists for this employee in the selected period.");
                        ViewBag.EmployeeId = new SelectList(_context.emp, "Id", "Code", d.EmployeeId);
                        ViewBag.PeriodSalery = new SelectList(_context.SalaryPeriod.Where(s => !s.IsPostedToGL), "SalaryPeriodID", "PeriodName", d.PeriodSalery);
                        return View(d);
                    }

                    var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;
                    d.createat = DateTime.Now;
                    d.createdby = currentUserId;

                _context.IncomeTaxDetuction.Add(d);
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
            var record = await _context.IncomeTaxDetuction
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
            var record = _context.IncomeTaxDetuction
                .Include(r => r.Employee)
                .FirstOrDefault(r => r.id == id);

            if (record == null)
                return NotFound();

            ViewBag.PeriodSalery = new SelectList(_context.SalaryPeriod.Where(s => !s.IsPostedToGL), "SalaryPeriodID", "PeriodName", record.PeriodSalery);
            return View(record);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(IncomeTaxDetuction d)
        {
            if (ModelState.IsValid)
            {
                if (await _lockService.IsRecordLocked(d.EmployeeId, d.PeriodSalery))
                {
                    ModelState.AddModelError("", "Salary Slip generated - so this record cannot be deleted or edited");
                    ViewBag.EmployeeId = new SelectList(_context.emp, "Id", "Code", d.EmployeeId);
                    ViewBag.PeriodSalery = new SelectList(_context.SalaryPeriod.Where(s => !s.IsPostedToGL), "SalaryPeriodID", "PeriodName", d.PeriodSalery);
                    return View(d);
                }
                var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;

                d.updateat = DateTime.Now;
                d.updatedby = currentUserId;

                _context.IncomeTaxDetuction.Update(d);
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
            builder.AppendLine("EmployeeCode,Amount");
            builder.AppendLine("EMP001,500.00"); // Sample row
            
            var csvBytes = System.Text.Encoding.UTF8.GetBytes(builder.ToString());
            return File(csvBytes, "text/csv", "IncomeTaxDeduction_Sample.csv");
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
            var recordsToSave = new List<IncomeTaxDetuction>();
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

                    // Skip Header (Row 1)
                    if (lineCount == 1) continue;
                    
                    // Skip Sample/Demonstration (Row 2) as requested
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

                    // Validation: Duplicate Code in File
                    if (codesInFile.Contains(code))
                    {
                        errors.Add($"Line {lineCount}: Duplicate Employee Code '{code}' found in the file.");
                        continue;
                    }
                    codesInFile.Add(code);

                    // Validation: Employee Existence and Active Status
                    var employee = await _context.emp.FirstOrDefaultAsync(e => e.Code == code && e.SalaryStatus == "Active");
                    if (employee == null)
                    {
                        errors.Add($"Line {lineCount}: Employee with code '{code}' not found or not active.");
                        continue;
                    }

                    // Validation: Duplicate in Database
                    var alreadyExists = await _context.IncomeTaxDetuction.AnyAsync(i => i.EmployeeId == employee.Id && i.PeriodSalery == PeriodSalery);
                    if (alreadyExists)
                    {
                        errors.Add($"Line {lineCount}: Tax record already exists for employee '{code}' in this period.");
                        continue;
                    }

                    // Validation: Numeric Amount
                    if (!decimal.TryParse(amountStr, out decimal amount))
                    {
                        errors.Add($"Line {lineCount}: Invalid amount '{amountStr}'.");
                        continue;
                    }

                    recordsToSave.Add(new IncomeTaxDetuction
                    {
                        EmployeeId = employee.Id,
                        PeriodSalery = PeriodSalery,
                        incometaxdetection = amount,
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
                _context.IncomeTaxDetuction.AddRange(recordsToSave);
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
            var record = _context.IncomeTaxDetuction
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
            var record = await _context.IncomeTaxDetuction.FindAsync(id);
            if (record != null)
            {
                if (await _lockService.IsRecordLocked(record.EmployeeId, record.PeriodSalery))
                {
                    TempData["Error"] = "Salary Slip generated - so this record cannot be deleted or edited";
                    return RedirectToAction(nameof(Index));
                }
                try
                {
                    _context.IncomeTaxDetuction.Remove(record);
                    _context.SaveChanges();
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
        [SkipPermissionCheck]
        public async Task<IActionResult> CalculateTaxDetails(int? EmployeeId, int? PeriodSalery, decimal? grossSalary)
        {
            decimal currentGross = grossSalary ?? 0;

            // 1. Auto-fill gross salary if employee is selected
            // We ALWAYS fetch this when an EmployeeId is provided because switching employees
            // should update the salary even if the previous employee's salary was already in the field.
            if (EmployeeId.HasValue && EmployeeId.Value > 0)
            {
                var payroll = await _context.PayRollGenrate
                    .Where(p => p.EmployeeFK == EmployeeId.Value)
                    .OrderByDescending(p => p.id) // Order by ID to get the absolute latest record
                    .FirstOrDefaultAsync();

                if (payroll != null)
                {
                    currentGross = payroll.salery;
                }
                else
                {
                    // If no payroll record exists, we reset to 0 to avoid showing previous employee's salary
                    currentGross = 0;
                }
            }

            ViewBag.GrossSalary = currentGross > 0 ? currentGross : (decimal?)null;

            // If we lack info to calculate tax, just return the gross salary update OOB
            if (!EmployeeId.HasValue || EmployeeId.Value <= 0 || !PeriodSalery.HasValue || PeriodSalery.Value <= 0 || currentGross <= 0)
            {
                return PartialView("_TaxCalculationResult");
            }

            var result = await PerformTaxCalculation(currentGross, PeriodSalery.Value);

            if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                ViewBag.ErrorMessage = result.ErrorMessage;
                return PartialView("_TaxCalculationResult");
            }

            ViewBag.FiscalYearName = result.FiscalYearName;
            ViewBag.AnnualGross = result.AnnualGross;
            ViewBag.TaxSlab = result.TaxSlab;
            ViewBag.TaxSurcharge = result.TaxSurcharge;
            ViewBag.FixedTax = result.FixedTax;
            ViewBag.TaxOnExceeding = result.TaxOnExceeding;
            ViewBag.SurchargeAmount = result.SurchargeAmount;
            ViewBag.AnnualTax = result.AnnualTax;
            ViewBag.MonthlyTax = result.MonthlyTax;
            ViewBag.CalculationSuccessful = true;

            return PartialView("_TaxCalculationResult");
        }

        private async Task<TaxCalculationResult> PerformTaxCalculation(decimal monthlyGross, int salaryPeriodId)
        {
            var res = new TaxCalculationResult { MonthlyGross = monthlyGross };

            var salaryPeriod = await _context.SalaryPeriod.FindAsync(salaryPeriodId);
            if (salaryPeriod == null)
            {
                res.ErrorMessage = "Invalid Salary Period.";
                return res;
            }

            var periodStart = DateOnly.FromDateTime(salaryPeriod.StartDate);
            var fiscalYear = await _context.FiscalYear
                .FirstOrDefaultAsync(fy => fy.startdate <= periodStart && fy.enddate >= periodStart);

            if (fiscalYear == null)
            {
                res.ErrorMessage = $"No Fiscal Year found covering ({periodStart.ToString("MMM yyyy")}).";
                return res;
            }

            var slabs = await _context.TaxSlabMaster
                .Where(t => t.FiscalFK == fiscalYear.id && t.IsActive)
                .ToListAsync();

            var surcharges = await _context.TaxSurcharge
                .Where(t => t.FiscalFK == fiscalYear.id && t.IsActive)
                .ToListAsync();

            return CalculateTaxInternal(monthlyGross, fiscalYear.title, slabs, surcharges);
        }

        private TaxCalculationResult CalculateTaxInternal(decimal monthlyGross, string fiscalYearName, List<TaxSlabMaster> slabs, List<TaxSurcharge> surcharges)
        {
            var res = new TaxCalculationResult 
            { 
                MonthlyGross = monthlyGross,
                FiscalYearName = fiscalYearName,
                AnnualGross = monthlyGross * 12
            };

            decimal annualGross = res.AnnualGross;

            res.TaxSlab = slabs
                .Where(t => t.IncomeFrom <= annualGross && (t.IncomeTo == null || t.IncomeTo >= annualGross))
                .FirstOrDefault();

            res.TaxSurcharge = surcharges
                .Where(t => t.IncomeFrom <= annualGross && (t.IncomeTo == null || t.IncomeTo >= annualGross))
                .FirstOrDefault();

            if (res.TaxSlab != null)
            {
                res.FixedTax = res.TaxSlab.FixedTax;
                res.TaxRate = res.TaxSlab.RatePercent;
                res.TaxOnExceeding = (annualGross - res.TaxSlab.IncomeFrom) * (res.TaxRate / 100);
                res.AnnualTax += res.FixedTax + res.TaxOnExceeding;
            }

            if (res.TaxSurcharge != null)
            {
                res.SurchargeAmount = (annualGross - res.TaxSurcharge.IncomeFrom) * (res.TaxSurcharge.RatePercent / 100);
                res.AnnualTax += res.SurchargeAmount;
            }

            res.MonthlyTax = res.AnnualTax / 12;
            return res;
        }

        private class TaxCalculationResult
        {
            public decimal MonthlyGross { get; set; }
            public decimal AnnualGross { get; set; }
            public decimal AnnualTax { get; set; }
            public decimal MonthlyTax { get; set; }
            public decimal FixedTax { get; set; }
            public decimal TaxRate { get; set; }
            public decimal TaxOnExceeding { get; set; }
            public decimal SurchargeAmount { get; set; }
            public string FiscalYearName { get; set; }
            public TaxSlabMaster TaxSlab { get; set; }
            public TaxSurcharge TaxSurcharge { get; set; }
            public string ErrorMessage { get; set; }
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

            // 1. Pre-fetch Salary Period and Fiscal Year
            var salaryPeriod = await _context.SalaryPeriod.FindAsync(periodId);
            if (salaryPeriod == null) return Content("<div class='alert alert-danger'>Invalid Salary Period selected.</div>");

            var periodStart = DateOnly.FromDateTime(salaryPeriod.StartDate);
            var fiscalYear = await _context.FiscalYear
                .FirstOrDefaultAsync(fy => fy.startdate <= periodStart && fy.enddate >= periodStart);

            if (fiscalYear == null)
            {
                return Content($"<div class='alert alert-warning'>No Fiscal Year found covering {salaryPeriod.PeriodName}. Please define a Fiscal Year first.</div>");
            }

            // 2. Pre-fetch Tax Rules for this Fiscal Year
            var slabs = await _context.TaxSlabMaster
                .Where(t => t.FiscalFK == fiscalYear.id && t.IsActive)
                .ToListAsync();

            var surcharges = await _context.TaxSurcharge
                .Where(t => t.FiscalFK == fiscalYear.id && t.IsActive)
                .ToListAsync();

            // 3. BATCH FETCH: Get all active employees and their latest salaries in ONE query
            var employeeData = await _context.emp
                .Where(e => e.SalaryStatus == "Active")
                .Select(e => new
                {
                    Employee = e,
                    LatestSalary = _context.PayRollGenrate
                        .Where(p => p.EmployeeFK == e.Id)
                        .OrderByDescending(p => p.id)
                        .Select(p => (decimal?)p.salery)
                        .FirstOrDefault() ?? 0
                })
                .ToListAsync();

            var previewList = new List<BulkAddPreviewItem>();

            // 4. Perform In-Memory calculations (Instant)
            foreach (var item in employeeData)
            {
                var taxRes = CalculateTaxInternal(item.LatestSalary, fiscalYear.title, slabs, surcharges);

                previewList.Add(new BulkAddPreviewItem
                {
                    EmployeeId = item.Employee.Id,
                    Code = item.Employee.Code,
                    Name = $"{item.Employee.FirstName} {item.Employee.LastName}",
                    GrossSalary = item.LatestSalary,
                    MonthlyTax = taxRes.MonthlyTax,
                    HasError = !string.IsNullOrEmpty(taxRes.ErrorMessage)
                });
            }

            ViewBag.PeriodId = periodId;
            return PartialView("_BulkAddPreview", previewList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkAdd(int periodId)
        {
            if (periodId <= 0)
            {
                TempData["Error"] = "Invalid period selected.";
                return RedirectToAction("BulkAdd");
            }

            // 1. Fetch Rules & Data once
            var salaryPeriod = await _context.SalaryPeriod.FindAsync(periodId);
            if (salaryPeriod == null) return RedirectToAction("BulkAdd");

            // Block if period is locked
            if (salaryPeriod.IsPostedToGL)
            {
                TempData["Error"] = "This Salary Period is already Posted to GL and cannot be modified.";
                return RedirectToAction("Index");
            }

            var periodStart = DateOnly.FromDateTime(salaryPeriod.StartDate);
            var fiscalYear = await _context.FiscalYear
                .FirstOrDefaultAsync(fy => fy.startdate <= periodStart && fy.enddate >= periodStart);

            if (fiscalYear == null)
            {
                TempData["Error"] = "No Fiscal Year found for the selected period.";
                return RedirectToAction("BulkAdd");
            }

            var slabs = await _context.TaxSlabMaster.Where(t => t.FiscalFK == fiscalYear.id && t.IsActive).ToListAsync();
            var surcharges = await _context.TaxSurcharge.Where(t => t.FiscalFK == fiscalYear.id && t.IsActive).ToListAsync();

            var employeesWithSalaries = await _context.emp
                .Where(e => e.SalaryStatus == "Active")
                .Select(e => new {
                    Id = e.Id,
                    LatestSalary = _context.PayRollGenrate
                        .Where(p => p.EmployeeFK == e.Id)
                        .OrderByDescending(p => p.id)
                        .Select(p => (decimal?)p.salery)
                        .FirstOrDefault() ?? 0
                })
                .ToListAsync();

            // 2. Fetch existing records for this period to skip duplicates
            var existingRecordSet = new HashSet<int>(await _context.IncomeTaxDetuction
                .Where(i => i.PeriodSalery == periodId)
                .Select(i => i.EmployeeId)
                .ToListAsync());

            var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;
            var recordsToSave = new List<IncomeTaxDetuction>();
            int skippedCount = 0;

            // 3. Process batches
            foreach (var empData in employeesWithSalaries)
            {
                if (existingRecordSet.Contains(empData.Id))
                {
                    skippedCount++;
                    continue;
                }

                if (empData.LatestSalary > 0)
                {
                    var taxRes = CalculateTaxInternal(empData.LatestSalary, fiscalYear.title, slabs, surcharges);
                    if (taxRes.MonthlyTax > 0)
                    {
                        recordsToSave.Add(new IncomeTaxDetuction
                        {
                            EmployeeId = empData.Id,
                            PeriodSalery = periodId,
                            incometaxdetection = taxRes.MonthlyTax,
                            createat = DateTime.Now,
                            createdby = currentUserId
                        });
                    }
                }
            }

            if (recordsToSave.Any())
            {
                _context.IncomeTaxDetuction.AddRange(recordsToSave);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Processed {employeesWithSalaries.Count} employees. Added: {recordsToSave.Count}, Skipped: {skippedCount} (already existing).";
            }
            else
            {
                TempData["Info"] = $"No new records added. {skippedCount} employees already had records for this period.";
            }

            return RedirectToAction("Index");
        }

        public class BulkAddPreviewItem
        {
            public int EmployeeId { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }
            public decimal GrossSalary { get; set; }
            public decimal MonthlyTax { get; set; }
            public bool HasError { get; set; }
        }

        [HttpGet]
        [SkipPermissionCheck]
        public async Task<IActionResult> SeedAllTaxData()
        {
            try
            {
                var messages = new List<string>();
                var rand = new Random();

                // 1. Seed Fiscal Year
                var fy = await _context.FiscalYear.FirstOrDefaultAsync(f => f.title == "2025-2026");
                if (fy == null)
                {
                    fy = new FiscalYear { title = "2025-2026", startdate = new DateOnly(2025, 4, 1), enddate = new DateOnly(2026, 3, 31) };
                    _context.FiscalYear.Add(fy);
                    await _context.SaveChangesAsync();
                    messages.Add("Generated Fiscal Year 2025-2026.");
                }

                // 2. Seed Salary Periods (12 months)
                if (!_context.SalaryPeriod.Any(s => s.PeriodName.Contains("2025")))
                {
                    for (int i = 4; i <= 12; i++)
                    {
                        _context.SalaryPeriod.Add(new SalaryPeriod { PeriodName = $"2025/{i:D2}", StartDate = new DateTime(2025, i, 1), EndDate = new DateTime(2025, i, DateTime.DaysInMonth(2025, i)), createdby = 1, createat = DateTime.Now });
                    }
                    for (int i = 1; i <= 3; i++)
                    {
                        _context.SalaryPeriod.Add(new SalaryPeriod { PeriodName = $"2026/{i:D2}", StartDate = new DateTime(2026, i, 1), EndDate = new DateTime(2026, i, DateTime.DaysInMonth(2026, i)), createdby = 1, createat = DateTime.Now });
                    }
                    await _context.SaveChangesAsync();
                    messages.Add("Generated 12 Salary Periods.");
                }

                // 3. Seed Tax Slabs
                if (!_context.TaxSlabMaster.Any(t => t.FiscalFK == fy.id))
                {
                    _context.TaxSlabMaster.AddRange(
                        new TaxSlabMaster { FiscalFK = fy.id, IncomeFrom = 0, IncomeTo = 500000, FixedTax = 0, RatePercent = 0, IsActive = true, createdby = 1, createat = DateTime.Now },
                        new TaxSlabMaster { FiscalFK = fy.id, IncomeFrom = 500001, IncomeTo = 1000000, FixedTax = 10000, RatePercent = 5, IsActive = true, createdby = 1, createat = DateTime.Now },
                        new TaxSlabMaster { FiscalFK = fy.id, IncomeFrom = 1000001, IncomeTo = 1500000, FixedTax = 40000, RatePercent = 10, IsActive = true, createdby = 1, createat = DateTime.Now },
                        new TaxSlabMaster { FiscalFK = fy.id, IncomeFrom = 1500001, IncomeTo = 3000000, FixedTax = 90000, RatePercent = 15, IsActive = true, createdby = 1, createat = DateTime.Now },
                        new TaxSlabMaster { FiscalFK = fy.id, IncomeFrom = 3000001, IncomeTo = null, FixedTax = 315000, RatePercent = 20, IsActive = true, createdby = 1, createat = DateTime.Now }
                    );
                    await _context.SaveChangesAsync();
                    messages.Add("Generated 5 Tax Slabs.");
                }

                // 4. Seed Tax Surcharges
                if (!_context.TaxSurcharge.Any(t => t.FiscalFK == fy.id))
                {
                    _context.TaxSurcharge.AddRange(
                        new TaxSurcharge { FiscalFK = fy.id, IncomeFrom = 5000000, IncomeTo = 10000000, RatePercent = 10, IsActive = true, createdby = 1, createat = DateTime.Now },
                        new TaxSurcharge { FiscalFK = fy.id, IncomeFrom = 10000001, IncomeTo = null, RatePercent = 15, IsActive = true, createdby = 1, createat = DateTime.Now }
                    );
                    await _context.SaveChangesAsync();
                    messages.Add("Generated 2 Tax Surcharges.");
                }

                // 5. Seed PayRollGenrate (Gross Salaries)
                var employees = await _context.emp.Take(15).ToListAsync();
                var def = await _context.PayRollDefinationFile.FirstOrDefaultAsync();
                int defId = def?.Id ?? 1;

                int payrollCount = 0;
                foreach (var emp in employees)
                {
                    if (!_context.PayRollGenrate.Any(p => p.EmployeeFK == emp.Id))
                    {
                        _context.PayRollGenrate.Add(new PayRollGenrate { EmployeeFK = emp.Id, PayRollDefinationFK = defId, salery = rand.Next(40000, 200000), createdby = 1, createat = DateTime.Now });
                        payrollCount++;
                    }
                }
                if (payrollCount > 0)
                {
                    await _context.SaveChangesAsync();
                    messages.Add($"Generated {payrollCount} Payroll records (Gross Salaries).");
                }

                // 6. Seed IncomeTaxDetuction
                var latestPeriod = await _context.SalaryPeriod.OrderByDescending(s => s.SalaryPeriodID).FirstOrDefaultAsync();
                int taxRecordCount = 0;

                if (latestPeriod != null)
                {
                    foreach (var emp in employees.Take(10))
                    {
                        if (!_context.IncomeTaxDetuction.Any(i => i.EmployeeId == emp.Id && i.PeriodSalery == latestPeriod.SalaryPeriodID))
                        {
                            // Reduced random value to strictly fit inside smaller decimal precision fields e.g., decimal(5,2)
                            _context.IncomeTaxDetuction.Add(new IncomeTaxDetuction { EmployeeId = emp.Id, PeriodSalery = latestPeriod.SalaryPeriodID, incometaxdetection = rand.Next(10, 900), createdby = 1, createat = DateTime.Now });
                            taxRecordCount++;
                        }
                    }
                    if (taxRecordCount > 0)
                    {
                        await _context.SaveChangesAsync();
                        messages.Add($"Generated {taxRecordCount} Income Tax Deduction records.");
                    }
                }

                if (!messages.Any()) messages.Add("All required data already exists! Nothing new was seeded.");

                return Content("<html><body style='font-family: Arial; padding: 20px;'><h2>Bulk Data Generation Complete</h2><ul><li>" + string.Join("</li><li>", messages) + "</li></ul><a href='/IncomeTaxDetuction'>Go back to Tax Dashboard</a></body></html>", "text/html");
            }
            catch (Exception ex)
            {
                var inner = ex.InnerException != null ? ex.InnerException.Message : "No inner exception.";
                return Content($"<html><body><h3>Error Seeding</h3><p>{ex.Message}</p><p><strong>Inner Exception:</strong> {inner}</p></body></html>", "text/html");
            }
        }
    }
}

