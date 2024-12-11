using GoceTransportApp.Web.ViewModels.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoceTransportApp.Web.ViewModels.Tickets
{
    public class TicketsForOrganizationViewModel
    {
        public string OrganizationId { get; set; }

        public IEnumerable<TicketDataViewModel> Tickets { get; set; }
    }
}
