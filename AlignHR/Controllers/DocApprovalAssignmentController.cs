using AlignHR.Data;
using AlignHR.Models;
using AlignHR.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace AlignHR.Controllers
{
    public class DocApprovalAssignmentController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public DocApprovalAssignmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        [RequireUrlPermission]
        public IActionResult Index(string search, int? page)
        {
            int pageSize = 15;
            int pageNumber = page ?? 1;

            var query = _context.DocApprovalAssignments
                .Include(a => a.User)
                .Include(a => a.ApprovalTemplate)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(a => 
                    a.User.FirstName.Contains(search) || 
                    a.User.LastName.Contains(search) || 
                    a.DmsDefinitionName.Contains(search) || 
                    a.ApprovalTemplate.TemplateName.Contains(search));
            }

            var pagedList = query.OrderBy(a => a.User.FirstName).ThenBy(a => a.DmsDefinitionName).ToPagedList(pageNumber, pageSize);
            return View(pagedList);
        }

        [RequireUrlPermission]
        public IActionResult Create()
        {
            PopulateDropdowns();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireUrlPermission]
        public IActionResult Create(DocApprovalAssignment assignment)
        {
            if (ModelState.IsValid)
            {
                // Check if user already has an active assignment for the same DMS Definition Name
                var duplicate = _context.DocApprovalAssignments
                    .Any(a => a.UserId == assignment.UserId && a.DmsDefinitionName == assignment.DmsDefinitionName && a.IsActive && assignment.IsActive);

                if (duplicate)
                {
                    TempData["ErrorMessage"] = "An active approval assignment already exists for this user and DMS definition.";
                    PopulateDropdowns(assignment);
                    return View(assignment);
                }

                var now = DateTime.Now;
                var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;

                assignment.createat = now;
                assignment.createdby = currentUserId;
                assignment.updateat = now;
                assignment.updatedby = currentUserId;

                _context.DocApprovalAssignments.Add(assignment);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Document Approval Assignment created successfully.";
                return RedirectToAction("Index");
            }

            PopulateDropdowns(assignment);
            return View(assignment);
        }

        [RequireUrlPermission]
        public IActionResult Edit(int id)
        {
            var assignment = _context.DocApprovalAssignments.Find(id);
            if (assignment == null) return NotFound();

            PopulateDropdowns(assignment);
            return View(assignment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireUrlPermission]
        public IActionResult Edit(int id, DocApprovalAssignment assignment)
        {
            if (id != assignment.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var existing = _context.DocApprovalAssignments.Find(id);
                if (existing == null) return NotFound();

                // Check for duplicates
                var duplicate = _context.DocApprovalAssignments
                    .Any(a => a.Id != id && a.UserId == assignment.UserId && a.DmsDefinitionName == assignment.DmsDefinitionName && a.IsActive && assignment.IsActive);

                if (duplicate)
                {
                    TempData["ErrorMessage"] = "An active approval assignment already exists for this user and DMS definition.";
                    PopulateDropdowns(assignment);
                    return View(assignment);
                }

                var now = DateTime.Now;
                var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;

                existing.UserId = assignment.UserId;
                existing.DmsDefinitionName = assignment.DmsDefinitionName;
                existing.ApprovalTemplateId = assignment.ApprovalTemplateId;
                existing.Notes = assignment.Notes;
                existing.IsActive = assignment.IsActive;
                existing.updateat = now;
                existing.updatedby = currentUserId;

                _context.SaveChanges();

                TempData["SuccessMessage"] = "Document Approval Assignment updated successfully.";
                return RedirectToAction("Index");
            }

            PopulateDropdowns(assignment);
            return View(assignment);
        }

        [RequireUrlPermission]
        public IActionResult Delete(int id)
        {
            var assignment = _context.DocApprovalAssignments
                .Include(a => a.User)
                .Include(a => a.ApprovalTemplate)
                .FirstOrDefault(a => a.Id == id);

            if (assignment == null) return NotFound();

            return View(assignment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [RequireUrlPermission]
        public IActionResult DeleteConfirmed(int id)
        {
            var assignment = _context.DocApprovalAssignments.Find(id);
            if (assignment != null)
            {
                _context.DocApprovalAssignments.Remove(assignment);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Document Approval Assignment deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }

        private void PopulateDropdowns(DocApprovalAssignment? selected = null)
        {
            var users = _context.users
                .Where(u => u.UsrIsActive == "Yes")
                .OrderBy(u => u.FirstName)
                .Select(u => new { Id = u.Id, Name = u.FirstName + " " + u.LastName })
                .ToList();

            var dmsDefinitions = _context.DmsDefinationFiles
                .Where(d => d.DefinitionName != null && !d.DefinitionName.StartsWith("Custom_"))
                .Select(d => d.DefinitionName)
                .Distinct()
                .OrderBy(n => n)
                .ToList();

            var templates = _context.ApprovalTemplates
                .Where(t => t.IsActive)
                .OrderBy(t => t.TemplateName)
                .ToList();

            ViewBag.Users = new SelectList(users, "Id", "Name", selected?.UserId);
            ViewBag.DmsDefinitions = new SelectList(dmsDefinitions, selected?.DmsDefinitionName);
            ViewBag.Templates = new SelectList(templates, "Id", "TemplateName", selected?.ApprovalTemplateId);
        }
    }
}
