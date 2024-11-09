namespace GoceTransportApp.Data.Models
{
    using GoceTransportApp.Data.Common.Models;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Ticket : BaseDeletableModel<Guid>
    {
        public Ticket()
        {
            this.Id = Guid.NewGuid();
        }

        [Required]
        public DateTime IssuedDate { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public Guid RouteId { get; set; }

        [ForeignKey(nameof(RouteId))]
        public Route Route { get; set; } = null!;

        [Required]
        public Guid OrganizationId { get; set; }

        [ForeignKey(nameof(OrganizationId))]
        public Organization Organization { get; set; } = null!;

        [Required]
        public Guid ScheduleId { get; set; }

        [ForeignKey(nameof(ScheduleId))]
        public Schedule TimeTable { get; set; }

        [Required]
        public string CustomerId { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public ApplicationUser Customer { get; set; }
    }
}
