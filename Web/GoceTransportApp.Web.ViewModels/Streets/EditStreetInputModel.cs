using GoceTransportApp.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static GoceTransportApp.Common.EntityValidationConstants.StreetConstants;
using static GoceTransportApp.Common.ErrorMessages.StreetMessages;

namespace GoceTransportApp.Web.ViewModels.Streets
{
    public class EditStreetInputModel
    {
        [Required]
        public string Id { get; set; }

        [Required(ErrorMessage = StreetNameRequiredMessage)]
        [MinLength(MinStreetLength)]
        [MaxLength(MaxStreetLength)]
        public string Street { get; set; }

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
