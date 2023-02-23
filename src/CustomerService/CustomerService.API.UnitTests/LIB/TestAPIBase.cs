using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Net.Http;

namespace CustomerService.API.UnitTests.Lib
{
    public abstract class TestAPIBase
    {
        IWebHostBuilder _hostBuilder;
        TestServer _testServer;
        IHttpTestClientFactory _clientFactory;

        public TestAPIBase()
        {
            _hostBuilder = new WebHostBuilder();
        }

        /// <summary>
        /// Access only after running Build()
        /// </summary>
        /// <returns></returns>
        public IHttpTestClientFactory TestClientFactory => _clientFactory ?? (_clientFactory = new TestClientFactory(_testServer));


        public TType GetService<TType>()
        {
            return _testServer.Host.Services.GetService<TType>();
        }

        public virtual void Start<TStartupConfig>()
            where TStartupConfig : class
        {
            Configure(_hostBuilder);
            _hostBuilder.ConfigureTestServices(ModifyConfiguredServices);
            _hostBuilder.UseStartup<TStartupConfig>();

            _testServer = new TestServer(_hostBuilder);
        }

        public HttpClient GetClient()
        {
            return _testServer.CreateClient();
        }

        public abstract void Configure(IWebHostBuilder webHostBuilder);
        public abstract void ModifyConfiguredServices(IServiceCollection services);

        public string Url => _testServer?.BaseAddress?.AbsoluteUri;

        public static void SetEnvFromConfig(string settingName, IConfiguration config)
        {
            var configValue = config[settingName];
            Environment.SetEnvironmentVariable(settingName, configValue);
        }
    }

}