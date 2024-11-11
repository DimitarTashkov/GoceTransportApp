using GoceTransportApp.Data.Common.Repositories;
using GoceTransportApp.Data.Models;
using GoceTransportApp.Services.Data.Base;
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
    public class CityService : BaseService, ICityService
    {
        private readonly IRepository<City> cityRepository;
        private readonly IRepository<Street> streetRepository;
        private readonly IRepository<CityStreet> cityStreetRepository;

        public CityService(IRepository<City> cityRepository, IRepository<Street> streetRepository, IRepository<CityStreet> cityStreetRepository)
        {
            this.cityRepository = cityRepository;
            this.streetRepository = streetRepository;
            this.cityStreetRepository = cityStreetRepository;
        }

        public async Task<bool> AddStreetToCity(Guid streetId, CitiesAddStreetInputModel model)
        {
            Street street = await streetRepository.GetByIdAsync(streetId);

            if (street == null)
            {
                return false;
            }

            ICollection<CityStreet> entitiesToAdd = new List<CityStreet>();
            foreach (StreetsCheckBoxItemInputModel cinemaInputModel in model.Streets)
            {
                Guid cityGuid = Guid.Empty;
                bool isCityGuidValid = this.IsGuidValid(cinemaInputModel.Id, ref cityGuid);
                if (!isCityGuidValid)
                {
                    return false;
                }

                City? city = await this.cityRepository
                    .GetByIdAsync(cityGuid);
                if (city == null)
                {
                    return false;
                }

                CityStreet? cityStreet = await this.cityStreetRepository.GetAllAttached()
                    .FirstOrDefaultAsync(cs => cs.StreetId == streetId &&
                                                     cs.CityId == cityGuid);
                if (cinemaInputModel.IsSelected)
                {
                    if (cityStreet == null)
                    {
                        entitiesToAdd.Add(new CityStreet()
                        {
                            City = city,
                            Street = street
                        });
                    }
                    else
                    {
                        cityStreet.IsDeleted = false;
                    }
                }
                else
                {
                    if (cityStreet != null)
                    {
                        cityStreet.IsDeleted = true;
                    }
                }

            }

            await this.cityStreetRepository.AddRangeAsync(entitiesToAdd.ToArray());

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

        public async Task<CitiesAddStreetInputModel> GetAddMovieToCinemaInputModelByIdAsync(Guid cityId)
        {
            City? city = await this.cityRepository
                .GetByIdAsync(cityId);

            CitiesAddStreetInputModel? viewModel = null;
            if (city != null)
            {
                viewModel = new CitiesAddStreetInputModel()
                {
                    Id = cityId.ToString(),
                    Name = city.Name,
                    Streets = await this.streetRepository
                        .GetAllAttached()
                        .Include(c => c.StreetsCities)
                        .ThenInclude(sc => sc.City)
                        .Select(c => new StreetsCheckBoxItemInputModel()
                        {
                            Id = c.Id.ToString(),
                            Name = c.Name,
                            IsSelected = c.StreetsCities
                                .Any(sc => sc.City.Id == cityId &&
                                           sc.IsDeleted == false)
                        })
                        .ToListAsync()
                };
            }

            return viewModel;
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

    }
}
