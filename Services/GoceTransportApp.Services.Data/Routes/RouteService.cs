﻿using GoceTransportApp.Data.Common.Repositories;
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
using System.Web.Mvc;

namespace GoceTransportApp.Services.Data.Routes
{
    public class RouteService : BaseService, IRouteService
    {
        // TODO: Validate guids for all foreign keys

        private readonly IDeletableEntityRepository<Route> routeReposiory;
        private readonly IDeletableEntityRepository<Street> streetReposiory;
        private readonly IDeletableEntityRepository<City> cityReposiory;

        public RouteService(IDeletableEntityRepository<Route> routeReposiory, IDeletableEntityRepository<Street> streetReposiory, IDeletableEntityRepository<City> cityReposiory)
        {
            this.routeReposiory = routeReposiory;
            this.cityReposiory = cityReposiory;
            this.streetReposiory = streetReposiory;
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
                OrganizationId = Guid.Parse(inputModel.OrganizationId),
                CreatedOn = DateTime.UtcNow,
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

            await routeReposiory.HardDelete(route);

            return true;
        }

        public async Task<bool> EditRouteAsync(EditRouteInputModel inputModel)
        {
            var route = await routeReposiory.GetByIdAsync(Guid.Parse(inputModel.Id));
            if (route == null)
            {
                return false;
            }

            route.Duration = inputModel.Duration;
            route.Distance = inputModel.Distance;
            route.FromCityId = Guid.Parse(inputModel.FromCityId);
            route.ToCityId = Guid.Parse(inputModel.ToCityId);
            route.FromStreetId = Guid.Parse(inputModel.FromStreetId);
            route.ToStreetId = Guid.Parse(inputModel.ToStreetId);
            route.ModifiedOn = DateTime.UtcNow;
            route.OrganizationId = Guid.Parse(inputModel.OrganizationId);
            route.RouteTickets = inputModel.RouteTickets;
            route.RouteSchedules = inputModel.RouteSchedules;

            bool result = await routeReposiory.UpdateAsync(route);

            return result;
        }

        public async Task<IEnumerable<RouteDataViewModel>> GetAllRoutesAsync(AllRoutesSearchFilterViewModel inputModel)
        {
            IQueryable<Route> allRoutesQuery = routeReposiory
                .AllAsNoTracking()
                .Include(c => c.FromCity)
                .Include(c => c.ToCity)
                .Include(c => c.FromStreet)
                .Include(c => c.ToStreet);

            if (!string.IsNullOrEmpty(inputModel.SearchQuery))
            {
                allRoutesQuery = allRoutesQuery.Where(r => r.FromCity.Name.Contains(inputModel.SearchQuery) ||
                                         r.ToCity.Name.Contains(inputModel.SearchQuery) ||
                                         r.FromStreet.Name.Contains(inputModel.SearchQuery) ||
                                         r.ToStreet.Name.Contains(inputModel.SearchQuery));
            }

            if (!string.IsNullOrEmpty(inputModel.DepartingCityFilter))
            {
                allRoutesQuery = allRoutesQuery.Where(r => r.FromCity.Name == inputModel.DepartingCityFilter);
            }

            if (!string.IsNullOrEmpty(inputModel.ArrivingCityFilter))
            {
                allRoutesQuery = allRoutesQuery.Where(r => r.ToCity.Name == inputModel.ArrivingCityFilter);
            }

            allRoutesQuery = allRoutesQuery
                .Skip((inputModel.EntitiesPerPage.Value * (inputModel.CurrentPage.Value - 1)) * inputModel.EntitiesPerPage.Value)
                .Take(inputModel.EntitiesPerPage.Value);

            return await allRoutesQuery
                .Select(r => ReturnDataViewModel(r))
                .ToArrayAsync();
        }

        public async Task<RemoveRouteViewModel> GetRouteForDeletionAsync(Guid id)
        {
            RemoveRouteViewModel editModel = await routeReposiory.AllAsNoTracking()
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

        public async Task<EditRouteInputModel> GetRouteForEditAsync(Guid id)
        {
            EditRouteInputModel editModel = await routeReposiory.AllAsNoTracking()
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
                   RouteSchedules = route.RouteSchedules,
                   RouteTickets = route.RouteTickets,
               })
               .FirstOrDefaultAsync(s => s.Id.ToLower() == id.ToString().ToLower());

            editModel.Streets = await streetReposiory
                .AllAsNoTracking()
                .ToListAsync();

            editModel.Cities = await cityReposiory
                .AllAsNoTracking()
                .ToListAsync();

            return editModel;
        }

        public async Task<RouteDetailsViewModel> GetRouteInformationAsync(Guid id)
        {
            RouteDetailsViewModel viewModel = null;

            Route? route =
               await routeReposiory.AllAsNoTracking()
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

        private static RouteDetailsViewModel ReturnDetailsViewModel(Route route)
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
                OrganizationId = route.OrganizationId.ToString(),
            };

            return viewModel;
        }

        private static RouteDataViewModel ReturnDataViewModel(Route route)
        {
            RouteDataViewModel? viewModel = new RouteDataViewModel()
            {
                Id = route.Id.ToString(),
                DepartingCity = route.FromCity.Name,
                ArrivingCity = route.ToCity.Name,
                DepartingStreet = route.FromStreet.Name,
                ArrivingStreet = route.ToStreet.Name,
                OrganizationId = route.OrganizationId.ToString(),
            };

            return viewModel;
        }

       

        public async Task<int> GetRoutesCountByFilterAsync(AllRoutesSearchFilterViewModel inputModel)
        {
            IQueryable<Route> allRoutesQuery = routeReposiory
                .AllAsNoTracking()
                .Include(c => c.FromCity)
                .Include(c => c.ToCity)
                .Include(c => c.FromStreet)
                .Include(c => c.ToStreet);

            if (!string.IsNullOrEmpty(inputModel.SearchQuery))
            {
                allRoutesQuery = allRoutesQuery.Where(r => r.FromCity.Name.Contains(inputModel.SearchQuery) ||
                                         r.ToCity.Name.Contains(inputModel.SearchQuery) ||
                                         r.FromStreet.Name.Contains(inputModel.SearchQuery) ||
                                         r.ToStreet.Name.Contains(inputModel.SearchQuery));
            }

            if (!string.IsNullOrEmpty(inputModel.DepartingCityFilter))
            {
                allRoutesQuery = allRoutesQuery.Where(r => r.FromCity.Name == inputModel.DepartingCityFilter);
            }

            if (!string.IsNullOrEmpty(inputModel.ArrivingCityFilter))
            {
                allRoutesQuery = allRoutesQuery.Where(r => r.ToCity.Name == inputModel.ArrivingCityFilter);
            }

            return await allRoutesQuery.CountAsync();
        }

        public async Task<IEnumerable<SelectListItem>> GetRoutesForOrganizationAsync(string organizationId)
        {
            return await routeReposiory.AllAsNoTracking()
                .Include(r => r.FromCity)
                .Include(r => r.FromStreet)
                .Include(r => r.ToCity)
                .Include(r => r.ToStreet)
                .Where(r => r.OrganizationId == Guid.Parse(organizationId))
                .Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = $"{r.FromCity.Name}, {r.FromStreet.Name} → {r.ToCity.Name}, {r.ToStreet.Name}"
                })
                .ToListAsync();
        }
    }
}
