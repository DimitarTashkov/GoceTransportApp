using GoceTransportApp.Data.Models.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static GoceTransportApp.Common.EntityValidationConstants.VehicleConstants;
using static GoceTransportApp.Common.ResultMessages.VehicleMessages;
using GoceTransportApp.Web.ViewModels.ValidationAttributes;


namespace GoceTransportApp.Web.ViewModels.Vehicles
{
    public class VehicleInputModel
    {
        [Required(ErrorMessage = VehicleNumberRequired)]
        [MinLength(MinNumberLength)]
        [MaxLength(MaxNumberLength)]
        [UniqueVehicleNumber]
        public string Number { get; set; } = null!;

        [Required(ErrorMessage = VehicleTypeRequired)]
        [MinLength(MinTypeLength)]
        [MaxLength(MaxTypeLength)]
        public string Type { get; set; } = null!;

        [Required(ErrorMessage = VehicleManufacturerRequired)]
        [MinLength(MinManufacturerLength)]
        [MaxLength(MaxManufacturerLength)]
        public string Manufacturer { get; set; } = null!;

        [Required(ErrorMessage = VehicleModelRequired)]
        [MinLength(MinModelLength)]
        [MaxLength(MaxModelLength)]
        public string Model { get; set; } = null!;

        [Required(ErrorMessage = VehicleCapacityRequired)]
        [Range(MinCapacityLength, MaxCapacityLength)]
        public int Capacity { get; set; }

        [Range(MinFuelConsumptionLength, MaxFuelConsumptionLength)]
        public double FuelConsumption { get; set; }

        [Required]
        [DefaultValue(VehicleStatus.Available)]
        public string Status { get; set; } = null!;

        [Required]
        public string OrganizationId { get; set; } = null!;
    }
}
