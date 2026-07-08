using AlignHR.Data;
using AlignHR.Models;
using AlignHR.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace AlignHR.Controllers
{
    public class PayrollDefinationFileController : BaseController
    {
        private readonly ApplicationDbContext _context;
        public PayrollDefinationFileController(ApplicationDbContext context)
        {
            _context = context;
        }

        [RequireUrlPermission]
        public IActionResult Index(string search, int? page)
        {
            int pageSize = 15;
            int pageNumber = page ?? 1;

            var obj = _context.PayRollDefinationFile
                .Where(d => d.DefinitionName != null && !d.DefinitionName.StartsWith("Custom_"))
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                obj = obj.Where(d => d.DefinitionName.Contains(search));
            }

            // Provide a reliable EF Core translation by grabbing Distinct string values first
            var distinctNames = obj.Select(d => d.DefinitionName).Distinct().OrderBy(n => n);
            var pagedNames = distinctNames.ToPagedList(pageNumber, pageSize);

            // Construct display entities in-memory
            var modelList = new List<PayRollDefinationFile>();
            foreach (var name in pagedNames)
            {
                var latestDate = _context.PayRollDefinationFile
                                         .Where(p => p.DefinitionName == name)
                                         .Max(p => p.createat);

                modelList.Add(new PayRollDefinationFile
                {
                    DefinitionName = name,
                    createat = latestDate
                });
            }

            var model = new StaticPagedList<PayRollDefinationFile>(modelList, pagedNames.PageNumber, pagedNames.PageSize, pagedNames.TotalItemCount);

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

            var query = _context.PayRollDefinationFile
                .Where(d => d.DefinitionName != null && !d.DefinitionName.StartsWith("Custom_"))
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(d => d.DefinitionName.Contains(search));
            }

            var distinctNames = query.Select(d => d.DefinitionName).Distinct().OrderBy(n => n).ToList();

            var list = new List<PayRollDefinationFile>();
            foreach (var name in distinctNames)
            {
                var latestDate = _context.PayRollDefinationFile
                                         .Where(p => p.DefinitionName == name)
                                         .Max(p => p.createat);

                list.Add(new PayRollDefinationFile
                {
                    DefinitionName = name,
                    createat = latestDate
                });
            }

            var selectedCols = !string.IsNullOrEmpty(columns)
                ? columns.Split(',').Select(c => c.Trim().ToLowerInvariant()).ToList()
                : new List<string> { "definitionname", "createdat" };

            var headersList = new List<string>();
            if (selectedCols.Contains("definitionname")) headersList.Add("Definition Name");
            if (selectedCols.Contains("createdat")) headersList.Add("Created At");

            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (var package = new OfficeOpenXml.ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("SalaryStructures");

                for (int col = 0; col < headersList.Count; col++)
                {
                    worksheet.Cells[1, col + 1].Value = headersList[col];
                    worksheet.Cells[1, col + 1].Style.Font.Bold = true;
                }

                int rowIdx = 2;
                foreach (var item in list)
                {
                    int colIdx = 1;
                    if (selectedCols.Contains("definitionname")) worksheet.Cells[rowIdx, colIdx++].Value = item.DefinitionName;
                    if (selectedCols.Contains("createdat")) worksheet.Cells[rowIdx, colIdx++].Value = item.createat.ToString("yyyy-MM-dd");
                    rowIdx++;
                }

                worksheet.Cells.AutoFitColumns();

                var fileBytes = package.GetAsByteArray();
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "salary_structures.xlsx");
            }
        }

        [RequireUrlPermission]
        public IActionResult create()
        {
            ViewBag.Valuesets = _context.valuesets.Where(v => v.GroupName == "PayRollCom" && v.IsActive).ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireUrlPermission]
        public IActionResult create(string DefinitionName, List<PayRollDefinationFile> items)
        {
            if (string.IsNullOrWhiteSpace(DefinitionName))
            {
                TempData["ErrorMessage"] = "Definition Name is required.";
                ViewBag.Valuesets = _context.valuesets.Where(v => v.GroupName == "PayRollCom" && v.IsActive).ToList();
                return View(items ?? new List<PayRollDefinationFile>());
            }

            if (items == null || items.Count == 0)
            {
                TempData["ErrorMessage"] = "No items provided.";
                ViewBag.Valuesets = _context.valuesets.Where(v => v.GroupName == "PayRollCom" && v.IsActive).ToList();
                return View(items ?? new List<PayRollDefinationFile>());
            }

            double totalPercentage = 0;
            foreach (var item in items)
            {
                if (double.TryParse(item.Percentage, out double pct))
                {
                    totalPercentage += pct;
                }
            }

            if (Math.Abs(totalPercentage - 100) > 0.01)
            {
                TempData["ErrorMessage"] = "Total percentage must be exactly 100%. Current total is " + totalPercentage + "%.";
                ViewBag.Valuesets = _context.valuesets.Where(v => v.GroupName == "PayRollCom" && v.IsActive).ToList();
                ViewBag.DefinitionName = DefinitionName;
                return View(items);
            }

            var now = DateTime.Now;
            var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;

            foreach (var item in items)
            {
                item.DefinitionName = DefinitionName;
                item.createat = now;
                item.createdby = currentUserId;
                _context.PayRollDefinationFile.Add(item);
            }

            _context.SaveChanges();
            TempData["SuccessMessage"] = "Payroll definitions created successfully.";
            return RedirectToAction("Index");
        }

        [RequireUrlPermission]
        public IActionResult Edit(string name)
        {
            // Edit by DefinitionName instead of ID
            if (string.IsNullOrEmpty(name)) return NotFound();
            
            var items = _context.PayRollDefinationFile.Where(p => p.DefinitionName == name).ToList();
            if (items.Count == 0) return NotFound();
            
            ViewBag.DefinitionName = name;
            ViewBag.Valuesets = _context.valuesets.Where(v => v.GroupName == "PayRollCom" && v.IsActive).ToList();
            return View(items);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireUrlPermission]
        public IActionResult Edit(string oldName, string DefinitionName, List<PayRollDefinationFile> items)
        {
            if (string.IsNullOrWhiteSpace(DefinitionName))
            {
                TempData["ErrorMessage"] = "Definition Name is required.";
                ViewBag.DefinitionName = oldName;
                ViewBag.Valuesets = _context.valuesets.Where(v => v.GroupName == "PayRollCom" && v.IsActive).ToList();
                return View(items ?? new List<PayRollDefinationFile>());
            }

            if (items == null || items.Count == 0)
            {
                TempData["ErrorMessage"] = "No items provided.";
                ViewBag.DefinitionName = DefinitionName;
                ViewBag.Valuesets = _context.valuesets.Where(v => v.GroupName == "PayRollCom" && v.IsActive).ToList();
                return View(items ?? new List<PayRollDefinationFile>());
            }

            double totalPercentage = 0;
            foreach (var item in items)
            {
                if (double.TryParse(item.Percentage, out double pct))
                {
                    totalPercentage += pct;
                }
            }

            if (Math.Abs(totalPercentage - 100) > 0.01)
            {
                TempData["ErrorMessage"] = "Total percentage must be exactly 100%. Current total is " + totalPercentage + "%.";
                ViewBag.DefinitionName = DefinitionName;
                ViewBag.Valuesets = _context.valuesets.Where(v => v.GroupName == "PayRollCom" && v.IsActive).ToList();
                return View(items);
            }

            if (ModelState.IsValid)
            {
                var existingItems = _context.PayRollDefinationFile.Where(p => p.DefinitionName == oldName).ToList();
                _context.PayRollDefinationFile.RemoveRange(existingItems);
                
                var now = DateTime.Now;
                var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;

                foreach (var item in items)
                {
                    item.DefinitionName = DefinitionName;
                    item.createat = existingItems.FirstOrDefault()?.createat ?? now;
                    item.createdby = existingItems.FirstOrDefault()?.createdby ?? currentUserId;
                    item.updatedby = currentUserId;
                    item.updateat = now;
                    _context.PayRollDefinationFile.Add(item);
                }
                
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Payroll definition updated successfully.";
                return RedirectToAction("Index");
            }

            ViewBag.DefinitionName = DefinitionName;
            ViewBag.Valuesets = _context.valuesets.Where(v => v.GroupName == "PayRollCom" && v.IsActive).ToList();
            return View(items);
        }

        [RequireUrlPermission]
        public IActionResult Delete(string name)
        {
            if (string.IsNullOrEmpty(name)) return NotFound();

            var item = _context.PayRollDefinationFile.FirstOrDefault(d => d.DefinitionName == name);
            if (item == null) return NotFound();

            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string DefinitionName)
        {
            var items = _context.PayRollDefinationFile.Where(p => p.DefinitionName == DefinitionName).ToList();
            if (items.Any())
            {
                _context.PayRollDefinationFile.RemoveRange(items);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}


