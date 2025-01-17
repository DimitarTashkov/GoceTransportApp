using GoceTransportApp.Data.Common.Repositories;
using GoceTransportApp.Data.Models;
using GoceTransportApp.Data.Models.Enumerations;
using GoceTransportApp.Services.Data.Base;
using GoceTransportApp.Web.ViewModels.Drivers;
using GoceTransportApp.Web.ViewModels.Streets;
using GoceTransportApp.Web.ViewModels.Vehicles;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static GoceTransportApp.Common.ErrorMessages.VehicleMessages;


namespace GoceTransportApp.Services.Data.Vehicles
{
    public class VehicleService : BaseService, IVehicleService
    {
        private IDeletableEntityRepository<Vehicle> vehicleRepository;

        public VehicleService(IDeletableEntityRepository<Vehicle> vehicleRepository)
        {
            this.vehicleRepository = vehicleRepository;
        }

        public async Task CreateAsync(VehicleInputModel inputModel)
        {
            if (!Enum.TryParse<VehicleStatus>(inputModel.Status, out var status))
            {
                throw new ArgumentException(InvalidVehicleStatus);
            }

            Vehicle vehicle = new Vehicle()
            {
                Number = inputModel.Number,
                Type = inputModel.Type,
                Manufacturer = inputModel.Manufacturer,
                Model = inputModel.Model,
                Capacity = inputModel.Capacity,
                FuelConsumption = inputModel.FuelConsumption,
                VehicleStatus = status,
                OrganizationId = Guid.Parse(inputModel.OrganizationId),
                CreatedOn = DateTime.UtcNow,
            };

            await vehicleRepository.AddAsync(vehicle);
            await vehicleRepository.SaveChangesAsync();
        }

        public async Task<bool> EditVehicleAsync(EditVehicleInputModel inputModel)
        {
            var vehicle = await vehicleRepository.GetByIdAsync(Guid.Parse(inputModel.Id));

            if (vehicle == null)
            {
                return false;
            }

            if (!Enum.TryParse<VehicleStatus>(inputModel.Status, out var status))
            {
                throw new ArgumentException(InvalidVehicleStatus);
            }

            vehicle.Number = inputModel.Number;
            vehicle.Type = inputModel.Type;
            vehicle.Manufacturer = inputModel.Manufacturer;
            vehicle.Model = inputModel.Model;
            vehicle.Capacity = inputModel.Capacity;
            vehicle.FuelConsumption = inputModel.FuelConsumption;
            vehicle.VehicleStatus = status;
            vehicle.OrganizationId = Guid.Parse(inputModel.OrganizationId);
            vehicle.ModifiedOn = DateTime.UtcNow;

            bool result = await vehicleRepository.UpdateAsync(vehicle);

            return result;
        }

        public async Task<IEnumerable<VehicleDataViewModel>> GetAllVehiclesAsync(AllVehiclesSearchFilterViewModel inputModel)
        {
            IQueryable<Vehicle> allVehiclesQuery = this.vehicleRepository
                .AllAsNoTracking();

            if (!String.IsNullOrWhiteSpace(inputModel.SearchQuery))
            {
                allVehiclesQuery = allVehiclesQuery
                    .Where(r => r.Manufacturer.Contains(inputModel.SearchQuery) ||
                                         r.Model.Contains(inputModel.SearchQuery) ||
                                         r.Type.Contains(inputModel.SearchQuery));
            }

            if (inputModel.CurrentPage.HasValue &&
                inputModel.EntitiesPerPage.HasValue)
            {
                allVehiclesQuery = allVehiclesQuery
                    .Skip(inputModel.EntitiesPerPage.Value * (inputModel.CurrentPage.Value - 1))
                    .Take(inputModel.EntitiesPerPage.Value);
            }

            IEnumerable<VehicleDataViewModel> model = await allVehiclesQuery
              .Select(c => new VehicleDataViewModel()
              {
                  Id = c.Id.ToString(),
                  Number = c.Number,
                  Type = c.Type,
                  Manufacturer = c.Manufacturer,
                  Model = c.Model,
                  OrganizationId = c.OrganizationId.ToString(),
              })
              .ToArrayAsync();

            return model;
        }

        public async Task<RemoveVehicleViewModel> GetVehicleForDeletionAsync(Guid id)
        {
            RemoveVehicleViewModel deleteModel = await vehicleRepository.AllAsNoTracking()
                .Select(vehicle => new RemoveVehicleViewModel()
                {
                    Id = vehicle.Id.ToString(),
                    Number = vehicle.Number,
                    OrganizationId = vehicle.OrganizationId.ToString(),
                })
                .FirstOrDefaultAsync(s => s.Id.ToLower() == id.ToString().ToLower());

            return deleteModel;
        }

        public async Task<EditVehicleInputModel> GetVehicleForEditAsync(Guid id)
        {
            EditVehicleInputModel editModel = await vehicleRepository.AllAsNoTracking()
              .Select(vehicle => new EditVehicleInputModel()
              {
                  Id = vehicle.Id.ToString(),
                  Number = vehicle.Number,
                  Type = vehicle.Type,
                  Manufacturer = vehicle.Manufacturer,
                  Model = vehicle.Model,
                  Capacity = vehicle.Capacity,
                  FuelConsumption = vehicle.FuelConsumption,
                  Status = vehicle.VehicleStatus.ToString(),
                  OrganizationId = vehicle.OrganizationId.ToString(),
                  Schedules = vehicle.Schedules.ToList(),
              })
              .FirstOrDefaultAsync(s => s.Id.ToLower() == id.ToString().ToLower());

            return editModel;
        }

        public async Task<bool> RemoveVehicleAsync(RemoveVehicleViewModel inputModel)
        {
            Guid vehicleGuid = Guid.Empty;
            bool isVehicleGuidValid = this.IsGuidValid(inputModel.Id, ref vehicleGuid);

            if (!isVehicleGuidValid)
            {
                return false;
            }

            Vehicle vehicle = await vehicleRepository
                .FirstOrDefaultAsync(s => s.Id == vehicleGuid);

            if (vehicle == null)
            {
                return false;
            }

            await vehicleRepository.DeleteAsync(vehicle);

            return true;
        }

        public async Task<VehicleDetailsViewModel> GetVehicleDetailsAsync(Guid id)
        {
            VehicleDetailsViewModel viewModel = null;

            Vehicle? vehicle = await vehicleRepository.AllAsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);
            if (vehicle != null)
            {
                viewModel = new VehicleDetailsViewModel()
                {
                    Number = vehicle.Number,
                    Type = vehicle.Type,
                    Manufacturer = vehicle.Manufacturer,
                    Model = vehicle.Model,
                    Capacity = vehicle.Capacity,
                    FuelConsumption = vehicle.FuelConsumption,
                    Status = vehicle.VehicleStatus.ToString(),
                    OrganizationId = vehicle.OrganizationId.ToString(),
                };
            }

            return viewModel;
        }

        public async Task<int> GetVehiclesCountByFilterAsync(AllVehiclesSearchFilterViewModel inputModel)
        {
            AllVehiclesSearchFilterViewModel inputModelCopy = new AllVehiclesSearchFilterViewModel()
            {
                CurrentPage = null,
                EntitiesPerPage = null,
                SearchQuery = inputModel.SearchQuery,
            };

            int vehiclesCount = (await this.GetAllVehiclesAsync(inputModelCopy))
                .Count();
            return vehiclesCount;
        }
    }
}
