namespace CustomerService.Common.Models.Configuration
{
    public abstract class APIClientOptionBase
    {
        public string Auth0ClientId { get; set; }
        public string Auth0Authority { get; set; }
        public string Auth0Audience { get; set; }
        public string Auth0ClientSecret { get; set; }
    }
}