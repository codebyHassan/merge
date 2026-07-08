using AlignHR.Data;
using AlignHR.Models;
using AlignHR.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace AlignHR.Controllers
{
    public class AttendenceExceptionController : BaseController
    {

        private readonly ApplicationDbContext _context;

        public AttendenceExceptionController(ApplicationDbContext context)
        {
            _context = context; 
        }
        public IActionResult Index(string search, int ?page)
        {
            int pageSize = 15;
            int pageNumber = page ?? 1;
            var obj = _context.AttendenceException
                .Include(d => d.Employee)
                .Include(d => d.ExpectionType)
                .AsQueryable(); 

            if (!string.IsNullOrEmpty(search))
            {
                obj = obj.Where(d => d.Employee.FirstName.Contains(search) || d.Employee.LastName.Contains(search) || d.Employee.Code.Contains(search));
            }
            obj = obj.OrderBy(d => d.id);

            // Convert to paged list
            var model = obj.ToPagedList(pageNumber, pageSize);

            return View(model);
        }
        public IActionResult create()
        {
            return View(); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult create(AttendenceException attendenceException)
        {
            return View();
        }
    }
}


