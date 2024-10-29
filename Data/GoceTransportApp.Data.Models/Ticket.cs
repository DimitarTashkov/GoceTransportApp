namespace GoceTransportApp.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using GoceTransportApp.Data.Common.Models;

    public class Ticket : BaseDeletableModel<Guid>
    {
        public Ticket()
        {
            this.Id = Guid.NewGuid();
        }

        //TODO: Add userId
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
    }
}
