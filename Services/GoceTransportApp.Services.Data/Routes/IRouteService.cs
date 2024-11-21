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

        Task<IEnumerable<RouteDataViewModel>> SearchForCity(string searchedTerm);

        Task<IEnumerable<RouteDataViewModel>> GetAllRoutesInOrganization(Guid organization);

        Task CreateAsync(RouteInputModel inputModel);

        Task<EditRouteInputModel> GetRouteForEdit(Guid id);

        Task<bool> EditRouteAsync(EditRouteInputModel inputModel);

        Task<RemoveRouteViewModel> GetRouteForDeletion(Guid id);

        Task<bool> ArchiveRouteAsync(RemoveRouteViewModel model);

        Task<bool> DeleteRouteAsync(RemoveRouteViewModel model);
    }
}
