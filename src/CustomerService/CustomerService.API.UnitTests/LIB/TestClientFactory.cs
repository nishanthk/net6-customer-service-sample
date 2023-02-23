using Microsoft.AspNetCore.TestHost;
using System.Net.Http;

namespace CustomerService.API.UnitTests.Lib
{
    class TestClientFactory : IHttpTestClientFactory
    {
        private readonly TestServer _testServer;
        HttpClient _httpClient;

        public TestClientFactory(TestServer testServer)
        {
            this._testServer = testServer;
        }

        public HttpClient CreateClient(string name)
        {
            _httpClient = _testServer.CreateClient();
            return _httpClient;
        }
    }

}