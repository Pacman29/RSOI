using System.Security.Claims;

namespace AuthServer.Extensions
{
    public static class UserClaimsPrincipalExtension
    {
        public static string GetJwtUserId(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}