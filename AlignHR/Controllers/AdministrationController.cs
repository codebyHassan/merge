using AlignHR.Data;
using Microsoft.AspNetCore.Mvc;

namespace AlignHR.Controllers
{
    public class AdministrationController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public AdministrationController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.TotalUsers       = _context.users.Count();
            ViewBag.ActiveUsers      = _context.users.Count(u => u.UsrIsActive == "Yes");
            ViewBag.TotalRoles       = _context.roles.Count();
            ViewBag.TotalFunctions   = _context.functions.Count();
            ViewBag.TotalValuesets   = _context.valuesets.Count();
            ViewBag.TotalUserRoles   = _context.UserRoles.Count();
            ViewBag.TotalFunctionRoles = _context.FunctionRoles.Count();

            return View();
        }
    }
}
