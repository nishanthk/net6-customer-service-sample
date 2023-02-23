using FluentValidation;
using CustomerService.Common.Models.DTO;

namespace CustomerService.API.Validators
{
    /// <summary>
    /// Validation for Equipment Pack request Payload
    /// </summary>
    public class EquipmentPackValidator: AbstractValidator<EquipmentPackDTO> 
    {
        /// <summary>
        /// Equipment Pack Validator
        /// </summary>
        public EquipmentPackValidator()
        {
            RuleFor(x => x.OrderNumber).IsRequired().IsMaxLengthValid(20).IsAllowedCharacters();
            RuleFor(x => x.BookingReference).IsRequired().IsMaxLengthValid(50).IsAllowedCharacters();
            RuleFor(x => x.CarrierSCAC).IsRequired().IsMaxLengthValid(10).IsAllowedCharacters();
            RuleFor(x => x.CustomerCode).IsRequired().IsMaxLengthValid(10).IsAllowedCharacters();
            RuleFor(x => x.EquipmentReference).IsRequired().IsMaxLengthValid(15).IsAllowedCharacters();
            RuleFor(x => x.ISOEquipmentGroupCode).IsRequired().IsMaxLengthValid(10).IsAllowedCharacters();
            RuleFor(x => x.PurposeCode).IsRequired().IsMaxLengthValid(20).IsAllowedCharacters();
            RuleFor(x => x.EventDateTime).IsRequired();
        }
    }
}
