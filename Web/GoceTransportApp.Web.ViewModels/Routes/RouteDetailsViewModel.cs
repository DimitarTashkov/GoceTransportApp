using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoceTransportApp.Web.ViewModels.Routes
{
    public class RouteDetailsViewModel
    {
        public string Id { get; set; } = null!;

        public string DepartingCity { get; set; } = null!;

        public string ArrivingCity { get; set; } = null!;

        public string DepartingStreet { get; set; } = null!;

        public string ArrivingStreet { get; set; } = null!;
        public string Organization { get; set; }

        public double Distance { get; set; }

        public double Duration { get; set; }
    }
}
