using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static GoceTransportApp.Common.EntityValidationConstants.CityConstants;

namespace GoceTransportApp.Web.ViewModels.Cities
{
    public class EditCityInputModel
    {
        [Required]
        public string Id { get; set; }

        [Required]
        [MinLength(MinNameLength)]
        [MaxLength(MaxNameLength)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(MaxStateLength)]
        [MinLength(MinStateLength)]
        public string State { get; set; } = null!;

        [Required]
        [MinLength(MinZipCodeLength)]
        [MaxLength(MaxZipCodeLength)]
        public string ZipCode { get; set; } = null!;
    }
}
