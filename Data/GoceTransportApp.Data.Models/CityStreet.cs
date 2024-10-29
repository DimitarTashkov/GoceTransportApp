namespace GoceTransportApp.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Microsoft.EntityFrameworkCore;

    [PrimaryKey(nameof(CityId), nameof(StreetId))]
    public class CityStreet
    {
        [Required]
        public Guid CityId { get; set; }

        [ForeignKey(nameof(CityId))]
        public City City { get; set; } = null!;

        [Required]
        public Guid StreetId { get; set; }

        [ForeignKey(nameof(StreetId))]
        public Street Street { get; set; } = null!;

        [Required]
        [MaxLength(30)]
        public string StreetCode { get; set; } = null!;
    }
}
