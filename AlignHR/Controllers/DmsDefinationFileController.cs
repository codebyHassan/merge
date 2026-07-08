using AlignHR.Data;
using AlignHR.Models;
using AlignHR.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace AlignHR.Controllers
{
    public class DmsDefinationFileController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public DmsDefinationFileController(ApplicationDbContext context)
        {
            _context = context;
        }

        [RequireUrlPermission]
        public IActionResult Index(string search, int? page)
        {
            int pageSize = 15;
            int pageNumber = page ?? 1;

            var obj = _context.DmsDefinationFiles
                .Where(d => d.DefinitionName != null && !d.DefinitionName.StartsWith("Custom_"))
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                obj = obj.Where(d => d.DefinitionName.Contains(search));
            }

            var distinctNames = obj.Select(d => d.DefinitionName).Distinct().OrderBy(n => n);
            var pagedNames = distinctNames.ToPagedList(pageNumber, pageSize);

            var modelList = new List<DmsDefinationFile>();
            foreach (var name in pagedNames)
            {
                var latestDate = _context.DmsDefinationFiles
                                         .Where(p => p.DefinitionName == name)
                                         .Max(p => p.createat);

                modelList.Add(new DmsDefinationFile
                {
                    DefinitionName = name,
                    createat = latestDate
                });
            }

            var model = new StaticPagedList<DmsDefinationFile>(modelList, pagedNames.PageNumber, pagedNames.PageSize, pagedNames.TotalItemCount);

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

            var query = _context.DmsDefinationFiles
                .Where(d => d.DefinitionName != null && !d.DefinitionName.StartsWith("Custom_"))
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(d => d.DefinitionName.Contains(search));
            }

            var distinctNames = query.Select(d => d.DefinitionName).Distinct().OrderBy(n => n).ToList();

            var list = new List<DmsDefinationFile>();
            foreach (var name in distinctNames)
            {
                var latestDate = _context.DmsDefinationFiles
                                         .Where(p => p.DefinitionName == name)
                                         .Max(p => p.createat);

                list.Add(new DmsDefinationFile
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
                var worksheet = package.Workbook.Worksheets.Add("DmsStructures");

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
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "dms_structures.xlsx");
            }
        }

        [RequireUrlPermission]
        public IActionResult create()
        {
            ViewBag.Categories = _context.DocumentCategories.Where(c => c.IsActive).OrderBy(c => c.CategoryName).ToList();
            ViewBag.DocumentTypes = _context.DocumentTypes.Where(t => t.IsActive).OrderBy(t => t.TypeName).ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireUrlPermission]
        public IActionResult create(string DefinitionName, List<DmsDefinationFile> items)
        {
            if (string.IsNullOrWhiteSpace(DefinitionName))
            {
                TempData["ErrorMessage"] = "Definition Name is required.";
                RepopulateDmsViewBag();
                return View(items ?? new List<DmsDefinationFile>());
            }

            if (items == null || items.Count == 0)
            {
                TempData["ErrorMessage"] = "No items provided.";
                RepopulateDmsViewBag();
                return View(items ?? new List<DmsDefinationFile>());
            }

            double totalPercentage = 0;
            foreach (var item in items)
            {
                if (double.TryParse(item.Percentage, out double pct))
                    totalPercentage += pct;
            }

            if (Math.Abs(totalPercentage - 100) > 0.01)
            {
                TempData["ErrorMessage"] = $"Total percentage must be exactly 100%. Current total is {totalPercentage}%.";
                ViewBag.DefinitionName = DefinitionName;
                RepopulateDmsViewBag();
                return View(items);
            }

            var now = DateTime.Now;
            var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;

            foreach (var item in items)
            {
                item.DefinitionName    = DefinitionName;
                item.ApprovalTemplateId = ResolveApprovalTemplateId(item.DocumentTypeID);
                item.createat          = now;
                item.createdby         = currentUserId;
                _context.DmsDefinationFiles.Add(item);
            }

            _context.SaveChanges();
            TempData["SuccessMessage"] = "DMS definitions created successfully.";
            return RedirectToAction("Index");
        }

        [RequireUrlPermission]
        public IActionResult Edit(string name)
        {
            if (string.IsNullOrEmpty(name)) return NotFound();

            var items = _context.DmsDefinationFiles.Where(p => p.DefinitionName == name).ToList();
            if (items.Count == 0) return NotFound();

            ViewBag.DefinitionName      = name;
            RepopulateDmsViewBag();
            return View(items);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireUrlPermission]
        public IActionResult Edit(string oldName, string DefinitionName, List<DmsDefinationFile> items)
        {
            if (string.IsNullOrWhiteSpace(DefinitionName))
            {
                TempData["ErrorMessage"] = "Definition Name is required.";
                ViewBag.DefinitionName = oldName;
                RepopulateDmsViewBag();
                return View(items ?? new List<DmsDefinationFile>());
            }

            if (items == null || items.Count == 0)
            {
                TempData["ErrorMessage"] = "No items provided.";
                ViewBag.DefinitionName = DefinitionName;
                RepopulateDmsViewBag();
                return View(items ?? new List<DmsDefinationFile>());
            }

            double totalPercentage = 0;
            foreach (var item in items)
            {
                if (double.TryParse(item.Percentage, out double pct))
                    totalPercentage += pct;
            }

            if (Math.Abs(totalPercentage - 100) > 0.01)
            {
                TempData["ErrorMessage"] = $"Total percentage must be exactly 100%. Current total is {totalPercentage}%.";
                ViewBag.DefinitionName = DefinitionName;
                RepopulateDmsViewBag();
                return View(items);
            }

            var existingItems  = _context.DmsDefinationFiles.Where(p => p.DefinitionName == oldName).ToList();
            _context.DmsDefinationFiles.RemoveRange(existingItems);

            var now           = DateTime.Now;
            var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;

            foreach (var item in items)
            {
                item.DefinitionName      = DefinitionName;
                item.ApprovalTemplateId  = ResolveApprovalTemplateId(item.DocumentTypeID);
                item.createat  = existingItems.FirstOrDefault()?.createat ?? now;
                item.createdby = existingItems.FirstOrDefault()?.createdby ?? currentUserId;
                item.updatedby = currentUserId;
                item.updateat  = now;
                _context.DmsDefinationFiles.Add(item);
            }

            _context.SaveChanges();
            TempData["SuccessMessage"] = "DMS definition updated successfully.";
            return RedirectToAction("Index");
        }

        [RequireUrlPermission]
        public IActionResult Delete(string name)
        {
            if (string.IsNullOrEmpty(name)) return NotFound();

            var item = _context.DmsDefinationFiles.FirstOrDefault(d => d.DefinitionName == name);
            if (item == null) return NotFound();

            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string DefinitionName)
        {
            var items = _context.DmsDefinationFiles.Where(p => p.DefinitionName == DefinitionName).ToList();
            if (items.Any())
            {
                _context.DmsDefinationFiles.RemoveRange(items);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult GetDocumentTypesByCategory(int categoryId)
        {
            var types = _context.DocumentTypes
                .Where(dt => dt.CategoryID == categoryId && dt.IsActive)
                .Select(dt => new { id = dt.DocumentTypeID, name = dt.TypeName })
                .ToList();

            return Json(types);
        }

        private void RepopulateDmsViewBag()
        {
            ViewBag.Categories       = _context.DocumentCategories.Where(c => c.IsActive).OrderBy(c => c.CategoryName).ToList();
            ViewBag.DocumentTypes    = _context.DocumentTypes.Where(t => t.IsActive).OrderBy(t => t.TypeName).ToList();
        }

        private int? ResolveApprovalTemplateId(int documentTypeId)
        {
            return _context.ApprovalTemplates
                .Where(t => t.IsActive && t.TypeId == documentTypeId)
                .OrderByDescending(t => t.Version)
                .ThenByDescending(t => t.Id)
                .Select(t => (int?)t.Id)
                .FirstOrDefault();
        }
    }
}
