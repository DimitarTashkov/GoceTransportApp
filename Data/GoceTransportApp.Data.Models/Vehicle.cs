using GoceTransportApp.Data.Common.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoceTransportApp.Data.Models
{
    public class Vehicle : BaseModel<Guid>
    {
        public Vehicle()
        {
            this.Id = Guid.NewGuid();
        }

        [Required]
        [MaxLength(50)]
        //[Unique]
        public string Number { get; set; } = null!;

        [Required]
        [MaxLength(30)]
        public string Type { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string Manufacturer { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string Model { get; set; } = null!;

        [Required]
        [Range(1, 100)]
        public int Capacity { get; set; }

        [Range(1, 50)]
        public decimal FuelConsumption { get; set; }

        [Required]
        public string VehicleStatus { get; set; } = null!;

        [Required]
        public Guid OrganizationId { get; set; }

        [ForeignKey(nameof(OrganizationId))]
        public Organization Organization { get; set; } = null!;
    }
}
