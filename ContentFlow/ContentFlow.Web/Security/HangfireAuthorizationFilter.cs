using ContentFlow.Application.Common;
using Hangfire.Dashboard;

namespace ContentFlow.Web.Security;

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        var identity = httpContext.User.Identity;
        
        if (identity is not { IsAuthenticated: true })
            return false;

        return httpContext.User.IsInRole(RoleConstants.Admin) || 
               httpContext.User.IsInRole(RoleConstants.Moderator);
    }
}