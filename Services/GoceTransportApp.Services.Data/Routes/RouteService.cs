using GoceTransportApp.Data.Common.Repositories;
using GoceTransportApp.Data.Models;
using GoceTransportApp.Services.Data.Base;
using GoceTransportApp.Web.ViewModels.Cities;
using GoceTransportApp.Web.ViewModels.Routes;
using GoceTransportApp.Web.ViewModels.Streets;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoceTransportApp.Services.Data.Routes
{
    public class RouteService : BaseService, IRouteService
    {
        // TODO: Validate guids for all foreign keys

        private readonly IDeletableEntityRepository<Route> routeReposiory;

        public RouteService(IDeletableEntityRepository<Route> routeReposiory)
        {
            this.routeReposiory = routeReposiory;
        }

        public async Task<bool> ArchiveRouteAsync(RemoveRouteViewModel model)
        {
            Guid routeGuid = Guid.Empty;
            bool isRouteGuidValid = this.IsGuidValid(model.Id, ref routeGuid);

            if (!isRouteGuidValid)
            {
                return false;
            }

            Route route = await routeReposiory
                .FirstOrDefaultAsync(s => s.Id == routeGuid);

            if (route == null)
            {
                return false;
            }

            await routeReposiory.DeleteAsync(route);

            return true;
        }

        public async Task CreateAsync(RouteInputModel inputModel)
        {
            Route route = new Route()
            {
                Duration = inputModel.Duration,
                Distance = inputModel.Distance,
                FromCityId = Guid.Parse(inputModel.FromCityId),
                ToCityId = Guid.Parse(inputModel.ToCityId),
                FromStreetId = Guid.Parse(inputModel.FromStreetId),
                ToStreetId = Guid.Parse(inputModel.ToStreetId),
                CreatedOn = DateTime.UtcNow
            };

            await routeReposiory.AddAsync(route);
            await routeReposiory.SaveChangesAsync();
        }

        public async Task<bool> DeleteRouteAsync(RemoveRouteViewModel inputModel)
        {
            Guid routeGuid = Guid.Empty;
            bool isRouteGuidValid = this.IsGuidValid(inputModel.Id, ref routeGuid);

            if (!isRouteGuidValid)
            {
                return false;
            }

            Route route = await routeReposiory
                .FirstOrDefaultAsync(s => s.Id == routeGuid);

            if (route == null)
            {
                return false;
            }

             routeReposiory.HardDelete(route);
            await routeReposiory.SaveChangesAsync();

            return true;
        }

        public async Task<bool> EditRouteAsync(EditRouteInputModel inputModel)
        {
            Route route = new Route()
            {
                Duration = inputModel.Duration,
                Distance = inputModel.Distance,
                FromCityId = Guid.Parse(inputModel.FromCityId),
                ToCityId = Guid.Parse(inputModel.ToCityId),
                FromStreetId = Guid.Parse(inputModel.FromStreetId),
                ToStreetId = Guid.Parse(inputModel.ToStreetId),
                CreatedOn = DateTime.UtcNow
            };

            bool result = await routeReposiory.UpdateAsync(route);

            return result;
        }

        public async Task<IEnumerable<RouteDataViewModel>> GetAllRoutesConnectedWithCity(Guid id)
        {
                IEnumerable<RouteDataViewModel> model = await routeReposiory.GetAllAttached()
                .Include(c => c.FromCity)
                .Include(c => c.ToCity)
                .Include(c => c.FromStreet)
                .Include(c => c.ToStreet)
               .Where(route => route.FromCityId == id || route.ToCityId == id)
               .Select(route => ReturnDataViewModel(route))
               .AsNoTracking()
               .ToArrayAsync();

                return model;
        }

        public async Task<IEnumerable<RouteDataViewModel>> GetAllRoutes()
        {
            IEnumerable<RouteDataViewModel> model = await routeReposiory.GetAllAttached()
                .Include(c => c.FromCity)
                .Include(c => c.ToCity)
                .Include(c => c.FromStreet)
                .Include(c => c.ToStreet)
               .Select(route => ReturnDataViewModel(route))
               .AsNoTracking()
               .ToArrayAsync();

            return model;
        }

        public async Task<RemoveRouteViewModel> GetRouteForDeletion(Guid id)
        {
            RemoveRouteViewModel editModel = await routeReposiory.GetAllAttached()
                .Include(c => c.FromCity)
                .Include(c => c.ToCity)
                .Include(c => c.FromStreet)
                .Include(c => c.ToStreet)
                .Select(route => new RemoveRouteViewModel()
                {
                    Id = route.Id.ToString(),
                    DepartingCity = route.FromCity.Name,
                    ArrivingCity = route.ToCity.Name,
                    DepartingStreet = route.FromStreet.Name,
                    ArrivingStreet = route.ToStreet.Name,
                    OrganizationId = route.OrganizationId.ToString(),
                })
                .FirstOrDefaultAsync(s => s.Id.ToLower() == id.ToString().ToLower());

            return editModel;
        }

        public async Task<EditRouteInputModel> GetRouteForEdit(Guid id)
        {
            EditRouteInputModel editModel = await routeReposiory.GetAllAttached()
               .Select(route => new EditRouteInputModel()
               {
                   Id = route.Id.ToString(),
                   Duration = route.Duration,
                   Distance = route.Distance,
                   FromCityId = route.FromCityId.ToString(),
                   ToCityId = route.ToCityId.ToString(),
                   FromStreetId = route.FromStreetId.ToString(),
                   ToStreetId = route.ToStreetId.ToString(),
                   OrganizationId = route.OrganizationId.ToString(),
               })
               .FirstOrDefaultAsync(s => s.Id.ToLower() == id.ToString().ToLower());

            return editModel;
        }

        public async Task<RouteDetailsViewModel> GetRouteInformation(Guid id)
        {
            RouteDetailsViewModel viewModel = null;

            Route? route =
               await routeReposiory.GetAllAttached()
                .Include(c => c.FromCity)
                .Include(c => c.ToCity)
                .Include(c => c.FromStreet)
                .Include(c => c.ToStreet)
                .Include(c => c.Organization)
               .FirstOrDefaultAsync(c => c.Id == id);

            if (route != null)
            {
                viewModel = ReturnDetailsViewModel(route);
            }

            return viewModel;
        }

        public async Task<RouteDetailsViewModel> GetRouteInformationFromCityToCity(Guid fromCity, Guid toCity)
        {
            RouteDetailsViewModel viewModel = null;

            Route? route =
               await routeReposiory.GetAllAttached()
                .Include(c => c.FromCity)
                .Include(c => c.ToCity)
                .Include(c => c.FromStreet)
                .Include(c => c.ToStreet)
                .Include(c => c.Organization)
               .FirstOrDefaultAsync(c => c.FromCityId == fromCity && c.ToCityId == toCity);

            if (route != null)
            {
                viewModel = ReturnDetailsViewModel(route);
            }

            return viewModel;
        }

        private RouteDetailsViewModel ReturnDetailsViewModel(Route route)
        {
            RouteDetailsViewModel? viewModel = new RouteDetailsViewModel()
            {
                Id = route.Id.ToString(),
                DepartingCity = route.FromCity.Name,
                ArrivingCity = route.ToCity.Name,
                DepartingStreet = route.FromStreet.Name,
                ArrivingStreet = route.ToStreet.Name,
                Distance = route.Distance,
                Duration = route.Duration,
                Organization = route.Organization.Name,
            };

            return viewModel;
        }

        private RouteDataViewModel ReturnDataViewModel(Route route)
        {
            RouteDataViewModel? viewModel = new RouteDataViewModel()
            {
                Id = route.Id.ToString(),
                DepartingCity = route.FromCity.Name,
                ArrivingCity = route.ToCity.Name,
                DepartingStreet = route.FromStreet.Name,
                ArrivingStreet = route.ToStreet.Name,
            };

            return viewModel;
        }

        public async Task<IEnumerable<RouteDataViewModel>> GetAllRoutesInOrganization(Guid organization)
        {
            IEnumerable<RouteDataViewModel> model = await routeReposiory.GetAllAttached()
                .Include(c => c.FromCity)
                .Include(c => c.ToCity)
                .Include(c => c.FromStreet)
                .Include(c => c.ToStreet)
                .Where(route => route.OrganizationId == organization)
               .Select(route => ReturnDataViewModel(route))
               .AsNoTracking()
               .ToArrayAsync();

            return model;
        }

    }
}
