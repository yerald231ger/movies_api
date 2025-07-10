using System.Security.Claims;

namespace Movies.Api.Auth;

public static class IdentityExtensions
{
    public static Guid? GetUserId(this ClaimsPrincipal user)
    {
        if (user.Identity is not { IsAuthenticated: true })
            return null;

        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
        
        if (userIdClaim is null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return null;

        return userId;
    }
}