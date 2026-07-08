using AlignHR.Data;
using AlignHR.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlignHR.Controllers
{
    public class PayrollRegisterController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public PayrollRegisterController(ApplicationDbContext context)
        {
            _context = context;
        }

        [RequireUrlPermission]
        public async Task<IActionResult> Index(int? periodId, int? departmentId, string search)
        {
            ViewBag.ActivePage = "PayrollRegister";
            
            var periods = await _context.SalaryPeriod.OrderByDescending(p => p.StartDate).ToListAsync();
            ViewBag.Periods = periods;

            var departments = await _context.department.OrderBy(d => d.Name).ToListAsync();
            ViewBag.Departments = departments;

            var query = _context.SalarySlipMasters
                .Include(s => s.Employee)
                .Include(s => s.SalaryPeriod)
                .Include(s => s.Details)
                .AsQueryable();

            if (periodId.HasValue)
            {
                ViewBag.SelectedPeriod = periodId.Value;
                query = query.Where(s => s.SalaryPeriodID == periodId.Value);
            }
            else
            {
                var emptyList = new List<AlignHR.Models.SalarySlipMaster>();
                if (Request.Headers["HX-Request"] == "true")
                {
                    return PartialView("_SlipsList", emptyList);
                }
                return View(emptyList);
            }

            if (departmentId.HasValue)
            {
                ViewBag.SelectedDepartment = departmentId.Value;
                query = query.Where(s => s.Employee.DepartmentFk == departmentId.Value);
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(s => s.EmployeeNameSnapshot.Contains(search) || 
                                       s.Employee.Code.Contains(search) || 
                                       s.DepartmentSnapshot.Contains(search));
            }

            var slips = await query.OrderBy(s => s.Employee.FirstName).ToListAsync();

            if (Request.Headers["HX-Request"] == "true")
            {
                return PartialView("_SlipsList", slips);
            }

            return View(slips);
        }

        [RequireUrlPermission]
        public async Task<IActionResult> Print(int id)
        {
            var slip = await _context.SalarySlipMasters
                .Include(s => s.Employee)
                .Include(s => s.SalaryPeriod)
                .Include(s => s.Details)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (slip == null)
                return NotFound();

            return View(slip);
        }
    }
}
