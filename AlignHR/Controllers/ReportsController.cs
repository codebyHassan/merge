using Microsoft.AspNetCore.Mvc;

namespace AlignHR.Controllers
{
    public class ReportsController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

