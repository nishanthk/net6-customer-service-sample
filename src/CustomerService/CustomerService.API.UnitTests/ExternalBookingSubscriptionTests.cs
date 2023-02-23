using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Threading.Tasks;
using System.Net.Http;
using Shouldly;
using System.Collections.Generic;
using CustomerService.Common.Models.DTO;
using System.Net.Http.Formatting;

namespace CustomerService.API.UnitTests
{
    [TestClass]
    public class ExternalBookingSubscriptionTests
    {
        static APIUnderTest _apiUnderTest;

        class EndpointUrls
        {
            public static string ExternalBookingSubscription => _apiUnderTest.Url + "v1/external-booking-subscriptions";
        }

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _apiUnderTest = new APIUnderTest(context);
            _apiUnderTest.Build();
        }

        [TestMethod]
        public async Task CallNonBookingRegistrationTestsWithValidModelReturnsNoContent()
        {
            //Arrange
            var externalBookingSubscriptionDTO = new ExternalBookingSubscriptionDTO
            {
                OrderNumber = "123123",
                BookingReference = "EquipmentPacked",
                CustomerCode = "FONTERRA",
                CarrierCode = "CA20"
            };
            _apiUnderTest.SetNonKothiRegistrationResponseCode(HttpStatusCode.NoContent);

            //Act
            var client = _apiUnderTest.GetClient();
            client.DefaultRequestHeaders.Add("authorization", "bearer some token");
            var response = await client.PutAsync(EndpointUrls.ExternalBookingSubscription, new ObjectContent<ExternalBookingSubscriptionDTO>(externalBookingSubscriptionDTO, new JsonMediaTypeFormatter()));

            //Assert
            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        }

        [TestMethod]
        public async Task CallNonBookingRegistrationTestsReturnsNoFound()
        {
            //Arrange
            var externalBookingSubscriptionDTO = new ExternalBookingSubscriptionDTO
            {
                OrderNumber = "123123",
                BookingReference = "EquipmentPacked",
                CustomerCode = "FONTERRA",
                CarrierCode = "CA20"
            };
            _apiUnderTest.SetNonKothiRegistrationResponseCode(HttpStatusCode.NotFound);

            //Act
            var client = _apiUnderTest.GetClient();
            client.DefaultRequestHeaders.Add("authorization", "bearer some token");
            var response = await client.PutAsync(EndpointUrls.ExternalBookingSubscription, new ObjectContent<ExternalBookingSubscriptionDTO>(externalBookingSubscriptionDTO, new JsonMediaTypeFormatter()));

            //Assert
            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [TestMethod]
        public async Task CallNonBookingRegistrationTestsReturnsBadRequest()
        {
            //Arrange
            var externalBookingSubscriptionDTO = new ExternalBookingSubscriptionDTO
            {
                OrderNumber = "123123",
                BookingReference = "EquipmentPacked",
                CustomerCode = "FONTERRA",
                CarrierCode = "CA20"
            };
            _apiUnderTest.SetNonKothiRegistrationResponseCode(HttpStatusCode.BadRequest);

            //Act
            var client = _apiUnderTest.GetClient();
            client.DefaultRequestHeaders.Add("authorization", "bearer some token");
            var response = await client.PutAsync(EndpointUrls.ExternalBookingSubscription, new ObjectContent<ExternalBookingSubscriptionDTO>(externalBookingSubscriptionDTO, new JsonMediaTypeFormatter()));

            //Assert
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task CallNonBookingRegistrationTestsReturnsInternalError()
        {
            //Arrange
            var externalBookingSubscriptionDTO = new ExternalBookingSubscriptionDTO
            {
                OrderNumber = "123123",
                BookingReference = "EquipmentPacked",
                CustomerCode = "FONTERRA",
                CarrierCode = "CA20"
            };
            _apiUnderTest.SetNonKothiRegistrationResponseCode(HttpStatusCode.InternalServerError);

            //Act
            var client = _apiUnderTest.GetClient();
            client.DefaultRequestHeaders.Add("authorization", "bearer some token");
            var response = await client.PutAsync(EndpointUrls.ExternalBookingSubscription, new ObjectContent<ExternalBookingSubscriptionDTO>(externalBookingSubscriptionDTO, new JsonMediaTypeFormatter()));

            //Assert
            response.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
        }
    }
}