using GoceTransportApp.Data.Common.Repositories;
using GoceTransportApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static GoceTransportApp.Common.ErrorMessages.VehicleMessages;

namespace GoceTransportApp.Web.ViewModels.ValidationAttributes
{
    public class UniqueVehicleNumberAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            var vehicleRepository = (IDeletableEntityRepository<Vehicle>)validationContext.GetService(typeof(IDeletableEntityRepository<Vehicle>));

            var isUnique = Task.Run(async () =>
                !await vehicleRepository.AllWithDeleted()
                    .AnyAsync(v => v.Number == value.ToString())
            ).Result;

            if (!isUnique)
            {
                return new ValidationResult(VehicleNumberExists);
            }

            return ValidationResult.Success;
        }

    }
}
