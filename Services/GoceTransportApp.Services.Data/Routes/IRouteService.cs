using GoceTransportApp.Web.ViewModels.Cities;
using GoceTransportApp.Web.ViewModels.Routes;
using GoceTransportApp.Web.ViewModels.Streets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoceTransportApp.Services.Data.Routes
{
    public interface IRouteService
    {
        Task<IEnumerable<RouteDataViewModel>> GetAllRoutes();

        Task<RouteDetailsViewModel> GetRouteInformation(Guid id);

        Task<RouteDetailsViewModel> GetRouteInformationFromCityToCity(Guid fromCity, Guid toCity);

        Task<IEnumerable<RouteDataViewModel>> GetAllDepartingRoutesFromCity(Guid fromCity);

        Task<IEnumerable<RouteDataViewModel>> GetAllArrivingRoutesToCity(Guid fromCity);

        Task<IEnumerable<RouteDataViewModel>> GetAllDepartingRoutesFromStreet(Guid fromCity);

        Task<IEnumerable<RouteDataViewModel>> GetAllArrivingRoutesToStreet(Guid fromCity);

        Task CreateAsync(RouteInputModel inputModel);

        Task<EditRouteInputModel> GetRouteForEdit(Guid id);

        Task<bool> EditRouteAsync(EditRouteInputModel inputModel);

        Task<RemoveRouteViewModel> GetRouteForDeletion(Guid id);

        Task<bool> ArchiveRouteAsync(RemoveRouteViewModel model);

        Task<bool> DeleteRouteAsync(RemoveRouteViewModel model);
    }
}
