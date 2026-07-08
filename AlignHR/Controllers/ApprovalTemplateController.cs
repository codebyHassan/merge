using AlignHR.Data;
using AlignHR.Models;
using AlignHR.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace AlignHR.Controllers
{
    public class ApprovalTemplateController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public ApprovalTemplateController(ApplicationDbContext context)
        {
            _context = context;
        }

        [RequireUrlPermission]
        public IActionResult Index(string search, int? page)
        {
            int pageSize   = 15;
            int pageNumber = page ?? 1;

            var query = _context.ApprovalTemplates
                .Include(t => t.Steps)
                .Include(t => t.DocumentType)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(t => t.TemplateName.Contains(search) ||
                                        (t.Description != null && t.Description.Contains(search)));

            var pagedList = query.OrderBy(t => t.TemplateName).ToPagedList(pageNumber, pageSize);
            return View(pagedList);
        }

        [RequireUrlPermission]
        public IActionResult Create()
        {
            ViewBag.Roles        = _context.roles.OrderBy(r => r.Name).ToList();
            ViewBag.DocumentTypes = new SelectList(
                _context.DocumentTypes.Where(dt => dt.IsActive).OrderBy(dt => dt.TypeName),
                "DocumentTypeID", "TypeName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireUrlPermission]
        public IActionResult Create(ApprovalTemplate template, List<ApprovalTemplateStep> items)
        {
            if (string.IsNullOrWhiteSpace(template.TemplateName))
            {
                TempData["ErrorMessage"] = "Template Name is required.";
                RepopulateViewBag();
                return View(template);
            }

            if (items == null || items.Count == 0)
            {
                TempData["ErrorMessage"] = "At least one approval step is required.";
                RepopulateViewBag();
                return View(template);
            }

            var now           = DateTime.Now;
            var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;

            template.createat   = now;
            template.createdby  = currentUserId;
            template.updateat   = now;
            template.updatedby  = currentUserId;
            template.Version    = 1;

            for (int i = 0; i < items.Count; i++)
            {
                var step = items[i];
                step.StepNo    = i + 1;
                step.createdby = currentUserId;
                step.createat  = now;
                template.Steps.Add(step);
            }

            _context.ApprovalTemplates.Add(template);
            _context.SaveChanges();
            SyncDmsDefinitionsForTemplate(template.Id, template.TypeId);

            TempData["SuccessMessage"] = "Approval Template created successfully.";
            return RedirectToAction("Index");
        }

        [RequireUrlPermission]
        public IActionResult Edit(int id)
        {
            var template = _context.ApprovalTemplates
                .Include(t => t.Steps.OrderBy(s => s.StepNo))
                    .ThenInclude(s => s.Role)
                .FirstOrDefault(t => t.Id == id);

            if (template == null) return NotFound();

            RepopulateViewBag();
            return View(template);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireUrlPermission]
        public IActionResult Edit(int id, ApprovalTemplate template, List<ApprovalTemplateStep> items)
        {
            if (string.IsNullOrWhiteSpace(template.TemplateName))
            {
                TempData["ErrorMessage"] = "Template Name is required.";
                RepopulateViewBag();
                return View(template);
            }

            if (items == null || items.Count == 0)
            {
                TempData["ErrorMessage"] = "At least one approval step is required.";
                RepopulateViewBag();
                return View(template);
            }

            var existing = _context.ApprovalTemplates
                .Include(t => t.Steps)
                .FirstOrDefault(t => t.Id == id);

            if (existing == null) return NotFound();

            var now           = DateTime.Now;
            var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;
            var previousTypeId = existing.TypeId;

            existing.TemplateName = template.TemplateName;
            existing.Description  = template.Description;
            existing.IsActive     = template.IsActive;
            existing.TypeId       = template.TypeId;
            existing.FlowType     = template.FlowType;
            existing.Version++;
            existing.updateat     = now;
            existing.updatedby    = currentUserId;

            // Replace all steps
            _context.ApprovalTemplateSteps.RemoveRange(existing.Steps);
            existing.Steps.Clear();

            for (int i = 0; i < items.Count; i++)
            {
                var step = items[i];
                step.TemplateId = id;
                step.StepNo     = i + 1;
                step.createdby  = currentUserId;
                step.createat   = now;
                _context.ApprovalTemplateSteps.Add(step);
            }

            _context.SaveChanges();
            SyncDmsDefinitionsForTemplate(existing.Id, existing.TypeId, previousTypeId: previousTypeId);

            TempData["SuccessMessage"] = "Approval Template updated successfully.";
            return RedirectToAction("Index");
        }

        [RequireUrlPermission]
        public IActionResult Delete(int id)
        {
            var template = _context.ApprovalTemplates
                .Include(t => t.Steps)
                .Include(t => t.DocumentType)
                .FirstOrDefault(t => t.Id == id);

            if (template == null) return NotFound();
            return View(template);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [RequireUrlPermission]
        public IActionResult DeleteConfirmed(int id)
        {
            var template = _context.ApprovalTemplates.FirstOrDefault(t => t.Id == id);
            if (template != null)
            {
                var templateId = template.Id;
                var templateTypeId = template.TypeId;
                _context.ApprovalTemplates.Remove(template);
                _context.SaveChanges();
                SyncDmsDefinitionsForTemplate(templateId, templateTypeId, removeOnly: true);
                TempData["SuccessMessage"] = "Approval Template deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }

        private void SyncDmsDefinitionsForTemplate(int templateId, int templateTypeId, int? previousTypeId = null, bool removeOnly = false)
        {
            var affectedTypes = new HashSet<int> { templateTypeId };
            if (previousTypeId.HasValue)
                affectedTypes.Add(previousTypeId.Value);

            var definitions = _context.DmsDefinationFiles
                .Where(d => affectedTypes.Contains(d.DocumentTypeID))
                .ToList();

            foreach (var def in definitions)
            {
                if (removeOnly)
                {
                    if (def.ApprovalTemplateId == templateId)
                        def.ApprovalTemplateId = null;
                    continue;
                }

                if (def.DocumentTypeID == templateTypeId)
                {
                    def.ApprovalTemplateId = templateId;
                }
                else if (previousTypeId.HasValue && def.ApprovalTemplateId == templateId)
                {
                    def.ApprovalTemplateId = null;
                }
            }

            _context.SaveChanges();
        }

        private void RepopulateViewBag()
        {
            ViewBag.Roles = _context.roles.OrderBy(r => r.Name).ToList();
            ViewBag.DocumentTypes = new SelectList(
                _context.DocumentTypes.Where(dt => dt.IsActive).OrderBy(dt => dt.TypeName),
                "DocumentTypeID", "TypeName");
        }
    }
}
