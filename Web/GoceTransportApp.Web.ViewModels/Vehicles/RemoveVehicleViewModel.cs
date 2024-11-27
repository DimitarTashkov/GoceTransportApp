using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoceTransportApp.Web.ViewModels.Vehicles
{
    public class RemoveVehicleViewModel
    {
        public string Id { get; set; } = null!;

        public string Number { get; set; } = null!;

        public string OrganizationId { get; set; } = null!;
    }
}
