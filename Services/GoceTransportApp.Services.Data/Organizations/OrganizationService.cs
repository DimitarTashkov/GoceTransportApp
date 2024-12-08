using GoceTransportApp.Data.Common.Repositories;
using GoceTransportApp.Data.Models;
using GoceTransportApp.Services.Data.Base;
using GoceTransportApp.Web.ViewModels.Organizations;
using GoceTransportApp.Web.ViewModels.Tickets;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoceTransportApp.Services.Data.Organizations
{
    public class OrganizationService : BaseService, IOrganizationService
    {
        public readonly IDeletableEntityRepository<Organization> organizationRepository;

        public OrganizationService(IDeletableEntityRepository<Organization> organizationRepository)
        {
            this.organizationRepository = organizationRepository;
        }

        public async Task CreateAsync(OrganizationInputModel inputModel)
        {
            Organization organization = new Organization()
            {
                Name = inputModel.Name,
                Address = inputModel.Address,
                Phone = inputModel.Phone,
                FounderId = inputModel.FounderId,
                ImageUrl = inputModel.ImageUrl,
                CreatedOn = DateTime.UtcNow,
            };

            await organizationRepository.AddAsync(organization);
            await organizationRepository.SaveChangesAsync();
        }

        public async Task<bool> EditOrganizationAsync(EditOrganizationInputModel inputModel)
        {
            var organization = await organizationRepository.GetByIdAsync(Guid.Parse(inputModel.Id));

            if (organization == null)
            {
                return false;
            }

            organization.Name = inputModel.Name;
            organization.Address = inputModel.Address;
            organization.Phone = inputModel.Phone;
            organization.ImageUrl = inputModel.ImageUrl;
            organization.FounderId = inputModel.FounderId;
            organization.OrganizationMessages = inputModel.OrganizationMessages;
            organization.OrganizationDrivers = inputModel.OrganizationDrivers;
            organization.OrganizationRoutes = inputModel.OrganizationRoutes;
            organization.OrganizationReports = inputModel.OrganizationReports;
            organization.OrganizationSchedules = inputModel.OrganizationSchedules;
            organization.OrganizationTickets = inputModel.OrganizationTickets;
            organization.OrganizationVehicles = inputModel.OrganizationVehicles;
            organization.ModifiedOn = DateTime.UtcNow;

            bool result = await organizationRepository.UpdateAsync(organization);

            return result;
        }

        public async Task<IEnumerable<OrganizationDataViewModel>> GetAllOrganizationsAsync()
        {
            IEnumerable<OrganizationDataViewModel> model = await organizationRepository.AllAsNoTracking()
              .Select(c => new OrganizationDataViewModel()
              {
                  Id = c.Id.ToString(),
                  Name = c.Name,
                  Address = c.Address,
                  ImageUrl = c.ImageUrl,
                  FounderId = c.FounderId
              })
              .ToArrayAsync();

            return model;
        }

        public async Task<OrganizationDetailsViewModel> GetOrganizationDetailsAsync(Guid id)
        {
            OrganizationDetailsViewModel viewModel = null;

            Organization? organization = await organizationRepository.AllAsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);

            if (organization != null)
            {
                viewModel.Id = organization.Id.ToString();
                viewModel.Name = organization.Name;
                viewModel.Address = organization.Address;
                viewModel.Phone = organization.Phone;
                viewModel.FounderId = organization.FounderId;
                viewModel.ImageUrl = organization.ImageUrl;
                viewModel.OrganizationMessages = organization.OrganizationMessages;
                viewModel.OrganizationDrivers = organization.OrganizationDrivers;
                viewModel.OrganizationRoutes = organization.OrganizationRoutes;
                viewModel.OrganizationReports = organization.OrganizationReports;
                viewModel.OrganizationSchedules = organization.OrganizationSchedules;
                viewModel.OrganizationTickets = organization.OrganizationTickets;
                viewModel.OrganizationVehicles = organization.OrganizationVehicles;
            }

            return viewModel;
        }

        public async Task<RemoveOrganizationViewModel> GetOrganizationForDeletionAsync(Guid id)
        {
            RemoveOrganizationViewModel deleteModel = await organizationRepository.AllAsNoTracking()
                .Include(user => user.Founder)
                .Select(organization => new RemoveOrganizationViewModel()
                {
                    Id = organization.Id.ToString(),
                    Name = organization.Name,
                    Address = organization.Address,
                    ImageUrl = organization.ImageUrl,
                    FounderId = organization.FounderId,
                    FounderName = organization.Founder.UserName,
                })
                .FirstOrDefaultAsync(s => s.Id.ToLower() == id.ToString().ToLower());

            return deleteModel;
        }

        public async Task<EditOrganizationInputModel> GetOrganizationForEditAsync(Guid id)
        {
            EditOrganizationInputModel editModel = await organizationRepository.AllAsNoTracking()
               .Select(organization => new EditOrganizationInputModel()
               {
                   Id = organization.Id.ToString(),
                   Name = organization.Name,
                   Address = organization.Address,
                   ImageUrl = organization.ImageUrl,
                   Phone = organization.Phone,
                   FounderId = organization.FounderId,
                   OrganizationDrivers = organization.OrganizationDrivers,
                   OrganizationMessages = organization.OrganizationMessages,
                   OrganizationReports = organization.OrganizationReports,
                   OrganizationRoutes = organization.OrganizationRoutes,
                   OrganizationSchedules = organization.OrganizationSchedules,
                   OrganizationTickets = organization.OrganizationTickets,
                   OrganizationVehicles = organization.OrganizationVehicles,
               })
               .FirstOrDefaultAsync(s => s.Id.ToLower() == id.ToString().ToLower());

            return editModel;
        }

        public async Task<bool> RemoveOrganizationAsync(RemoveOrganizationViewModel inputModel)
        {
            Guid organizationGuid = Guid.Empty;
            bool isOrganizationGuidValid = this.IsGuidValid(inputModel.Id, ref organizationGuid);

            if (!isOrganizationGuidValid)
            {
                return false;
            }

            Organization organization = await organizationRepository
                .FirstOrDefaultAsync(s => s.Id == organizationGuid);

            if (organization == null)
            {
                return false;
            }

            await organizationRepository.DeleteAsync(organization);

            return true;
        }
    }
}
