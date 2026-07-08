using Microsoft.AspNetCore.Mvc;

namespace AlignHR.Controllers
{
    public class HomeController : BaseController
    {
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Dashboard");
        }
    }
}

