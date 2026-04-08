namespace GoceTransportApp.Web.ViewModels.Routes
{
    using System;

    public class RouteStopViewModel
    {
        public string Id { get; set; } = null!;

        public string Name { get; set; } = null!;

        public int Order { get; set; }

        public TimeSpan ArrivalTime { get; set; }

        public TimeSpan DepartureTime { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }
    }
}
