using GoceTransportApp.Data.Common.Repositories;
using GoceTransportApp.Data.Models;
using GoceTransportApp.Services.Data.Base;
using GoceTransportApp.Web.ViewModels.Cities;
using GoceTransportApp.Web.ViewModels.Routes;
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

        public async Task<IEnumerable<RouteDataViewModel>> GetAllArrivingRoutesToCity(Guid fromCity)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<RouteDataViewModel>> GetAllArrivingRoutesToStreet(Guid fromCity)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<RouteDataViewModel>> GetAllDepartingRoutesFromCity(Guid fromCity)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<RouteDataViewModel>> GetAllDepartingRoutesFromStreet(Guid fromCity)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<RouteDataViewModel>> GetAllRoutes()
        {
            throw new NotImplementedException();
        }

        public async Task<RemoveRouteViewModel> GetRouteForDeletion(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<EditRouteInputModel> GetRouteForEdit(Guid id)
        {
            EditRouteInputModel editModel = await routeReposiory.GetAllAttached()
               .Select(c => new EditRouteInputModel()
               {
                   Id = c.Id.ToString(),
                   Duration = c.Duration,
                   Distance = c.Distance,
                   FromCityId = c.FromCityId.ToString(),
                   ToCityId = c.ToCityId.ToString(),
                   FromStreetId = c.FromStreetId.ToString(),
                   ToStreetId = c.ToStreetId.ToString(),
                   OrganizationId = c.OrganizationId.ToString(),
               })
               .FirstOrDefaultAsync(s => s.Id.ToLower() == id.ToString().ToLower());

            return editModel;
        }

        public async Task<RouteDetailsViewModel> GetRouteInformation(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<RouteDetailsViewModel> GetRouteInformationFromCityToCity(Guid fromCity, Guid toCity)
        {
            throw new NotImplementedException();
        }
    }
}
