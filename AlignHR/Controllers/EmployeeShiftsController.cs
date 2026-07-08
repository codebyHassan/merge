using AlignHR.Data;
using AlignHR.Models;
using AlignHR.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace AlignHR.Controllers
{
    public class EmployeeShiftsController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public EmployeeShiftsController(ApplicationDbContext context)
        {
            _context = context;
        }

        //[RequireUrlPermission]
        //public async Task<IActionResult> Index(string search)
        //{
        //    // Start with the base query including Employee navigation property
        //    var query = _context.EmployeeShifts
        //                        .Include(es => es.Employee)
        //                        .AsQueryable();

        //    // Apply search if it's not null or empty
        //    if (!string.IsNullOrEmpty(search))
        //    {
        //        query = query.Where(es =>
        //            es.Employee.FirstName.Contains(search) ||   // search by employee name
        //            es.Employee.LastName.Contains(search)
                     
        //        );
        //    }

        //    // Execute the query
        //    var shifts = await query.ToListAsync();

        //    return View(shifts);
        //}


     

[RequireUrlPermission]
    public async Task<IActionResult> Index(string search, int? page)
    {
        // Base query with Employee navigation property
        var query = _context.EmployeeShifts
                            .Include(es => es.Employee)
                            .Include(es => es.Shift)
                            .AsQueryable();

        // Apply search if provided
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(es =>
                es.Employee.FirstName.Contains(search) ||
                es.Employee.LastName.Contains(search)
            );
        }

        // Pagination setup
        int pageSize = 15;             // Number of items per page
        int pageNumber = page ?? 1;    // Current page, default = 1

        // Execute query as paged list
        var pagedShifts = await query
            .OrderBy(es => es.id)       // optional ordering
            .ToPagedListAsync(pageNumber, pageSize);

        return View(pagedShifts);
    }

    [RequireUrlPermission]
    public IActionResult Export(string search, string downloadToken)
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

        var query = _context.EmployeeShifts
            .Include(es => es.Employee)
            .Include(es => es.Shift)
            .AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(es =>
                es.Employee.FirstName.Contains(search) ||
                es.Employee.LastName.Contains(search));
        }

        var data = query.OrderBy(es => es.id).ToList();

        OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
        using var package = new OfficeOpenXml.ExcelPackage();
        var ws = package.Workbook.Worksheets.Add("EmployeeShifts");

        ws.Cells[1, 1].Value = "Employee Code";
        ws.Cells[1, 2].Value = "Employee Name";
        ws.Cells[1, 3].Value = "Shift";
        ws.Cells[1, 4].Value = "Group";
        ws.Cells[1, 5].Value = "Effective From";
        ws.Cells[1, 6].Value = "Effective To";
        ws.Cells[1, 7].Value = "Rest Day";

        for (int i = 0; i < data.Count; i++)
        {
            var row = i + 2;
            var item = data[i];
            ws.Cells[row, 1].Value = item.Employee?.Code;
            ws.Cells[row, 2].Value = item.Employee != null ? $"{item.Employee.FirstName} {item.Employee.LastName}" : "";
            ws.Cells[row, 3].Value = item.Shift?.ShiftName;
            ws.Cells[row, 4].Value = item.Shift?.GroupName?.ToString();
            ws.Cells[row, 5].Value = item.EffiectiveForm.ToString("yyyy-MM-dd");
            ws.Cells[row, 6].Value = item.EffiectiveTo?.ToString("yyyy-MM-dd") ?? "";
            ws.Cells[row, 7].Value = item.RestDay.ToString();
        }

        ws.Cells[ws.Dimension.Address].AutoFitColumns();
        var bytes = package.GetAsByteArray();
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "EmployeeShifts.xlsx");
    }

    [RequireUrlPermission]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var employeeShift = await _context.EmployeeShifts
                .Include(es => es.Employee)
                .ThenInclude(e => e.Department)
                .FirstOrDefaultAsync(m => m.id == id);

            if (employeeShift == null) return NotFound();

            // Fetch shift name
            ViewBag.ShiftName = _context.shifts.FirstOrDefault(s => s.Id == employeeShift.ShiftId)?.ShiftName ?? "Unknown";

            return View(employeeShift);
        }

        [RequireUrlPermission]
        public IActionResult Create()
        {
            ViewBag.Departments = _context.department
                .Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Name })
                .ToList();
            ViewBag.Shifts = _context.shifts
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.ShiftName + " - " + s.GroupName })
                .ToList();
            return View();
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

            if (!hasDept && !hasQuery)
            {
                var defaults = query
                    .OrderBy(e => e.FirstName)
                    .Take(20)
                    .ToList();

                return PartialView("_EmployeeSearchResults", defaults);
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

            return PartialView("_EmployeeSearchResults", results);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int[] EmployeeIds, int ShiftId, DateOnly EffiectiveForm, DateOnly? EffiectiveTo, DayOfWeek RestDay)
        {
            if (EmployeeIds != null && EmployeeIds.Length > 0 && ShiftId > 0)
            {
                var employeesWithActiveShifts = await _context.EmployeeShifts
                    .Where(es => EmployeeIds.Contains(es.EmployeeId) && es.EffiectiveTo == null)
                    .Select(es => es.EmployeeId)
                    .Distinct()
                    .ToListAsync();

                if (employeesWithActiveShifts.Any())
                {
                    var names = await _context.emp
                        .Where(e => employeesWithActiveShifts.Contains(e.Id))
                        .Select(e => e.FirstName + " " + e.LastName)
                        .ToListAsync();
                    
                    var message = "The following employees already have an active shift: " + string.Join(", ", names) + ". Please set the 'Effective To' date on their existing shifts before adding a new one.";
                    ModelState.AddModelError("", message);
                    TempData["Error"] = message;
                }

                if (ModelState.IsValid)
                {
                    var createdBy = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;
                    var now = DateTime.Now;

                    foreach (var empId in EmployeeIds)
                    {
                        var employeeShift = new EmployeeShifts
                        {
                            EmployeeId = empId,
                            ShiftId = ShiftId,
                            EffiectiveForm = EffiectiveForm,
                            EffiectiveTo = EffiectiveTo,
                            RestDay = RestDay,
                            createdby = createdBy,
                            createat = now
                        };
                        _context.Add(employeeShift);
                    }

                    await _context.SaveChangesAsync();
                    TempData["Success"] = $"Shift assigned to {EmployeeIds.Length} employee(s).";
                    return RedirectToAction(nameof(Index));
                }
            }
            else if (EmployeeIds == null || EmployeeIds.Length == 0)
            {
                ModelState.AddModelError("", "Please select at least one employee.");
            }
            else
            {
                TempData["Error"] = "Unable to save shift assignment. Please review the form.";
            }

            // Repopulate ViewBags for return
            ViewBag.Departments = await _context.department
                .Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Name })
                .ToListAsync();
            ViewBag.Shifts = await _context.shifts
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.ShiftName + " - " + s.GroupName })
                .ToListAsync();

            if (EmployeeIds != null && EmployeeIds.Length > 0)
            {
                ViewBag.SelectedEmployees = await _context.emp
                    .Where(e => EmployeeIds.Contains(e.Id))
                    .Select(e => new SelectListItem { Value = e.Id.ToString(), Text = e.FirstName + " " + e.LastName + " (" + e.Code + ")" })
                    .ToListAsync();
            }

            return View();
        }

        [RequireUrlPermission]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var employeeShift = await _context.EmployeeShifts
                .Include(es => es.Employee)
                .ThenInclude(e => e.Department)
                .FirstOrDefaultAsync(es => es.id == id);
            if (employeeShift == null) return NotFound();

            ViewBag.EmployeeDisplayName = employeeShift.Employee != null
                ? $"{employeeShift.Employee.FirstName} {employeeShift.Employee.LastName} ({employeeShift.Employee.Code})"
                : "Unknown Employee";
            ViewBag.DepartmentName = employeeShift.Employee?.Department?.Name ?? "-";
            ViewBag.Shifts = _context.shifts
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.ShiftName + " - " + s.GroupName })
                .ToList();
            return View(employeeShift);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int[] EmployeeIds, int ShiftId, DateOnly EffiectiveForm, DateOnly? EffiectiveTo, DayOfWeek RestDay)
        {
            var employeeShift = await _context.EmployeeShifts.FindAsync(id);
            if (employeeShift == null) return NotFound();

            if (EmployeeIds == null || EmployeeIds.Length == 0)
            {
                ModelState.AddModelError("", "Please select at least one employee.");
            }

            if (ModelState.IsValid)
            {
                var otherActiveShifts = await _context.EmployeeShifts
                    .Where(es => EmployeeIds.Contains(es.EmployeeId) && es.EffiectiveTo == null && es.id != id)
                    .Select(es => es.Employee.FirstName + " " + es.Employee.LastName)
                    .ToListAsync();

                if (otherActiveShifts.Any() && EffiectiveTo == null)
                {
                    var message = "The following employees already have active shifts: " + string.Join(", ", otherActiveShifts);
                    ModelState.AddModelError("", message);
                    TempData["Error"] = message;
                }
                else
                {
                    try
                    {
                        var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;
                        var now = DateTime.Now;

                        employeeShift.EmployeeId = EmployeeIds[0];
                        employeeShift.ShiftId = ShiftId;
                        employeeShift.EffiectiveForm = EffiectiveForm;
                        employeeShift.EffiectiveTo = EffiectiveTo;
                        employeeShift.RestDay = RestDay;
                        employeeShift.updatedby = currentUserId;
                        employeeShift.updateat = now;

                        _context.Update(employeeShift);

                        if (EmployeeIds.Length > 1)
                        {
                            for (int i = 1; i < EmployeeIds.Length; i++)
                            {
                                var newShift = new EmployeeShifts
                                {
                                    EmployeeId = EmployeeIds[i],
                                    ShiftId = ShiftId,
                                    EffiectiveForm = EffiectiveForm,
                                    EffiectiveTo = EffiectiveTo,
                                    RestDay = RestDay,
                                    createdby = currentUserId,
                                    createat = now
                                };
                                _context.Add(newShift);
                            }
                        }

                        await _context.SaveChangesAsync();
                        TempData["Success"] = "Shift assignment updated successfully.";
                        return RedirectToAction(nameof(Index));
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!EmployeeShiftExists(employeeShift.id)) return NotFound();
                        else throw;
                    }
                }
            }
            else
            {
                TempData["Error"] = "Unable to update shift assignment. Please review the form.";
            }

            // Repopulate ViewBags for return
            ViewBag.Shifts = await _context.shifts
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.ShiftName + " - " + s.GroupName })
                .ToListAsync();
            var selectedEmployee = await _context.emp
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.Id == employeeShift.EmployeeId);
            ViewBag.EmployeeDisplayName = selectedEmployee != null
                ? $"{selectedEmployee.FirstName} {selectedEmployee.LastName} ({selectedEmployee.Code})"
                : "Unknown Employee";
            ViewBag.DepartmentName = selectedEmployee?.Department?.Name ?? "-";

            return View(employeeShift);
        }

        [RequireUrlPermission]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var employeeShift = await _context.EmployeeShifts
                .Include(es => es.Employee)
                .FirstOrDefaultAsync(m => m.id == id);

            if (employeeShift == null) return NotFound();

            ViewBag.ShiftName = _context.shifts.FirstOrDefault(s => s.Id == employeeShift.ShiftId)?.ShiftName ?? "Unknown";

            return View(employeeShift);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employeeShift = await _context.EmployeeShifts.FindAsync(id);
            if (employeeShift != null)
            {
                _context.EmployeeShifts.Remove(employeeShift);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Shift assignment deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeShiftExists(int id)
        {
            return _context.EmployeeShifts.Any(e => e.id == id);
        }
    }
}

