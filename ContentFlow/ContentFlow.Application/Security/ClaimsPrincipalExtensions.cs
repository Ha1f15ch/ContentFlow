using System.Security;
using System.Security.Claims;

namespace ContentFlow.Application.Security;

public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirst(ClaimTypes.NameIdentifier);
        if (!int.TryParse(value.Value, out var id))
            throw new SecurityException($"Invalid or missing user ID.");
        
        return id;
    }
}