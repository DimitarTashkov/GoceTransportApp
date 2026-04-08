using GoceTransportApp.Data.Models;
using GoceTransportApp.Web.Infrastructure.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using static GoceTransportApp.Common.ResultMessages.ScheduleMessages;
using static GoceTransportApp.Common.EntityValidationConstants.ScheduleConstants;

namespace GoceTransportApp.Web.ViewModels.Schedules
{
    public class EditScheduleInputModel
    {
        [Required]
        public string Id { get; set; } = null!;

        [Required]
        public string RecurrencePattern { get; set; } = "SpecificDay";

        public string Day { get; set; }

        [Required]
        [IsBefore("Arrival", ErrorMessage = InvalidScheduleDepartureTime)]
        public DateTime Departure { get; set; }

        [Required]
        public DateTime Arrival { get; set; }

        [Required]
        public string OrganizationId { get; set; }

        [Required]
        public string VehicleId { get; set; }

        [Required]
        public string RouteId { get; set; }
        public IEnumerable<SelectListItem> Routes { get; set; } = new List<SelectListItem>();

        public IEnumerable<SelectListItem> Vehicles { get; set; } = new List<SelectListItem>();

        [MaxLength(200)]
        public string? LiveStatus { get; set; }

        public HashSet<Ticket> ScheduleTickets { get; set; }
        = new HashSet<Ticket>();
    }
}
