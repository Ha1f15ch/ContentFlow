using System.Security.Claims;
using Microsoft.AspNetCore.Http.Abstractions;

namespace ContentFlow.Application.Security;

public static class ClaimsPrincipalExtensions
{
    public static int? GetUserId(this ClaimsPrincipal user)
    {
        var claim = user.FindFirst(ClaimTypes.NameIdentifier);
        return claim?.Value is string value && int.TryParse(value, out var id) ? id : null;
    }
    
    public static int GetAuthenticatedUserId(this ClaimsPrincipal user)
    {
        if (!user.Identity?.IsAuthenticated == true)
            throw new InvalidOperationException("User is not authenticated.");

        var claim = user.FindFirst(ClaimTypes.NameIdentifier)
                    ?? throw new InvalidOperationException("User ID claim is missing.");

        if (!int.TryParse(claim.Value, out var id))
            throw new InvalidOperationException("User ID is invalid or not a number.");

        return id;
    }
}