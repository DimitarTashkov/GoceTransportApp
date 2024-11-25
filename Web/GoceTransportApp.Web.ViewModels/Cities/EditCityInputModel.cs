using GoceTransportApp.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static GoceTransportApp.Common.EntityValidationConstants.CityConstants;
using static GoceTransportApp.Common.ErrorMessages.CityMessages;
namespace GoceTransportApp.Web.ViewModels.Cities
{
    public class EditCityInputModel
    {
        [Required]
        public string Id { get; set; }

        [Required(ErrorMessage = CityNameRequiredMessage)]
        [MinLength(MinNameLength)]
        [MaxLength(MaxNameLength)]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = CityStateRequiredMessage)]
        [MaxLength(MaxStateLength)]
        [MinLength(MinStateLength)]
        public string State { get; set; } = null!;

        [Required(ErrorMessage = CityZipRequiredMessage)]
        [MinLength(MinZipCodeLength)]
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
