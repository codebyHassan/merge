using AlignHR.Security;
using Microsoft.AspNetCore.Mvc;

namespace AlignHR.Controllers
{
    /// <summary>
    /// Base controller for all authenticated and permission-controlled pages.
    /// Enforces that the user must be logged in and have access to the current URL.
    /// </summary>
    [RequireUrlPermission]
    public abstract class BaseController : Controller
    {
    }
}

