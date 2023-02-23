namespace CustomerService.Common.Models.Configuration
{
    public class MilestoneAPIClientOptions : APIClientOptionBase
    {
        public MilestoneAPIClientOptions(
            string clientId,
            string authority, 
            string audience,
            string clientSecret)
        {
            Auth0ClientId = clientId;
            Auth0Authority = authority;
            Auth0Audience = audience;
            Auth0ClientSecret = clientSecret;
        }
    }
}