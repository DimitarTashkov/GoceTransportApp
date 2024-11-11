using GoceTransportApp.Data.Common.Repositories;
using GoceTransportApp.Data.Models;
using GoceTransportApp.Web.ViewModels.Cities;
using GoceTransportApp.Web.ViewModels.Streets;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoceTransportApp.Services.Data.Cities
{
    public class CityService : ICityService
    {
        private readonly IRepository<City> cityRepository;
        private readonly IRepository<Street> streetRepository;

        public CityService(IRepository<City> cityRepository, IRepository<Street> streetRepository)
        {
            this.cityRepository = cityRepository;
            this.streetRepository = streetRepository;
        }

        public async Task<bool> AddStreetToCity(Guid cityId, Guid streetId)
        {
            City city = await cityRepository.GetByIdAsync(cityId);
            Street street = await streetRepository.GetByIdAsync(streetId);

            if (city == null || street == null)
            {
                return false;
            }

            bool isExists = city.CityStreets.Any(cs => cs.CityId == cityId && cs.StreetId == streetId);

            if (isExists)
            {
                return false;
            }

            city.CityStreets.Add(new CityStreet()
            {
                CityId = cityId,
                StreetId = streetId
            });

            await cityRepository.SaveChangesAsync();

            return true;

        }

        public async Task CreateAsync(CitiesInputModel inputModel)
        {
            City city = new City()
            {
                Name = inputModel.Name,
                State = inputModel.State,
                ZipCode = inputModel.ZipCode,
                CreatedOn = DateTime.UtcNow
            };

            await cityRepository.AddAsync(city);
            await cityRepository.SaveChangesAsync();
        }

        public async Task<bool> DeleteCityAsync(Guid id)
        {
            City city = await cityRepository
                .FirstOrDefaultAsync(s => s.Id == id);

            if (city == null)
            {
                return false;
            }

            await cityRepository.DeleteAsync(city);

            return true;
        }

        public async Task<bool> EditCityAsync(EditCitiesInputModel inputModel)
        {
            City city = new City()
            {
                Name = inputModel.Name,
                State = inputModel.State,
                ZipCode = inputModel.ZipCode,
                ModifiedOn = DateTime.UtcNow
            };

            bool result = await cityRepository.UpdateAsync(city);

            return result;
        }

        public async Task<IEnumerable<CitiesDataViewModel>> GetAllCities()
        {
            IEnumerable<CitiesDataViewModel> model = await cityRepository.GetAllAttached()
               .Select(c => new CitiesDataViewModel()
               {
                   Id = c.Id.ToString(),
                   Name = c.Name,
                   State = c.Name,
                   ZipCode = c.ZipCode,
               })
               .AsNoTracking()
               .ToArrayAsync();

            return model;
        }

        public async Task<IEnumerable<StreetsDataViewModel>> GetAllStreetsInCity(Guid cityId)
        {
            IEnumerable<StreetsDataViewModel> model = await streetRepository.GetAllAttached()
                .Where(s => s.StreetsCities.Any(sc => sc.CityId == cityId))
                .Select(s => new StreetsDataViewModel()
                {
                    Id = s.Id.ToString(),
                    Name = s.Name,
                })
                .AsNoTracking()
                .ToArrayAsync();

            return model;
        }

        public async Task<CitiesDetailsViewModel> GetCityDetails(Guid id)
        {
            CitiesDetailsViewModel viewModel = null;

            City? city =
               await cityRepository.GetAllAttached()
               .Include(c => c.CityStreets)
               .ThenInclude(cs => cs.Street)
               .FirstOrDefaultAsync(c => c.Id == id);

            if (city != null)
            {
                viewModel = new CitiesDetailsViewModel()
                {
                    Id = city.Id.ToString(),
                    Name = city.Name,
                    State = city.Name,
                    ZipCode = city.ZipCode,
                    Streets = city.CityStreets
                    .Where(cs => cs.CityId == id).Select(cs => new StreetsDataViewModel()
                    {
                        Id = cs.StreetId.ToString(),
                        Name = cs.Street.Name
                    })
                   .ToArray()
                };
            }

            return viewModel;
        }

        public async Task<CitiesDetailsViewModel> GetCityDetailsByName(string name)
        {
            CitiesDetailsViewModel viewModel = null;

            City? city =
               await cityRepository.GetAllAttached()
               .Include(c => c.CityStreets)
               .ThenInclude(cs => cs.Street)
               .FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());

            if (city != null)
            {
                viewModel = new CitiesDetailsViewModel()
                {
                    Id = city.Id.ToString(),
                    Name = city.Name,
                    State = city.Name,
                    ZipCode = city.ZipCode,
                    Streets = city.CityStreets
                    .Where(cs => cs.City.Name.ToLower() == name.ToLower()).Select(cs => new StreetsDataViewModel()
                    {
                        Id = cs.StreetId.ToString(),
                        Name = cs.Street.Name
                    })
                   .ToArray()
                };
            }

            return viewModel;
        }

        public async Task<EditCitiesInputModel> GetCityForEdit(Guid id)
        {
            EditCitiesInputModel editModel = await cityRepository.GetAllAttached()
                .Select(c => new EditCitiesInputModel()
                {
                    Id = c.Id.ToString(),
                    Name = c.Name,
                    State = c.State,
                    ZipCode = c.ZipCode
                })
                .FirstOrDefaultAsync(s => s.Id.ToLower() == id.ToString().ToLower());

            return editModel;
        }

        public Task<bool> RemoveStreetFromCity(Guid cityId, Guid streetId)
        {
            throw new NotImplementedException();
        }
    }
}
