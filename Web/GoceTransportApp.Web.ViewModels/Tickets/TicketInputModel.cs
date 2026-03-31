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
using System.Web.Mvc;

namespace GoceTransportApp.Web.ViewModels.Tickets
{
    public class TicketInputModel
    {
        [Required]
        [IsBefore("ExpiryDate", ErrorMessage = InvalidTicketIssueTime)]

        public DateTime IssuedDate { get; set; }

        [Required]

        public DateTime ExpiryDate { get; set; }

        [Required]
        [Range(0.01, 10000, ErrorMessage = "Price must be between 0.01 and 10,000")]
        public decimal Price { get; set; }

        [Required]
        public string RouteId { get; set; }

        [Required]
        public string OrganizationId { get; set; }

        [Required]
        public string ScheduleId { get; set; }

        public IEnumerable<SelectListItem> Routes { get; set; } = new List<SelectListItem>();

        public IEnumerable<SelectListItem> Schedules { get; set; } = new List<SelectListItem>();

    }
}
