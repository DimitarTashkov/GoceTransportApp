using GoceTransportApp.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoceTransportApp.Web.Infrastructure.ValidationAttributes;

using static GoceTransportApp.Common.ResultMessages.TicketMessages;

namespace GoceTransportApp.Web.ViewModels.Tickets
{
    public class TicketInputModel
    {
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
    }
}
