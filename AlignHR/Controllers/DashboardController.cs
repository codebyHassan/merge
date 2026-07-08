using Microsoft.AspNetCore.Mvc;
using AlignHR.Models;
using AlignHR.Data;

namespace AlignHR.Controllers
{

    public class DashboardController : BaseController
    {
        private readonly ApplicationDbContext _context;
        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var now = DateTime.Now;
            ViewBag.TotalEmployees = _context.emp.Count();
            ViewBag.ActiveEmployees = _context.emp.Count(e => e.EmploymentStatus != null && e.EmploymentStatus.Name == "Active");
            ViewBag.NewHiresThisMonth = _context.emp.Count(e => e.createat.Month == now.Month && e.createat.Year == now.Year);
            ViewBag.TotalDepartments = _context.department.Count();
            ViewBag.TotalLocations = _context.location.Count();
            
            // Fetch Department Distribution
            var deptStats = _context.department
                .Select(d => new {
                    Name = d.Name,
                    Count = d.Employees.Count()
                })
                .Where(d => d.Count > 0)
                .ToList();

            ViewBag.DeptLabels = deptStats.Select(d => d.Name).ToArray();
            ViewBag.DeptData = deptStats.Select(d => d.Count).ToArray();

            return View();
        }
    }
}

