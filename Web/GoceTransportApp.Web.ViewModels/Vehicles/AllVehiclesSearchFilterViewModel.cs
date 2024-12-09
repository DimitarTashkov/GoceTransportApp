using GoceTransportApp.Web.ViewModels.Streets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoceTransportApp.Web.ViewModels.Vehicles
{
    public class AllVehiclesSearchFilterViewModel
    {
        public IEnumerable<VehicleDataViewModel> Vehicles { get; set; }

        public string SearchQuery { get; set; }

        public int? CurrentPage { get; set; } = 1;

        public int? EntitiesPerPage { get; set; } = 20;

        public int? TotalPages { get; set; }
    }
}
