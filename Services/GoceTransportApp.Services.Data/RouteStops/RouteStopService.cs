namespace GoceTransportApp.Services.Data.RouteStops
{
    using GoceTransportApp.Data.Common.Repositories;
    using GoceTransportApp.Data.Models;
    using GoceTransportApp.Services.Data.Base;
    using GoceTransportApp.Web.ViewModels.Routes;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class RouteStopService : BaseService, IRouteStopService
    {
        private readonly IDeletableEntityRepository<RouteStop> routeStopRepository;

        public RouteStopService(IDeletableEntityRepository<RouteStop> routeStopRepository)
        {
            this.routeStopRepository = routeStopRepository;
        }

        public async Task<IEnumerable<RouteStopViewModel>> GetStopsForRouteAsync(Guid routeId)
        {
            return await this.routeStopRepository
                .AllAsNoTracking()
                .Where(s => s.RouteId == routeId)
                .OrderBy(s => s.Order)
                .Select(s => new RouteStopViewModel
                {
                    Id = s.Id.ToString(),
                    Name = s.Name,
                    Order = s.Order,
                    ArrivalTime = s.ArrivalTime,
                    DepartureTime = s.DepartureTime,
                    Latitude = s.Latitude,
                    Longitude = s.Longitude,
                })
                .ToListAsync();
        }

        public async Task AddStopAsync(RouteStopInputModel model)
        {
            var stop = new RouteStop
            {
                Name = model.Name,
                Order = model.Order,
                ArrivalTime = model.ArrivalTime,
                DepartureTime = model.DepartureTime,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                RouteId = Guid.Parse(model.RouteId),
                CreatedOn = DateTime.UtcNow,
            };

            await this.routeStopRepository.AddAsync(stop);
            await this.routeStopRepository.SaveChangesAsync();
        }

        public async Task<bool> RemoveStopAsync(Guid stopId)
        {
            RouteStop? stop = await this.routeStopRepository
                .FirstOrDefaultAsync(s => s.Id == stopId);

            if (stop == null)
            {
                return false;
            }

            await this.routeStopRepository.DeleteAsync(stop);
            return true;
        }
    }
}
