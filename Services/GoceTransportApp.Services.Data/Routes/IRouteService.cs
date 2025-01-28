using GoceTransportApp.Web.ViewModels.Cities;
using GoceTransportApp.Web.ViewModels.Routes;
using GoceTransportApp.Web.ViewModels.Streets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GoceTransportApp.Services.Data.Routes
{
    public interface IRouteService
    {
        Task<IEnumerable<RouteDataViewModel>> GetAllRoutesAsync(AllRoutesSearchFilterViewModel inputModel);

        Task<RouteDetailsViewModel> GetRouteInformationAsync(Guid id);

        Task CreateAsync(RouteInputModel inputModel);

        Task<EditRouteInputModel> GetRouteForEditAsync(Guid id);

        Task<bool> EditRouteAsync(EditRouteInputModel inputModel);

        Task<RemoveRouteViewModel> GetRouteForDeletionAsync(Guid id);

        Task<bool> ArchiveRouteAsync(RemoveRouteViewModel model);

        Task<bool> DeleteRouteAsync(RemoveRouteViewModel model);

        Task<int> GetRoutesCountByFilterAsync(AllRoutesSearchFilterViewModel inputModel);

        Task<IEnumerable<SelectListItem>> GetRoutesForOrganizationAsync(string organizationId);

    }
}
