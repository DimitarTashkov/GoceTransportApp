namespace GoceTransportApp.Services.Data.Analytics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using GoceTransportApp.Data.Common.Repositories;
    using GoceTransportApp.Data.Models;
    using GoceTransportApp.Data.Models.Enumerations;
    using GoceTransportApp.Web.ViewModels.Analytics;

    using Microsoft.EntityFrameworkCore;

    public class AnalyticsService : IAnalyticsService
    {
        private readonly IDeletableEntityRepository<Route>    routeRepository;
        private readonly IDeletableEntityRepository<Schedule> scheduleRepository;
        private readonly IDeletableEntityRepository<Vehicle>  vehicleRepository;
        private readonly IDeletableEntityRepository<Driver>   driverRepository;
        private readonly IDeletableEntityRepository<Ticket>   ticketRepository;
        private readonly IDeletableEntityRepository<RouteStop> routeStopRepository;
        private readonly IDeletableEntityRepository<Organization> organizationRepository;
        private readonly IRepository<UserFavoriteOrganization> favoriteRepository;
        private readonly IRepository<UserTicket>              userTicketRepository;
        private readonly IRepository<Review>                  reviewRepository;

        public AnalyticsService(
            IDeletableEntityRepository<Route>    routeRepository,
            IDeletableEntityRepository<Schedule> scheduleRepository,
            IDeletableEntityRepository<Vehicle>  vehicleRepository,
            IDeletableEntityRepository<Driver>   driverRepository,
            IDeletableEntityRepository<Ticket>   ticketRepository,
            IDeletableEntityRepository<RouteStop> routeStopRepository,
            IDeletableEntityRepository<Organization> organizationRepository,
            IRepository<UserFavoriteOrganization> favoriteRepository,
            IRepository<UserTicket>              userTicketRepository,
            IRepository<Review>                  reviewRepository)
        {
            this.routeRepository        = routeRepository;
            this.scheduleRepository     = scheduleRepository;
            this.vehicleRepository      = vehicleRepository;
            this.driverRepository       = driverRepository;
            this.ticketRepository       = ticketRepository;
            this.routeStopRepository    = routeStopRepository;
            this.organizationRepository = organizationRepository;
            this.favoriteRepository     = favoriteRepository;
            this.userTicketRepository   = userTicketRepository;
            this.reviewRepository       = reviewRepository;
        }

        public async Task<OrganizationAnalyticsViewModel> GetOrganizationAnalyticsAsync(Guid organizationId)
        {
            var orgName = await organizationRepository.AllAsNoTracking()
                .Where(o => o.Id == organizationId)
                .Select(o => o.Name)
                .FirstOrDefaultAsync() ?? string.Empty;

            // ── Basic counts ────────────────────────────────────────────────
            var routes    = await routeRepository.AllAsNoTracking()
                .Where(r => r.OrganizationId == organizationId)
                .Select(r => new { r.Distance })
                .ToListAsync();

            int totalRoutes    = routes.Count;
            int totalSchedules = await scheduleRepository.AllAsNoTracking()
                .CountAsync(s => s.OrganizationId == organizationId);
            int totalVehicles  = await vehicleRepository.AllAsNoTracking()
                .CountAsync(v => v.OrganizationId == organizationId);
            int totalDrivers   = await driverRepository.AllAsNoTracking()
                .CountAsync(d => d.OrganizationId == organizationId);
            int totalTickets   = await ticketRepository.AllAsNoTracking()
                .CountAsync(t => t.OrganizationId == organizationId);

            // ── Extended ────────────────────────────────────────────────────
            // Pull raw values to client first — EF cannot translate enum.ToString() to SQL
            var scheduleBreakdown = await scheduleRepository.AllAsNoTracking()
                .Where(s => s.OrganizationId == organizationId)
                .Select(s => new { s.RecurrencePattern, s.Day })
                .ToListAsync();

            var byRecurrence = scheduleBreakdown
                .GroupBy(x => x.RecurrencePattern.ToString())
                .ToDictionary(g => g.Key, g => g.Count());

            var byDay = scheduleBreakdown
                .Where(x => x.RecurrencePattern == RecurrencePattern.SpecificDay)
                .GroupBy(x => x.Day.ToString())
                .ToDictionary(g => g.Key, g => g.Count());

            // RouteStops: join via RouteId → pull route IDs to client, then count
            var orgRouteIds = await routeRepository.AllAsNoTracking()
                .Where(r => r.OrganizationId == organizationId)
                .Select(r => r.Id)
                .ToListAsync();

            int routeStops = await routeStopRepository.AllAsNoTracking()
                .CountAsync(rs => orgRouteIds.Contains(rs.RouteId));

            double totalKm   = routes.Sum(r => r.Distance);
            double avgKm     = totalRoutes > 0 ? totalKm / totalRoutes : 0;

            int followers = await favoriteRepository.AllAsNoTracking()
                .CountAsync(f => f.OrganizationId == organizationId);

            // ── Full ────────────────────────────────────────────────────────
            var ticketIds = await ticketRepository.AllAsNoTracking()
                .Where(t => t.OrganizationId == organizationId)
                .Select(t => t.Id)
                .ToListAsync();

            decimal totalRevenue = await ticketRepository.AllAsNoTracking()
                .Where(t => t.OrganizationId == organizationId)
                .SumAsync(t => (decimal?)t.Price) ?? 0m;

            int boarded = await userTicketRepository.AllAsNoTracking()
                .CountAsync(ut => ticketIds.Contains(ut.TicketId) && ut.IsBoarded);

            var reviews = await reviewRepository.AllAsNoTracking()
                .Where(r => r.OrganizationId == organizationId)
                .Select(r => r.Rating)
                .ToListAsync();

            double avgRating    = reviews.Count > 0 ? reviews.Average() : 0;
            int    totalReviews = reviews.Count;

            // Top routes by ticket count
            var topRoutes = await ticketRepository.AllAsNoTracking()
                .Where(t => t.OrganizationId == organizationId)
                .Include(t => t.Route)
                    .ThenInclude(r => r.FromCity)
                .Include(t => t.Route)
                    .ThenInclude(r => r.ToCity)
                .GroupBy(t => new
                {
                    From = t.Route.FromCity.Name,
                    To   = t.Route.ToCity.Name,
                })
                .Select(g => new RoutePerformanceItem
                {
                    RouteLabel  = g.Key.From + " → " + g.Key.To,
                    TicketCount = g.Count(),
                    Revenue     = g.Sum(t => t.Price),
                })
                .OrderByDescending(r => r.TicketCount)
                .Take(5)
                .ToListAsync();

            return new OrganizationAnalyticsViewModel
            {
                OrganizationId           = organizationId.ToString(),
                OrganizationName         = orgName,
                TotalRoutes              = totalRoutes,
                TotalSchedules           = totalSchedules,
                TotalVehicles            = totalVehicles,
                TotalDrivers             = totalDrivers,
                TotalTickets             = totalTickets,
                SchedulesByRecurrence    = byRecurrence,
                SchedulesByDay           = byDay,
                TotalRouteStops          = routeStops,
                TotalRouteDistanceKm     = totalKm,
                AverageRouteDistanceKm   = Math.Round(avgKm, 1),
                TotalFollowers           = followers,
                TotalRevenue             = totalRevenue,
                TotalBoardedPassengers   = boarded,
                AverageRating            = Math.Round(avgRating, 1),
                TotalReviews             = totalReviews,
                TopRoutes                = topRoutes,
            };
        }
    }
}
