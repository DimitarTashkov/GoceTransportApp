using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoceTransportApp.Web.ViewModels.Tickets
{
    public class TicketDataViewModel
    {
        public string Id { get; set; } = null!;

        public string IssuedDate { get; set; }

        public string ExpiryDate { get; set; }

        public string Price { get; set; } = null!;

        public string FromCity { get; set; }

        public string ToCity { get; set; }

        public string OrganizationId { get; set; } = null!;

    }
}
