namespace GoceTransportApp.Web.ViewModels.Analytics
{
    using System.Collections.Generic;

    public class OrganizationAnalyticsViewModel
    {
        public string OrganizationId { get; set; } = null!;
        public string OrganizationName { get; set; } = null!;

        // ── Basic (Free+) ──────────────────────────────────────────────────
        public int TotalRoutes { get; set; }
        public int TotalSchedules { get; set; }
        public int TotalVehicles { get; set; }
        public int TotalDrivers { get; set; }
        public int TotalTickets { get; set; }

        // ── Extended (Starter+) ────────────────────────────────────────────
        public Dictionary<string, int> SchedulesByRecurrence { get; set; }
            = new Dictionary<string, int>();

        public Dictionary<string, int> SchedulesByDay { get; set; }
            = new Dictionary<string, int>();

        public int TotalRouteStops { get; set; }
        public double TotalRouteDistanceKm { get; set; }
        public double AverageRouteDistanceKm { get; set; }
        public int TotalFollowers { get; set; }

        // ── Full (Pro+) ────────────────────────────────────────────────────
        public decimal TotalRevenue { get; set; }
        public int TotalBoardedPassengers { get; set; }
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }

        public IEnumerable<RoutePerformanceItem> TopRoutes { get; set; }
            = new List<RoutePerformanceItem>();
    }

    public class RoutePerformanceItem
    {
        public string RouteLabel { get; set; } = null!;   // "Sofia → Plovdiv"
        public int TicketCount { get; set; }
        public decimal Revenue { get; set; }
    }
}
