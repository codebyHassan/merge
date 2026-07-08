using AlignHR.Security;
using AlignHR.Services;
using Microsoft.AspNetCore.Mvc;

namespace AlignHR.Controllers
{
    public class HrAtsDashboardController : BaseController
    {
        private readonly IHrApplicationService _service;

        public HrAtsDashboardController(IHrApplicationService service)
        {
            _service = service;
        }

        [RequireUrlPermission]
        public async Task<IActionResult> Index()
        {
            ViewBag.ActivePage = "HrAtsDashboard";
            var model = await _service.GetDashboardAsync();
            return View(model);
        }
    }
}
