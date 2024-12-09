using GoceTransportApp.Web.ViewModels.Cities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoceTransportApp.Web.ViewModels.Routes
{
    public class AllRoutesSearchFilterViewModel
    {
        public IEnumerable<RouteDataViewModel> Routes { get; set; }

        public string SearchQuery { get; set; }

        public string DepartingCityFilter { get; set; }

        public string ArrivingCityFilter { get; set; }

        public int? CurrentPage { get; set; } = 1;

        public int? EntitiesPerPage { get; set; } = 20;

        public int? TotalPages { get; set; }
    }
}
