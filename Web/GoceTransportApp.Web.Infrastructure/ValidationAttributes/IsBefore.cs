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
            if (value == null)
            {
                return new ValidationResult($"{validationContext.DisplayName} is required.");
            }

            var property = validationContext.ObjectType.GetProperty(comparisonProperty);
            if (property == null)
            {
                return new ValidationResult($"Property '{comparisonProperty}' not found.");
            }

            var currentDate = (DateTime)value; 
            var comparisonValue = property.GetValue(validationContext.ObjectInstance);

            if (comparisonValue == null)
            {
                return new ValidationResult($"{comparisonProperty} is required.");
            }

            var comparisonDate = (DateTime)comparisonValue;

            if (currentDate >= comparisonDate)
            {
                return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} must be before {comparisonProperty}.");
            }

            return ValidationResult.Success;
        }
    }
}
