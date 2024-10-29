namespace GoceTransportApp.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using GoceTransportApp.Data.Common.Models;

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
        public decimal Duration { get; set; }

        [Required]
        public decimal Distance { get; set; }

        [Required]
        public Guid ToCityId { get; set; }

        [ForeignKey(nameof(ToCityId))]
        public City ToCity { get; set; } = null!;

        [Required]
        public Guid ToStreetId { get; set; }

        [ForeignKey(nameof(ToStreetId))]
        public Street ToStreet { get; set; } = null!;
    }
}
