using FluentValidation;
using CustomerService.Common.Models.DTO;

namespace CustomerService.API.Validators
{
    public class GetOrderMilestonesRequestValidator : AbstractValidator<GetOrderMilestonesRequestDTO>
    {
        public GetOrderMilestonesRequestValidator()
        {
            RuleFor(x => x.OrderNumber).IsAllowedCharacters();
            RuleFor(x => x.CustomerCode).IsAllowedCharacters();
            RuleFor(x => x.EquipmentReference).IsAllowedCharacters();
            RuleFor(x => x.BookingReference).IsAllowedCharacters();
            RuleFor(x => x.CarrierSCAC).IsAllowedCharacters();

            When(x => !string.IsNullOrEmpty(x.OrderNumber), () =>
            {
                RuleFor(x => x.OrderNumber).IsRequired().IsMaxLengthValid(20).IsAllowedCharacters();
                RuleFor(x => x.CustomerCode).IsRequired("CustomerCode is required when OrderNumber is provided.").IsMaxLengthValid(10).IsAllowedCharacters();
            });

            When(x => !string.IsNullOrEmpty(x.EquipmentReference), () =>
            {
                RuleFor(x => x.EquipmentReference).IsRequired().IsMaxLengthValid(15).IsAllowedCharacters();
                RuleFor(x => x.CustomerCode).IsRequired("CustomerCode is required when EquipmentReference is provided.").IsMaxLengthValid(10).IsAllowedCharacters();
            });

            When(x => !string.IsNullOrEmpty(x.CustomerCode) && string.IsNullOrEmpty(x.EquipmentReference) && string.IsNullOrEmpty(x.OrderNumber), () =>
            {
                RuleFor(x => x.OrderNumber).IsRequired("OrderNumber or EquipmentReference is required when CustomerCode is provided.").IsAllowedCharacters();
                RuleFor(x => x.CustomerCode).IsMaxLengthValid(10).IsAllowedCharacters();
            });

            When(x => !string.IsNullOrEmpty(x.BookingReference), () =>
            {
                RuleFor(x => x.BookingReference).IsRequired().IsMaxLengthValid(30).IsAllowedCharacters();
                RuleFor(x => x.CarrierSCAC).IsRequired("CarrierSCAC is required when BookingReference is provided.").IsMaxLengthValid(10).IsAllowedCharacters();
            });

            When(x => !string.IsNullOrEmpty(x.CarrierSCAC), () =>
            {
                RuleFor(x => x.BookingReference).IsRequired("BookingReference is required when CarrierSCAC is provided.").IsMaxLengthValid(30).IsAllowedCharacters();
                RuleFor(x => x.CarrierSCAC).IsMaxLengthValid(10).IsAllowedCharacters();
            });

            RuleFor(x => x).Custom((values, context) =>
            {
                if ((string.IsNullOrWhiteSpace(values.OrderNumber) || string.IsNullOrWhiteSpace(values.EquipmentReference)) &&
                     string.IsNullOrWhiteSpace(values.CustomerCode) &&
                     string.IsNullOrWhiteSpace(values.BookingReference) &&
                     string.IsNullOrWhiteSpace(values.CarrierSCAC))
                {
                    context.ThrowBusinessException("Must provide either a combination of {CustomerCode, OrderNumber}, {CustomerCode, EquipmentReference}, {BookingReference, CarrierSCAC}");
                }
            });
        }
    }
}
