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
    public class DocumentCategoriesController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public DocumentCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? page, string searchQuery)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            ViewData["CurrentFilter"] = searchQuery;

            var query = _context.DocumentCategories.AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(c => c.CategoryName.Contains(searchQuery) || (c.Description != null && c.Description.Contains(searchQuery)));
            }

            var categories = query.OrderByDescending(c => c.createat).ToPagedList(pageNumber, pageSize);

            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DocumentCategory category)
        {
            if (ModelState.IsValid)
            {
                category.createdby = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;
                category.createat = DateTime.Now;
                _context.Add(category);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Document category created successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var category = await _context.DocumentCategories.FindAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DocumentCategory category)
        {
            if (id != category.CategoryID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.DocumentCategories.AsNoTracking().FirstOrDefaultAsync(c => c.CategoryID == id);
                    if (existing == null) return NotFound();

                    category.createdby = existing.createdby;
                    category.createat = existing.createat;
                    category.updatedby = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;
                    category.updateat = DateTime.Now;

                    _context.Update(category);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Document category updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.DocumentCategories.Any(c => c.CategoryID == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.DocumentCategories.FindAsync(id);
            if (category == null) return RedirectToAction(nameof(Index));

            try
            {
                _context.DocumentCategories.Remove(category);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Document category deleted successfully.";
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = "Cannot delete. This category is linked with other records.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
