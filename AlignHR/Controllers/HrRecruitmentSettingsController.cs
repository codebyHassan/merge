using AlignHR.Security;
using Microsoft.AspNetCore.Mvc;

namespace AlignHR.Controllers
{
    public class HrRecruitmentSettingsController : BaseController
    {
        [RequireUrlPermission]
        public IActionResult Index()
        {
            return RedirectToAction("Index", "HrRecruitmentUnits");
        }
    }
}
