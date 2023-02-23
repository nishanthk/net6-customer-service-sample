using AspNetCoreRateLimit;
using Microsoft.Extensions.Options;

namespace CustomerService.API.RateLimit
{
    /// <summary>
    /// Customomize resolver contributors so we can customize the extraction of the IP or ClientId.
    /// </summary>
    public class RateLimitConfiguration : RateLimitConfiguration
    {
        /// <summary>
        /// 
        /// </summary>
        public RateLimitConfiguration(IOptions<IpRateLimitOptions> ipOptions, IOptions<ClientRateLimitOptions> clientOptions)
            : base(ipOptions, clientOptions) { }

        /// <summary>
        /// Override RegisterResolvers where the customize classes can be registered.
        /// </summary>
        public override void RegisterResolvers()
        {
            base.RegisterResolvers();

            ClientResolvers.Add(new JWTClientIdResolveContributor());
        }
    }
}
