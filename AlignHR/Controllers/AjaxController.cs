using AlignHR.Data;
using AlignHR.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlignHR.Controllers
{
    public class AjaxController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public AjaxController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> SearchEmployees(string term)
        {
            if (string.IsNullOrEmpty(term) || term.Length < 2)
            {
                return PartialView("_EmployeeSearchResults", new List<Employee>());
            }

            var employees = await _context.emp
                .Include(e => e.Department)
                .Where(e => e.FirstName.Contains(term) || 
                            e.LastName.Contains(term) || 
                            e.Code.Contains(term))
                .Take(10)
                .ToListAsync();

            return PartialView("_EmployeeSearchResults", employees);
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployeeCard(int id)
        {
            var employee = await _context.emp
                .Include(e => e.Department)
                .Include(e => e.Location)
                .Include(e => e.EmploymentType)
                .Include(e => e.EmploymentStatus)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null) return Content("");

            return PartialView("_EmployeeCard", employee);
        }
    }
}
