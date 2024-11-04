using GoceTransportApp.Data.Common.Models;
using GoceTransportApp.Data.Models.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using static GoceTransportApp.Common.EntityValidationConstants.VehicleConstants;

namespace GoceTransportApp.Data.Models
{
    public class Vehicle : BaseModel<Guid>
    {
        public Vehicle()
        {
            this.Id = Guid.NewGuid();
        }

        [Required]
        [MaxLength(MaxNumberLength)]
        public string Number { get; set; } = null!;

        [Required]
        [MaxLength(MaxTypeLength)]
        public string Type { get; set; } = null!;

        [Required]
        [MaxLength(MaxManufacturerLength)]
        public string Manufacturer { get; set; } = null!;

        [Required]
        [MaxLength(MaxModelLength)]
        public string Model { get; set; } = null!;

        [Required]
        [Range(MinCapacityLength, MaxCapacityLength)]
        public int Capacity { get; set; }

        [Range(MinFuelConsumptionLength, MaxFuelConsumptionLength)]
        public double FuelConsumption { get; set; }

        [Required]
        [DefaultValue(VehicleStatus.Available)]
        public VehicleStatus VehicleStatus { get; set; }

        [Required]
        public Guid OrganizationId { get; set; }

        [ForeignKey(nameof(OrganizationId))]
        public Organization Organization { get; set; } = null!;

        public HashSet<Schedule> Schedules { get; set; }
        = new HashSet<Schedule>();
    }
}
