using GoceTransportApp.Web.ViewModels.Cities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoceTransportApp.Web.ViewModels.Organizations
{
    public class AllOrganizationsSearchFilterViewModel
    {
        public IEnumerable<OrganizationDataViewModel> Organizations { get; set; }

        public string SearchQuery { get; set; }

        public int? CurrentPage { get; set; } = 1;

        public int? EntitiesPerPage { get; set; } = 20;

        public int? TotalPages { get; set; }
    }
}
