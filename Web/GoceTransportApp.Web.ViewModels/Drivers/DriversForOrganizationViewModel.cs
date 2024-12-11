using GoceTransportApp.Web.ViewModels.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoceTransportApp.Web.ViewModels.Drivers
{
    public class DriversForOrganizationViewModel
    {
        public string OrganizationId { get; set; }

        public IEnumerable<DriverDataViewModel> Drivers { get; set; }
    }
}
