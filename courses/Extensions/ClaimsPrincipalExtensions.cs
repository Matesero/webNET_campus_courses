using System.Security.Claims;
using courses.Middleware;

namespace courses.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var userIdClaim = user.Claims.FirstOrDefault(c => c.Type == "userId");

        if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value) || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            throw new UnauthorizedException("Invalid or missing userId in token");
        }
        return userId;
    }
}