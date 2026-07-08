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

namespace AlignHR.Controllers
{
    public class DocumentTypesController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public DocumentTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? page, string searchQuery)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            ViewData["CurrentFilter"] = searchQuery;

            var query = _context.DocumentTypes
                .Include(dt => dt.Category)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(dt => dt.TypeName.Contains(searchQuery) || dt.Category.CategoryName.Contains(searchQuery));
            }

            var types = query.OrderByDescending(dt => dt.createat).ToPagedList(pageNumber, pageSize);

            return View(types);
        }

        public IActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(_context.DocumentCategories.Where(c => c.IsActive), "CategoryID", "CategoryName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DocumentType documentType)
        {
            if (ModelState.IsValid)
            {
                documentType.createdby = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;
                documentType.createat = DateTime.Now;
                _context.Add(documentType);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Document type created successfully.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.CategoryID = new SelectList(_context.DocumentCategories.Where(c => c.IsActive), "CategoryID", "CategoryName", documentType.CategoryID);
            return View(documentType);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var documentType = await _context.DocumentTypes.FindAsync(id);
            if (documentType == null) return NotFound();

            ViewBag.CategoryID = new SelectList(_context.DocumentCategories.Where(c => c.IsActive), "CategoryID", "CategoryName", documentType.CategoryID);
            return View(documentType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DocumentType documentType)
        {
            if (id != documentType.DocumentTypeID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.DocumentTypes.AsNoTracking().FirstOrDefaultAsync(dt => dt.DocumentTypeID == id);
                    if (existing == null) return NotFound();

                    documentType.createdby = existing.createdby;
                    documentType.createat = existing.createat;
                    documentType.updatedby = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;
                    documentType.updateat = DateTime.Now;

                    _context.Update(documentType);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Document type updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.DocumentTypes.Any(dt => dt.DocumentTypeID == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.CategoryID = new SelectList(_context.DocumentCategories.Where(c => c.IsActive), "CategoryID", "CategoryName", documentType.CategoryID);
            return View(documentType);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var documentType = await _context.DocumentTypes.FindAsync(id);
            if (documentType == null) return RedirectToAction(nameof(Index));

            try
            {
                _context.DocumentTypes.Remove(documentType);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Document type deleted successfully.";
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = "Cannot delete. This document type is linked with other records.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
