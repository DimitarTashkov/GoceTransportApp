using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoceTransportApp.Web.ViewModels.Streets
{
    public class AllStreetsSearchFilterViewModel
    {
        public IEnumerable<StreetDataViewModel> Streets { get; set; }

        public string SearchQuery { get; set; }

        public int? CurrentPage { get; set; } = 1;

        public int? EntitiesPerPage { get; set; } = 5;

        public int? TotalPages { get; set; }
    }
}
