using AspNetCoreRateLimit;
using CustomerService.Common.Models.Constant;
using Microsoft.AspNetCore.Http;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerService.API.RateLimit
{
    /// <summary>
    /// A resolver contributors to extract ClientId from JWT.
    /// </summary>
    public class JWTClientIdResolveContributor: IClientResolveContributor
    {
        /// <summary>
        /// 
        /// </summary>
        public JWTClientIdResolveContributor() { }

        /// <summary>
        /// This is an override of ResolveClientAsync to extract ClientId from JWT.
        /// </summary>
        public Task<string> ResolveClientAsync(HttpContext httpContext)
        {
            string clientId = null;
            if (httpContext.Request.Headers.TryGetValue("Authorization", out var authorizationValue))
            {
                var jwtToken = authorizationValue.First()?.Replace("Bearer ","");
                var token = new JwtSecurityTokenHandler().ReadJwtToken(jwtToken);
                clientId = token?.Claims?.First(c => c.Type == JWTKeyConstant.AuthClientID)?.Value;
            }

            return Task.FromResult(clientId);
        }
    }
}
