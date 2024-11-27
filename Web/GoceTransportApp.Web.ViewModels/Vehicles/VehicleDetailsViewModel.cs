using GoceTransportApp.Data.Models.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoceTransportApp.Web.ViewModels.Vehicles
{
    public class VehicleDetailsViewModel
    {
        public string Id { get; set; } = null!;

        public string Number { get; set; } = null!;

        public string Type { get; set; } = null!;

        public string Manufacturer { get; set; } = null!;

        public string Model { get; set; } = null!;

        public int Capacity { get; set; }

        public double? FuelConsumption { get; set; }

        public string Status { get; set; } = null!;

        public string OrganizationId { get; set; } = null!;
    }
}
