using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoceTransportApp.Web.Infrastructure.ValidationAttributes
{
    public class IsBefore : ValidationAttribute
    {
        private readonly string comparisonProperty;

        public IsBefore(string comparisonProperty)
        {
            this.comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var currentValue = value as string;

            var property = validationContext.ObjectType.GetProperty(comparisonProperty);
            if (property == null)
            {
                return new ValidationResult($"Property '{comparisonProperty}' not found.");
            }

            var comparisonValue = property.GetValue(validationContext.ObjectInstance) as string;

            if (!DateTime.TryParse(currentValue, out var currentDate))
            {
                return new ValidationResult("Invalid IssuedDate format.");
            }

            if (!DateTime.TryParse(comparisonValue, out var comparisonDate))
            {
                return new ValidationResult("Invalid ExpiryDate format.");
            }

            if (currentDate >= comparisonDate)
            {
                return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} must be before {comparisonProperty}.");
            }

            return ValidationResult.Success;
        }
    }
}
