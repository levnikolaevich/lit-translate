using SDT.Data.Enums;
using System.Security.Claims;

namespace SDT.Bl.Helpers
{
    public static class AuthHelper
    {
        public static bool IsRoleAvailable(ClaimsPrincipal user, params UserRole[] roles)
        {
            return roles.ToList().Exists(role => user.IsInRole(role.ToString()));
        }
    }
}