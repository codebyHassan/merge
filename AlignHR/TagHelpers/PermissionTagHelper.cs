using Microsoft.AspNetCore.Razor.TagHelpers;
using AlignHR.Security;

namespace AlignHR.TagHelpers
{
    /// <summary>
    /// Tag Helper to conditionally render content based on user permissions.
    ///
    /// Usage:
    /// &lt;permission function="USERS_CREATE"&gt;
    ///     &lt;button&gt;Create User&lt;/button&gt;
    /// &lt;/permission&gt;
    ///
    /// Multiple functions (user needs ANY):
    /// &lt;permission function="USERS_CREATE,USERS_EDIT"&gt;
    ///     &lt;button&gt;Save&lt;/button&gt;
    /// &lt;/permission&gt;
    ///
    /// Check by URL:
    /// &lt;permission url="/Users/Create"&gt;
    ///     &lt;button&gt;Create User&lt;/button&gt;
    /// &lt;/permission&gt;
    /// </summary>
    [HtmlTargetElement("permission", Attributes = "function")]
    [HtmlTargetElement("permission", Attributes = "url")]
    public class PermissionTagHelper : TagHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PermissionTagHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// The function name(s) required to view this content.
        /// Comma-separated for multiple functions.
        /// </summary>
        [HtmlAttributeName("function")]
        public string Function { get; set; } = string.Empty;

        /// <summary>
        /// The URL/Address required to view this content.
        /// </summary>
        [HtmlAttributeName("url")]
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// If true, user must have ALL specified functions.
        /// If false (default), user needs ANY of the specified functions.
        /// </summary>
        [HtmlAttributeName("require-all")]
        public bool RequireAll { get; set; } = false;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // Remove the <permission> tag wrapper, keeping only inner content (if authorized)
            output.TagName = null;

            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                output.SuppressOutput();
                return;
            }

            var session = PermissionHelper.GetUserSession(httpContext);
            if (session == null)
            {
                output.SuppressOutput();
                return;
            }

            bool hasPermission = false;

            // Check by URL if provided
            if (!string.IsNullOrEmpty(Url))
            {
                hasPermission = session.HasUrlAccess(Url);
            }

            // Check by Function if provided
            if (!hasPermission && !string.IsNullOrEmpty(Function))
            {
                var functions = Function.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (functions.Length > 0)
                {
                    if (RequireAll)
                    {
                        hasPermission = functions.All(f => session.HasFunction(f));
                    }
                    else
                    {
                        hasPermission = functions.Any(f => session.HasFunction(f));
                    }
                }
            }

            if (!hasPermission)
            {
                output.SuppressOutput();
            }
        }
    }

    /// <summary>
    /// Tag Helper to show content only if user is logged in.
    ///
    /// Usage:
    /// &lt;logged-in&gt;
    ///     &lt;span&gt;Welcome!&lt;/span&gt;
    /// &lt;/logged-in&gt;
    /// </summary>
    [HtmlTargetElement("logged-in")]
    public class LoggedInTagHelper : TagHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoggedInTagHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = null;

            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null || !PermissionHelper.IsLoggedIn(httpContext))
            {
                output.SuppressOutput();
            }
        }
    }

    /// <summary>
    /// Tag Helper to show content only if user is NOT logged in.
    ///
    /// Usage:
    /// &lt;logged-out&gt;
    ///     &lt;a href="/User/Login"&gt;Login&lt;/a&gt;
    /// &lt;/logged-out&gt;
    /// </summary>
    [HtmlTargetElement("logged-out")]
    public class LoggedOutTagHelper : TagHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoggedOutTagHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = null;

            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null && PermissionHelper.IsLoggedIn(httpContext))
            {
                output.SuppressOutput();
            }
        }
    }
}
