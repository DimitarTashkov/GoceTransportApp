namespace GoceTransportApp.Data.Models
{
    using GoceTransportApp.Data.Common.Models;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using static GoceTransportApp.Common.EntityValidationConstants.StreetConstants;
    public class Street : BaseDeletableModel<Guid>
    {
        public Street()
        {
            this.Id = Guid.NewGuid();
        }

        [Required]
        [MaxLength(MaxStreetLength)]
        public string Name { get; set; } = null!;

        [InverseProperty("FromStreet")]
        public HashSet<Route> FromStreetRoutes { get; set; }
        = new HashSet<Route>();

        [InverseProperty("ToStreet")]
        public HashSet<Route> ToStreetRoutes { get; set; }
        = new HashSet<Route>();

        public HashSet<CityStreet> StreetsCities { get; set; }
       = new HashSet<CityStreet>();
    }
}
