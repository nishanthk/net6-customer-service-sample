using CustomerService.Common.Models.Constant;
using CustomerService.Common.Repository.Interfaces;
using System.Net.Http;
using System.Threading.Tasks;

namespace CustomerService.Common.Repository.Wrapper
{
    public class HttpClientWrapper: IHttpClientWrapper
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HttpClientWrapper(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage)
        {
            var httpClient = _httpClientFactory.CreateClient(HttpClientConstant.HttpClient);
            return await httpClient.SendAsync(httpRequestMessage);
        }
    }
}
