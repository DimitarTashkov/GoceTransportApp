namespace GoceTransportApp.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using GoceTransportApp.Data.Common.Models;

    public class Street : BaseModel<Guid>
    {
        public Street()
        {
            this.Id = Guid.NewGuid();
        }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = null!;

        public HashSet<CityStreet> StreetsCities { get; set; }
       = new HashSet<CityStreet>();
    }
}
