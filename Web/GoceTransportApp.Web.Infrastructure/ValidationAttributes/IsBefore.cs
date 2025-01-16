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
        private const string DateTimeFormat = "dd/MM/yyyy";
        private readonly DateTime date;

        public IsBefore(string dateInput)
        {
            date = DateTime.ParseExact(dateInput, DateTimeFormat, CultureInfo.InvariantCulture);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if ((DateTime)value >= date) return new ValidationResult(ErrorMessage);
            return ValidationResult.Success;
        }
    }
}
