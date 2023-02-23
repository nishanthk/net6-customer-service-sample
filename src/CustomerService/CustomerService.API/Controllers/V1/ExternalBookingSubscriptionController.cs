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
    /// External booking subscriptions controller
    /// </summary>
    [Route("v1/external-booking-subscriptions")]
    [ApiController]
    public class ExternalBookingSubscriptionController : AuthControllerBase 
    {
        private readonly IExternalBookingSubscriptionService _externalBookingSubscriptionService;

        /// <summary>
        /// External Booking Subscriptions controller
        /// </summary>
        /// <param name="externalBookingSubscriptionService"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public ExternalBookingSubscriptionController(IExternalBookingSubscriptionService externalBookingSubscriptionService) 
        {
            _externalBookingSubscriptionService = externalBookingSubscriptionService ?? throw new ArgumentNullException(nameof(externalBookingSubscriptionService));        
        }

        /// <summary>
        /// A put method to register external booking
        /// </summary>
        /// <param name="externalBookingSubscriptionDTO"></param>
        /// <returns></returns>
        /// <exception cref="APIResponseException"></exception>
        /// <exception cref="NotFoundException"></exception>
        [HttpPut("", Name = "PutExternalBookingSubscription")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(ClientAppRoles.M2M, ClientAppRoles.DigitalTeam, AuthenticationSchemes = "Auth0")]
        public async Task<IActionResult> PutAsync([FromBody] ExternalBookingSubscriptionDTO externalBookingSubscriptionDTO) 
        {
            if (externalBookingSubscriptionDTO == null) 
            { 
                throw new APIResponseException("Invalid empty request", HttpStatusCode.BadRequest);
            }

            if (HttpContext != null)
            {
                var requestTelemetry = HttpContext.Features.Get<RequestTelemetry>();
                requestTelemetry.Properties["CustomerCode"] = externalBookingSubscriptionDTO.CustomerCode;
                requestTelemetry.Properties["OrderNumber"] = externalBookingSubscriptionDTO.OrderNumber;
                requestTelemetry.Properties["BookingReference"] = externalBookingSubscriptionDTO.BookingReference;
                requestTelemetry.Properties["CarrierCode"] = externalBookingSubscriptionDTO.CarrierCode;
            }

            await _externalBookingSubscriptionService.Register(externalBookingSubscriptionDTO);
            return NoContent();
        }
    }
}