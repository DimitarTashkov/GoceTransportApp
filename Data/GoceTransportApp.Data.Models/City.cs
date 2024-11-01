namespace GoceTransportApp.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using GoceTransportApp.Data.Common.Models;

    public class City : BaseModel<Guid>
    {
        public City()
        {
            this.Id = Guid.NewGuid();
        }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string State { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string Country { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public string ZipCode { get; set; } = null!;

        public HashSet<CityStreet> CitiesStreets { get; set; }
        = new HashSet<CityStreet>();
    }
}
