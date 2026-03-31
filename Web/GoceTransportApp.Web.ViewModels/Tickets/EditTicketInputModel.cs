using GoceTransportApp.Data.Models;
using GoceTransportApp.Web.Infrastructure.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using static GoceTransportApp.Common.ResultMessages.TicketMessages;


namespace GoceTransportApp.Web.ViewModels.Tickets
{
    public class EditTicketInputModel
    {
        [Required]
        public string Id { get; set; }

        [Required]
        [IsBefore("ExpiryDate", ErrorMessage = InvalidTicketIssueTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        public DateTime IssuedDate { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

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

        public HashSet<UserTicket> TicketsUsers { get; set; }
        = new HashSet<UserTicket>();
        public IEnumerable<SelectListItem> Routes { get; set; } = new List<SelectListItem>();

        public IEnumerable<SelectListItem> Schedules { get; set; } = new List<SelectListItem>();
    }
}
