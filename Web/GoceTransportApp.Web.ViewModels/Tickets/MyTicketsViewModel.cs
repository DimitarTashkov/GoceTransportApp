using System.Collections.Generic;

namespace GoceTransportApp.Web.ViewModels.Tickets
{
    public class MyTicketsViewModel
    {
        public IEnumerable<MyTicketViewModel> Upcoming { get; set; } = new List<MyTicketViewModel>();

        public IEnumerable<MyTicketViewModel> Past { get; set; } = new List<MyTicketViewModel>();
    }
}
