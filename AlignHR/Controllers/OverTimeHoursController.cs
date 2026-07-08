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
    public class OverTimeHoursController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IPayrollLockService _lockService;

        public OverTimeHoursController(ApplicationDbContext context, IPayrollLockService lockService)
        {
            _context = context;
            _lockService = lockService;
        }

        [RequireUrlPermission]
        public IActionResult Index(string search, int? page)
        {
            int pageSize = 15;               
            int pageNumber = page ?? 1;      

            var obj = _context.OverTimeHours
                .Include(o => o.Employee)
                .Include(o => o.SalaryPeriod)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                obj = obj.Where(d => d.Employee.FirstName.Contains(search) || d.Employee.LastName.Contains(search) || d.SalaryPeriod.PeriodName.Contains(search));
            }

            obj = obj.OrderByDescending(d => d.id);

            var model = obj.ToPagedList(pageNumber, pageSize);

            if (Request.Headers["HX-Request"] == "true")
            {
                ViewBag.Search = search;
                return PartialView("_OverTimeHoursTable", model);
            }

            return View(model);
        }

        [RequireUrlPermission]
        public IActionResult Create()
        {
            ViewBag.EmployeeId = new SelectList(_context.emp.Select(e => new { e.Id, Name = e.FirstName + " " + e.LastName }), "Id", "Name");
            ViewBag.SalaryPeriodFK = new SelectList(_context.SalaryPeriod.Where(s => !s.IsPostedToGL), "SalaryPeriodID", "PeriodName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OverTimeHours d)
        {
            if (ModelState.IsValid)
            {
                if (await _lockService.IsRecordLocked(d.EmployeeId, d.SalaryPeriodFK))
                {
                    ModelState.AddModelError("", "Salary Slip generated or Period Posted to GL - so this record cannot be added, edited or deleted");
                    ViewBag.EmployeeId = new SelectList(_context.emp.Select(e => new { e.Id, Name = e.FirstName + " " + e.LastName }), "Id", "Name", d.EmployeeId);
                    ViewBag.SalaryPeriodFK = new SelectList(_context.SalaryPeriod.Where(s => !s.IsPostedToGL), "SalaryPeriodID", "PeriodName", d.SalaryPeriodFK);
                    return View(d);
                }
                var exists = await _context.OverTimeHours.AnyAsync(o => o.EmployeeId == d.EmployeeId && o.SalaryPeriodFK == d.SalaryPeriodFK);
                if (exists)
                {
                    ModelState.AddModelError("", "Overtime record already exists for this employee in the selected period.");
                    ViewBag.EmployeeId = new SelectList(_context.emp.Select(e => new { e.Id, Name = e.FirstName + " " + e.LastName }), "Id", "Name", d.EmployeeId);
                    ViewBag.SalaryPeriodFK = new SelectList(_context.SalaryPeriod.Where(s => !s.IsPostedToGL), "SalaryPeriodID", "PeriodName", d.SalaryPeriodFK);
                    return View(d);
                }

                var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;
                d.createat = DateTime.Now;
                d.createdby = currentUserId;

                _context.OverTimeHours.Add(d);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.EmployeeId = new SelectList(_context.emp.Select(e => new { e.Id, Name = e.FirstName + " " + e.LastName }), "Id", "Name", d.EmployeeId);
            ViewBag.SalaryPeriodFK = new SelectList(_context.SalaryPeriod.Where(s => !s.IsPostedToGL), "SalaryPeriodID", "PeriodName", d.SalaryPeriodFK);
            return View(d);
        }

        [RequireUrlPermission]
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var ot = _context.OverTimeHours
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
            var ot = await _context.OverTimeHours.FindAsync(id);
            if (ot != null)
            {
                if (await _lockService.IsRecordLocked(ot.EmployeeId, ot.SalaryPeriodFK))
                {
                    TempData["Error"] = "Salary Slip generated or Period Posted to GL - so this record cannot be added, edited or deleted";
                    return RedirectToAction(nameof(Index));
                }
                try
                {
                    _context.OverTimeHours.Remove(ot);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "OverTime Hours deleted successfully.";
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
            var ot = await _context.OverTimeHours
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
            var ot = _context.OverTimeHours.Find(id);
            if (ot == null)
                return NotFound();

            ViewBag.EmployeeId = new SelectList(_context.emp.Select(e => new { e.Id, Name = e.FirstName + " " + e.LastName }), "Id", "Name", ot.EmployeeId);
            ViewBag.SalaryPeriodFK = new SelectList(_context.SalaryPeriod.Where(s => !s.IsPostedToGL), "SalaryPeriodID", "PeriodName", ot.SalaryPeriodFK);

            return View(ot);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(OverTimeHours d)
        {
            if (ModelState.IsValid)
            {
                if (await _lockService.IsRecordLocked(d.EmployeeId, d.SalaryPeriodFK))
                {
                    ModelState.AddModelError("", "Salary Slip generated or Period Posted to GL - so this record cannot be added, edited or deleted");
                    ViewBag.EmployeeId = new SelectList(_context.emp.Select(e => new { e.Id, Name = e.FirstName + " " + e.LastName }), "Id", "Name", d.EmployeeId);
                    ViewBag.SalaryPeriodFK = new SelectList(_context.SalaryPeriod.Where(s => !s.IsPostedToGL), "SalaryPeriodID", "PeriodName", d.SalaryPeriodFK);
                    return View(d);
                }
                var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;

                d.updateat = DateTime.Now;
                d.updatedby = currentUserId;

                _context.OverTimeHours.Update(d);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            ViewBag.EmployeeId = new SelectList(_context.emp.Select(e => new { e.Id, Name = e.FirstName + " " + e.LastName }), "Id", "Name", d.EmployeeId);
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
            builder.AppendLine("EmployeeCode,OTHours,Amount");
            builder.AppendLine("EMP001,10.5,5000"); 
            
            var csvBytes = System.Text.Encoding.UTF8.GetBytes(builder.ToString());
            return File(csvBytes, "text/csv", "OverTimeHours_Sample.csv");
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
            var recordsToSave = new List<OverTimeHours>();
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
                    if (parts.Length < 3)
                    {
                        errors.Add($"Line {lineCount}: Invalid format. Expected 'EmployeeCode,OTHours,Amount'.");
                        continue;
                    }

                    var code = parts[0].Trim();
                    var otHours = parts[1].Trim();
                    var amountStr = parts[2].Trim();

                    if (codesInFile.Contains(code))
                    {
                        errors.Add($"Line {lineCount}: Duplicate Employee Code '{code}' found in the file.");
                        continue;
                    }
                    codesInFile.Add(code);

                    var employee = await _context.emp.FirstOrDefaultAsync(e => e.Code == code && e.SalaryStatus == "Active");
                    if (employee == null)
                    {
                        errors.Add($"Line {lineCount}: Employee with code '{code}' not found or not active.");
                        continue;
                    }

                    var alreadyExists = await _context.OverTimeHours.AnyAsync(o => o.EmployeeId == employee.Id && o.SalaryPeriodFK == SalaryPeriodFK);
                    if (alreadyExists)
                    {
                        errors.Add($"Line {lineCount}: Overtime record already exists for employee '{code}' in this period.");
                        continue;
                    }

                    if (!int.TryParse(amountStr, out int amount))
                    {
                        errors.Add($"Line {lineCount}: Invalid amount '{amountStr}'. Must be an integer.");
                        continue;
                    }

                    recordsToSave.Add(new OverTimeHours
                    {
                        EmployeeId = employee.Id,
                        SalaryPeriodFK = SalaryPeriodFK,
                        OTHours = otHours,
                        Amount = amount,
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
                _context.OverTimeHours.AddRange(recordsToSave);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Successfully uploaded {recordsToSave.Count} records.";
            }
            else
            {
                TempData["Error"] = "No valid records found after skipping sample row.";
            }

            return RedirectToAction("Index");
        }
    }
}

