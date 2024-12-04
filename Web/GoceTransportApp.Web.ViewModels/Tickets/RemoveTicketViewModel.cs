using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoceTransportApp.Web.ViewModels.Tickets
{
    public class RemoveTicketViewModel
    {
        public string Id { get; set; } = null!;

        public string IssuedDate { get; set; } = null!;

        public string ExpiryDate { get; set; } = null!;

        public string OrganizationId { get; set; } = null!;
    }
}
