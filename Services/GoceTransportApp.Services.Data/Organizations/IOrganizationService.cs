using GoceTransportApp.Web.ViewModels.Drivers;
using GoceTransportApp.Web.ViewModels.Organizations;
using GoceTransportApp.Web.ViewModels.Routes;
using GoceTransportApp.Web.ViewModels.Schedules;
using GoceTransportApp.Web.ViewModels.Tickets;
using GoceTransportApp.Web.ViewModels.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoceTransportApp.Services.Data.Organizations
{
    public interface IOrganizationService
    {
        Task<IEnumerable<OrganizationDataViewModel>> GetAllOrganizationsAsync(AllOrganizationsSearchFilterViewModel inputModel);

        Task<IEnumerable<OrganizationDataViewModel>> GetUserOrganizationsAsync(string userId);

        Task CreateAsync(OrganizationInputModel inputModel);

        Task<EditOrganizationInputModel> GetOrganizationForEditAsync(Guid id);

        Task<bool> EditOrganizationAsync(EditOrganizationInputModel inputModel);

        Task<RemoveOrganizationViewModel> GetOrganizationForDeletionAsync(Guid id);

        Task<bool> RemoveOrganizationAsync(RemoveOrganizationViewModel inputModel);

        Task<OrganizationDetailsViewModel> GetOrganizationDetailsAsync(Guid id);

        Task<IEnumerable<RouteDataViewModel>> GetRoutesByOrganizationId(Guid organizationId);

        Task<IEnumerable<DriverDataViewModel>> GetDriversByOrganizationId(Guid organizationId);

        Task<IEnumerable<VehicleDataViewModel>> GetVehiclesByOrganizationId(Guid organizationId);

        Task<IEnumerable<TicketDataViewModel>> GetTicketsByOrganizationId(Guid organizationId);

        Task<IEnumerable<ScheduleDataViewModel>> GetSchedulesByOrganizationId(Guid organizationId);

        Task<int> GetOrganizationsCountByFilterAsync(AllOrganizationsSearchFilterViewModel inputModel);

    }
}
