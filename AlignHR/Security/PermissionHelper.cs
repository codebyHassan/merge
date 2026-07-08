using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace AlignHR.Security
{
    /// <summary>
    /// Static helper class for managing user session and permissions.
    /// Provides methods to store, retrieve, and check user permissions.
    /// </summary>
    public static class PermissionHelper
    {
        private const string SessionKey = "UserSession";
        private const string CacheKey = "CachedUserSession";

        /// <summary>
        /// Store user session data in the HTTP session as JSON.
        /// Call this after successful login.
        /// </summary>
        public static void SetUserSession(HttpContext context, UserSession session)
        {
            var json = JsonSerializer.Serialize(session);
            context.Session.SetString(SessionKey, json);
            
            // Cache it for the current request
            context.Items[CacheKey] = session;
        }

        /// <summary>
        /// Retrieve user session data from the HTTP session.
        /// Returns null if user is not logged in.
        /// Optimized with request-level caching.
        /// </summary>
        public static UserSession? GetUserSession(HttpContext context)
        {
            // 1. Check if we already loaded it during THIS request
            if (context.Items.ContainsKey(CacheKey))
                return context.Items[CacheKey] as UserSession;

            // 2. Otherwise, fetch from Session (Expensive JSON Deserialization)
            var json = context.Session.GetString(SessionKey);
            if (string.IsNullOrEmpty(json))
                return null;

            try
            {
                var session = JsonSerializer.Deserialize<UserSession>(json);
                
                // 3. Store in Request Items so next calls are instant
                if (session != null)
                    context.Items[CacheKey] = session;

                return session;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Check if the current user has a specific permission/function.
        /// </summary>
        public static bool HasPermission(HttpContext context, string functionName)
        {
            var session = GetUserSession(context);
            return session?.HasFunction(functionName) ?? false;
        }

        /// <summary>
        /// Check if the current user has any of the specified permissions.
        /// </summary>
        public static bool HasAnyPermission(HttpContext context, params string[] functionNames)
        {
            var session = GetUserSession(context);
            return session?.HasAnyFunction(functionNames) ?? false;
        }

        /// <summary>
        /// Check if a user is currently logged in.
        /// </summary>
        public static bool IsLoggedIn(HttpContext context)
        {
            return GetUserSession(context) != null;
        }

        /// <summary>
        /// Clear the user session (logout).
        /// </summary>
        public static void ClearSession(HttpContext context)
        {
            context.Session.Remove(SessionKey);
            context.Session.Clear();
            context.Items.Remove(CacheKey);
        }


        /// <summary>
        /// Get the current user's ID if logged in.
        /// </summary>
        public static int? GetCurrentUserId(HttpContext context)
        {
            return GetUserSession(context)?.UserId;
        }

        /// <summary>
        /// Get the current user's username/email if logged in.
        /// </summary>
        public static string? GetCurrentUsername(HttpContext context)
        {
            return GetUserSession(context)?.Username;
        }

        /// <summary>
        /// Get the current user's full name if logged in.
        /// </summary>
        public static string? GetCurrentFullName(HttpContext context)
        {
            return GetUserSession(context)?.FullName;
        }

        /// <summary>
        /// Check if the current user has access to a specific URL.
        /// </summary>
        public static bool HasUrlAccess(HttpContext context, string url)
        {
            var session = GetUserSession(context);
            return session?.HasUrlAccess(url) ?? false;
        }

        /// <summary>
        /// Check if the current user has access to a specific controller (for sidebar rendering).
        /// Usage in Razor: PermissionHelper.HasControllerAccess(Context, "Roles")
        /// </summary>
        public static bool HasControllerAccess(HttpContext context, string controllerName)
        {
            var session = GetUserSession(context);
            return session?.HasControllerAccess(controllerName) ?? false;
        }

        /// <summary>
        /// Check if the current user has access to a specific Controller/Action (for CRUD in views).
        /// Usage in Razor: PermissionHelper.HasActionAccess(Context, "Employees", "Create")
        /// </summary>
        public static bool HasActionAccess(HttpContext context, string controllerName, string actionName)
        {
            var session = GetUserSession(context);
            return session?.HasActionAccess(controllerName, actionName) ?? false;
        }
    }
}
