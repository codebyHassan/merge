using AlignHR.Data;
using AlignHR.Models;
using AlignHR.Security;
using AlignHR.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using OfficeOpenXml;

namespace AlignHR.Controllers
{
    public class EmployeesController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IHrOnboardingService _onboardingService;
        public EmployeesController(ApplicationDbContext context, IHrOnboardingService onboardingService)
        {
            _context = context;
            _onboardingService = onboardingService;
        }


        [RequireUrlPermission]

        public IActionResult Index(string search, int? departmentId, int? subDepartmentId, int? gradeId, int? divisionId, string status, int? page)
        {
            int pageSize = 10;
            int pageNumber = page ?? 1;

            // ── Filter dropdowns: read-only, no tracking ──
            ViewBag.DepartmentsFilter    = new SelectList(_context.department.AsNoTracking().OrderBy(d => d.Name), "Id", "Name", departmentId);
            ViewBag.SubDepartmentsFilter = new SelectList(_context.valuesets.AsNoTracking().Where(v => v.GroupName == "SubDepartment" && v.IsActive).OrderBy(v => v.Name), "Id", "Name", subDepartmentId);
            ViewBag.GradesFilter         = new SelectList(_context.valuesets.AsNoTracking().Where(v => v.GroupName == "Grade"         && v.IsActive).OrderBy(v => v.Name), "Id", "Name", gradeId);
            ViewBag.DivisionsFilter      = new SelectList(_context.valuesets.AsNoTracking().Where(v => v.GroupName == "Division"      && v.IsActive).OrderBy(v => v.Name), "Id", "Name", divisionId);
            ViewBag.StatusFilter         = status;

            // ── Main query: filter FIRST, then include, NoTracking + SplitQuery ──
            var obj = _context.emp.AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                // Trim once and use StartsWith for the indexed Code lookup
                // (Code is typically searched as a prefix). Names + dept + location
                // remain Contains because users expect substring matching.
                var s = search.Trim();
                obj = obj.Where(d => d.Code.StartsWith(s)            ||
                                     d.FirstName.Contains(s)         ||
                                     d.LastName.Contains(s)          ||
                                     d.Department.Name.Contains(s)   ||
                                     d.Location.Name.Contains(s));
            }

            if (departmentId.HasValue    && departmentId    > 0) obj = obj.Where(e => e.DepartmentFk    == departmentId.Value);
            if (subDepartmentId.HasValue && subDepartmentId > 0) obj = obj.Where(e => e.SubDepartmentFk == subDepartmentId.Value);
            if (gradeId.HasValue         && gradeId         > 0) obj = obj.Where(e => e.GradeFk         == gradeId.Value);
            if (divisionId.HasValue      && divisionId      > 0) obj = obj.Where(e => e.DivisionFk      == divisionId.Value);
            if (!string.IsNullOrEmpty(status))                   obj = obj.Where(e => e.SalaryStatus    == status);

            // Includes happen AFTER filtering so joins run on the smaller filtered set.
            // AsSplitQuery() runs each Include as a separate SQL — avoids cartesian explosion.
            obj = obj
                .Include(e => e.Department)
                .Include(e => e.Location)
                .Include(e => e.EmploymentType)
                .Include(e => e.EmploymentStatus)
                .Include(e => e.Designation)
                .Include(e => e.Grade)
                .Include(e => e.SubDepartment)
                .Include(e => e.Division)
                .AsSplitQuery()
                .OrderBy(d => d.Id);

            var model = obj.ToPagedList(pageNumber, pageSize);

            // Load assigned users for employees on this page
            var empIds = model.Select(e => e.Id).ToList();
            ViewBag.UsersByEmployeeId = _context.users
                .AsNoTracking()
                .Where(u => u.EmployeeId != null && empIds.Contains(u.EmployeeId.Value))
                .ToDictionary(u => u.EmployeeId!.Value, u => u.Email);

            return View(model);
        }

        [HttpGet]
        public IActionResult Export(string search, int? departmentId, int? subDepartmentId, int? gradeId, int? divisionId, string status, string columns, string downloadToken)
        {
            if (!string.IsNullOrEmpty(downloadToken))
            {
                Response.Cookies.Append("fileDownloadToken", downloadToken, new Microsoft.AspNetCore.Http.CookieOptions
                {
                    Path = "/",
                    HttpOnly = false, // Must be readable by JavaScript
                    Secure = false
                });
            }

            var query = _context.emp
                .Include(e => e.Department)
                .Include(e => e.Location)
                .Include(e => e.EmploymentType)
                .Include(e => e.EmploymentStatus)
                .Include(e => e.Designation)
                .Include(e => e.Grade)
                .Include(e => e.SubDepartment)
                .Include(e => e.Division)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(d => d.FirstName.Contains(search) ||
                                         d.LastName.Contains(search) || 
                                         d.Code.Contains(search) ||
                                         d.Department.Name.Contains(search) ||
                                         d.Location.Name.Contains(search));
            }

            if (departmentId.HasValue && departmentId > 0)
            {
                query = query.Where(e => e.DepartmentFk == departmentId.Value);
            }

            if (subDepartmentId.HasValue && subDepartmentId > 0)
            {
                query = query.Where(e => e.SubDepartmentFk == subDepartmentId.Value);
            }

            if (gradeId.HasValue && gradeId > 0)
            {
                query = query.Where(e => e.GradeFk == gradeId.Value);
            }

            if (divisionId.HasValue && divisionId > 0)
            {
                query = query.Where(e => e.DivisionFk == divisionId.Value);
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(e => e.SalaryStatus == status);
            }

            var list = query.OrderBy(e => e.Code).ToList();

            var selectedCols = !string.IsNullOrEmpty(columns) 
                ? columns.Split(',').Select(c => c.Trim().ToLowerInvariant()).ToList() 
                : new List<string> { "code", "name", "reptoemp", "department", "sub-dept", "designation", "grade", "location", "employment type", "join date", "priority", "status" };

            var headersList = new List<string>();
            if (selectedCols.Contains("code")) headersList.Add("Code");
            if (selectedCols.Contains("name")) headersList.Add("Name");
            if (selectedCols.Contains("reptoemp")) headersList.Add("RepToEmp");
            if (selectedCols.Contains("department")) headersList.Add("Department");
            if (selectedCols.Contains("sub-dept")) headersList.Add("Sub-Department");
            if (selectedCols.Contains("designation")) headersList.Add("Designation");
            if (selectedCols.Contains("grade")) headersList.Add("Grade");
            if (selectedCols.Contains("location")) headersList.Add("Location");
            if (selectedCols.Contains("employment type")) headersList.Add("Employment Type");
            if (selectedCols.Contains("join date")) headersList.Add("Join Date");
            if (selectedCols.Contains("priority")) headersList.Add("Priority");
            if (selectedCols.Contains("status")) headersList.Add("Status");

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Employees");

                // Add headers
                for (int col = 0; col < headersList.Count; col++)
                {
                    worksheet.Cells[1, col + 1].Value = headersList[col];
                    worksheet.Cells[1, col + 1].Style.Font.Bold = true;
                }

                // Add data rows
                int rowIdx = 2;
                foreach (var emp in list)
                {
                    int colIdx = 1;
                    if (selectedCols.Contains("code")) worksheet.Cells[rowIdx, colIdx++].Value = emp.Code;
                    if (selectedCols.Contains("name")) worksheet.Cells[rowIdx, colIdx++].Value = $"{emp.FirstName} {emp.LastName}";
                    if (selectedCols.Contains("reptoemp")) {
                        var owner = !string.IsNullOrWhiteSpace(emp.LineManagerEmpNo)
                            ? emp.LineManagerEmpNo
                            : (!string.IsNullOrWhiteSpace(emp.LeaveApproverEmpNo) ? emp.LeaveApproverEmpNo : "-");
                        worksheet.Cells[rowIdx, colIdx++].Value = owner;
                    }
                    if (selectedCols.Contains("department")) worksheet.Cells[rowIdx, colIdx++].Value = emp.Department?.Name ?? "-";
                    if (selectedCols.Contains("sub-dept")) worksheet.Cells[rowIdx, colIdx++].Value = emp.SubDepartment?.Name ?? "-";
                    if (selectedCols.Contains("designation")) worksheet.Cells[rowIdx, colIdx++].Value = emp.Designation?.Name ?? "-";
                    if (selectedCols.Contains("grade")) worksheet.Cells[rowIdx, colIdx++].Value = emp.Grade?.Name ?? "-";
                    if (selectedCols.Contains("location")) worksheet.Cells[rowIdx, colIdx++].Value = emp.Location?.Name ?? "-";
                    if (selectedCols.Contains("employment type")) worksheet.Cells[rowIdx, colIdx++].Value = emp.EmploymentType?.Name ?? "-";
                    if (selectedCols.Contains("join date")) worksheet.Cells[rowIdx, colIdx++].Value = emp.Dateofjoin.ToString("dd MMM yyyy");
                    if (selectedCols.Contains("priority")) {
                        var priority = emp.EmploymentType?.Name ?? emp.Location?.Name ?? "-";
                        worksheet.Cells[rowIdx, colIdx++].Value = priority;
                    }
                    if (selectedCols.Contains("status")) {
                        var rawStatus = emp.EmploymentStatus?.Name ?? emp.SalaryStatus ?? "Active";
                        worksheet.Cells[rowIdx, colIdx++].Value = rawStatus;
                    }
                    rowIdx++;
                }

                worksheet.Cells.AutoFitColumns();

                var fileBytes = package.GetAsByteArray();
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "employees.xlsx");
            }
        }

        [RequireUrlPermission]

        public async Task<IActionResult> Create(decimal? candidateId, decimal? onboardingId, string? hireType)
        {
            var prefill = new Employee();

            if (candidateId.HasValue)
            {
                var candidate = await _context.HrCandidates
                    .FirstOrDefaultAsync(c => c.Id == candidateId.Value);
                if (candidate != null)
                {
                    ViewBag.Candidate = candidate;
                    prefill.FirstName = candidate.FirstName;
                    prefill.LastName = candidate.LastName ?? string.Empty;
                }
            }

            if (onboardingId.HasValue)
            {
                var onboarding = await _context.HrOnboardings
                    .Include(o => o.Offer)
                        .ThenInclude(of => of!.JobApplication)
                            .ThenInclude(a => a!.JobPosting)
                    .FirstOrDefaultAsync(o => o.Id == onboardingId.Value);

                if (onboarding != null)
                {
                    // Joining date from candidate's acceptance
                    if (onboarding.Offer?.CandidateJoiningDate.HasValue == true)
                        prefill.Dateofjoin = DateOnly.FromDateTime(onboarding.Offer.CandidateJoiningDate.Value);

                    // Department from requisition
                    var requisitionFk = onboarding.Offer?.JobApplication?.JobPosting?.RequisitionFK;
                    if (requisitionFk.HasValue)
                    {
                        var req = await _context.HrRequisitions.FirstOrDefaultAsync(r => r.Id == requisitionFk.Value);
                        if (req?.DepartmentFK.HasValue == true)
                            prefill.DepartmentFk = (int)req.DepartmentFK.Value;
                    }
                }
            }

            ViewBag.OnboardingId = onboardingId;
            ViewBag.HireType = hireType;

            ViewBag.Departments = new SelectList(_context.department, "Id", "Name", prefill.DepartmentFk);
            ViewBag.Locations = new SelectList(_context.location, "Id", "Name");
            ViewBag.EmploymentTypes = new SelectList(_context.valuesets.Where(v => v.GroupName == "EmploymentType" && v.IsActive), "Id", "Name");
            ViewBag.EmploymentStatuses = new SelectList(_context.valuesets.Where(v => v.GroupName == "EmploymentStatus" && v.IsActive), "Id", "Name");
            ViewBag.Designations = new SelectList(_context.valuesets.Where(v => v.GroupName == "Designation" && v.IsActive), "Id", "Name");
            ViewBag.SubDepartments = new SelectList(_context.valuesets.Where(v => v.GroupName == "SubDepartment" && v.IsActive), "Id", "Name");
            ViewBag.Grades = new SelectList(_context.valuesets.Where(v => v.GroupName == "Grade" && v.IsActive), "Id", "Name");
            ViewBag.Divisions = new SelectList(_context.valuesets.Where(v => v.GroupName == "Division" && v.IsActive), "Id", "Name");
            ViewBag.BankNames = new SelectList(_context.valuesets.Where(v => v.GroupName == "BankName" && v.IsActive), "Id", "Name");
            ViewBag.OvertimePolicies = new SelectList(_context.Overtimes.Where(o => o.IsActive).ToList(), "OvertimeId", "PolicyName");
            ViewBag.PoliciesList = _context.Overtimes.Where(o => o.IsActive).ToList();
            ViewBag.DefaultOvertimePolicyId = _context.Overtimes.FirstOrDefault(o => o.IsDefault)?.OvertimeId;

            return View(prefill);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> create(Employee e, IFormFile? ImageFile, decimal? onboardingId, decimal? candidateFk, string? hireTypeHidden)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "employees");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        ImageFile.CopyTo(fileStream);
                    }
                    e.ProfileImage = uniqueFileName;
                }

                var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;

                e.createat = DateTime.Now;
                e.createdby = currentUserId;

                _context.emp.Add(e);
                _context.SaveChanges();

                if (onboardingId.HasValue)
                {
                    var joiningDate = e.Dateofjoin != default
                        ? e.Dateofjoin.ToDateTime(TimeOnly.MinValue)
                        : DateTime.Today;

                    // Resolve offer/requisition from onboarding for the history record
                    var onboarding = await _context.HrOnboardings
                        .Include(o => o.Offer)
                            .ThenInclude(of => of!.JobApplication)
                                .ThenInclude(a => a!.JobPosting)
                        .FirstOrDefaultAsync(o => o.Id == onboardingId.Value);

                    decimal? requisitionFk = onboarding?.Offer?.JobApplication?.JobPosting?.RequisitionFK;

                    int? replacedEmployeeId = null;
                    if ((hireTypeHidden ?? "New") == "Replacement" && requisitionFk.HasValue)
                    {
                        var req = await _context.HrRequisitions.FirstOrDefaultAsync(r => r.Id == requisitionFk.Value);
                        if (req?.ReplacementEmployeeId.HasValue == true)
                            replacedEmployeeId = (int)req.ReplacementEmployeeId.Value;
                    }

                    await _context.EmployeeHireHistories.AddAsync(new EmployeeHireHistory
                    {
                        EmployeeId = e.Id,
                        HireType = hireTypeHidden ?? "New",
                        CandidateFk = candidateFk,
                        OfferId = onboarding?.OfferId,
                        RequisitionFk = requisitionFk,
                        ReplacedEmployeeId = replacedEmployeeId,
                        EffectiveDate = e.Dateofjoin != default ? e.Dateofjoin : DateOnly.FromDateTime(DateTime.Today),
                        CreatedBy = PermissionHelper.GetCurrentUsername(HttpContext),
                        CreatedOn = DateTime.Now
                    });
                    await _context.SaveChangesAsync();

                    await _onboardingService.OnEmployeeCreatedAsync(onboardingId.Value, joiningDate);
                    TempData["Success"] = "Employee created. Onboarding tasks are now ready.";
                    return RedirectToAction("Details", "HrOnboarding", new { id = onboardingId.Value });
                }

                TempData["Success"] = "Employee created successfully.";
                return RedirectToAction("index");
            }
            ViewBag.Departments = new SelectList(_context.department, "Id", "Name", e.DepartmentFk);
            ViewBag.Locations = new SelectList(_context.location, "Id", "Name", e.LocationFk);
            ViewBag.EmploymentTypes = new SelectList(_context.valuesets.Where(v => v.GroupName == "EmploymentType" && v.IsActive), "Id", "Name", e.EmploymentTypeFk);
            ViewBag.EmploymentStatuses = new SelectList(_context.valuesets.Where(v => v.GroupName == "EmploymentStatus" && v.IsActive), "Id", "Name", e.EmploymentStatusFk);
            ViewBag.Designations = new SelectList(_context.valuesets.Where(v => v.GroupName == "Designation" && v.IsActive), "Id", "Name", e.DesiginationId);
            ViewBag.SubDepartments = new SelectList(_context.valuesets.Where(v => v.GroupName == "SubDepartment" && v.IsActive), "Id", "Name", e.SubDepartmentFk);
            ViewBag.Grades = new SelectList(_context.valuesets.Where(v => v.GroupName == "Grade" && v.IsActive), "Id", "Name", e.GradeFk);
            ViewBag.Divisions = new SelectList(_context.valuesets.Where(v => v.GroupName == "Division" && v.IsActive), "Id", "Name", e.DivisionFk);
            ViewBag.BankNames = new SelectList(_context.valuesets.Where(v => v.GroupName == "BankName" && v.IsActive), "Id", "Name", e.BankNameFk);
            
            
            ViewBag.OvertimePolicies = new SelectList(_context.Overtimes.Where(o => o.IsActive).ToList(), "OvertimeId", "PolicyName", e.OvertimePolicyId);
            ViewBag.DefaultOvertimePolicyId = _context.Overtimes.FirstOrDefault(o => o.IsDefault)?.OvertimeId;
            ViewBag.PoliciesList = _context.Overtimes.Where(o => o.IsActive).ToList();



            return View(e);
        }

        [RequireUrlPermission]
        public async Task<IActionResult> Details(int id)
        {
            var employee = await _context.emp
                .Include(e => e.Department)
                .Include(e => e.Location)
                .Include(e => e.EmploymentType)
                .Include(e => e.EmploymentStatus)
                .Include(e => e.Designation)
                .Include(e => e.BankName)
                .Include(e => e.OvertimePolicy)
                .Include(e => e.SubDepartment)
                .Include(e => e.Division)
                .Include(e => e.Grade)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (employee == null)
                return NotFound();

            // Calculate DMS Progress
            var linkedUser = await _context.users.FirstOrDefaultAsync(u => u.EmployeeId == id);
            double progressPercent = 0;
            bool hasDms = false;
            string dmsName = "";

            if (linkedUser?.DmsDefinationFK != null)
            {
                var dmsDef = await _context.DmsDefinationFiles.FirstOrDefaultAsync(d => d.Id == linkedUser.DmsDefinationFK.Value);
                if (dmsDef != null)
                {
                    hasDms = true;
                    dmsName = dmsDef.DefinitionName ?? "";
                    var checklistItems = await _context.DmsDefinationFiles
                        .Where(d => d.DefinitionName == dmsDef.DefinitionName)
                        .ToListAsync();

                    var uploadedDocs = await _context.Documents
                        .Where(d => d.EmployeeID == id)
                        .ToListAsync();

                    double totalWeight = 0;
                    double uploadedWeight = 0;
                    foreach (var item in checklistItems)
                    {
                        if (double.TryParse(item.Percentage, out double weight))
                        {
                            totalWeight += weight;
                            var isUploaded = uploadedDocs.Any(d => d.CategoryID == item.CategoryID && d.DocumentTypeID == item.DocumentTypeID);
                            if (isUploaded)
                            {
                                uploadedWeight += weight;
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

            return View(employee);
        }

        [RequireUrlPermission]

        public IActionResult Edit(int id)
        {
            var employee = _context.emp.Find(id);
            if (employee == null)
                return NotFound();

            ViewBag.Departments = new SelectList(_context.department, "Id", "Name", employee.DepartmentFk);
            ViewBag.Locations = new SelectList(_context.location, "Id", "Name", employee.LocationFk);
            ViewBag.EmploymentTypes = new SelectList(_context.valuesets.Where(v => v.GroupName == "EmploymentType" && v.IsActive), "Id", "Name", employee.EmploymentTypeFk);
            ViewBag.EmploymentStatuses = new SelectList(_context.valuesets.Where(v => v.GroupName == "EmploymentStatus" && v.IsActive), "Id", "Name", employee.EmploymentStatusFk);
            
            ViewBag.Designations = new SelectList(_context.valuesets.Where(v => v.GroupName == "Designation" && v.IsActive), "Id", "Name", employee.DesiginationId);
            ViewBag.SubDepartments = new SelectList(_context.valuesets.Where(v => v.GroupName == "SubDepartment" && v.IsActive), "Id", "Name", employee.SubDepartmentFk);
            ViewBag.Grades = new SelectList(_context.valuesets.Where(v => v.GroupName == "Grade" && v.IsActive), "Id", "Name", employee.GradeFk);
            ViewBag.Divisions = new SelectList(_context.valuesets.Where(v => v.GroupName == "Division" && v.IsActive), "Id", "Name", employee.DivisionFk);
            ViewBag.BankNames = new SelectList(_context.valuesets.Where(v => v.GroupName == "BankName" && v.IsActive), "Id", "Name", employee.BankNameFk);
            
            
            ViewBag.OvertimePolicies = new SelectList(_context.Overtimes.Where(o => o.IsActive).ToList(), "OvertimeId", "PolicyName", employee.OvertimePolicyId);
            ViewBag.PoliciesList = _context.Overtimes.Where(o => o.IsActive).ToList();
            ViewBag.DefaultOvertimePolicyId = _context.Overtimes.FirstOrDefault(o => o.IsDefault)?.OvertimeId;

            return View(employee);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Employee e, IFormFile? ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "employees");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        ImageFile.CopyTo(fileStream);
                    }
                    
                    // Optional: Delete old image if needed
                    e.ProfileImage = uniqueFileName;
                }
                else
                {
                    // Keep the existing image if no new one is uploaded
                    var existing = _context.emp.AsNoTracking().FirstOrDefault(x => x.Id == e.Id);
                    if (existing != null) e.ProfileImage = existing.ProfileImage;
                }

                var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;

                e.updateat = DateTime.Now;
                e.updatedby = currentUserId;

                _context.emp.Update(e);
                _context.SaveChanges();
                TempData["Success"] = "Employee updated successfully.";

                return RedirectToAction("Index");
            }

            ViewBag.Departments = new SelectList(_context.department, "Id", "Name", e.DepartmentFk);
            ViewBag.Locations = new SelectList(_context.location, "Id", "Name", e.LocationFk);
            ViewBag.EmploymentTypes = new SelectList(_context.valuesets.Where(v => v.GroupName == "EmploymentType" && v.IsActive), "Id", "Name", e.EmploymentTypeFk);
            ViewBag.EmploymentStatuses = new SelectList(_context.valuesets.Where(v => v.GroupName == "EmploymentStatus" && v.IsActive), "Id", "Name", e.EmploymentStatusFk);
            ViewBag.Designations = new SelectList(_context.valuesets.Where(v => v.GroupName == "Designation" && v.IsActive), "Id", "Name", e.DesiginationId);
            ViewBag.SubDepartments = new SelectList(_context.valuesets.Where(v => v.GroupName == "SubDepartment" && v.IsActive), "Id", "Name", e.SubDepartmentFk);
            ViewBag.Grades = new SelectList(_context.valuesets.Where(v => v.GroupName == "Grade" && v.IsActive), "Id", "Name", e.GradeFk);
            ViewBag.Divisions = new SelectList(_context.valuesets.Where(v => v.GroupName == "Division" && v.IsActive), "Id", "Name", e.DivisionFk);
            ViewBag.BankNames = new SelectList(_context.valuesets.Where(v => v.GroupName == "BankName" && v.IsActive), "Id", "Name", e.BankNameFk);
            
            ViewBag.OvertimePolicies = new SelectList(_context.Overtimes.Where(o => o.IsActive).ToList(), "OvertimeId", "PolicyName", e.OvertimePolicyId);
            ViewBag.DefaultOvertimePolicyId = _context.Overtimes.FirstOrDefault(o => o.IsDefault)?.OvertimeId;
            ViewBag.PoliciesList = _context.Overtimes.Where(o => o.IsActive).ToList();



            return View(e);
        }

        // GET: Departments/Delete/5
        [RequireUrlPermission]

        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var employee = _context.emp.FirstOrDefault(d => d.Id == id);
            if (employee == null)
                return NotFound();

            return View(employee);
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var employee = _context.emp.Find(id);
            if (employee != null) { try { _context.emp.Remove(employee); _context.SaveChanges(); TempData["Success"] = "Deleted successfully."; } catch (Microsoft.EntityFrameworkCore.DbUpdateException) { TempData["Error"] = "Cannot delete. This record is linked with other data."; } }

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> GetEmployeeByCode(string code, int? id)
        {
            Employee? employee = null;

            if (id.HasValue && id > 0)
            {
                // Initial load for Edit view
                employee = await _context.emp
                    .Include(e => e.Department)
                    .Include(e => e.Location)
                    .Include(e => e.EmploymentStatus)
                    .FirstOrDefaultAsync(e => e.Id == id.Value);
            }
            else if (!string.IsNullOrWhiteSpace(code))
            {
                // HTMX search by Code - Filter by Active Status as requested
                employee = await _context.emp
                    .Include(e => e.Department)
                    .Include(e => e.Location)
                    .Include(e => e.EmploymentStatus)
                    .FirstOrDefaultAsync(e => e.Code == code && e.SalaryStatus == "Active");
            }

            return PartialView("_EmployeeProfileCard", employee);
        }

        [HttpGet]
        public IActionResult SearchEmployees(string q, string term, int? deptId)
        {
            var query = _context.emp
                .Include(e => e.Department)
                .AsQueryable();

            var searchTerm = string.IsNullOrWhiteSpace(q) ? term : q;
            bool hasDept = deptId.HasValue && deptId > 0;
            bool hasQuery = !string.IsNullOrWhiteSpace(searchTerm);

            // Avoid returning a huge list on initial load when no filter/search is provided.
            if (!hasDept && !hasQuery)
            {
                return PartialView("_EmployeeSearchResults", new List<dynamic>());
            }

            if (hasDept)
            {
                query = query.Where(e => e.DepartmentFk == deptId.Value);
            }

            if (hasQuery)
            {
                query = query.Where(e =>
                    e.FirstName.Contains(searchTerm) ||
                    e.LastName.Contains(searchTerm) ||
                    e.Code.Contains(searchTerm));
            }

            var results = query
                .OrderBy(e => e.FirstName)
                .Take(20)
                .ToList();

            return PartialView("~/Views/Shared/_EmployeeSearchResults.cshtml", results);
        }

        [RequireUrlPermission]
        public async Task<IActionResult> UploadChecklist(int id)
        {
            var employee = await _context.emp
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
                return NotFound();

            var linkedUser = await _context.users.FirstOrDefaultAsync(u => u.EmployeeId == id);
            
            bool hasDms = false;
            string dmsName = "";
            double progressPercent = 0;
            var checklistItems = new List<DmsDefinationFile>();
            var uploadedDocs = new List<Document>();

            if (linkedUser?.DmsDefinationFK != null)
            {
                var dmsDef = await _context.DmsDefinationFiles.FirstOrDefaultAsync(d => d.Id == linkedUser.DmsDefinationFK.Value);
                if (dmsDef != null)
                {
                    hasDms = true;
                    dmsName = dmsDef.DefinitionName ?? "";
                    
                    checklistItems = await _context.DmsDefinationFiles
                        .Include(d => d.Category)
                        .Include(d => d.DocumentType)
                        .Where(d => d.DefinitionName == dmsDef.DefinitionName)
                        .ToListAsync();

                    uploadedDocs = await _context.Documents
                        .Include(d => d.Category)
                        .Include(d => d.DocumentType)
                        .Where(d => d.EmployeeID == id)
                        .ToListAsync();

                    double totalWeight = 0;
                    double uploadedWeight = 0;
                    foreach (var item in checklistItems)
                    {
                        if (double.TryParse(item.Percentage, out double weight))
                        {
                            totalWeight += weight;
                            var isUploaded = uploadedDocs.Any(d => d.CategoryID == item.CategoryID && d.DocumentTypeID == item.DocumentTypeID);
                            if (isUploaded)
                            {
                                uploadedWeight += weight;
                            }
                        }
                    }
                    progressPercent = totalWeight > 0 ? (uploadedWeight / totalWeight) * 100 : 0;
                    if (progressPercent > 100) progressPercent = 100;
                    progressPercent = Math.Round(progressPercent, 0);
                }
            }

            ViewBag.Employee = employee;
            ViewBag.HasDmsDefinition = hasDms;
            ViewBag.DmsDefinitionName = dmsName;
            ViewBag.DmsProgress = progressPercent;
            ViewBag.UploadedDocuments = uploadedDocs;

            return View(checklistItems);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadChecklistFile(int EmployeeID, int CategoryID, int DocumentTypeID, string Title, IFormFile File)
        {
            if (File == null || File.Length == 0)
            {
                TempData["Error"] = "Please select a valid file to upload.";
                return RedirectToAction("UploadChecklist", new { id = EmployeeID });
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

                var doc = new Document
                {
                    DocumentNo = "DOC-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                    EmployeeID = EmployeeID,
                    CategoryID = CategoryID,
                    DocumentTypeID = DocumentTypeID,
                    Title = string.IsNullOrWhiteSpace(Title) ? File.FileName : Title,
                    FileName = File.FileName,
                    FilePath = "/uploads/documents/" + uniqueFileName,
                    FileExtension = Path.GetExtension(File.FileName),
                    FileSize = File.Length,
                    UploadDate = DateTime.Now,
                    UploadedBy = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0,
                    Status = "Approved"
                };

                _context.Documents.Add(doc);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Document uploaded successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error uploading document: " + ex.Message;
            }

            return RedirectToAction("UploadChecklist", new { id = EmployeeID });
        }
    }
}


