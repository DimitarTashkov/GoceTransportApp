namespace GoceTransportApp.Data.Models
{
    using GoceTransportApp.Data.Common.Models;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Route : BaseDeletableModel<Guid>
    {
        public Route()
        {
            this.Id = Guid.NewGuid();
        }

        [Required]
        public Guid FromCityId { get; set; }

        [ForeignKey(nameof(FromCityId))]
        public City FromCity { get; set; } = null!;

        [Required]
        public Guid FromStreetId { get; set; }

        [ForeignKey(nameof(FromStreetId))]
        public Street FromStreet { get; set; } = null!;

        [Required]
        public Guid ToCityId { get; set; }

        [ForeignKey(nameof(ToCityId))]
        public City ToCity { get; set; } = null!;

        [Required]
        public Guid ToStreetId { get; set; }

        [ForeignKey(nameof(ToStreetId))]
        public Street ToStreet { get; set; } = null!;

        [Required]
        public double Duration { get; set; }

        [Required]
        public double Distance { get; set; }

        public HashSet<Schedule> RouteSchedules { get; set; }
        = new HashSet<Schedule>();

        public HashSet<Ticket> RouteTickets { get; set; }
        = new HashSet<Ticket>();
    }
}
