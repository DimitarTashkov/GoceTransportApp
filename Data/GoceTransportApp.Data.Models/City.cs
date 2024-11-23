namespace GoceTransportApp.Data.Models
{
    using GoceTransportApp.Data.Common.Models;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using static GoceTransportApp.Common.EntityValidationConstants.CityConstants;
    
    public class City : BaseDeletableModel<Guid>
    {
        public City()
        {
            this.Id = Guid.NewGuid();
        }

        [Required]
        [MaxLength(MaxNameLength)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(MaxStateLength)]
        public string State { get; set; } = null!;

        [Required]
        [MaxLength(MaxZipCodeLength)]
        public string ZipCode { get; set; } = null!;

        [InverseProperty("FromCity")]
        public HashSet<Route> FromCityRoutes { get; set; }
        = new HashSet<Route>();

        [InverseProperty("ToCity")]
        public HashSet<Route> ToCityRoutes { get; set; }
        = new HashSet<Route>();

        public HashSet<CityStreet> CityStreets { get; set; }
        = new HashSet<CityStreet>();

    }
}
