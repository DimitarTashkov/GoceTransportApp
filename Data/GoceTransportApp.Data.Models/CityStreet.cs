namespace GoceTransportApp.Data.Models
{
    using GoceTransportApp.Data.Common.Models;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [PrimaryKey(nameof(CityId), nameof(StreetId))]
    public class CityStreet : BaseDeletableModel<Guid>
    {
        [Required]
        public Guid CityId { get; set; }

        [ForeignKey(nameof(CityId))]
        public City City { get; set; } = null!;

        [Required]
        public Guid StreetId { get; set; }

        [ForeignKey(nameof(StreetId))]
        public Street Street { get; set; } = null!;
    }
}
