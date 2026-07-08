using Microsoft.AspNetCore.Mvc;

namespace AlignHR.Controllers
{
    public class SettingsController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

