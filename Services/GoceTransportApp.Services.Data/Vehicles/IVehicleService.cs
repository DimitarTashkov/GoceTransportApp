using GoceTransportApp.Web.ViewModels.Drivers;
using GoceTransportApp.Web.ViewModels.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoceTransportApp.Services.Data.Vehicles
{
    public interface IVehicleService
    {
        Task<IEnumerable<VehicleDataViewModel>> GetAllVehiclesAsync();

        Task CreateAsync(VehicleInputModel inputModel);

        Task<EditVehicleInputModel> GetVehicleForEditAsync(Guid id);

        Task<bool> EditVehicleAsync(EditVehicleInputModel inputModel);

        Task<RemoveVehicleViewModel> GetVehicleForDeletionAsync(Guid id);

        Task<bool> RemoveVehicleAsync(RemoveVehicleViewModel inputModel);

        Task<VehicleDetailsViewModel> GetVehicleDetailsAsync(Guid id);
    }
}
