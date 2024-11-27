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
        Task<IEnumerable<VehicleDataViewModel>> GetAllVehicles();

        Task CreateAsync(VehicleInputModel inputModel);

        Task<EditVehicleInputModel> GetVehicleForEdit(Guid id);

        Task<bool> EditVehicleAsync(EditVehicleInputModel inputModel);

        Task<RemoveVehicleViewModel> GetVehicleForDeletion(Guid id);

        Task<bool> RemoveVehicleAsync(RemoveVehicleViewModel inputModel);

        Task<VehicleDetailsViewModel> VehicleDetails(Guid id);
    }
}
