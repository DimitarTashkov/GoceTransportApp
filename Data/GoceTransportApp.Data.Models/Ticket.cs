namespace GoceTransportApp.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using GoceTransportApp.Data.Common.Models;

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
        public Guid ShceduleId { get; set; }

        [ForeignKey(nameof(ShceduleId))]
        public Schedule TimeTable { get; set; }

        [Required]
        public string CustomerId { get; set; } = null!;

        [ForeignKey(nameof(CustomerId))]
        public ApplicationUser Customer { get; set; }

        public HashSet<Ticket> Tickets { get; set; }
        = new HashSet<Ticket>();
    }
}
