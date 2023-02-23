using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Threading.Tasks;
using System.Net.Http;
using Shouldly;
using System.Collections.Generic;
using CustomerService.Common.Models.DTO;
using System.Net.Http.Formatting;
using System;

namespace CustomerService.API.UnitTests
{

    [TestClass]
    public class EquipmentPackControllerTests
    {
        static APIUnderTest _apiUnderTest;

        class EndpointUrls
        {
            public static string EquipmentPack => _apiUnderTest.Url + "v1/equipment-packing";
        }

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _apiUnderTest = new APIUnderTest(context);
            _apiUnderTest.Build();
        }

        [TestMethod]
        public async Task CallEquipmentPacksEndpointWithValidModelReturnsNoContent()
        {
            //Arrange
            var equipmentPack = new EquipmentPackDTO
            {
                OrderNumber = "123123",
                BookingReference = "EquipmentPacked",
                PurposeCode = "Packed",
                CarrierSCAC = "CA20",
                EquipmentReference = "TCNU6869044",
                ISOEquipmentGroupCode = "GP22",
                CustomerCode = "FONTERRA",
                EventDateTime = System.DateTime.UtcNow
            };

            //Act
            var client = _apiUnderTest.GetClient();
            client.DefaultRequestHeaders.Add("authorization", "bearer some token");
            var response = await client.PostAsync(EndpointUrls.EquipmentPack, new ObjectContent<EquipmentPackDTO>(equipmentPack, new JsonMediaTypeFormatter()));

            //Assert
            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        }

        [TestMethod]
        public async Task CallEquipmentPacksEndpointWithInValidEventDateReturnsBadRequest()
        {
            //Arrange
            var equipmentPack = new EquipmentPackDTO
            {
                OrderNumber = "123123",
                BookingReference = "EquipmentPacked",
                PurposeCode = "Packed",
                CarrierSCAC = "CA20",
                EquipmentReference = "TCNU6869044",
                ISOEquipmentGroupCode = "GP22",
                CustomerCode = "FONTERRA"
            };

            //Act
            var client = _apiUnderTest.GetClient();
            client.DefaultRequestHeaders.Add("authorization", "bearer some token");
            var response = await client.PostAsync(EndpointUrls.EquipmentPack, new ObjectContent<EquipmentPackDTO>(equipmentPack, new JsonMediaTypeFormatter()));

            //Assert
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task CallEquipmentPacksEndpointWithInValidModelReturnsBadRequest()
        {
            //Arrange
            var scac = "CA20";
            var equipmentPack = new EquipmentPackDTO
            {
                OrderNumber = "123123",
                BookingReference = "bookRef",
                PurposeCode = "Packed",
                CarrierSCAC = $"{scac}-Invalid",
                EquipmentReference = "TCNU6869044",
                ISOEquipmentGroupCode = "GP22",
                CustomerCode = "FONTERRA",
                EventDateTime = System.DateTime.UtcNow
            };

            //Act
            var client = _apiUnderTest.GetClient();
            client.DefaultRequestHeaders.Add("authorization", "bearer some token");
            var response = await client.PostAsync(EndpointUrls.EquipmentPack, new ObjectContent<EquipmentPackDTO>(equipmentPack, new JsonMediaTypeFormatter()));

            //Assert
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task CallEquipmentPacksEndpointWithInValidMismatchedContainerTypeReturnsBadRequest()
        {
            //Arrange
            var containerType = "GP22";
            var equipmentPack = new EquipmentPackDTO
            {
                OrderNumber = "123123",
                BookingReference = "bookRef",
                PurposeCode = "EquipmentPacked",
                CarrierSCAC = "CA20NotMatching",
                EquipmentReference = "TCNU6869044",
                ISOEquipmentGroupCode = $"{containerType}-invalid",
                CustomerCode = "FONTERRA",
                EventDateTime = System.DateTime.UtcNow
            };

            //Act
            var client = _apiUnderTest.GetClient();
            client.DefaultRequestHeaders.Add("authorization", "bearer some token");
            var response = await client.PostAsync(EndpointUrls.EquipmentPack, new ObjectContent<EquipmentPackDTO>(equipmentPack, new JsonMediaTypeFormatter()));

            //Assert
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        [DataRow("", "", "", "", "", "", "", null)]
        [DataRow("", "123", "123", "123", "123", "123", "123", "12/01/2010")]
        [DataRow("123", "", "123", "123", "123", "123", "123", "12/01/2010")]
        [DataRow("123", "123", "", "123", "123", "123", "123", "12/01/2010")]
        [DataRow("123", "123", "123", "", "123", "123", "123", "12/01/2010")]
        [DataRow("123", "123", "123", "123", "", "123", "123", "12/01/2010")]
        [DataRow("123", "123", "123", "123", "123", "", "123", "12/01/2010")]
        [DataRow("123", "123", "123", "123", "123", "123", "", "12/01/2010")]
        [DataRow("123", "123", "123", "123", "123", "123", "123", "")]
        [DataRow("123!@#", "1_+", "123", "123", "123", "123", "123", "12/01/2010")]
        public async Task CallEquipmentPacksEndpointWithInValidModelsReturnsBadRequest(
            string orderNumber,
            string bookingReference,
            string purposeCode,
            string scac,
            string equipmentRef,
            string equipmentCode,
            string customerCode,
            string date)
        {
            //Arrange
            DateTime.TryParse(date, out DateTime dateTime);
            var equipmentPack = new EquipmentPackDTO
            {
                OrderNumber = orderNumber,
                BookingReference = bookingReference,
                PurposeCode = purposeCode,
                CarrierSCAC = scac,
                EquipmentReference = equipmentRef,
                ISOEquipmentGroupCode = equipmentCode,
                CustomerCode = customerCode,
                EventDateTime = dateTime
            };

            //Act
            var client = _apiUnderTest.GetClient();
            client.DefaultRequestHeaders.Add("authorization", "bearer some token");
            var response = await client.PostAsync(EndpointUrls.EquipmentPack, new ObjectContent<EquipmentPackDTO>(equipmentPack, new JsonMediaTypeFormatter()));

            //Assert
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }
    }
}