using AlignHR.Data;
using AlignHR.Models;
using AlignHR.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace AlignHR.Controllers
{
    public class PayRollGenrateController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public PayRollGenrateController(ApplicationDbContext context)
        {
            _context = context;
        }

        [RequireUrlPermission]
        public IActionResult Index(string search, int? page)
        {
            int pageSize = 15;
            int pageNumber = page ?? 1;

            var query = _context.PayRollGenrate
                .AsNoTracking()
                .Include(p => p.Employee)
                .Include(p => p.PayRollDefinationFile)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Employee.FirstName.Contains(search) || 
                                       p.Employee.LastName.Contains(search) || 
                                       p.Employee.Code.Contains(search));
            }

            var model = query.OrderByDescending(p => p.createat).ToPagedList(pageNumber, pageSize);

            // Handle HTMX requests for smooth searching
            if (Request.Headers["HX-Request"].ToString() == "true")
            {
                return PartialView("_PayrollTable", model);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Export(string search, string columns, string downloadToken)
        {
            if (!string.IsNullOrEmpty(downloadToken))
            {
                Response.Cookies.Append("fileDownloadToken", downloadToken, new Microsoft.AspNetCore.Http.CookieOptions
                {
                    Path = "/",
                    HttpOnly = false,
                    Secure = false
                });
            }

            var query = _context.PayRollGenrate
                .Include(p => p.Employee)
                .Include(p => p.PayRollDefinationFile)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Employee.FirstName.Contains(search) || 
                                       p.Employee.LastName.Contains(search) || 
                                       p.Employee.Code.Contains(search));
            }

            var list = query.OrderByDescending(p => p.createat).ToList();

            var selectedCols = !string.IsNullOrEmpty(columns)
                ? columns.Split(',').Select(c => c.Trim().ToLowerInvariant()).ToList()
                : new List<string> { "employee name", "code", "definition", "gross salary", "generated date" };

            var headersList = new List<string>();
            if (selectedCols.Contains("employee name")) headersList.Add("Employee Name");
            if (selectedCols.Contains("code")) headersList.Add("Code");
            if (selectedCols.Contains("definition")) headersList.Add("Definition");
            if (selectedCols.Contains("gross salary")) headersList.Add("Gross Salary");
            if (selectedCols.Contains("generated date")) headersList.Add("Generated Date");

            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (var package = new OfficeOpenXml.ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("SalaryBreakups");

                for (int col = 0; col < headersList.Count; col++)
                {
                    worksheet.Cells[1, col + 1].Value = headersList[col];
                    worksheet.Cells[1, col + 1].Style.Font.Bold = true;
                }

                int rowIdx = 2;
                foreach (var item in list)
                {
                    int colIdx = 1;
                    if (selectedCols.Contains("employee name")) worksheet.Cells[rowIdx, colIdx++].Value = $"{item.Employee?.FirstName} {item.Employee?.LastName}";
                    if (selectedCols.Contains("code")) worksheet.Cells[rowIdx, colIdx++].Value = item.Employee?.Code;
                    if (selectedCols.Contains("definition")) worksheet.Cells[rowIdx, colIdx++].Value = item.PayRollDefinationFile?.DefinitionName;
                    if (selectedCols.Contains("gross salary")) worksheet.Cells[rowIdx, colIdx++].Value = item.salery;
                    if (selectedCols.Contains("generated date")) worksheet.Cells[rowIdx, colIdx++].Value = item.createat.ToString("yyyy-MM-dd");
                    rowIdx++;
                }

                worksheet.Cells.AutoFitColumns();

                var fileBytes = package.GetAsByteArray();
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "salary_breakups.xlsx");
            }
        }

        [RequireUrlPermission]
        public IActionResult Create()
        {
            ViewBag.Employees = _context.emp.OrderBy(e => e.FirstName).ToList();
            var definitions = _context.PayRollDefinationFile
                .Where(d => d.DefinitionName != null && !d.DefinitionName.StartsWith("Custom_"))
                .ToList()
                .GroupBy(d => d.DefinitionName)
                .Select(g => g.First())
                .OrderBy(d => d.DefinitionName)
                .ToList();
            ViewBag.Definitions = definitions;
            ViewBag.Valuesets = _context.valuesets.Where(v => v.GroupName == "PayRollCom" && v.IsActive).ToList();
            return View(new PayRollGenrate());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireUrlPermission]
        public IActionResult Create(PayRollGenrate model, string IsManual, List<PayRollDefinationFile> manualItems)
        {
            ModelState.Clear();
            var errors = new List<string>();

            if (model.EmployeeFK <= 0) errors.Add("Please select an Employee.");
            if (model.salery <= 0) errors.Add("Please enter a valid Gross Salary.");

            if (IsManual == "true")
            {
                if (manualItems == null || !manualItems.Any())
                {
                    errors.Add("Please add at least one manual component.");
                }
                else
                {
                    double totalPct = manualItems.Sum(i => double.TryParse(i.Percentage, out double p) ? p : 0);
                    if (Math.Abs(totalPct - 100) > 0.01) errors.Add("Total manual percentage must be exactly 100%.");
                }
            }
            else
            {
                if (model.PayRollDefinationFK <= 0) errors.Add("Please select a Payroll Definition.");
            }

            // --- DUPLICATE CHECK ---
            if (model.EmployeeFK > 0)
            {
                var alreadyExists = _context.PayRollGenrate.Any(p => p.EmployeeFK == model.EmployeeFK);
                if (alreadyExists)
                {
                    errors.Add("A payroll record already exists for this employee. Please edit the existing record instead of creating a new one.");
                }
            }

            if (!errors.Any())
            {
                try
                {
                    var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;
                    var now = DateTime.Now;

                    if (IsManual == "true")
                    {
                        var emp = _context.emp.Find(model.EmployeeFK);
                        string customDefName = $"Custom_{emp?.Code ?? emp?.Id.ToString()}_{now:yyyyMMddHHmm}";
                        
                        foreach (var item in manualItems)
                        {
                            item.DefinitionName = customDefName;
                            item.createat = now;
                            item.createdby = currentUserId;
                            _context.PayRollDefinationFile.Add(item);
                        }
                        _context.SaveChanges();

                        model.PayRollDefinationFK = manualItems.First().Id;
                    }

                    model.createat = now;
                    model.createdby = currentUserId;

                    _context.PayRollGenrate.Add(model);
                    _context.SaveChanges();

                    TempData["Success"] = "Payroll generated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "An error occurred while saving: " + ex.Message;
                    errors.Add("An error occurred while saving: " + ex.Message);
                }
            }

            if (errors.Any()) TempData["Error"] = string.Join(" ", errors);
            foreach (var err in errors) ModelState.AddModelError("", err);

            ViewBag.Employees = _context.emp.OrderBy(e => e.FirstName).ToList();
            ViewBag.Definitions = _context.PayRollDefinationFile
                .Where(d => d.DefinitionName != null && !d.DefinitionName.StartsWith("Custom_"))
                .ToList()
                .GroupBy(d => d.DefinitionName)
                .Select(g => g.First())
                .OrderBy(d => d.DefinitionName)
                .ToList();
            ViewBag.Valuesets = _context.valuesets.Where(v => v.GroupName == "PayRollCom" && v.IsActive).ToList();
            return View(model);
        }

        [RequireUrlPermission]
        public IActionResult Edit(int id)
        {
            var model = _context.PayRollGenrate.Find(id);
            if (model == null) return NotFound();

            ViewBag.Employees = _context.emp.OrderBy(e => e.FirstName).ToList();

            // Get only standard (non-custom) definitions for the dropdown
            var definitions = _context.PayRollDefinationFile
                .Where(d => d.DefinitionName != null && !d.DefinitionName.StartsWith("Custom_"))
                .ToList()
                .GroupBy(d => d.DefinitionName)
                .Select(g => g.First())
                .OrderBy(d => d.DefinitionName)
                .ToList();
            ViewBag.Definitions = definitions;
            
            var def = _context.PayRollDefinationFile.Find(model.PayRollDefinationFK);
            bool isManual = def?.DefinitionName?.StartsWith("Custom_") ?? false;
            ViewBag.IsManual = isManual;
            ViewBag.SelectedDefinitionName = def?.DefinitionName;
            ViewBag.SelectedDefinitionId = model.PayRollDefinationFK;

            if (def != null && !string.IsNullOrEmpty(def.DefinitionName))
            {
                var componentItems = _context.PayRollDefinationFile
                    .Where(d => d.DefinitionName == def.DefinitionName)
                    .ToList();

                ViewBag.BreakdownItems = componentItems;
            }

            ViewBag.Valuesets = _context.valuesets.Where(v => v.GroupName == "PayRollCom" && v.IsActive).ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireUrlPermission]
        public IActionResult Edit(PayRollGenrate model, string IsManual, List<PayRollDefinationFile> manualItems)
        {
            ModelState.Clear();
            var errors = new List<string>();

            if (model.EmployeeFK <= 0) errors.Add("Please select an Employee.");
            if (model.salery <= 0) errors.Add("Please enter a valid Gross Salary.");

            if (IsManual == "true")
            {
                if (manualItems == null || !manualItems.Any())
                {
                    errors.Add("Please add at least one manual component.");
                }
                else
                {
                    double totalPct = manualItems.Sum(i => double.TryParse(i.Percentage, out double p) ? p : 0);
                    if (Math.Abs(totalPct - 100) > 0.01) errors.Add("Total manual percentage must be exactly 100%.");
                }
            }
            else
            {
                if (model.PayRollDefinationFK <= 0) errors.Add("Please select a Payroll Definition.");
            }

            if (!errors.Any())
            {
                try
                {
                    var existing = _context.PayRollGenrate.Find(model.id);
                    if (existing == null) return NotFound();

                    var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;
                    var now = DateTime.Now;

                    string oldCustomDefName = null;
                    var oldDef = _context.PayRollDefinationFile.Find(existing.PayRollDefinationFK);
                    if (oldDef != null && oldDef.DefinitionName != null && oldDef.DefinitionName.StartsWith("Custom_"))
                    {
                        oldCustomDefName = oldDef.DefinitionName;
                    }

                    if (IsManual == "true")
                    {
                        var emp = _context.emp.Find(model.EmployeeFK);
                        string customDefName = $"Custom_{emp?.Code ?? emp?.Id.ToString()}_{now:yyyyMMddHHmm}";

                        foreach (var item in manualItems)
                        {
                            item.DefinitionName = customDefName;
                            item.createat = now;
                            item.createdby = currentUserId;
                            _context.PayRollDefinationFile.Add(item);
                        }
                        _context.SaveChanges();

                        existing.PayRollDefinationFK = manualItems.First().Id;
                    }
                    else
                    {
                        existing.PayRollDefinationFK = model.PayRollDefinationFK;
                    }

                    existing.EmployeeFK = model.EmployeeFK;
                    existing.salery = model.salery;
                    existing.updateat = now;
                    existing.updatedby = currentUserId;

                    _context.SaveChanges();

                    // Cleanup old custom definition if it changed
                    if (!string.IsNullOrEmpty(oldCustomDefName))
                    {
                        var newDef = _context.PayRollDefinationFile.Find(existing.PayRollDefinationFK);
                        if (newDef == null || newDef.DefinitionName != oldCustomDefName)
                        {
                            var oldItems = _context.PayRollDefinationFile.Where(d => d.DefinitionName == oldCustomDefName).ToList();
                            if (oldItems.Any())
                            {
                                _context.PayRollDefinationFile.RemoveRange(oldItems);
                                _context.SaveChanges();
                            }
                        }
                    }

                    TempData["Success"] = "Payroll updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "An error occurred while saving: " + ex.Message;
                    errors.Add("An error occurred while saving: " + ex.Message);
                }
            }

            if (errors.Any()) TempData["Error"] = string.Join(" ", errors);
            foreach (var err in errors) ModelState.AddModelError("", err);

            ViewBag.Employees = _context.emp.OrderBy(e => e.FirstName).ToList();
            ViewBag.Definitions = _context.PayRollDefinationFile
                .Where(d => d.DefinitionName != null && !d.DefinitionName.StartsWith("Custom_"))
                .ToList()
                .GroupBy(d => d.DefinitionName)
                .Select(g => g.First())
                .OrderBy(d => d.DefinitionName)
                .ToList();
            ViewBag.Valuesets = _context.valuesets.Where(v => v.GroupName == "PayRollCom" && v.IsActive).ToList();
            
            // Re-set view state for Edit form
            var currentDef = _context.PayRollDefinationFile.Find(model.PayRollDefinationFK);
            ViewBag.IsManual = IsManual == "true";
            ViewBag.SelectedDefinitionName = currentDef?.DefinitionName;
            ViewBag.SelectedDefinitionId = model.PayRollDefinationFK;
            
            return View(model);
        }

        [RequireUrlPermission]
        public IActionResult Delete(int id)
        {
            var model = _context.PayRollGenrate
                .Include(p => p.Employee)
                .FirstOrDefault(p => p.id == id);
            if (model == null) return NotFound();

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [RequireUrlPermission]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var model = _context.PayRollGenrate.Find(id);
            if (model != null)
            {
                // Remember custom definition name for cleanup
                var def = _context.PayRollDefinationFile.Find(model.PayRollDefinationFK);
                string customDefName = (def != null && def.DefinitionName != null && def.DefinitionName.StartsWith("Custom_"))
                    ? def.DefinitionName : null;

                // 1. Remove the PayRollGenrate FIRST (releases the FK reference)
                _context.PayRollGenrate.Remove(model);
                _context.SaveChanges();

                // 2. NOW safe to clean up orphaned custom definition items
                if (!string.IsNullOrEmpty(customDefName))
                {
                    var customItems = _context.PayRollDefinationFile
                        .Where(d => d.DefinitionName == customDefName)
                        .ToList();
                    if (customItems.Any())
                    {
                        _context.PayRollDefinationFile.RemoveRange(customItems);
                        _context.SaveChanges();
                    }
                }

                TempData["Success"] = "Payroll record deleted.";
            }
            return RedirectToAction(nameof(Index));
        }



        [HttpGet]
        [SkipPermissionCheck]
        public IActionResult GenerateBreakdown(int definitionId, int salary)
        {
            if (definitionId <= 0 || salary <= 0)
            {
                // Silently return for initial empty states to avoid disruptive alerts
                return Content(""); 
            }

            var baseItem = _context.PayRollDefinationFile.Find(definitionId);
            if (baseItem == null || string.IsNullOrEmpty(baseItem.DefinitionName))
            {
                return Content("<div class='alert alert-danger'>Definition not found.</div>");
            }

            var components = _context.PayRollDefinationFile
                .Where(d => d.DefinitionName == baseItem.DefinitionName)
                .ToList();

            if (!components.Any()) return Content("<div class='alert alert-danger'>Definition components not found.</div>");

            ViewBag.TotalSalary = salary;
            return PartialView("_PayrollBreakdown", components);
        }

        [HttpGet]
        [SkipPermissionCheck]
        public IActionResult GetDefinitionsByEmployee(int employeeId)
        {
            // Placeholder: If employees have specific definitions assigned, filter here.
            // For now, return all.
            var definitions = _context.PayRollDefinationFile
                .Select(d => d.DefinitionName)
                .Distinct()
                .OrderBy(n => n)
                .ToList();

            return PartialView("_DefinitionDropdown", definitions);
        }
    }
}


