using API.SDK.Authorization;
using API.SDK.Controllers;
using Common.SharedAppInterfaces.Exceptions;
using CustomerService.Common.Models.DTO;
using CustomerService.Common.Services.Interfaces;
using MilestoneService.DTO.API.V1;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;

namespace CustomerService.API.Controllers.V1
{
    /// <summary>
    /// Order milestons controller
    /// </summary>
    [Route("v1/order-milestones")]
    [ApiController]
    public class OrderMilestonesController : AuthControllerBase
    {
        private readonly IOrderMilestonesService _orderMilestoneService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderMilestoneService"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public OrderMilestonesController(IOrderMilestonesService orderMilestoneService)
        {
            _orderMilestoneService = orderMilestoneService ?? throw new ArgumentNullException(nameof(orderMilestoneService));
        }

        /// <summary>
        /// A GET method that returns a the order and all the milestones
        /// </summary>
        /// <returns>Success response</returns>
        [HttpGet("", Name = "GetOrderMilestones")]
        [ProducesResponseType(typeof(OrderMilestoneDTO[]), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
        [Authorize(ClientAppRoles.M2M, ClientAppRoles.DigitalTeam, AuthenticationSchemes = "Auth0")]
        public async Task<IActionResult> Get([FromQuery] GetOrderMilestonesRequestDTO request)
        {
            if (request == null)
            {
                throw new APIResponseException("Invalid empty request", HttpStatusCode.BadRequest);
            }

            if (HttpContext != null)
            {
                var requestTelemetry = HttpContext.Features.Get<RequestTelemetry>();
                requestTelemetry.Properties["CustomerCode"] = request.CustomerCode;
                requestTelemetry.Properties["OrderNumber"] = request.OrderNumber;
                requestTelemetry.Properties["EquipmentReference"] = request.EquipmentReference;
                requestTelemetry.Properties["BookingReference"] = request.BookingReference;
                requestTelemetry.Properties["CarrierSCAC"] = request.CarrierSCAC;
            }

            var response = await _orderMilestoneService.GetOrderMilestones(request);
            return Ok(response);
        }        
    }
}