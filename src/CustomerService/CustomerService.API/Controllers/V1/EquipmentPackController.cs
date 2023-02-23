using CustomerService.Common.Models.DTO;
using CustomerService.Common.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;

namespace CustomerService.API.Controllers.V1
{
    /// <summary>
    /// 
    /// </summary>
    [Route("v1/equipment-packing")]
    [ApiController]
    public class EquipmentPackController : Controller
    {
        private readonly IEquipmentPackService _equipmentPackService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="equipmentPackService"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public EquipmentPackController(IEquipmentPackService equipmentPackService)
        {
            _equipmentPackService = equipmentPackService ?? throw new ArgumentNullException(nameof(equipmentPackService));
        }

        /// <summary>
        /// Post Equipment Pack
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [Authorize(ClientAppRoles.M2M, ClientAppRoles.DigitalTeam, AuthenticationSchemes = "Auth0")]
        public async Task<IActionResult> PostAsync([FromBody] EquipmentPackDTO request)
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

            await _equipmentPackService.Create(request);
            return NoContent();
        }
    }
}
