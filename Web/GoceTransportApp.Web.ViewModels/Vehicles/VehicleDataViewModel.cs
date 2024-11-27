using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoceTransportApp.Web.ViewModels.Vehicles
{
    public class VehicleDataViewModel
    {
        public string Id { get; set; } = null!;

        public string Number { get; set; } = null!;

        public string Type { get; set; } = null!;

        public string Manufacturer { get; set; } = null!;

        public string Model { get; set; } = null!;
    }
}
