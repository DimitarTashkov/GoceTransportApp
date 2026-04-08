namespace GoceTransportApp.Data.Models
{
    using GoceTransportApp.Data.Common.Models;
    using GoceTransportApp.Data.Models.Enumerations;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Schedule : BaseDeletableModel<Guid>
    {
        public Schedule()
        {
            this.Id = Guid.NewGuid();
        }

        [Required]
        public DayOfWeek Day { get; set; }

        public RecurrencePattern RecurrencePattern { get; set; } = RecurrencePattern.SpecificDay;

        [Required]
        public DateTime Departure { get; set; }

        [Required]
        public DateTime Arrival { get; set; }

        [Required]
        public Guid OrganizationId { get; set; }

        [ForeignKey(nameof(OrganizationId))]
        public Organization Organization { get; set; }

        [Required]
        public Guid VehicleId { get; set; }

        [ForeignKey(nameof(VehicleId))]
        public Vehicle Vehicle { get; set; }

        [Required]
        public Guid RouteId { get; set; }

        [ForeignKey(nameof(RouteId))]
        public Route Route { get; set; } = null!;

        [MaxLength(200)]
        public string? LiveStatus { get; set; }

        public HashSet<Ticket> ScheduleTickets { get; set; }
        = new HashSet<Ticket>();
    }
}
