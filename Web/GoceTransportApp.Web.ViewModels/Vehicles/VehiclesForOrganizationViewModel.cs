using GoceTransportApp.Web.ViewModels.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoceTransportApp.Web.ViewModels.Vehicles
{
    public class VehiclesForOrganizationViewModel
    {
        public string OrganizationId { get; set; }

        public IEnumerable<VehicleDataViewModel> Vehicles { get; set; }
    }
}
