using GoceTransportApp.Data.Common.Repositories;
using GoceTransportApp.Data.Models;
using GoceTransportApp.Services.Data.Base;
using GoceTransportApp.Web.ViewModels.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoceTransportApp.Services.Data.Vehicles
{
    public class VehicleService : BaseService, IVehicleService
    {
        private IDeletableEntityRepository<Vehicle> vehicleRepository;

        public VehicleService(IDeletableEntityRepository<Vehicle> vehicleRepository)
        {
            this.vehicleRepository = vehicleRepository;
        }

        public Task CreateAsync(VehicleInputModel inputModel)
        {
            throw new NotImplementedException();
        }

        public Task<bool> EditVehicleAsync(EditVehicleInputModel inputModel)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<VehicleDataViewModel>> GetAllVehicles()
        {
            throw new NotImplementedException();
        }

        public Task<RemoveVehicleViewModel> GetVehicleForDeletion(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<EditVehicleInputModel> GetVehicleForEdit(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveVehicleAsync(RemoveVehicleViewModel inputModel)
        {
            throw new NotImplementedException();
        }

        public Task<VehicleDetailsViewModel> VehicleDetails(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
