using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AlignHR.Data;
using AlignHR.Models;
using X.PagedList;
using AlignHR.Security;
using AlignHR.Services;

namespace AlignHR.Controllers
{
    public class DocumentsController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IWorkflowEngine _workflowEngine;

        public DocumentsController(ApplicationDbContext context, IWebHostEnvironment env, IWorkflowEngine workflowEngine)
        {
            _context = context;
            _env = env;
            _workflowEngine = workflowEngine;
        }

        public async Task<IActionResult> Index(int? page, string searchQuery)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            ViewData["CurrentFilter"] = searchQuery;

            var query = _context.Documents
                .Include(d => d.Employee)
                .Include(d => d.DocumentType)
                .Include(d => d.Category)
                .Include(d => d.UploadedByUser)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(d =>
                    d.DocumentNo.Contains(searchQuery) ||
                    d.Title.Contains(searchQuery) ||
                    d.Employee.FirstName.Contains(searchQuery) ||
                    d.Employee.LastName.Contains(searchQuery));
            }

            var documents = query.OrderByDescending(d => d.UploadDate).ToPagedList(pageNumber, pageSize);

            return View(documents);
        }

        public IActionResult Create()
        {
            ViewBag.EmployeeID = new SelectList(_context.emp.OrderBy(e => e.FirstName), "Id", "FullName");
            ViewBag.CategoryID = new SelectList(_context.DocumentCategories.Where(c => c.IsActive), "CategoryID", "CategoryName");
            ViewBag.DocumentTypeID = new SelectList(Enumerable.Empty<SelectListItem>(), "Value", "Text");
            ViewBag.StatusList = new SelectList(new[]
            {
                new { Value = "Draft", Text = "Draft" },
                new { Value = "Active", Text = "Active" },
                new { Value = "Archived", Text = "Archived" },
                new { Value = "Expired", Text = "Expired" }
            }, "Value", "Text");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Document document, IFormFile? file)
        {
            ModelState.Remove(nameof(document.FileName));
            ModelState.Remove(nameof(document.FilePath));
            ModelState.Remove(nameof(document.UploadedBy));

            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("file", "Please upload a file.");
            }

            if (await _context.Documents.AnyAsync(d => d.DocumentNo == document.DocumentNo))
            {
                ModelState.AddModelError("DocumentNo", "Document Number must be unique.");
            }

            if (ModelState.IsValid)
            {
                if (file != null && file.Length > 0)
                {
                    var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", "documents");
                    Directory.CreateDirectory(uploadsDir);

                    var fileName = $"{Guid.NewGuid():N}_{file.FileName}";
                    var filePath = Path.Combine(uploadsDir, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    document.FileName = file.FileName;
                    document.FilePath = $"/uploads/documents/{fileName}";
                    document.FileExtension = Path.GetExtension(file.FileName);
                    document.FileSize = file.Length;
                    document.MimeType = file.ContentType;
                }

                document.UploadedBy = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;
                document.UploadDate = DateTime.Now;
                document.Status     = "Draft";

                _context.Add(document);
                await _context.SaveChangesAsync();

                // Auto-trigger approval workflow if the document type requires it
                try
                {
                    var docType = await _context.DocumentTypes.FindAsync(document.DocumentTypeID);

                    if (docType?.RequiresApproval == true)
                    {
                        var uploaderUser = await _context.users.FindAsync(document.UploadedBy);
                        ApprovalTemplate? workflow = null;
                        if (uploaderUser?.DmsDefinationFK != null)
                        {
                            var dmsDef = await _context.DmsDefinationFiles
                                .FirstOrDefaultAsync(d => d.Id == uploaderUser.DmsDefinationFK && d.ApprovalTemplateId != null);
                            if (dmsDef?.ApprovalTemplateId != null)
                            {
                                workflow = await _context.ApprovalTemplates
                                    .Include(t => t.Steps)
                                    .FirstOrDefaultAsync(t => t.Id == dmsDef.ApprovalTemplateId && t.IsActive);
                            }
                        }

                        if (workflow != null && workflow.Steps.Any())
                        {
                            await _workflowEngine.StartWorkflowAsync(document, workflow);
                            TempData["Success"] = "Document uploaded and submitted for approval.";
                        }
                        else
                        {
                            TempData["Success"] = "Document uploaded. No active workflow configured for this user's definition.";
                        }
                    }
                    else
                    {
                        document.Status = "Active";
                        await _context.SaveChangesAsync();
                        TempData["Success"] = "Document uploaded successfully.";
                    }
                }
                catch (Exception ex)
                {
                    TempData["Warning"] = "Document saved but workflow could not be triggered: " + ex.Message;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewBag.EmployeeID = new SelectList(_context.emp.OrderBy(e => e.FirstName), "Id", "FullName", document.EmployeeID);
            ViewBag.CategoryID = new SelectList(_context.DocumentCategories.Where(c => c.IsActive), "CategoryID", "CategoryName", document.CategoryID);
            ViewBag.DocumentTypeID = new SelectList(_context.DocumentTypes.Where(dt => dt.CategoryID == document.CategoryID), "DocumentTypeID", "TypeName", document.DocumentTypeID);
            ViewBag.StatusList = new SelectList(new[]
            {
                new { Value = "Draft", Text = "Draft" },
                new { Value = "Active", Text = "Active" },
                new { Value = "Archived", Text = "Archived" },
                new { Value = "Expired", Text = "Expired" }
            }, "Value", "Text", document.Status);
            return View(document);
        }

        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null) return NotFound();

            var document = await _context.Documents
                .Include(d => d.Employee)
                .FirstOrDefaultAsync(d => d.DocumentID == id);
            if (document == null) return NotFound();

            ViewBag.EmployeeID = new SelectList(_context.emp.OrderBy(e => e.FirstName), "Id", "FullName", document.EmployeeID);
            ViewBag.CategoryID = new SelectList(_context.DocumentCategories.Where(c => c.IsActive), "CategoryID", "CategoryName", document.CategoryID);
            ViewBag.DocumentTypeID = new SelectList(_context.DocumentTypes.Where(dt => dt.CategoryID == document.CategoryID), "DocumentTypeID", "TypeName", document.DocumentTypeID);
            ViewBag.StatusList = new SelectList(new[]
            {
                new { Value = "Draft", Text = "Draft" },
                new { Value = "Active", Text = "Active" },
                new { Value = "Archived", Text = "Archived" },
                new { Value = "Expired", Text = "Expired" }
            }, "Value", "Text", document.Status);
            return View(document);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, Document document, IFormFile? file)
        {
            if (id != document.DocumentID) return NotFound();

            ModelState.Remove(nameof(document.FileName));
            ModelState.Remove(nameof(document.FilePath));
            ModelState.Remove(nameof(document.UploadedBy));

            if (await _context.Documents.AnyAsync(d => d.DocumentNo == document.DocumentNo && d.DocumentID != id))
            {
                ModelState.AddModelError("DocumentNo", "Document Number must be unique.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.Documents.AsNoTracking().FirstOrDefaultAsync(d => d.DocumentID == id);
                    if (existing == null) return NotFound();

                    if (file != null && file.Length > 0)
                    {
                        var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", "documents");
                        Directory.CreateDirectory(uploadsDir);

                        var fileName = $"{Guid.NewGuid():N}_{file.FileName}";
                        var filePath = Path.Combine(uploadsDir, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        document.FileName = file.FileName;
                        document.FilePath = $"/uploads/documents/{fileName}";
                        document.FileExtension = Path.GetExtension(file.FileName);
                        document.FileSize = file.Length;
                        document.MimeType = file.ContentType;
                    }
                    else
                    {
                        document.FileName = existing.FileName;
                        document.FilePath = existing.FilePath;
                        document.FileExtension = existing.FileExtension;
                        document.FileSize = existing.FileSize;
                        document.MimeType = existing.MimeType;
                    }

                    document.UploadedBy = existing.UploadedBy;
                    document.UploadDate = existing.UploadDate;

                    _context.Update(document);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Document updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Documents.Any(d => d.DocumentID == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.EmployeeID = new SelectList(_context.emp.OrderBy(e => e.FirstName), "Id", "FullName", document.EmployeeID);
            ViewBag.CategoryID = new SelectList(_context.DocumentCategories.Where(c => c.IsActive), "CategoryID", "CategoryName", document.CategoryID);
            ViewBag.DocumentTypeID = new SelectList(_context.DocumentTypes.Where(dt => dt.CategoryID == document.CategoryID), "DocumentTypeID", "TypeName", document.DocumentTypeID);
            ViewBag.StatusList = new SelectList(new[]
            {
                new { Value = "Draft", Text = "Draft" },
                new { Value = "Active", Text = "Active" },
                new { Value = "Archived", Text = "Archived" },
                new { Value = "Expired", Text = "Expired" }
            }, "Value", "Text", document.Status);
            return View(document);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var document = await _context.Documents.FindAsync(id);
            if (document == null) return RedirectToAction(nameof(Index));

            try
            {
                _context.Documents.Remove(document);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Document deleted successfully.";
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = "Cannot delete. This document is linked with other records.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public JsonResult GetDocumentTypesByCategory(int categoryId)
        {
            var types = _context.DocumentTypes
                .Where(dt => dt.CategoryID == categoryId && dt.IsActive)
                .Select(dt => new { dt.DocumentTypeID, dt.TypeName })
                .ToList();

            return Json(types);
        }
    }
}
