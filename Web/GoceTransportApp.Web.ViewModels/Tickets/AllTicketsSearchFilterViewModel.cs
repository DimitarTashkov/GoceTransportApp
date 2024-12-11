using GoceTransportApp.Web.ViewModels.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoceTransportApp.Web.ViewModels.Tickets
{
    public class AllTicketsSearchFilterViewModel
    {
        public IEnumerable<TicketDataViewModel> Tickets { get; set; }

        public string SearchQuery { get; set; }

        public decimal? PriceFrom { get; set; }

        public decimal? PriceTo { get; set; }

        public int? CurrentPage { get; set; } = 1;

        public int? EntitiesPerPage { get; set; } = 20;

        public int? TotalPages { get; set; }
    }
}
