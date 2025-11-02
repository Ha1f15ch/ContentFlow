using Hangfire.Dashboard;

namespace ContentFlow.Web.Security;

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        // Проверяем: пользователь залогинен?
        if (!httpContext.User.Identity?.IsAuthenticated == true)
            return false;

        // Проверяем: роль Admin или Moderator?
        return httpContext.User.IsInRole("Admin") || 
               httpContext.User.IsInRole("Moderator");
    }
}