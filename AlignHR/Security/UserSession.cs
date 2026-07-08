namespace AlignHR.Security
{
    /// <summary>
    /// Model to store user session data including roles and permissions.
    /// This is serialized to JSON and stored in the session after login.
    /// </summary>
    public class UserSession
    {
        /// <summary>User's unique identifier</summary>
        public int UserId { get; set; }

        /// <summary>User's login email</summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>User's full display name</summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>List of role names assigned to the user</summary>
        public List<string> Roles { get; set; } = new List<string>();

        /// <summary>List of function names assigned to the user</summary>
        public List<string> Functions { get; set; } = new List<string>();

        /// <summary>List of allowed URLs for this user</summary>
        public List<string> AllowedUrls { get; set; } = new List<string>();

        /// <summary>List of menu items for the sidebar</summary>
        public List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();

        /// <summary>Linked Employee ID</summary>
        public int? EmployeeId { get; set; }

        /// <summary>Linked Employee's Department ID</summary>
        public int? DepartmentId { get; set; }

        /// <summary>Whether the user's linked employee can apply for leaves for others</summary>
        public bool CanApplyForOthers { get; set; }

        /// <summary>Check if user has a specific function/permission</summary>
        public bool HasFunction(string functionName)
        {
            return Functions.Contains(functionName, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>Check if user has any of the specified functions</summary>
        public bool HasAnyFunction(params string[] functionNames)
        {
            return functionNames.Any(f => HasFunction(f));
        }

        /// <summary>Check if user has a specific role</summary>
        public bool HasRole(string roleName)
        {
            return Roles.Contains(roleName, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Check if user has access to a URL. Matches at the ACTION level:
        /// Request "/Employees/Edit/5" → extracts "employees/edit" and matches against stored route "/Employees/Edit"
        /// </summary>
        public bool HasUrlAccess(string url)
        {
            if (string.IsNullOrEmpty(url)) return false;

            // Super Admin bypass
            if (Roles.Contains("Super Admin", StringComparer.OrdinalIgnoreCase))
                return true;

            // Normalize path
            var path = url.TrimStart('~').TrimStart('/').TrimEnd('/');

            // Handle root path
            if (string.IsNullOrEmpty(path))
            {
                path = "dashboard/index";
            }

            var parts = path.ToLower().Split('/');
            var requestController = parts.Length > 0 ? parts[0] : "";
            var requestAction = parts.Length > 1 ? parts[1] : "index";

            // Special case: if it's just controller name like "/Employees", treat it as "/Employees/Index"
            if (string.IsNullOrEmpty(requestAction)) requestAction = "index";

            return AllowedUrls.Any(u =>
            {
                var allowedPath = u.TrimStart('~').TrimStart('/').TrimEnd('/');
                var allowedParts = allowedPath.ToLower().Split('/');
                var allowedController = allowedParts.Length > 0 ? allowedParts[0] : "";
                var allowedAction = allowedParts.Length > 1 ? allowedParts[1] : "index";

                return requestController == allowedController && requestAction == allowedAction;
            });
        }

        /// <summary>
        /// Check if user has access to a specific controller (any action). Used for sidebar visibility.
        /// </summary>
        public bool HasControllerAccess(string controllerName)
        {
            if (string.IsNullOrEmpty(controllerName)) return false;

            if (Roles.Contains("Super Admin", StringComparer.OrdinalIgnoreCase))
                return true;

            return AllowedUrls.Any(u =>
            {
                var controller = u.TrimStart('~').TrimStart('/').ToLower().Split('/').FirstOrDefault() ?? "";
                return controller == controllerName.ToLower();
            });
        }

        /// <summary>
        /// Check if user has access to a specific Controller/Action combination.
        /// Usage: session.HasActionAccess("Employees", "Create")
        /// </summary>
        public bool HasActionAccess(string controllerName, string actionName)
        {
            if (string.IsNullOrEmpty(controllerName)) return false;

            if (Roles.Contains("Super Admin", StringComparer.OrdinalIgnoreCase))
                return true;

            var targetController = controllerName.ToLower();
            var targetAction = actionName.ToLower();

            return AllowedUrls.Any(u =>
            {
                var parts = u.TrimStart('~').TrimStart('/').ToLower().TrimEnd('/').Split('/');
                var c = parts.Length > 0 ? parts[0] : "";
                var a = parts.Length > 1 ? parts[1] : "index";
                return c == targetController && a == targetAction;
            });
        }
    }
}
