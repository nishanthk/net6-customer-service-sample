using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Linq;
using System;
using Microsoft.Extensions.Configuration.Json;
using System.Reflection;
using CustomerService.API.UnitTests.Lib;
using API.SDK.Authorization;
using API.SDK.Authorization.Models;
using CustomerService.Common.Repository.Interfaces;
using ASNManagement.ApiClient.V1;
using CustomerService.Common.Models.DTO;
using HTTP.Clients.Models;
using System.Threading.Tasks;
using System.Net;
using Common.SharedAppInterfaces.Exceptions;
using Newtonsoft.Json;
using MilestoneService.APIClient.V1;
using MilestoneService.DTO.API.V1;
using System.Text.Json;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Mvc;

namespace CustomerService.API.UnitTests
{
    public class APIUnderTest : TestAPIBase
    {
        private readonly TestContext _testContext;

        public APIUnderTest(TestContext testContext)
        {
            _testContext = testContext;
        }


        public void Build()
        {
            Start<Startup>();
        }

        public override void Configure(IWebHostBuilder webHostBuilder)
        {
            var configuration = new ConfigurationBuilder()
                                 .AddJsonFile("launchSettings.json", true, true)
                                 .Build();

            var provider = (JsonConfigurationProvider)configuration.Providers.First();
            var dataElement = provider.GetType().GetProperty("Data", BindingFlags.Instance | BindingFlags.NonPublic);
            var dataValue = (Dictionary<string, string>)dataElement.GetValue(provider);
            foreach (var pair in dataValue)
            {
                Environment.SetEnvironmentVariable(pair.Key, pair.Value);
            }

            webHostBuilder.UseConfiguration(configuration)
                .UseEnvironment("LOCAL");
        }

        public override void ModifyConfiguredServices(IServiceCollection services)
        {
            var equipmentPackApiClientMock = new Mock<IEquipmentPackApiClient>();
            equipmentPackApiClientMock.Setup(m => m.Create(It.IsAny<ASNManagement.Models.DTO.V1.EquipmentPackDTO>(), It.IsAny<HttpRequestOptions>(), It.IsAny<CancellationToken>()))
                .Returns(GetApiResponse());
            services.ReplaceWithMock<IEquipmentPackApiClient>(equipmentPackApiClientMock.Object);

            var MilestonesApiClient = new Mock<IMilestonesApiClient>();
            MilestonesApiClient.Setup(m => m.RegisterToNonBooking(It.IsAny<string>(), It.IsAny<NonBookingRegistrationDTO>(), It.IsAny<CancellationToken>()))
                .Returns(() => GetMilestoneNonRegistrationApiResponse());

            MilestonesApiClient.Setup(m => m.GetOrderMilestones(It.IsAny<string>(), It.IsAny<GetOrderMilestonesRequest>(), It.IsAny<CancellationToken>()))
                .Returns(() => GetOrderMilestoneApiResponse());

            services.ReplaceWithMock<IMilestonesApiClient>(MilestonesApiClient.Object);

            var customerAuthRepositoryMock = new Mock<ICustomerAuthRepository>();
            customerAuthRepositoryMock.Setup(p => p.GetByClientId())
                .ReturnsAsync(() => new CustomerAuthenticationDTO { ClientId = Guid.NewGuid().ToString(), UserId = "testuserId" });
            services.ReplaceWithMock<ICustomerAuthRepository>(customerAuthRepositoryMock.Object);

            var rateLimitMock = new Mock<IRateLimitConfiguration>();
            services.ReplaceWithMock<IRateLimitConfiguration>(rateLimitMock.Object);

            MockAuthentication(services);
        }

        private Task<ApiResponse<bool>> GetApiResponse()
        {
            return Task.FromResult(new ApiResponse<bool>(true, null, HttpStatusCode.NoContent, null, null));
        }

        private Task<ApiResponse<bool>> GetMilestoneNonRegistrationApiResponse()
        {
            return Task.FromResult(_nonKothiRegistrationResponse);
        }

        ApiResponse<bool> _nonKothiRegistrationResponse = new ApiResponse<bool>(true, null, HttpStatusCode.NoContent, null, null);
        public void SetNonKothiRegistrationResponseCode(HttpStatusCode statusCode)
        {
            var problemDetails = new { type = "asdas", detail = "", title = "1231", status = "s", errors = "" };
            _nonKothiRegistrationResponse = new ApiResponse<bool>(true, JsonConvert.SerializeObject(problemDetails), statusCode, null, null);
        }

        private Task<ApiResponse<IList<OrderMilestoneDTO>>> GetOrderMilestoneApiResponse()
        {
            return Task.FromResult(_getOrderMilestoneApiResponse);
        }

        ApiResponse<IList<OrderMilestoneDTO>> _getOrderMilestoneApiResponse = new ApiResponse<IList<OrderMilestoneDTO>>(null, null, HttpStatusCode.OK, null, null);
        public void SetOrderMilestoneApiResponseCode(IList<OrderMilestoneDTO> orderMilestoneDTOs, HttpStatusCode statusCode)
        {
            _getOrderMilestoneApiResponse = new ApiResponse<IList<OrderMilestoneDTO>>(orderMilestoneDTOs, null, statusCode, null, null);
        }

        /// <summary>
        /// Mock the service Authentication and Authorization, we do not test them in the DTO mapping integration tests.
        /// </summary>
        /// <param name="services"></param>
        private void MockAuthentication(IServiceCollection services)
        {
            services.AddAuthentication("Test")
                .AddScheme<AuthenticationSchemeOptions, MockAuthentication>("Test", options => { });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("TestAuthenticationPolicy", builder =>
                {
                    builder.AuthenticationSchemes.Add("Test");
                    builder.RequireAuthenticatedUser();
                });
                options.DefaultPolicy = options.GetPolicy("TestAuthenticationPolicy");
            });

            var claimHelperMock = new Mock<IClaimHelper>();
            claimHelperMock.Setup(m => m.GetType(It.IsAny<ClaimsIdentity>())).Returns(AuthTypes.M2M);

            services.ReplaceWithMock(claimHelperMock.Object);
        }
    }
}