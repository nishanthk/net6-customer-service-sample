using FluentValidation;
using CustomerService.Common.Models.DTO;

namespace CustomerService.API.Validators
{
    /// <summary>
    /// Validation for External Booking Registrations Payload
    /// </summary>
    public class ExternalBookingSubscriptionValidator : AbstractValidator<ExternalBookingSubscriptionDTO>
    {
        /// <summary>
        /// External Booking Registrations Validator
        /// </summary>
        public ExternalBookingSubscriptionValidator()
        {
            RuleFor(x => x.OrderNumber).IsRequired().IsMaxLengthValid(20).IsAllowedCharacters().MaximumLength(10);
            RuleFor(x => x.BookingReference).IsRequired().IsMaxLengthValid(30).IsAllowedCharacters();
            RuleFor(x => x.CarrierCode).IsRequired().IsMaxLengthValid(10).IsAllowedCharacters();
            RuleFor(x => x.CustomerCode).IsRequired().IsMaxLengthValid(10).IsAllowedCharacters();
        }
    }
}
