using AlignHR.Data;
using AlignHR.Helpers;
using AlignHR.Models;
using AlignHR.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AlignHR.Services;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace AlignHR.Controllers
{
    public class UserController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: User
        public async Task<IActionResult> Index()
        {
            var users = await _context.users.AsNoTracking().ToListAsync();

            // Load role assignments separately to avoid correlated subquery translation issues
            // and anonymous-type dynamic binding problems in Razor views.
            var roleRows = await _context.UserRoles
                .AsNoTracking()
                .Join(_context.roles.AsNoTracking(),
                      ur => ur.RoleId,
                      r  => r.Id,
                      (ur, r) => new { ur.UserId, RoleName = r.Name ?? "No Role" })
                .ToListAsync();

            var roleMap = roleRows
                .GroupBy(x => x.UserId)
                .ToDictionary(g => g.Key, g => g.First().RoleName);

            ViewBag.UserRoles = roleMap;
            return View(users);
        }

        // ─── LOGIN ───────────────────────────────────────────────────────────

        // GET: User/Login
        [SkipPermissionCheck]
        public IActionResult Login()
        {
            if (PermissionHelper.IsLoggedIn(HttpContext))
                return RedirectToAction("Index", "Dashboard");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SkipPermissionCheck]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Email and password are required.");
                return View();
            }

            // Attempt to find user with linked Employee
            var user = await _context.users
                .Include(u => u.Employee)
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

            if (user != null)
            {
                // Verify password using BCrypt
                if (!PasswordHelper.VerifyPassword(password, user.Password))
                {
                    ModelState.AddModelError("", "Invalid email or password. Please try again.");
                    return View();
                }


                // Verify status
                if (user.UsrIsActive?.Trim().ToLower() != "yes")
                {
                    ModelState.AddModelError("", "Your account is currently inactive. Please contact support.");
                    return View();
                }

                // Load roles for this user
                var roleIds = await _context.UserRoles
                    .Where(ur => ur.UserId == user.Id)
                    .Select(ur => ur.RoleId)
                    .ToListAsync();

                var roleNames = await _context.roles
                    .Where(r => roleIds.Contains(r.Id))
                    .Select(r => r.Name ?? "")
                    .ToListAsync();

                // Load allowed functions for this user's roles in 2 steps to avoid complex SQL generation (CTE/WITH)
                var functionIds = await _context.FunctionRoles
                    .Where(fr => roleIds.Contains(fr.RoleId))
                    .Select(fr => fr.FunctionId)
                    .Distinct()
                    .ToListAsync();

                var allowedFunctions = await _context.functions
                    .Where(f => functionIds.Contains(f.Id))
                    .Select(f => new
                    {
                        f.Name,
                        f.route
                    })
                    .ToListAsync();

                var functionNames = allowedFunctions
                    .Select(f => f.Name ?? "")
                    .ToList();

                var allowedUrls = allowedFunctions
                    .Where(f => !string.IsNullOrEmpty(f.route))
                    .Select(f => f.route!)
                    .ToList();

                // Build menu items from allowed functions
                var menuItems = allowedFunctions
                    .Where(f => !string.IsNullOrEmpty(f.route))
                    .Select(f => new MenuItem
                    {
                        Title = f.Name ?? "",
                        Url = f.route ?? "",
                        FunctionName = f.Name ?? "",
                        Icon = "fas fa-circle"
                    })
                    .ToList();

                // Create and store the session
                var session = new UserSession
                {
                    UserId = user.Id,
                    Username = user.Email ?? "",
                    FullName = (user.FirstName ?? "") + " " + (user.LastName ?? ""),
                    Roles = roleNames,
                    Functions = functionNames,
                    AllowedUrls = allowedUrls,
                    MenuItems = menuItems,
                    EmployeeId = user.Employee?.Id,
                    DepartmentId = user.Employee?.DepartmentFk,
                    CanApplyForOthers = user.Employee?.CanApplyForOthers ?? false
                };

                PermissionHelper.SetUserSession(HttpContext, session);

                return RedirectToAction("Index", "Dashboard");
            }

            ModelState.AddModelError("", "Invalid email or password. Please try again.");
            return View();
        }

        // GET/POST: Logout
        [SkipPermissionCheck]
        public IActionResult Logout()
        {
            PermissionHelper.ClearSession(HttpContext);
            return RedirectToAction("Login");
        }

        // GET: User/AccessDenied
        [SkipPermissionCheck]
        public IActionResult AccessDenied()
        {
            return View();
        }

        // ─── CRUD ─────────────────────────────────────────────────────────────

        // GET: User/Create
        public IActionResult Create()
        {
            ViewBag.Roles = new SelectList(_context.roles.ToList(), "Id", "Name");
            ViewBag.Employees = new SelectList(_context.emp.Select(e => new { e.Id, FullName = e.FirstName + " " + e.LastName }).ToList(), "Id", "FullName");
            var dmsList = _context.DmsDefinationFiles
                .Where(d => d.DefinitionName != null && !d.DefinitionName.StartsWith("Custom_"))
                .GroupBy(d => d.DefinitionName)
                .Select(g => new { Id = g.First().Id, DefinitionName = g.Key })
                .ToList();
            ViewBag.DmsDefinitions = new SelectList(dmsList, "Id", "DefinitionName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user, int? RoleId)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;
                user.createdby = currentUserId;
                user.createat = DateTime.Now;
                user.updatedby = currentUserId;
                user.updateat = DateTime.Now;
                user.UsrIsActive = "Yes";

                // Hash password
                user.Password = PasswordHelper.HashPassword(user.Password);

                _context.Add(user);
                await _context.SaveChangesAsync();

                // Assign role if selected
                if (RoleId.HasValue && RoleId.Value > 0)
                {
                    var userRole = new UserRole
                    {
                        UserId = user.Id,
                        RoleId = RoleId.Value,
                        createdby = currentUserId,
                        createat = DateTime.Now
                    };
                    _context.UserRoles.Add(userRole);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("Index");
            }

            ViewBag.Roles = new SelectList(_context.roles.ToList(), "Id", "Name");
            ViewBag.Employees = new SelectList(_context.emp.Select(e => new { e.Id, FullName = e.FirstName + " " + e.LastName }).ToList(), "Id", "FullName", user.EmployeeId);
            var dmsList = _context.DmsDefinationFiles
                .Where(d => d.DefinitionName != null && !d.DefinitionName.StartsWith("Custom_"))
                .GroupBy(d => d.DefinitionName)
                .Select(g => new { Id = g.First().Id, DefinitionName = g.Key })
                .ToList();
            ViewBag.DmsDefinitions = new SelectList(dmsList, "Id", "DefinitionName", user.DmsDefinationFK);
            return View(user);
        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.users.FindAsync(id);
            if (user == null) return NotFound();

            var currentRole = await _context.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == id);
            ViewBag.Roles = new SelectList(_context.roles.ToList(), "Id", "Name", currentRole?.RoleId);
            ViewBag.CurrentRoleId = currentRole?.RoleId;

            ViewBag.Employees = new SelectList(_context.emp.Select(e => new { e.Id, FullName = e.FirstName + " " + e.LastName }).ToList(), "Id", "FullName", user.EmployeeId);
            var dmsList = _context.DmsDefinationFiles
                .Where(d => d.DefinitionName != null && !d.DefinitionName.StartsWith("Custom_"))
                .GroupBy(d => d.DefinitionName)
                .Select(g => new { Id = g.First().Id, DefinitionName = g.Key })
                .ToList();
            ViewBag.DmsDefinitions = new SelectList(dmsList, "Id", "DefinitionName", user.DmsDefinationFK);

            return View(user);
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User user, int? RoleId)
        {
            if (id != user.Id) return NotFound();

            // If password is blank, remove it from validation as we'll keep the existing one
            if (string.IsNullOrWhiteSpace(user.Password))
            {
                ModelState.Remove("Password");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;
                    user.updatedby = currentUserId;
                    user.updateat = DateTime.Now;

                    // Don't overwrite password if blank
                    if (!string.IsNullOrWhiteSpace(user.Password))
                        user.Password = PasswordHelper.HashPassword(user.Password);
                    else
                    {
                        var existing = await _context.users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
                        user.Password = existing?.Password;
                    }

                    _context.Update(user);
                    await _context.SaveChangesAsync();

                    // Update role assignment
                    var existingRole = await _context.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == id);
                    if (RoleId.HasValue && RoleId.Value > 0)
                    {
                        if (existingRole != null)
                        {
                            existingRole.RoleId = RoleId.Value;
                            _context.UserRoles.Update(existingRole);
                        }
                        else
                        {
                            _context.UserRoles.Add(new UserRole
                            {
                                UserId = id,
                                RoleId = RoleId.Value,
                                createdby = currentUserId,
                                createat = DateTime.Now
                            });
                        }
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.users.Any(e => e.Id == id)) return NotFound();
                    else throw;
                }
                return RedirectToAction("Index");
            }

            ViewBag.Roles = new SelectList(_context.roles.ToList(), "Id", "Name");
            ViewBag.Employees = new SelectList(_context.emp.Select(e => new { e.Id, FullName = e.FirstName + " " + e.LastName }).ToList(), "Id", "FullName", user.EmployeeId);
            var dmsList = _context.DmsDefinationFiles
                .Where(d => d.DefinitionName != null && !d.DefinitionName.StartsWith("Custom_"))
                .GroupBy(d => d.DefinitionName)
                .Select(g => new { Id = g.First().Id, DefinitionName = g.Key })
                .ToList();
            ViewBag.DmsDefinitions = new SelectList(dmsList, "Id", "DefinitionName", user.DmsDefinationFK);
            return View(user);
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.users.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null) return NotFound();

            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.users.FindAsync(id);
            if (user != null) { try { var userRoles = _context.UserRoles.Where(ur => ur.UserId == id); _context.UserRoles.RemoveRange(userRoles); _context.users.Remove(user); await _context.SaveChangesAsync(); TempData["Success"] = "User deleted successfully."; } catch (Microsoft.EntityFrameworkCore.DbUpdateException) { TempData["Error"] = "Cannot delete. This user is linked with other records."; } }
            return RedirectToAction("Index");
        }

        // GET: User/Profile
        [HttpGet]
        [SkipPermissionCheck]
        [RequireLogin]
        public async Task<IActionResult> Profile()
        {
            var userId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;
            if (userId == 0)
            {
                return RedirectToAction("Login", "User");
            }

            var user = await _context.users
                .Include(u => u.Employee)
                    .ThenInclude(e => e.Department)
                .Include(u => u.Employee)
                    .ThenInclude(e => e.Location)
                .Include(u => u.Employee)
                    .ThenInclude(e => e.EmploymentType)
                .Include(u => u.Employee)
                    .ThenInclude(e => e.EmploymentStatus)
                .Include(u => u.Employee)
                    .ThenInclude(e => e.Designation)
                .Include(u => u.Employee)
                    .ThenInclude(e => e.Grade)
                .Include(u => u.Employee)
                    .ThenInclude(e => e.SubDepartment)
                .Include(u => u.Employee)
                    .ThenInclude(e => e.Division)
                .Include(u => u.Employee)
                    .ThenInclude(e => e.BankName)
                .Include(u => u.Employee)
                    .ThenInclude(e => e.OvertimePolicy)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            // Load user roles
            var roleRows = await _context.UserRoles
                .AsNoTracking()
                .Where(ur => ur.UserId == userId)
                .Join(_context.roles.AsNoTracking(),
                      ur => ur.RoleId,
                      r  => r.Id,
                      (ur, r) => r.Name)
                .ToListAsync();
            var roleNames = roleRows.Where(r => !string.IsNullOrEmpty(r)).ToList();
            ViewBag.RoleName = roleNames.Any() ? string.Join(", ", roleNames) : "User";

            var emp = user.Employee;
            if (emp != null)
            {
                // Load Line Manager Name
                if (!string.IsNullOrWhiteSpace(emp.LineManagerEmpNo))
                {
                    var lm = await _context.emp.AsNoTracking().FirstOrDefaultAsync(x => x.Code == emp.LineManagerEmpNo);
                    ViewBag.LineManagerName = lm != null ? $"{lm.FirstName} {lm.LastName}" : emp.LineManagerEmpNo;
                }
                else
                {
                    ViewBag.LineManagerName = "—";
                }

                // Load Leave Approver Name
                if (!string.IsNullOrWhiteSpace(emp.LeaveApproverEmpNo))
                {
                    var la = await _context.emp.AsNoTracking().FirstOrDefaultAsync(x => x.Code == emp.LeaveApproverEmpNo);
                    ViewBag.LeaveApproverName = la != null ? $"{la.FirstName} {la.LastName}" : emp.LeaveApproverEmpNo;
                }
                else
                {
                    ViewBag.LeaveApproverName = "—";
                }

                // Load DMS/Checklist information
                // Priority 1: DocApprovalAssignment (formal workflow assignment)
                // Priority 2: User.DmsDefinationFK (direct assignment from admin User edit page)
                bool hasDms = false;
                string dmsName = "";
                double progressPercent = 0;
                var checklistItems = new List<DmsDefinationFile>();
                var uploadedDocs = new List<Document>();

                // Resolve the definition name — try DocApprovalAssignment first, then fall back to DmsDefinationFK
                string resolvedDefinitionName = "";

                var dmsAssignment = await _context.DocApprovalAssignments.AsNoTracking()
                    .FirstOrDefaultAsync(a => a.UserId == userId && a.IsActive);

                if (dmsAssignment != null)
                {
                    resolvedDefinitionName = dmsAssignment.DmsDefinitionName;
                }
                else if (user.DmsDefinationFK != null)
                {
                    // Fall back: DmsDefinationFK points to a single row — use its DefinitionName as the group key
                    var singleDef = await _context.DmsDefinationFiles.AsNoTracking()
                        .FirstOrDefaultAsync(d => d.Id == user.DmsDefinationFK);
                    if (singleDef != null)
                        resolvedDefinitionName = singleDef.DefinitionName ?? "";
                }

                if (!string.IsNullOrEmpty(resolvedDefinitionName))
                {
                    var dmsDef = await _context.DmsDefinationFiles.AsNoTracking()
                        .FirstOrDefaultAsync(d => d.DefinitionName == resolvedDefinitionName);

                    if (dmsDef != null)
                    {
                        hasDms = true;
                        dmsName = dmsDef.DefinitionName ?? "";

                        checklistItems = await _context.DmsDefinationFiles
                            .Include(d => d.Category)
                            .Include(d => d.DocumentType)
                            .Where(d => d.DefinitionName == dmsName)
                            .ToListAsync();

                        uploadedDocs = await _context.Documents
                            .Include(d => d.Category)
                            .Include(d => d.DocumentType)
                            .Where(d => d.EmployeeID == emp.Id)
                            .OrderByDescending(d => d.UploadDate)
                            .ToListAsync();

                        var docIds = uploadedDocs.Select(d => d.DocumentID).ToList();
                        var approvalInstances = await _context.DocApprovalInstances
                            .Include(i => i.InstanceSteps)
                                .ThenInclude(s => s.TemplateStep)
                                    .ThenInclude(ts => ts.Role)
                            .Include(i => i.InstanceSteps)
                                .ThenInclude(s => s.Approver)
                            .Include(i => i.Workflow)
                            .Where(i => docIds.Contains(i.DocumentID))
                            .ToListAsync();
                        ViewBag.ApprovalInstances = approvalInstances;

                        double totalWeight = 0;
                        double uploadedWeight = 0;
                        foreach (var item in checklistItems)
                        {
                            if (double.TryParse(item.Percentage, out double weight))
                            {
                                totalWeight += weight;

                                var matchedDoc = uploadedDocs.FirstOrDefault(d =>
                                    d.CategoryID == item.CategoryID &&
                                    d.DocumentTypeID == item.DocumentTypeID);

                                if (matchedDoc != null)
                                {
                                    bool requiresApproval = item.DocumentType?.RequiresApproval ?? false;
                                    if (!requiresApproval)
                                    {
                                        uploadedWeight += weight;
                                    }
                                    else
                                    {
                                        var inst = approvalInstances.FirstOrDefault(i => i.DocumentID == matchedDoc.DocumentID);
                                        if (inst != null && inst.Status == "Approved")
                                            uploadedWeight += weight;
                                    }
                                }
                            }
                        }
                        progressPercent = totalWeight > 0 ? (uploadedWeight / totalWeight) * 100 : 0;
                        if (progressPercent > 100) progressPercent = 100;
                        progressPercent = Math.Round(progressPercent, 0);
                    }
                }

                ViewBag.HasDmsDefinition = hasDms;
                ViewBag.DmsDefinitionName = dmsName;
                ViewBag.DmsProgress = progressPercent;
                ViewBag.ChecklistItems = checklistItems;
                ViewBag.UploadedDocuments = uploadedDocs;
            }

            return View(user);
        }

        // POST: User/UploadChecklistFile
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SkipPermissionCheck]
        [RequireLogin]
        public async Task<IActionResult> UploadChecklistFile(
            int EmployeeID, 
            int CategoryID, 
            int DocumentTypeID, 
            string Title, 
            string Description, 
            DateTime EffectiveDate, 
            DateTime ExpiryDate, 
            string Remarks, 
            IFormFile File)
        {
            if (File == null || File.Length == 0)
            {
                TempData["Error"] = "Please select a valid file to upload.";
                return RedirectToAction("Profile", new { activeTab = "documents" });
            }

            try
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "documents");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + File.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await File.CopyToAsync(stream);
                }

                var docType = await _context.DocumentTypes.FindAsync(DocumentTypeID);
                bool requiresApproval = docType != null && docType.RequiresApproval;

                var doc = new Document
                {
                    DocumentNo = "DOC-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                    EmployeeID = EmployeeID,
                    CategoryID = CategoryID,
                    DocumentTypeID = DocumentTypeID,
                    Title = string.IsNullOrWhiteSpace(Title) ? File.FileName : Title,
                    Description = Description,
                    EffectiveDate = EffectiveDate,
                    ExpiryDate = ExpiryDate,
                    Remarks = Remarks,
                    FileName = File.FileName,
                    FilePath = "/uploads/documents/" + uniqueFileName,
                    FileExtension = Path.GetExtension(File.FileName),
                    FileSize = File.Length,
                    UploadDate = DateTime.Now,
                    UploadedBy = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0,
                    Status = requiresApproval ? "Pending Approval" : "Approved"
                };

                _context.Documents.Add(doc);
                await _context.SaveChangesAsync();

                if (requiresApproval)
                {
                    ApprovalTemplate? workflow = null;

                    // Use DocApprovalAssignment — the authoritative link between a user and a DMS definition group + approval template
                    var dmsAssignment = await _context.DocApprovalAssignments
                        .FirstOrDefaultAsync(a => a.UserId == doc.UploadedBy && a.IsActive);

                    if (dmsAssignment != null)
                    {
                        workflow = await _context.ApprovalTemplates
                            .Include(t => t.Steps)
                            .FirstOrDefaultAsync(t => t.Id == dmsAssignment.ApprovalTemplateId && t.IsActive);
                    }

                    if (workflow != null && workflow.Steps.Any())
                    {
                        var workflowEngine = HttpContext.RequestServices.GetRequiredService<IWorkflowEngine>();
                        await workflowEngine.StartWorkflowAsync(doc, workflow);
                        TempData["Success"] = "Document uploaded and submitted for approval.";
                    }
                    else
                    {
                        doc.Status = "Pending";
                        await _context.SaveChangesAsync();
                        TempData["Success"] = "Document uploaded. No active workflow configured for this user's definition.";
                    }
                }
                else
                {
                    TempData["Success"] = "Document uploaded successfully.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error uploading document: " + ex.Message;
            }

            return RedirectToAction("Profile", new { activeTab = "documents" });
        }
    }
}