using AlignHR.Data;
using AlignHR.Models;
using AlignHR.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace AlignHR.Controllers
{
    public class MasterLoanAdvanceController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public MasterLoanAdvanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        [RequireUrlPermission]
        public IActionResult Index(string search, int? page)
        {
            int pageSize = 15;
            int pageNumber = page ?? 1;

            var query = _context.MasterLoanAdvance
                .Include(l => l.Employee)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(l => l.Employee!.FirstName.Contains(search) || 
                                       l.Employee!.LastName.Contains(search) || 
                                       l.Employee!.Code.Contains(search));
                ViewBag.Search = search;
            }

            var model = query.OrderByDescending(l => l.id).ToPagedList(pageNumber, pageSize);
            return View(model);
        }

        [RequireUrlPermission]
        public IActionResult Create()
        {
            return View(new MasterLoanAdvance
            {
                RequestDate = DateTime.Now,
                TransactionType = TransactionType.Salary
            });
        }

        [HttpGet]
        public async Task<IActionResult> SearchEmployee(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return Content("<div class='alert alert-warning'>Please enter an employee code.</div>");
            }

            var employee = await _context.emp
                .Include(e => e.Department)
                .Include(e => e.Location)
                .Include(e => e.EmploymentStatus)
                .FirstOrDefaultAsync(e => e.Code == code && e.SalaryStatus == "Active");

            if (employee == null)
            {
                return Content("<div class='alert alert-danger'>Active employee not found with this code.</div>");
            }

            // Fetch latest gross salary
            ViewBag.GrossSalary = await GetEmployeeGrossSalary(employee.Id);

            return PartialView("_EmployeeSearchResult", employee);
        }

        [HttpGet]
        public async Task<IActionResult> GetFields(TransactionType TransactionType, int? id)
        {
            MasterLoanAdvance model;
            if (id.HasValue && id > 0)
            {
                model = await _context.MasterLoanAdvance.FindAsync(id.Value) ?? new MasterLoanAdvance { RequestDate = DateTime.Now };
            }
            else
            {
                model = new MasterLoanAdvance { RequestDate = DateTime.Now, TransactionType = TransactionType };
            }

            if (TransactionType == TransactionType.Salary)
            {
                return PartialView("_SalaryFields", model);
            }
            else if (TransactionType == TransactionType.Loan)
            {
                return PartialView("_LoanFields", model);
            }

            return Content("");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MasterLoanAdvance model)
        {
            decimal grossSalary = await GetEmployeeGrossSalary(model.EmployeeID);
            decimal limit = grossSalary * 0.5m;

            if (model.TransactionType == TransactionType.Salary)
            {
                model.TenureMonths = 1;
                model.InterestRate = 0;
                model.ApprovedAmount = model.TotalAmount;

                // Validation: Advance amount <= 50% Gross Salary
                if (model.TotalAmount > limit)
                {
                    ModelState.AddModelError("TotalAmount", $"Salary Advance amount cannot exceed 50% of the employee's gross monthly salary (Limit: {limit:N2}).");
                }
            }
            else if (model.TransactionType == TransactionType.Loan)
            {
                // Validation: Monthly installment <= 50% Gross Salary
                if (model.MonthlyInstallment > limit)
                {
                    ModelState.AddModelError("MonthlyInstallment", $"Monthly Installment cannot exceed 50% of the employee's gross monthly salary (Limit: {limit:N2}).");
                }
            }

            if (ModelState.IsValid)
            {
                var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;
                model.createdby = currentUserId;
                model.createat = DateTime.Now;

                _context.MasterLoanAdvance.Add(model);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Transaction created successfully.";
                return RedirectToAction(nameof(Index));
            }

            // Capture validation errors for debugging
            var errors = string.Join(" | ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));
                
            TempData["Error"] = "Validation failed: " + (string.IsNullOrEmpty(errors) ? "Unknown error" : errors);

            if (model.EmployeeID > 0)
            {
                model.Employee = await _context.emp.FindAsync(model.EmployeeID);
            }

            return View("Create", model);
        }

        [RequireUrlPermission]
        public async Task<IActionResult> Details(int id)
        {
            var model = await _context.MasterLoanAdvance
                .Include(l => l.Employee)
                .ThenInclude(e => e!.Department)
                .FirstOrDefaultAsync(l => l.id == id);

            if (model == null) return NotFound();

            return View(model);
        }

        [RequireUrlPermission]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _context.MasterLoanAdvance
                .Include(l => l.Employee)
                .ThenInclude(e => e!.Department)
                .FirstOrDefaultAsync(l => l.id == id);

            if (model == null) return NotFound();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MasterLoanAdvance model)
        {
            decimal grossSalary = await GetEmployeeGrossSalary(model.EmployeeID);
            decimal limit = grossSalary * 0.5m;

            if (model.TransactionType == TransactionType.Salary)
            {
                model.TenureMonths = 1;
                model.InterestRate = 0;
                model.ApprovedAmount = model.TotalAmount;

                // Validation: Advance amount <= 50% Gross Salary
                if (model.TotalAmount > limit)
                {
                    ModelState.AddModelError("TotalAmount", $"Salary Advance amount cannot exceed 50% of the employee's gross monthly salary (Limit: {limit:N2}).");
                }
            }
            else if (model.TransactionType == TransactionType.Loan)
            {
                // Validation: Monthly installment <= 50% Gross Salary
                if (model.MonthlyInstallment > limit)
                {
                    ModelState.AddModelError("MonthlyInstallment", $"Monthly Installment cannot exceed 50% of the employee's gross monthly salary (Limit: {limit:N2}).");
                }
            }

            if (ModelState.IsValid)
            {
                var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;
                model.updatedby = currentUserId;
                model.updateat = DateTime.Now;

                _context.MasterLoanAdvance.Update(model);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Transaction updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            var errors = string.Join(" | ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));

            TempData["Error"] = "Update failed: " + (string.IsNullOrEmpty(errors) ? "Unknown error" : errors);
            
            // Reload employee data for the view
            model.Employee = await _context.emp.FindAsync(model.EmployeeID);
            return View("Edit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _context.MasterLoanAdvance.FindAsync(id);
            if (model != null)
            {
                _context.MasterLoanAdvance.Remove(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Record deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<decimal> GetEmployeeGrossSalary(int employeeId)
        {
            // Fetch Latest Salary entry for the employee from PayRollGenrate
            // Following the pattern from EOBIDetuctionController and IncomeTaxDetuctionController
            var latestSalary = await _context.PayRollGenrate
                .Where(p => p.EmployeeFK == employeeId)
                .OrderByDescending(p => p.id)
                .Select(p => (decimal?)p.salery)
                .FirstOrDefaultAsync() ?? 0;

            return latestSalary;
        }

        [HttpGet]
        public IActionResult Export(string search, string columns, string downloadToken)
        {
            if (!string.IsNullOrEmpty(downloadToken))
            {
                Response.Cookies.Append("fileDownloadToken", downloadToken, new Microsoft.AspNetCore.Http.CookieOptions { Path = "/", HttpOnly = false, Secure = false });
            }

            var query = _context.MasterLoanAdvance.Include(l => l.Employee).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(l => l.Employee!.FirstName.Contains(search) || 
                                       l.Employee!.LastName.Contains(search) || 
                                       l.Employee!.Code.Contains(search));
            }

            var list = query.OrderByDescending(d => d.id).ToList();

            var selectedCols = !string.IsNullOrEmpty(columns)
                ? columns.Split(',').Select(c => c.Trim().ToLowerInvariant()).ToList()
                : new List<string> { "employee", "type", "date", "amount", "status", "deduction" };

            var headersList = new List<string>();
            if (selectedCols.Contains("employee")) headersList.Add("Employee");
            if (selectedCols.Contains("type")) headersList.Add("Type");
            if (selectedCols.Contains("date")) headersList.Add("Date");
            if (selectedCols.Contains("amount")) headersList.Add("Amount");
            if (selectedCols.Contains("status")) headersList.Add("Status");
            if (selectedCols.Contains("deduction")) headersList.Add("Deduction");

            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (var package = new OfficeOpenXml.ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("LoanAdvances");
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
                    if (selectedCols.Contains("type")) worksheet.Cells[rowIdx, colIdx++].Value = item.TransactionType.ToString();
                    if (selectedCols.Contains("date")) worksheet.Cells[rowIdx, colIdx++].Value = item.RequestDate.ToString("yyyy-MM-dd");
                    if (selectedCols.Contains("amount")) worksheet.Cells[rowIdx, colIdx++].Value = item.TotalAmount;
                    if (selectedCols.Contains("status")) worksheet.Cells[rowIdx, colIdx++].Value = item.Status.ToString();
                    if (selectedCols.Contains("deduction")) worksheet.Cells[rowIdx, colIdx++].Value = item.LoanDetuction ? "Exited" : "Active";
                    rowIdx++;
                }
                worksheet.Cells.AutoFitColumns();
                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "loans_advances.xlsx");
            }
        }
    }
}
