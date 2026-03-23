using System;

namespace GoceTransportApp.Web.ViewModels.Tickets
{
    public class MyTicketViewModel
    {
        public string TicketId { get; set; } = null!;

        public string FromCity { get; set; } = null!;

        public string ToCity { get; set; } = null!;

        public string Day { get; set; } = null!;

        public string DepartingTime { get; set; } = null!;

        public string ArrivingTime { get; set; } = null!;

        public DateTime DepartureDateTime { get; set; }

        public string Price { get; set; } = null!;

        public string OrganizationName { get; set; } = null!;

        public bool CanCancel { get; set; }
    }
}
