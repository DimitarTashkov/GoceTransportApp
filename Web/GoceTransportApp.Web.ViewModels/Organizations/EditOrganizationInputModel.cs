using GoceTransportApp.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static GoceTransportApp.Common.EntityValidationConstants.OrganizationConstants;
using static GoceTransportApp.Common.GlobalConstants;
using static GoceTransportApp.Common.ErrorMessages.OrganizationMessages;

namespace GoceTransportApp.Web.ViewModels.Organizations
{
    public class EditOrganizationInputModel
    {
        [Required]
        public string Id { get; set; } = null!;

        [Required(ErrorMessage = OrganizationNameRequired)]
        [MaxLength(MaxNameLength)]
        public string Name { get; set; } = null!;

        [MaxLength(MaxAddressLength)]
        public string Address { get; set; }

        [DefaultValue(DefaultOrganizationImageUrl)]
        public string ImageUrl { get; set; }

        [Required]
        public string FounderId { get; set; }

        [MaxLength(MaxPhoneNumberLength)]
        public string Phone { get; set; }

        public HashSet<Route> OrganizationRoutes { get; set; }
           = new HashSet<Route>();

        public HashSet<Driver> OrganizationDrivers { get; set; }
            = new HashSet<Driver>();

        public HashSet<Vehicle> OrganizationVehicles { get; set; }
            = new HashSet<Vehicle>();

        public HashSet<Ticket> OrganizationTickets { get; set; }
            = new HashSet<Ticket>();

        public HashSet<Schedule> OrganizationSchedules { get; set; }
            = new HashSet<Schedule>();

        public HashSet<Report> OrganizationReports { get; set; }
            = new HashSet<Report>();

        public HashSet<Message> OrganizationMessages { get; set; }
            = new HashSet<Message>();
    }
}
