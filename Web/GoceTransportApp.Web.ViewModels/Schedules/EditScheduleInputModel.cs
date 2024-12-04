using GoceTransportApp.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoceTransportApp.Web.ViewModels.Schedules
{
    public class EditScheduleInputModel
    {
        [Required]
        public string Id { get; set; } = null!;

        [Required]
        public string Day { get; set; }

        [Required]
        public string Departure { get; set; }

        [Required]
        public string Arrival { get; set; }

        [Required]
        public string OrganizationId { get; set; }

        [Required]
        public string VehicleId { get; set; }

        [Required]
        public string RouteId { get; set; }

        public HashSet<Ticket> ScheduleTickets { get; set; }
        = new HashSet<Ticket>();
    }
}
