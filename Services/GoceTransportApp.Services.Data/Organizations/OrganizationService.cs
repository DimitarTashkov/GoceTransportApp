using GoceTransportApp.Data.Common.Repositories;
using GoceTransportApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using GoceTransportApp.Services.Data.Base;
using GoceTransportApp.Web.ViewModels.Drivers;
using GoceTransportApp.Web.ViewModels.Organizations;
using GoceTransportApp.Web.ViewModels.Routes;
using GoceTransportApp.Web.ViewModels.Schedules;
using GoceTransportApp.Web.ViewModels.Tickets;
using GoceTransportApp.Web.ViewModels.Vehicles;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoceTransportApp.Services.Data.Organizations
{
    public class OrganizationService : BaseService, IOrganizationService
    {
        public readonly IDeletableEntityRepository<Organization> organizationRepository;
        public readonly IDeletableEntityRepository<Route> routeRepository;
        public readonly IDeletableEntityRepository<Driver> driverRepository;
        public readonly IDeletableEntityRepository<Vehicle> vehicleRepository;
        public readonly IDeletableEntityRepository<Ticket> ticketRepository;
        public readonly IDeletableEntityRepository<Schedule> scheduleRepository;
        private readonly IRepository<UserFavoriteOrganization> favoriteRepository;

        public OrganizationService(
            IDeletableEntityRepository<Organization> organizationRepository,
            IDeletableEntityRepository<Route> routeRepository,
            IDeletableEntityRepository<Driver> driverRepository,
            IDeletableEntityRepository<Vehicle> vehicleRepository,
            IDeletableEntityRepository<Ticket> ticketRepository,
            IDeletableEntityRepository<Schedule> scheduleRepository,
            IRepository<UserFavoriteOrganization> favoriteRepository)
        {
            this.organizationRepository = organizationRepository;
            this.routeRepository = routeRepository;
            this.driverRepository = driverRepository;
            this.vehicleRepository = vehicleRepository;
            this.ticketRepository = ticketRepository;
            this.scheduleRepository = scheduleRepository;
            this.favoriteRepository = favoriteRepository;
        }

        public async Task<Guid> CreateAsync(OrganizationInputModel inputModel, string? imageUrl)
        {
            Organization organization = new Organization()
            {
                Name = inputModel.Name,
                Address = inputModel.Address,
                Phone = inputModel.Phone,
                FounderId = inputModel.FounderId,
                ImageUrl = imageUrl,
                IsOnTrial = true,
                TrialExpiresOn = DateTime.UtcNow.AddDays(14),
                CreatedOn = DateTime.UtcNow,
            };

            await organizationRepository.AddAsync(organization);
            await organizationRepository.SaveChangesAsync();

            return organization.Id;
        }

        public async Task<bool> EditOrganizationAsync(EditOrganizationInputModel inputModel, string? imageUrl)
        {
            var organization = await organizationRepository.GetByIdAsync(Guid.Parse(inputModel.Id));

            if (organization == null)
            {
                return false;
            }

            organization.Name = inputModel.Name;
            organization.Address = inputModel.Address;
            organization.Phone = inputModel.Phone;
            organization.FounderId = inputModel.FounderId;
            organization.ImageUrl = imageUrl;
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

        public async Task<IEnumerable<OrganizationDataViewModel>> GetAllOrganizationsAsync(AllOrganizationsSearchFilterViewModel inputModel)
        {
            IQueryable<Organization> allOrganizationQuery = organizationRepository
                .AllAsNoTracking()
                .Include(o => o.Founder);

            if (!string.IsNullOrEmpty(inputModel.SearchQuery))
            {
                allOrganizationQuery = allOrganizationQuery.Where(r => r.Name.Contains(inputModel.SearchQuery) ||
                                         r.Founder.UserName.Contains(inputModel.SearchQuery));
            }

            allOrganizationQuery = allOrganizationQuery
                .Skip((inputModel.EntitiesPerPage.Value * (inputModel.CurrentPage.Value - 1)) * inputModel.EntitiesPerPage.Value)
                .Take(inputModel.EntitiesPerPage.Value);

            return await allOrganizationQuery
                 .Select(c => new OrganizationDataViewModel()
                 {
                     Id = c.Id.ToString(),
                     Name = c.Name,
                     Address = c.Address,
                     FounderId = c.FounderId,
                     Founder = c.Founder.UserName,
                     ImageUrl = c.ImageUrl,
                 })
                .ToArrayAsync();
        }

        public async Task<IEnumerable<DriverDataViewModel>> GetDriversByOrganizationId(Guid organizationId)
        {
            IEnumerable<DriverDataViewModel> model = await driverRepository.AllAsNoTracking()
                .Where(o => o.OrganizationId == organizationId)
                .Select(o => new DriverDataViewModel()
                {
                    Id = o.Id.ToString(),
                    FirstName = o.FirstName,
                    LastName = o.LastName,
                    OrganizationId = o.OrganizationId.ToString(),
                })
                .ToArrayAsync();

            return model;
        }

        public async Task<OrganizationDetailsViewModel> GetOrganizationDetailsAsync(Guid id)
        {
            OrganizationDetailsViewModel viewModel = null;

            Organization? organization = await organizationRepository.AllAsNoTracking()
                .Include(o => o.Founder)
                .Include(o => o.OrganizationRoutes)
                .Include(o => o.OrganizationDrivers)
                .Include(o => o.OrganizationVehicles)
                .Include(o => o.OrganizationSchedules)
                .Include(o => o.OrganizationTickets)
                    .ThenInclude(t => t.Route)
                        .ThenInclude(r => r.FromCity)
                .Include(o => o.OrganizationTickets)
                    .ThenInclude(t => t.Route)
                        .ThenInclude(r => r.ToCity)
                .Include(o => o.OrganizationMessages)
                .Include(o => o.OrganizationReports)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (organization != null)
            {
                viewModel = new OrganizationDetailsViewModel()
                {
                    Id = organization.Id.ToString(),
                    Name = organization.Name,
                    Address = organization.Address,
                    Phone = organization.Phone,
                    FounderId = organization.FounderId,
                    Founder = organization.Founder.UserName,
                    ImageUrl = organization.ImageUrl,
                    IsOnTrial = organization.IsOnTrial && organization.TrialExpiresOn > DateTime.UtcNow,
                    TrialExpiresOn = organization.TrialExpiresOn,
                    OrganizationMessages = organization.OrganizationMessages,
                    OrganizationDrivers = organization.OrganizationDrivers,
                    OrganizationRoutes = organization.OrganizationRoutes,
                    OrganizationReports = organization.OrganizationReports,
                    OrganizationSchedules = organization.OrganizationSchedules,
                    OrganizationTickets = organization.OrganizationTickets,
                    OrganizationVehicles = organization.OrganizationVehicles,
                };
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
                   Phone = organization.Phone,
                   FounderId = organization.FounderId,
                   ExistingImageUrl = organization.ImageUrl,
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

        public async Task<int> GetOrganizationsCountByFilterAsync(AllOrganizationsSearchFilterViewModel inputModel)
        {
            IQueryable<Organization> allOrganizationQuery = organizationRepository
                .AllAsNoTracking()
                .Include(o => o.Founder);

            if (!string.IsNullOrEmpty(inputModel.SearchQuery))
            {
                allOrganizationQuery = allOrganizationQuery.Where(r => r.Name.Contains(inputModel.SearchQuery) ||
                                         r.Founder.UserName.Contains(inputModel.SearchQuery));
            }

            return await allOrganizationQuery.CountAsync();

        }

        public async Task<IEnumerable<RouteDataViewModel>> GetRoutesByOrganizationId(Guid organizationId)
        {
            IEnumerable<RouteDataViewModel> model = await routeRepository.AllAsNoTracking()
                .Include(c => c.FromCity)
                .Include(c => c.ToCity)
                .Include(c => c.FromStreet)
                .Include(c => c.ToStreet)
                .Where(o => o.OrganizationId == organizationId)
                .Select(route => new RouteDataViewModel()
                {
                    Id = route.Id.ToString(),
                    DepartingCity = route.FromCity.Name,
                    ArrivingCity = route.ToCity.Name,
                    DepartingStreet = route.FromStreet.Name,
                    ArrivingStreet = route.ToStreet.Name,
                    OrganizationId = route.OrganizationId.ToString(),
                })
                .ToArrayAsync();


            return model;
        }

        public async Task<IEnumerable<ScheduleDataViewModel>> GetSchedulesByOrganizationId(Guid organizationId)
        {
            IEnumerable<ScheduleDataViewModel> model = await scheduleRepository.AllAsNoTracking()
                .Include(r => r.Route)
                .ThenInclude(route => route.FromCity)
                .Include(r => r.Route)
                .ThenInclude(route => route.ToCity)
                .Where(o => o.OrganizationId == organizationId)
              .Select(c => new ScheduleDataViewModel()
              {
                  Id = c.Id.ToString(),
                  Day = c.Day.ToString(),
                  Departing = c.Departure.ToString("HH:mm"),
                  Arriving = c.Arrival.ToString("HH:mm"),
                  FromCity = c.Route.FromCity.Name,
                  ToCity = c.Route.ToCity.Name,
                  OrganizationId = c.OrganizationId.ToString(),
              })
              .ToArrayAsync();

            return model;
        }

        public async Task<IEnumerable<TicketDataViewModel>> GetTicketsByOrganizationId(Guid organizationId)
        {
            IEnumerable<TicketDataViewModel> model = await ticketRepository.AllAsNoTracking()
                .Include(r => r.TimeTable)
                .Where(o => o.OrganizationId == organizationId)
              .Select(c => new TicketDataViewModel()
              {
                  Id = c.Id.ToString(),
                  IssuedDate = c.IssuedDate.Date.ToString(),
                  ExpiryDate = c.ExpiryDate.Date.ToString(),
                  Price = c.Price.ToString(),
                  FromCity = c.Route.FromCity.Name,
                  ToCity = c.Route.ToCity.Name,
                  OrganizationId = c.OrganizationId.ToString(),
              })
              .ToArrayAsync();

            return model;
        }

        public async Task<IEnumerable<OrganizationDataViewModel>> GetUserOrganizationsAsync(string userId)
        {
            IEnumerable<OrganizationDataViewModel> model = await organizationRepository.AllAsNoTracking()
                .Include(o => o.Founder)
                .Where(o => o.FounderId == userId)
              .Select(c => new OrganizationDataViewModel()
              {
                  Id = c.Id.ToString(),
                  Name = c.Name,
                  Address = c.Address,
                  FounderId = c.FounderId,
                  Founder = c.Founder.UserName,
                  ImageUrl = c.ImageUrl
              })
              .ToArrayAsync();

            return model;
        }

        public async Task<IEnumerable<VehicleDataViewModel>> GetVehiclesByOrganizationId(Guid organizationId)
        {
            IEnumerable<VehicleDataViewModel> model = await vehicleRepository.AllAsNoTracking()
                .Where(o => o.OrganizationId == organizationId)
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

        public async Task<bool> ToggleFavoriteAsync(string userId, string orgId)
        {
            if (!Guid.TryParse(orgId, out Guid orgGuid))
            {
                return false;
            }

            var existing = await favoriteRepository
                .FirstOrDefaultAsync(f => f.UserId == userId && f.OrganizationId == orgGuid);

            if (existing != null)
            {
                await favoriteRepository.DeleteAsync(existing);
                return false;
            }

            await favoriteRepository.AddAsync(new UserFavoriteOrganization
            {
                UserId = userId,
                OrganizationId = orgGuid,
                CreatedOn = DateTime.UtcNow,
            });
            await favoriteRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsOrganizationFavoriteAsync(string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(orgId, out Guid orgGuid))
            {
                return false;
            }

            return await favoriteRepository.AllAsNoTracking()
                .AnyAsync(f => f.UserId == userId && f.OrganizationId == orgGuid);
        }

        public async Task<IEnumerable<OrganizationDataViewModel>> GetFavoriteOrganizationsByUserIdAsync(string userId)
        {
            return await favoriteRepository.AllAsNoTracking()
                .Where(f => f.UserId == userId)
                .Select(f => new OrganizationDataViewModel
                {
                    Id = f.Organization.Id.ToString(),
                    Name = f.Organization.Name,
                    Address = f.Organization.Address,
                    ImageUrl = f.Organization.ImageUrl,
                    FounderId = f.Organization.FounderId,
                    Founder = f.Organization.Founder.UserName,
                })
                .ToArrayAsync();
        }

        public async Task<bool> UserHasOrganizationsAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return false;
            }

            return await organizationRepository.AllAsNoTracking()
                .AnyAsync(o => o.FounderId == userId);
        }
    }
}
