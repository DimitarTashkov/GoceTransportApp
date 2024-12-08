namespace GoceTransportApp.Data.Models
{
    using GoceTransportApp.Data.Common.Models;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using static GoceTransportApp.Common.EntityValidationConstants.OrganizationConstants;
    using static GoceTransportApp.Common.GlobalConstants;

    public class Organization : BaseDeletableModel<Guid>
    {
        public Organization()
        {
            this.Id = Guid.NewGuid();
        }

        [Required]
        [MaxLength(MaxNameLength)]
        public string Name { get; set; } = null!;

        [MaxLength(MaxAddressLength)]
        public string Address { get; set; }

        [Required]
        [DefaultValue(DefaultOrganizationImageUrl)]
        public string ImageUrl { get; set; } = null!;

        [Required]
        public string FounderId { get; set; }

        [ForeignKey(nameof(FounderId))]
        public ApplicationUser Founder { get; set; }

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
