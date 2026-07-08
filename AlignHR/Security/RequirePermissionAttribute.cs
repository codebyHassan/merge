using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AlignHR.Security
{
    /// <summary>
    /// Custom action filter attribute to require specific permissions for controller actions.
    /// Usage:
    /// [RequirePermission("USERS_VIEW")]
    /// public IActionResult Index() { }
    ///
    /// [RequirePermission("USERS_CREATE", "USERS_EDIT")]  // Requires ANY of these
    /// public IActionResult Create() { }
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class RequirePermissionAttribute : ActionFilterAttribute
    {
        private readonly string[] _requiredFunctions;
        private readonly bool _requireAll;

        /// <summary>
        /// Require one or more permissions. By default, user needs ANY of the specified permissions.
        /// </summary>
        public RequirePermissionAttribute(params string[] functions)
        {
            _requiredFunctions = functions;
            _requireAll = false;
        }

        /// <summary>
        /// Set to true if user must have ALL specified permissions (default is ANY)
        /// </summary>
        public bool RequireAll
        {
            get => _requireAll;
            init => _requireAll = value;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var httpContext = context.HttpContext;

            // Check if user is logged in
            if (!PermissionHelper.IsLoggedIn(httpContext))
            {
                context.Result = new RedirectToActionResult("Login", "User", null);
                return;
            }

            // Check if user has required permission(s)
            var session = PermissionHelper.GetUserSession(httpContext);
            if (session == null)
            {
                context.Result = new RedirectToActionResult("Login", "User", null);
                return;
            }

            bool hasPermission;
            if (_requireAll)
            {
                hasPermission = _requiredFunctions.All(f => session.HasFunction(f));
            }
            else
            {
                hasPermission = _requiredFunctions.Any(f => session.HasFunction(f));
            }

            if (!hasPermission)
            {
                // FALLBACK: check if they have access to the current URL
                var currentPath = httpContext.Request.Path.Value;
                if (!string.IsNullOrEmpty(currentPath) && PermissionHelper.HasUrlAccess(httpContext, currentPath))
                {
                    hasPermission = true;
                }
            }

            if (!hasPermission)
            {
                context.Result = new RedirectToActionResult("AccessDenied", "User", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }

    /// <summary>
    /// Attribute to require user to have access to the current URL.
    /// Access is checked against the user's session AllowedUrls.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class RequireUrlPermissionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Check if action is marked with [SkipPermissionCheck]
            var actionDescriptor = context.ActionDescriptor as Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor;
            if (actionDescriptor != null)
            {
                var skipCheck = actionDescriptor.MethodInfo.GetCustomAttributes(typeof(SkipPermissionCheckAttribute), false).Any();
                if (skipCheck)
                {
                    base.OnActionExecuting(context);
                    return;
                }
            }

            var httpContext = context.HttpContext;

            // 1. Check if user is logged in
            if (!PermissionHelper.IsLoggedIn(httpContext))
            {
                context.Result = new RedirectToActionResult("Login", "User", null);
                return;
            }

            // 2. Extract current path
            var currentPath = httpContext.Request.Path.Value;

            // 3. Check URL access
            if (!string.IsNullOrEmpty(currentPath) && !PermissionHelper.HasUrlAccess(httpContext, currentPath))
            {
                // Export is gated by the corresponding Index permission — no separate grant needed
                if (actionDescriptor?.ActionName.Equals("Export", StringComparison.OrdinalIgnoreCase) == true)
                {
                    var controllerSegment = currentPath.TrimStart('/').Split('/').FirstOrDefault() ?? "";
                    if (PermissionHelper.HasUrlAccess(httpContext, $"/{controllerSegment}/Index"))
                    {
                        base.OnActionExecuting(context);
                        return;
                    }
                }

                context.Result = new RedirectToActionResult("AccessDenied", "User", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }

    /// <summary>
    /// Attribute to require user to be logged in (but no specific permission).
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class RequireLoginAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!PermissionHelper.IsLoggedIn(context.HttpContext))
            {
                context.Result = new RedirectToActionResult("Login", "User", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }

    /// <summary>
    /// Attribute to skip the URL permission check for specific actions.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class SkipPermissionCheckAttribute : Attribute
    {
    }
}
