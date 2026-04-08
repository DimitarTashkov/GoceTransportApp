namespace GoceTransportApp.Web.ViewModels.Routes
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class RouteStopInputModel
    {
        [Required]
        public string RouteId { get; set; } = null!;

        public string OrganizationId { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        [Display(Name = "Stop Name")]
        public string Name { get; set; } = null!;

        [Required]
        [Range(1, 100)]
        public int Order { get; set; }

        [Required]
        [Display(Name = "Arrival Time")]
        public TimeSpan ArrivalTime { get; set; }

        [Required]
        [Display(Name = "Departure Time")]
        public TimeSpan DepartureTime { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        // Populated by the controller for the Manage view
        public IEnumerable<RouteStopViewModel> ExistingStops { get; set; }
            = new List<RouteStopViewModel>();
    }
}
