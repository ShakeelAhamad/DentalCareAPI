using System.Security.Claims;

namespace DentalCareAPI.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            var userIdClaims = user.FindFirst(ClaimTypes.NameIdentifier) ?? user.FindFirst("sub");
            if(userIdClaims == null || !int.TryParse(userIdClaims.Value,out int userId))
            {
                throw new UnauthorizedAccessException("User ID not found in token");
            }
            return userId;
        }
    }
}
