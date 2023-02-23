using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Threading.Tasks;
using System.Net.Http;
using Shouldly;
using System.Collections.Generic;
using CustomerService.Common.Models.DTO;
using System.Net.Http.Formatting;
using MilestoneService.DTO.API.V1;

namespace CustomerService.API.UnitTests
{
    [TestClass]
    public class OrderMilestonesControllerTests
    {
        static APIUnderTest _apiUnderTest;

        class EndpointUrls
        {
            public static string orderMilestones => _apiUnderTest.Url + "v1/order-milestones";
        }

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _apiUnderTest = new APIUnderTest(context);
            _apiUnderTest.Build();
        }

        [TestMethod]
        public async Task CallGetOrderMilestonesWithNoCustomerCodeTestsReturnsBadRequest()
        {
            //Arrange
            var milestonesDto = new List<OrderMilestoneDTO>()
            {
                new OrderMilestoneDTO(){
                    CustomerCode ="Test",
                    Equipments = new List<EquipmentDTO>(){
                        new EquipmentDTO(){}
                    },
                    Bookings = new List<ShipmentDTO>()
                    {
                        new ShipmentDTO(){}
                    }
                }
            };
            _apiUnderTest.SetOrderMilestoneApiResponseCode(milestonesDto, HttpStatusCode.BadRequest);

            //Act
            var client = _apiUnderTest.GetClient();
            client.DefaultRequestHeaders.Add("authorization", "bearer some token");
            var response = await client.GetAsync(EndpointUrls.orderMilestones);

            //Assert
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task CallGetOrderMilestonesTestsWithNoFilters()
        {
            //Arrange
            var milestonesDto = new List<OrderMilestoneDTO>()
            {
                new OrderMilestoneDTO(){
                    CustomerCode ="Test",
                    Equipments = new List<EquipmentDTO>(){
                        new EquipmentDTO(){}
                    },
                    Bookings = new List<ShipmentDTO>()
                    {
                        new ShipmentDTO(){}
                    }
                }
            };
            _apiUnderTest.SetOrderMilestoneApiResponseCode(milestonesDto, HttpStatusCode.OK);

            //Act
            var client = _apiUnderTest.GetClient();
            client.DefaultRequestHeaders.Add("authorization", "bearer some token");
            var response = await client.GetAsync($"{EndpointUrls.orderMilestones}?OrderNumber=Test&CustomerCode=Test");

            //Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }
    }
}