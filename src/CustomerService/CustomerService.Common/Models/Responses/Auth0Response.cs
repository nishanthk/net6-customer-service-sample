namespace CustomerService.Common.Models.Responses
{
    public class Auth0Response
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string expires_in { get; set; }
    }
}
