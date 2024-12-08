using GoceTransportApp.Web.ViewModels.Organizations;
using GoceTransportApp.Web.ViewModels.Tickets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoceTransportApp.Services.Data.Organizations
{
    public interface IOrganizationService
    {
        Task<IEnumerable<OrganizationDataViewModel>> GetAllOrganizationsAsync();

        Task CreateAsync(OrganizationInputModel inputModel);

        Task<EditOrganizationInputModel> GetOrganizationForEditAsync(Guid id);

        Task<bool> EditOrganizationAsync(EditOrganizationInputModel inputModel);

        Task<RemoveOrganizationViewModel> GetOrganizationForDeletionAsync(Guid id);

        Task<bool> RemoveOrganizationAsync(RemoveOrganizationViewModel inputModel);

        Task<OrganizationDetailsViewModel> GetOrganizationDetailsAsync(Guid id);
    }
}
