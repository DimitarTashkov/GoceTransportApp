using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoceTransportApp.Web.ViewModels.Routes
{
    public class RoutesForOrganizationViewModel
    {
        public string OrganizationId { get; set; }
        public IEnumerable<RouteDataViewModel> Routes { get; set; }
    }
}
