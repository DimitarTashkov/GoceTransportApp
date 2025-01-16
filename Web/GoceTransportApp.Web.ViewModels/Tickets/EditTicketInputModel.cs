using GoceTransportApp.Data.Models;
using GoceTransportApp.Web.Infrastructure.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static GoceTransportApp.Common.ErrorMessages.TicketMessages;


namespace GoceTransportApp.Web.ViewModels.Tickets
{
    public class EditTicketInputModel
    {
        [Required]
        public string Id { get; set; }

        [Required]
        [IsBefore("ExpiryDate", ErrorMessage = InvalidTicketIssueTime)]

        public string IssuedDate { get; set; }

        [Required]
        public string ExpiryDate { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string RouteId { get; set; }

        [Required]
        public string OrganizationId { get; set; }

        [Required]
        public string ScheduleId { get; set; }

        public HashSet<UserTicket> TicketsUsers { get; set; }
        = new HashSet<UserTicket>();
    }
}
