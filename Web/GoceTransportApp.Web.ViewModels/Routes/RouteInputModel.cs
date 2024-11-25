using GoceTransportApp.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static GoceTransportApp.Common.ErrorMessages.RouteMessages;

namespace GoceTransportApp.Web.ViewModels.Routes
{
    public class RouteInputModel
    {
        [Required]
        public string FromCityId { get; set; } = null!;

        [Required]
        public string ToCityId { get; set; } = null!;

        [Required]
        public string FromStreetId { get; set; } = null!;

        [Required]
        public string ToStreetId { get; set; } = null!;

        [Required]
        public string OrganizationId { get; set; }

        [Required(ErrorMessage = RouteDistanceRequiredMessage)]
        public double Distance { get; set; }

        [Required(ErrorMessage = RouteDurationRequiredMessage)]
        public double Duration { get; set; }
    }
}
