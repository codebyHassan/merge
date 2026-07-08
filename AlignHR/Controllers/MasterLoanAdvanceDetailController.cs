using AlignHR.Data;
using AlignHR.Models;
using AlignHR.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace AlignHR.Controllers
{
    public class MasterLoanAdvanceDetailController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public MasterLoanAdvanceDetailController(ApplicationDbContext context)
        {
            _context = context;
        }

        private async Task<bool> IsRecordLocked(string deductionMonth)
        {
            // deductionMonth is "yyyy/MM"
            // Find salary period that matches this month
            var period = await _context.SalaryPeriod.FirstOrDefaultAsync(s => s.StartDate.ToString("yyyy/MM") == deductionMonth);
            if (period != null && period.IsPostedToGL) return true;

            return false;
        }

        [RequireUrlPermission]
        public async Task<IActionResult> Index(string search, int? page)
        {
            int pageSize = 15;
            int pageNumber = page ?? 1;

            var query = _context.MasterLoanAdvanceDetail
                .Include(d => d.MasterLoanAdvance)
                    .ThenInclude(m => m!.Employee)
                .Where(d => d.MasterLoanAdvance!.TransactionType == TransactionType.Loan)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(d => d.MasterLoanAdvance!.Employee!.Code.Contains(search) || 
                                       d.MasterLoanAdvance.Employee.FirstName.Contains(search) ||
                                       d.DeductionMonth.Contains(search));
                ViewBag.Search = search;
            }

            // Pre-fetch locked months
            var lockedMonths = await _context.SalaryPeriod
                .Where(p => p.IsPostedToGL)
                .Select(p => p.StartDate.ToString("yyyy/MM"))
                .ToListAsync();
            ViewBag.LockedMonths = lockedMonths;

            var model = query.OrderByDescending(d => d.id).ToPagedList(pageNumber, pageSize);
            return View(model);
        }

        [RequireUrlPermission]
        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View(new MasterLoanAdvanceDetail { createat = DateTime.Now });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MasterLoanAdvanceDetail model)
        {
            if (ModelState.IsValid)
            {
                if (await IsRecordLocked(model.DeductionMonth))
                {
                    ModelState.AddModelError("", "This month is already Posted to GL and cannot be modified.");
                    await PopulateDropdowns(model.MasterId);
                    return View(model);
                }
                var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;
                model.createdby = currentUserId;
                model.createat = DateTime.Now;

                _context.MasterLoanAdvanceDetail.Add(model);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Installment record created successfully.";
                return RedirectToAction(nameof(Index));
            }

            await PopulateDropdowns(model.MasterId);
            return View(model);
        }

        [RequireUrlPermission]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _context.MasterLoanAdvanceDetail.FindAsync(id);
            if (model == null) return NotFound();

            await PopulateDropdowns(model.MasterId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MasterLoanAdvanceDetail model)
        {
            if (ModelState.IsValid)
            {
                if (await IsRecordLocked(model.DeductionMonth))
                {
                    ModelState.AddModelError("", "This month is already Posted to GL and cannot be modified.");
                    await PopulateDropdowns(model.MasterId);
                    return View(model);
                }
                var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;
                model.updatedby = currentUserId;
                model.updateat = DateTime.Now;

                _context.MasterLoanAdvanceDetail.Update(model);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Installment record updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            await PopulateDropdowns(model.MasterId);
            return View(model);
        }

        [RequireUrlPermission]
        public async Task<IActionResult> Details(int id)
        {
            var model = await _context.MasterLoanAdvanceDetail
                .Include(d => d.MasterLoanAdvance)
                    .ThenInclude(m => m!.Employee)
                .FirstOrDefaultAsync(d => d.id == id);

            if (model == null) return NotFound();
            return View(model);
        }

        [RequireUrlPermission]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _context.MasterLoanAdvanceDetail
                .Include(d => d.MasterLoanAdvance)
                    .ThenInclude(m => m!.Employee)
                .FirstOrDefaultAsync(d => d.id == id);

            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var model = await _context.MasterLoanAdvanceDetail.FindAsync(id);
            if (model != null)
            {
                if (await IsRecordLocked(model.DeductionMonth))
                {
                    TempData["Error"] = "This month is already Posted to GL and cannot be modified.";
                    return RedirectToAction(nameof(Index));
                }
                _context.MasterLoanAdvanceDetail.Remove(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Installment record deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetInstallmentFields(int masterId)
        {
            var loan = await _context.MasterLoanAdvance
                .Include(m => m.Details)
                .FirstOrDefaultAsync(m => m.id == masterId);

            if (loan == null) return Content("");

            // --- Flat Interest & Global Balance Logic ---
            // Formula: (Principal * Rate%) + Principal - Total Deductions
            decimal principal = loan.ApprovedAmount;
            decimal flatInterestTotal = principal * (loan.InterestRate / 100);
            decimal initialTotalDebt = principal + flatInterestTotal;
            
            decimal totalDeductedSoFar = loan.Details.Sum(d => d.DeductedAmount);
            
            // Current Net Balance before this new entry
            decimal netBalanceBeforeThis = initialTotalDebt - totalDeductedSoFar;

            // Remaining Tenure
            int remainingTenure = loan.TenureMonths - loan.Details.Count;

            // Calculate suggestion for this installment interest (distributed if needed, or just 0 if fixed upfront)
            // But usually, we might want to show how much interest is in this EMI if they are equal-EMI
            decimal interestPerMonth = remainingTenure > 0 ? (flatInterestTotal / loan.TenureMonths) : 0;

            // Calculate next installment number
            var nextInstallmentNo = loan.Details.Count + 1;

            var model = new MasterLoanAdvanceDetail
            {
                MasterId = masterId,
                InstallmentNo = nextInstallmentNo,
                DeductionMonth = DateTime.Now.ToString("yyyy/MM"),
                TotalDeductionAmount = loan.MonthlyInstallment,
                DeductedAmount = loan.MonthlyInstallment,
                InterestAmount = Math.Round(interestPerMonth, 2),
                Status = LoanInstallmentStatus.Pending
            };

            // Informational data for the view
            ViewBag.LoanPrincipal = principal;
            ViewBag.FlatInterest = flatInterestTotal;
            ViewBag.InitialTotalDebt = initialTotalDebt;
            ViewBag.TotalPaid = totalDeductedSoFar;
            ViewBag.NetBalance = netBalanceBeforeThis;
            ViewBag.RemainingTenure = remainingTenure;

            // Suggest remaining balance AFTER this installment
            model.RemainingAmount = netBalanceBeforeThis - model.DeductedAmount;

            return PartialView("_InstallmentFields", model);
        }

        private async Task PopulateDropdowns(int? selectedMaster = null)
        {
            var masters = await _context.MasterLoanAdvance
                .Include(m => m.Employee)
                .Where(m => m.Status == LoanStatus.Approved && m.TransactionType == TransactionType.Loan && m.LoanDetuction == false)
                .Select(m => new
                {
                    id = m.id,
                    DisplayText = $"{m.Employee!.Code} - {m.Employee.FirstName} (Interest: {m.InterestRate}% | Principal: {m.ApprovedAmount:N0})"
                })
                .ToListAsync();

            ViewBag.MasterId = new SelectList(masters, "id", "DisplayText", selectedMaster);
        }

        [HttpGet]
        public IActionResult Export(string search, string columns, string downloadToken)
        {
            if (!string.IsNullOrEmpty(downloadToken))
            {
                Response.Cookies.Append("fileDownloadToken", downloadToken, new Microsoft.AspNetCore.Http.CookieOptions { Path = "/", HttpOnly = false, Secure = false });
            }

            var query = _context.MasterLoanAdvanceDetail.Include(d => d.MasterLoanAdvance).ThenInclude(m => m!.Employee).Where(d => d.MasterLoanAdvance!.TransactionType == TransactionType.Loan).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(d => d.MasterLoanAdvance!.Employee!.Code.Contains(search) || d.MasterLoanAdvance.Employee.FirstName.Contains(search) || d.DeductionMonth.Contains(search));
            }

            var list = query.OrderByDescending(d => d.id).ToList();

            var selectedCols = !string.IsNullOrEmpty(columns)
                ? columns.Split(',').Select(c => c.Trim().ToLowerInvariant()).ToList()
                : new List<string> { "installment", "employee", "type", "month", "emi amount", "deducted", "net balance", "status" };

            var headersList = new List<string>();
            if (selectedCols.Contains("installment")) headersList.Add("Installment");
            if (selectedCols.Contains("employee")) headersList.Add("Employee");
            if (selectedCols.Contains("type")) headersList.Add("Type");
            if (selectedCols.Contains("month")) headersList.Add("Month");
            if (selectedCols.Contains("emi amount")) headersList.Add("EMI Amount");
            if (selectedCols.Contains("deducted")) headersList.Add("Deducted");
            if (selectedCols.Contains("net balance")) headersList.Add("Net Balance");
            if (selectedCols.Contains("status")) headersList.Add("Status");

            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (var package = new OfficeOpenXml.ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Installments");
                for (int col = 0; col < headersList.Count; col++)
                {
                    worksheet.Cells[1, col + 1].Value = headersList[col];
                    worksheet.Cells[1, col + 1].Style.Font.Bold = true;
                }
                int rowIdx = 2;
                foreach (var item in list)
                {
                    int colIdx = 1;
                    if (selectedCols.Contains("installment")) worksheet.Cells[rowIdx, colIdx++].Value = item.InstallmentNo;
                    if (selectedCols.Contains("employee")) worksheet.Cells[rowIdx, colIdx++].Value = $"{item.MasterLoanAdvance?.Employee?.FirstName} {item.MasterLoanAdvance?.Employee?.LastName} ({item.MasterLoanAdvance?.Employee?.Code})";
                    if (selectedCols.Contains("type")) worksheet.Cells[rowIdx, colIdx++].Value = item.MasterLoanAdvance?.TransactionType.ToString();
                    if (selectedCols.Contains("month")) worksheet.Cells[rowIdx, colIdx++].Value = item.DeductionMonth;
                    if (selectedCols.Contains("emi amount")) worksheet.Cells[rowIdx, colIdx++].Value = item.TotalDeductionAmount;
                    if (selectedCols.Contains("deducted")) worksheet.Cells[rowIdx, colIdx++].Value = item.DeductedAmount;
                    if (selectedCols.Contains("net balance")) worksheet.Cells[rowIdx, colIdx++].Value = item.RemainingAmount;
                    if (selectedCols.Contains("status")) worksheet.Cells[rowIdx, colIdx++].Value = item.Status.ToString();
                    rowIdx++;
                }
                worksheet.Cells.AutoFitColumns();
                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "installments.xlsx");
            }
        }
    }
}
