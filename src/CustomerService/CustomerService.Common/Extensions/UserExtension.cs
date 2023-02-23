using CustomerService.Common.Models.Constant;
using System.Security.Claims;

namespace CustomerService.Common.Extensions
{
    public static class UserExtension
    {
        public static bool HasClientId(this ClaimsPrincipal claims) 
            => !string.IsNullOrWhiteSpace(GetValue(claims, JWTKeyConstant.AuthClientID));

        public static string GetValue(this ClaimsPrincipal claims, string type)
            => claims.FindFirst(x => x.Type == type)?.Value;
    }
}
