using AlignHR.Data;
using AlignHR.Models;
using AlignHR.Security;
using AlignHR.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using OfficeOpenXml;
using System.IO;

namespace AlignHR.Controllers
{
    public class SalaryAdjustmentController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IPayrollLockService _lockService;

        public SalaryAdjustmentController(ApplicationDbContext context, IPayrollLockService lockService)
        {
            _context = context;
            _lockService = lockService;
        }

        [RequireUrlPermission]
        public IActionResult Index(string search, int? page)
        {
            ViewBag.ActivePage = "SalaryAdjustment";
            int pageSize = 15;
            int pageNumber = page ?? 1;

            var query = _context.SalaryAdjustments
                .Include(s => s.employee)
                .Include(s => s.SalaryPeriod)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(s => s.employee.FirstName.Contains(search) || 
                                       s.employee.LastName.Contains(search) || 
                                       s.employee.Code.Contains(search) ||
                                       s.AdjustmentCategory.Contains(search));
            }

            var model = query.OrderByDescending(s => s.CreatedAt).ToPagedList(pageNumber, pageSize);

            if (Request.Headers["HX-Request"].ToString() == "true")
            {
                return PartialView("_AdjustmentTable", model);
            }

            return View(model);
        }

        [RequireUrlPermission]
        public IActionResult Create()
        {
            ViewBag.ActivePage = "SalaryAdjustment";
            ViewBag.Employees = new SelectList(_context.emp.OrderBy(e => e.FirstName).Select(e => new { e.Id, Name = e.FirstName + " " + e.LastName + " (" + e.Code + ")" }), "Id", "Name");
            ViewBag.SalaryPeriods = new SelectList(_context.SalaryPeriod.Where(s => !s.IsPostedToGL).OrderByDescending(s => s.SalaryPeriodID), "SalaryPeriodID", "PeriodName");
            ViewBag.AdjustmentCategories = _context.valuesets.Where(v => v.GroupName == "PayRollCom" && v.IsActive).ToList();
            
            return View(new SalaryAdjustments { CreatedAt = DateTime.Now });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireUrlPermission]
        public async Task<IActionResult> Create(SalaryAdjustments model)
        {
            if (ModelState.IsValid)
            {
                if (await _lockService.IsRecordLocked(model.EmployeeId, model.SalaryPeriodId))
                {
                    ModelState.AddModelError("", "Salary Slip generated - so this record cannot be deleted or edited");
                    ViewBag.ActivePage = "SalaryAdjustment";
                    ViewBag.Employees = new SelectList(_context.emp.OrderBy(e => e.FirstName).Select(e => new { e.Id, Name = e.FirstName + " " + e.LastName + " (" + e.Code + ")" }), "Id", "Name", model.EmployeeId);
                    ViewBag.SalaryPeriods = new SelectList(_context.SalaryPeriod.Where(s => !s.IsPostedToGL).OrderByDescending(s => s.SalaryPeriodID), "SalaryPeriodID", "PeriodName", model.SalaryPeriodId);
                    ViewBag.AdjustmentCategories = _context.valuesets.Where(v => v.GroupName == "PayRollCom" && v.IsActive).ToList();
                    return View(model);
                }
                model.CreatedAt = DateTime.Now;
                model.CreatedBy = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;
                
                _context.SalaryAdjustments.Add(model);
                _context.SaveChanges();
                
                TempData["Success"] = "Salary adjustment created successfully.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.ActivePage = "SalaryAdjustment";
            ViewBag.Employees = new SelectList(_context.emp.OrderBy(e => e.FirstName).Select(e => new { e.Id, Name = e.FirstName + " " + e.LastName + " (" + e.Code + ")" }), "Id", "Name", model.EmployeeId);
            ViewBag.SalaryPeriods = new SelectList(_context.SalaryPeriod.Where(s => !s.IsPostedToGL).OrderByDescending(s => s.SalaryPeriodID), "SalaryPeriodID", "PeriodName", model.SalaryPeriodId);
            ViewBag.AdjustmentCategories = _context.valuesets.Where(v => v.GroupName == "PayRollCom" && v.IsActive).ToList();
            
            return View(model);
        }

        [RequireUrlPermission]
        public async Task<IActionResult> Details(int id)
        {
            ViewBag.ActivePage = "SalaryAdjustment";
            var model = await _context.SalaryAdjustments
                .Include(s => s.employee)
                .Include(s => s.SalaryPeriod)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (model == null) return NotFound();

            return View(model);
        }

        [RequireUrlPermission]
        public IActionResult Edit(int id)
        {
            ViewBag.ActivePage = "SalaryAdjustment";
            var model = _context.SalaryAdjustments.Find(id);
            if (model == null) return NotFound();

            ViewBag.Employees = new SelectList(_context.emp.OrderBy(e => e.FirstName).Select(e => new { e.Id, Name = e.FirstName + " " + e.LastName + " (" + e.Code + ")" }), "Id", "Name", model.EmployeeId);
            ViewBag.SalaryPeriods = new SelectList(_context.SalaryPeriod.Where(s => !s.IsPostedToGL).OrderByDescending(s => s.SalaryPeriodID), "SalaryPeriodID", "PeriodName", model.SalaryPeriodId);
            ViewBag.AdjustmentCategories = _context.valuesets.Where(v => v.GroupName == "PayRollCom" && v.IsActive).ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireUrlPermission]
        public async Task<IActionResult> Edit(SalaryAdjustments model)
        {
            if (ModelState.IsValid)
            {
                var existing = _context.SalaryAdjustments.Find(model.Id);
                if (existing == null) return NotFound();

                if (await _lockService.IsRecordLocked(existing.EmployeeId, existing.SalaryPeriodId) || await _lockService.IsRecordLocked(model.EmployeeId, model.SalaryPeriodId))
                {
                    ModelState.AddModelError("", "Salary Slip generated - so this record cannot be deleted or edited");
                    ViewBag.ActivePage = "SalaryAdjustment";
                    ViewBag.Employees = new SelectList(_context.emp.OrderBy(e => e.FirstName).Select(e => new { e.Id, Name = e.FirstName + " " + e.LastName + " (" + e.Code + ")" }), "Id", "Name", model.EmployeeId);
                    ViewBag.SalaryPeriods = new SelectList(_context.SalaryPeriod.Where(s => !s.IsPostedToGL).OrderByDescending(s => s.SalaryPeriodID), "SalaryPeriodID", "PeriodName", model.SalaryPeriodId);
                    ViewBag.AdjustmentCategories = _context.valuesets.Where(v => v.GroupName == "PayRollCom" && v.IsActive).ToList();
                    return View(model);
                }

                existing.EmployeeId = model.EmployeeId;
                existing.SalaryPeriodId = model.SalaryPeriodId;
                existing.Amount = model.Amount;
                existing.Type = model.Type;
                existing.AdjustmentCategory = model.AdjustmentCategory;
                existing.Reason = model.Reason;
                existing.IsApproved = model.IsApproved;

                _context.SaveChanges();

                TempData["Success"] = "Salary adjustment updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.ActivePage = "SalaryAdjustment";
            ViewBag.Employees = new SelectList(_context.emp.OrderBy(e => e.FirstName).Select(e => new { e.Id, Name = e.FirstName + " " + e.LastName + " (" + e.Code + ")" }), "Id", "Name", model.EmployeeId);
            ViewBag.SalaryPeriods = new SelectList(_context.SalaryPeriod.Where(s => !s.IsPostedToGL).OrderByDescending(s => s.SalaryPeriodID), "SalaryPeriodID", "PeriodName", model.SalaryPeriodId);
            ViewBag.AdjustmentCategories = _context.valuesets.Where(v => v.GroupName == "PayRollCom" && v.IsActive).ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireUrlPermission]
        public IActionResult Delete(int id)
        {
            var model = _context.SalaryAdjustments.Find(id);
            if (model != null)
            {
                if (_lockService.IsRecordLocked(model.EmployeeId, model.SalaryPeriodId).Result)
                {
                    TempData["Error"] = "Salary Slip generated - so this record cannot be deleted or edited";
                    return RedirectToAction(nameof(Index));
                }
                _context.SalaryAdjustments.Remove(model);
                _context.SaveChanges();
                TempData["Success"] = "Adjustment deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [SkipPermissionCheck]
        public IActionResult GetAdjustmentCategories(int employeeId, PayrollComponentTypes type)
        {
            // ... (existing code)
            List<string> categories = new List<string>();
            var employee = _context.emp.Find(employeeId);

            if (employeeId > 0)
            {
                var payroll = _context.PayRollGenrate
                    .Include(p => p.PayRollDefinationFile)
                    .OrderByDescending(p => p.id)
                    .FirstOrDefault(p => p.EmployeeFK == employeeId);

                if (type == PayrollComponentTypes.Earning)
                {
                    if (payroll != null && payroll.PayRollDefinationFile != null)
                    {
                        categories = _context.PayRollDefinationFile
                            .Include(d => d.PayRollCom)
                            .Where(d => d.DefinitionName == payroll.PayRollDefinationFile.DefinitionName)
                            .Select(d => d.PayRollCom != null ? d.PayRollCom.Name : d.label)
                            .Where(name => !string.IsNullOrEmpty(name))
                            .Distinct()
                            .ToList()!;
                    }
                    var standardEarnings = new List<string> { "Bonus", "Arrears", "Incentive", "Overtime Adjustment", "Commission", "Other Earning" };
                    categories.AddRange(standardEarnings);
                }
                else
                {
                    if (employee != null)
                    {
                        if (employee.isEobi) categories.Add("EOBI Deduction");
                        if (employee.isPfMember) categories.Add("Provident Fund Deduction");
                        if (employee.isSocailSecuirty) categories.Add("Social Security Deduction");
                        if (employee.IsFiler) categories.Add("Income Tax Adjustment");
                    }
                    var standardDeductions = new List<string> { "Fine", "Loan Recovery Adjustment", "Late Arrival Deduction", "Absent Deduction", "Security Deposit", "Shortage Deduction", "Other Deduction" };
                    categories.AddRange(standardDeductions);
                }
            }

            var generalVsets = _context.valuesets.Where(v => v.GroupName == "PayRollCom" && v.IsActive).Select(v => v.Name).ToList();
            if (type == PayrollComponentTypes.Earning) categories.AddRange(generalVsets);
            else categories.AddRange(generalVsets.Where(v => v.ToLower().Contains("deduction") || v.ToLower().Contains("recovery") || v.ToLower().Contains("tax") || v.ToLower().Contains("fine")));

            return PartialView("_CategoryOptions", categories.Where(c => !string.IsNullOrEmpty(c)).Distinct().OrderBy(c => c).ToList());
        }

        [HttpGet]
        [RequireUrlPermission]
        public IActionResult BulkUpload()
        {
            ViewBag.ActivePage = "SalaryAdjustment";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireUrlPermission]
        public async Task<IActionResult> BulkUpload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Please select a valid Excel file.";
                return RedirectToAction(nameof(BulkUpload));
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var adjustments = new List<SalaryAdjustments>();
            var errors = new List<string>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension?.Rows ?? 0;

                    if (rowCount < 2)
                    {
                        TempData["Error"] = "The Excel file is empty or missing headers.";
                        return RedirectToAction(nameof(BulkUpload));
                    }

                    // Column Mapping: A:EmpCode, B:PeriodName, C:Category, D:Type(Earning/Deduction), E:Amount, F:Reason
                    for (int row = 2; row <= rowCount; row++)
                    {
                        var empCode = worksheet.Cells[row, 1].Value?.ToString()?.Trim();
                        var periodName = worksheet.Cells[row, 2].Value?.ToString()?.Trim();
                        var category = worksheet.Cells[row, 3].Value?.ToString()?.Trim();
                        var typeStr = worksheet.Cells[row, 4].Value?.ToString()?.Trim();
                        var amountStr = worksheet.Cells[row, 5].Value?.ToString()?.Trim();
                        var reason = worksheet.Cells[row, 6].Value?.ToString()?.Trim();

                        if (string.IsNullOrEmpty(empCode) || string.IsNullOrEmpty(periodName) || string.IsNullOrEmpty(category) || string.IsNullOrEmpty(amountStr))
                        {
                            errors.Add($"Row {row}: Missing required data.");
                            continue;
                        }

                        var employee = _context.emp.FirstOrDefault(e => e.Code == empCode);
                        if (employee == null)
                        {
                            errors.Add($"Row {row}: Employee with code '{empCode}' not found.");
                            continue;
                        }

                        var period = _context.SalaryPeriod.FirstOrDefault(p => p.PeriodName == periodName);
                        if (period == null)
                        {
                            errors.Add($"Row {row}: Salary Period '{periodName}' not found.");
                            continue;
                        }

                        if (!decimal.TryParse(amountStr, out decimal amount))
                        {
                            errors.Add($"Row {row}: Invalid amount '{amountStr}'.");
                            continue;
                        }

                        var type = typeStr?.Equals("Deduction", StringComparison.OrdinalIgnoreCase) == true 
                            ? PayrollComponentTypes.Deduction 
                            : PayrollComponentTypes.Earning;

                        adjustments.Add(new SalaryAdjustments
                        {
                            EmployeeId = employee.Id,
                            SalaryPeriodId = period.SalaryPeriodID,
                            AdjustmentCategory = category,
                            Type = type,
                            Amount = amount,
                            Reason = reason,
                            CreatedAt = DateTime.Now,
                            CreatedBy = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0,
                            IsApproved = false // Require manual approval after bulk upload for safety
                        });
                    }
                }
            }

            if (errors.Any())
            {
                TempData["Error"] = string.Join("<br/>", errors.Take(5));
                if (errors.Count > 5) TempData["Error"] += $"<br/>...and {errors.Count - 5} more errors.";
                return RedirectToAction(nameof(BulkUpload));
            }

            if (adjustments.Any())
            {
                _context.SalaryAdjustments.AddRange(adjustments);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Successfully uploaded {adjustments.Count} adjustments.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [SkipPermissionCheck]
        public IActionResult DownloadTemplate()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("SalaryAdjustments");
                worksheet.Cells[1, 1].Value = "EmployeeCode";
                worksheet.Cells[1, 2].Value = "SalaryPeriodName";
                worksheet.Cells[1, 3].Value = "Category";
                worksheet.Cells[1, 4].Value = "Type (Earning/Deduction)";
                worksheet.Cells[1, 5].Value = "Amount";
                worksheet.Cells[1, 6].Value = "Reason";

                // Add some example data
                worksheet.Cells[2, 1].Value = "EMP001";
                worksheet.Cells[2, 2].Value = DateTime.Now.ToString("yyyy-MM");
                worksheet.Cells[2, 3].Value = "Bonus";
                worksheet.Cells[2, 4].Value = "Earning";
                worksheet.Cells[2, 5].Value = 5000;
                worksheet.Cells[2, 6].Value = "Performance Bonus";

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SalaryAdjustmentTemplate.xlsx");
            }
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

            return PartialView("_EmployeeCard", employee);
        }

        [HttpGet]
        [SkipPermissionCheck]
        public IActionResult GetEmployeeSalaryBreakdown(int employeeId)
        {
            var payroll = _context.PayRollGenrate
                .Include(p => p.PayRollDefinationFile)
                .OrderByDescending(p => p.id)
                .FirstOrDefault(p => p.EmployeeFK == employeeId);

            if (payroll == null)
            {
                return Content("<div class='empty-state-card mt-3'><i class='fas fa-exclamation-triangle fa-2x mb-2 text-warning'></i><p class='small'>No payroll structure found for this employee.</p></div>");
            }

            var components = _context.PayRollDefinationFile
                .Include(d => d.PayRollCom)
                .Where(d => d.DefinitionName == payroll.PayRollDefinationFile.DefinitionName)
                .ToList();

            ViewBag.GrossSalary = payroll.salery;
            return PartialView("_EmployeeSalaryBreakdown", components);
        }

        [HttpGet]
        public IActionResult Export(string search, string columns, string downloadToken)
        {
            if (!string.IsNullOrEmpty(downloadToken))
            {
                Response.Cookies.Append("fileDownloadToken", downloadToken, new Microsoft.AspNetCore.Http.CookieOptions { Path = "/", HttpOnly = false, Secure = false });
            }

            var query = _context.SalaryAdjustments.Include(s => s.employee).Include(s => s.SalaryPeriod).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(s => s.employee.FirstName.Contains(search) || 
                                       s.employee.LastName.Contains(search) || 
                                       s.employee.Code.Contains(search) ||
                                       s.AdjustmentCategory.Contains(search));
            }

            var list = query.OrderByDescending(s => s.CreatedAt).ToList();

            var selectedCols = !string.IsNullOrEmpty(columns)
                ? columns.Split(',').Select(c => c.Trim().ToLowerInvariant()).ToList()
                : new List<string> { "employee", "period", "category", "type", "amount", "status" };

            var headersList = new List<string>();
            if (selectedCols.Contains("employee")) headersList.Add("Employee");
            if (selectedCols.Contains("period")) headersList.Add("Period");
            if (selectedCols.Contains("category")) headersList.Add("Category");
            if (selectedCols.Contains("type")) headersList.Add("Type");
            if (selectedCols.Contains("amount")) headersList.Add("Amount");
            if (selectedCols.Contains("status")) headersList.Add("Status");

            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (var package = new OfficeOpenXml.ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("SalaryAdjustments");
                for (int col = 0; col < headersList.Count; col++)
                {
                    worksheet.Cells[1, col + 1].Value = headersList[col];
                    worksheet.Cells[1, col + 1].Style.Font.Bold = true;
                }
                int rowIdx = 2;
                foreach (var item in list)
                {
                    int colIdx = 1;
                    if (selectedCols.Contains("employee")) worksheet.Cells[rowIdx, colIdx++].Value = $"{item.employee?.FullName} ({item.employee?.Code})";
                    if (selectedCols.Contains("period")) worksheet.Cells[rowIdx, colIdx++].Value = item.SalaryPeriod?.PeriodName;
                    if (selectedCols.Contains("category")) worksheet.Cells[rowIdx, colIdx++].Value = item.AdjustmentCategory;
                    if (selectedCols.Contains("type")) worksheet.Cells[rowIdx, colIdx++].Value = item.Type.ToString();
                    if (selectedCols.Contains("amount")) worksheet.Cells[rowIdx, colIdx++].Value = item.Amount;
                    if (selectedCols.Contains("status")) worksheet.Cells[rowIdx, colIdx++].Value = item.IsApproved ? "Approved" : "Pending";
                    rowIdx++;
                }
                worksheet.Cells.AutoFitColumns();
                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "salary_adjustments.xlsx");
            }
        }
    }
}
