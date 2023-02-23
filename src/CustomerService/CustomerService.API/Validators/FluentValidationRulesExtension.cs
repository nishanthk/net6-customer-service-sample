using FluentValidation;
using FluentValidation.Validators;
using Common.SharedAppInterfaces.Exceptions;
using System;
using System.Text.RegularExpressions;

namespace CustomerService.API.Validators
{
    public static class FluentValidationRulesExtension
    {
        public static IRuleBuilderOptionsConditions<T, string> IsRequired<T>(this IRuleBuilder<T, string> ruleBuilderInitial, string errorMessage = null)
        {
            ruleBuilderInitial.SetValidator(new NotEmptyValidator<T, string>());
            return ruleBuilderInitial.Custom((property, context) =>
            {
                if (string.IsNullOrWhiteSpace(property))
                {
                    var message = string.IsNullOrEmpty(errorMessage) ? $"{context.PropertyName} is required." : errorMessage;
                    context.ThrowBusinessException(message);
                }
            });
        }

        public static IRuleBuilderOptionsConditions<T, DateTime?> IsRequired<T>(this IRuleBuilder<T, DateTime?> ruleBuilderInitial, string errorMessage = null)
        {
            ruleBuilderInitial.SetValidator(new NotNullValidator<T, DateTime>());
            return ruleBuilderInitial.Custom((property, context) =>
            {
                if (!property.HasValue || property.Value == DateTime.MinValue)
                {
                    var message = string.IsNullOrEmpty(errorMessage) ? $"{context.PropertyName} is required." : errorMessage;
                    context.ThrowBusinessException(message);
                }
            });
        }

        public static IRuleBuilderOptionsConditions<T, string> IsAllowedCharacters<T>(this IRuleBuilder<T, string> ruleBuilderInitial, string errorMessage = null)
        {
            var expression = @"^[a-zA-Z0-9-_\s]+$";
            ruleBuilderInitial.SetValidator(new RegularExpressionValidator<T>(expression));
            return ruleBuilderInitial.Custom((property, context) =>
            {
                var letterOrDigitOnly = new Regex(expression);
                if (!string.IsNullOrWhiteSpace(property) && !letterOrDigitOnly.IsMatch(property))
                {
                    var message = string.IsNullOrEmpty(errorMessage) ? $"{context.PropertyName} could contain only letters and numbers." : errorMessage;
                    context.ThrowBusinessException(message);
                }
            });
        }

        public static IRuleBuilderOptionsConditions<T, string> IsMaxLengthValid<T>(this IRuleBuilder<T, string> ruleBuilderInitial, int max, string errorMessage = null)
        {
            ruleBuilderInitial.SetValidator(new MaximumLengthValidator<T>(max));
            return ruleBuilderInitial.Custom((property, context) =>
            {
                if (property?.Length > max)
                {
                    var message = string.IsNullOrEmpty(errorMessage) ? $"{context.PropertyName} maximum length cannot exceed {max} chars." : errorMessage;
                    context.ThrowBusinessException(message);
                }
            });
        }

        public static void ThrowBusinessException<T>(this ValidationContext<T> context, string message)
        {
            context.AddFailure(message);
            throw new BusinessException(message);
        }
    }
}
