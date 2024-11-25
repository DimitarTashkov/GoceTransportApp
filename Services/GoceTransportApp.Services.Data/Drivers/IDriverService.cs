using GoceTransportApp.Web.ViewModels.Drivers;
using GoceTransportApp.Web.ViewModels.Routes;
using GoceTransportApp.Web.ViewModels.Streets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoceTransportApp.Services.Data.Drivers
{
    public interface IDriverService
    {
        Task<IEnumerable<DriverDataViewModel>> GetAllDrivers();

        Task CreateAsync(DriverInputModel inputModel);

        Task<EditDriverInputModel> GetDriverForEdit(Guid id);

        Task<bool> EditDriverAsync(EditDriverInputModel inputModel);

        Task<RemoveDriverViewModel> GetDriverForDeletion(Guid id);

        Task<bool> RemoveDriverAsync(RemoveDriverViewModel inputModel);
        Task<DriverDetailsViewModel> DriverDetails(Guid id);
    }
}
