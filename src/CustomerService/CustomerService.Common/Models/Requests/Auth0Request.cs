using Newtonsoft.Json;

namespace CustomerService.Common.Models.Requests
{
    public class Auth0Request
    {
        [JsonProperty("client_id")]
        public string ClientId { get; set; }
        [JsonProperty("client_secret")]
        public string ClientSecret { get; set; }
        [JsonProperty("audience")]
        public string Audience { get; set; }
        [JsonProperty("grant_type")]
        public string GrantType { get; set; }
        [JsonProperty("user_id")]
        public string UserId { get; set; }

        public Auth0Request(string clientId, string clientSecret, string audience, string userId,
            string grantType = "client_credentials")
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
            Audience = audience;
            GrantType = grantType;
            UserId = userId;
        }
    }
}
