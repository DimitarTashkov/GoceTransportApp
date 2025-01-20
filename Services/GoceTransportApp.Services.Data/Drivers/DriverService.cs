using GoceTransportApp.Data.Common.Repositories;
using GoceTransportApp.Data.Models;
using GoceTransportApp.Data.Models.Enumerations;
using GoceTransportApp.Services.Data.Base;
using GoceTransportApp.Web.ViewModels.Cities;
using GoceTransportApp.Web.ViewModels.Drivers;
using GoceTransportApp.Web.ViewModels.Routes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static GoceTransportApp.Common.ResultMessages.DriverMessages;

namespace GoceTransportApp.Services.Data.Drivers
{
    public class DriverService : BaseService, IDriverService
    {
        private readonly IDeletableEntityRepository<Driver> driverRepository;

        public DriverService(IDeletableEntityRepository<Driver> driverRepository)
        {
            this.driverRepository = driverRepository;
        }

        public async Task CreateAsync(DriverInputModel inputModel)
        {
            if (!Enum.TryParse<DriverExperience>(inputModel.DrivingExperience, out var experience))
            {
                throw new ArgumentException(InvalidDrivingExperience);
            }

            Driver driver = new Driver()
            {
                FirstName = inputModel.FirstName,
                LastName = inputModel.LastName,
                Age = inputModel.Age,
                Experience = experience,
                CreatedOn = DateTime.UtcNow,
                OrganizationId = Guid.Parse(inputModel.OrganizationId)
            };

            await driverRepository.AddAsync(driver);
            await driverRepository.SaveChangesAsync();
        }

        public async Task<DriverDetailsViewModel> GetDriverDetailsAsync(Guid id)
        {
            DriverDetailsViewModel viewModel = null;

            Driver? driver = await driverRepository.AllAsNoTracking()
                .Include(o => o.Organization)
                .FirstOrDefaultAsync(d => d.Id == id);
            if (driver != null)
            {
                viewModel = new DriverDetailsViewModel()
                {
                    Id = driver.Id.ToString(),
                    FirstName = driver.FirstName,
                    LastName = driver.LastName,
                    Age = driver.Age,
                    DrivingExperience = driver.Experience.ToString(),
                    OrganizationId = driver.OrganizationId.ToString(),
                    OrganizationName = driver.Organization.Name,
                };
            }

            return viewModel;
        }

        public async Task<bool> EditDriverAsync(EditDriverInputModel inputModel)
        {
            var driver = await driverRepository.GetByIdAsync(Guid.Parse(inputModel.Id));

            if (driver == null)
            {
                return false;
            }

            if (!Enum.TryParse<DriverExperience>(inputModel.DrivingExperience, out var experience))
            {
                throw new ArgumentException(InvalidDrivingExperience);
            }

            driver.FirstName = inputModel.FirstName;
            driver.LastName = inputModel.LastName;
            driver.Age = inputModel.Age;
            driver.Experience = experience;
            driver.ModifiedOn = DateTime.UtcNow;
            driver.OrganizationId = Guid.Parse(inputModel.OrganizationId);

            bool result = await driverRepository.UpdateAsync(driver);

            return result;
        }

        public async Task<IEnumerable<DriverDataViewModel>> GetAllDriversAsync()
        {
            IEnumerable<DriverDataViewModel> model = await driverRepository.AllAsNoTracking()
              .Select(c => new DriverDataViewModel()
              {
                  Id = c.Id.ToString(),
                  FirstName = c.FirstName,
                  LastName = c.LastName,
                  OrganizationId = c.OrganizationId.ToString(),
              })
              .ToArrayAsync();

            return model;
        }

        public async Task<RemoveDriverViewModel> GetDriverForDeletionAsync(Guid id)
        {
            RemoveDriverViewModel deleteModel = await driverRepository.AllAsNoTracking()
                .Select(driver => new RemoveDriverViewModel()
                {
                    Id = driver.Id.ToString(),
                    FirstName = driver.FirstName,
                    LastName = driver.LastName,
                    OrganizationId = driver.OrganizationId.ToString()
                })
                .FirstOrDefaultAsync(s => s.Id.ToLower() == id.ToString().ToLower());

            return deleteModel;
        }

        public async Task<EditDriverInputModel> GetDriverForEditAsync(Guid id)
        {
            EditDriverInputModel editModel = await driverRepository.AllAsNoTracking()
              .Select(driver => new EditDriverInputModel()
              {
                  Id = driver.Id.ToString(),
                  FirstName = driver.FirstName,
                  LastName = driver.LastName,
                  Age = driver.Age,
                  DrivingExperience = driver.Experience.ToString(),
                  OrganizationId = driver.OrganizationId.ToString(),
              })
              .FirstOrDefaultAsync(s => s.Id.ToLower() == id.ToString().ToLower());

            return editModel;
        }

        public async Task<bool> RemoveDriverAsync(RemoveDriverViewModel inputModel)
        {
            Guid driverGuid = Guid.Empty;
            bool isDriverGuidValid = this.IsGuidValid(inputModel.Id, ref driverGuid);

            if (!isDriverGuidValid)
            {
                return false;
            }

            Driver driver = await driverRepository
                .FirstOrDefaultAsync(s => s.Id == driverGuid);

            if (driver == null)
            {
                return false;
            }

            bool result = await driverRepository.DeleteAsync(driver);

            return result;
        }
    }
}
