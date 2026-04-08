namespace GoceTransportApp.Services.Data.RouteStops
{
    using GoceTransportApp.Web.ViewModels.Routes;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IRouteStopService
    {
        Task<IEnumerable<RouteStopViewModel>> GetStopsForRouteAsync(Guid routeId);

        Task AddStopAsync(RouteStopInputModel model);

        Task<bool> RemoveStopAsync(Guid stopId);
    }
}
