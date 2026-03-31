using GoceTransportApp.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using static GoceTransportApp.Common.ResultMessages.RouteMessages;

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
        [Range(0.1, 10000, ErrorMessage = "Distance must be between 0.1 and 10,000 km")]
        public double Distance { get; set; }

        [Required(ErrorMessage = RouteDurationRequiredMessage)]
        [Display(Name = "Duration (minutes)")]
        [Range(1, 1440, ErrorMessage = "Duration must be between 1 and 1440 minutes (24h)")]
        public double Duration { get; set; }
    }
}
