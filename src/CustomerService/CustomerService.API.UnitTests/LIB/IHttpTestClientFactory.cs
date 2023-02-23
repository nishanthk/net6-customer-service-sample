using System.Net.Http;

namespace CustomerService.API.UnitTests.Lib
{
    /// <summary>
    /// Special interface for testing purposes only. Allows overriding of htpp client factory for use in tests with TestHost
    /// </summary>
    public interface IHttpTestClientFactory : IHttpClientFactory
    {
    }

}