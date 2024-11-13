using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoceTransportApp.Web.ViewModels.Routes
{
    public class RemoveRouteViewModel
    {
        public string Id { get; set; } = null!;

        public string DepartingCity { get; set; } = null!;

        public string ArrivingCity { get; set; } = null!;

        public string DepartingStreet { get; set; } = null!;

        public string ArrivingStreet { get; set; } = null!;

        public string OrganizationId { get; set; }
    }
}
