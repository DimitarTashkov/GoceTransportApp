﻿using GoceTransportApp.Data.Common.Repositories;
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
        private readonly IDeletableEntityRepository<City> cityRepository;
        private readonly IDeletableEntityRepository<Street> streetRepository;
        private readonly IDeletableEntityRepository<CityStreet> cityStreetRepository;

        public CityService(IDeletableEntityRepository<City> cityRepository, IDeletableEntityRepository<Street> streetRepository, IDeletableEntityRepository<CityStreet> cityStreetRepository)
        {
            this.cityRepository = cityRepository;
            this.streetRepository = streetRepository;
            this.cityStreetRepository = cityStreetRepository;
        }

        public async Task<bool> AddStreetToCityAsync(Guid cityId, CityAddStreetInputModel model)
        {
            City city = await cityRepository.GetByIdAsync(cityId);

            if (city == null)
            {
                return false;
            }

            ICollection<CityStreet> streetsToAdd = new List<CityStreet>();
            foreach (StreetCheckBoxItemInputModel cinemaInputModel in model.Streets)
            {
                Guid streetGuid = Guid.Empty;
                bool isCityGuidValid = this.IsGuidValid(cinemaInputModel.Id, ref streetGuid);
                if (!isCityGuidValid)
                {
                    return false;
                }

                Street? street = await this.streetRepository
                    .GetByIdAsync(streetGuid);
                if (street == null)
                {
                    return false;
                }

                CityStreet? cityStreet = await this.cityStreetRepository.AllWithDeleted()
                    .FirstOrDefaultAsync(cs => cs.StreetId == streetGuid &&
                                                     cs.CityId == cityId);
                if (cinemaInputModel.IsSelected)
                {
                    if (cityStreet == null)
                    {
                        streetsToAdd.Add(new CityStreet()
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
                        cityStreet.DeletedOn = DateTime.UtcNow;
                    }
                }

            }

            await this.cityStreetRepository.AddRangeAsync(streetsToAdd.ToArray());

            return true;

        }

        public async Task CreateAsync(CityInputModel inputModel)
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

            bool result = await cityRepository.DeleteAsync(city);

            return result;
        }

        public async Task<bool> EditCityAsync(EditCityInputModel inputModel)
        {
            var city = await cityRepository.GetByIdAsync(Guid.Parse(inputModel.Id));

            if (city == null)
            {
                return false;
            }

            city.Name = inputModel.Name;
            city.State = inputModel.State;
            city.ZipCode = inputModel.ZipCode;
            city.CityStreets = inputModel.CityStreets;
            city.FromCityRoutes = inputModel.FromCityRoutes;
            city.ToCityRoutes = inputModel.ToCityRoutes;
            city.ModifiedOn = DateTime.UtcNow;

            bool result = await cityRepository.UpdateAsync(city);

            return result;
        }

        public async Task<CityAddStreetInputModel> GetAddStreetToCityModelAsync(Guid cityId)
        {
            City? city = await this.cityRepository
                .GetByIdAsync(cityId);

            CityAddStreetInputModel? viewModel = null;
            if (city != null)
            {
                viewModel = new CityAddStreetInputModel()
                {
                    Id = cityId.ToString(),
                    Name = city.Name,
                    Streets = await this.streetRepository
                        .AllAsNoTracking()
                        .Include(c => c.StreetsCities)
                        .ThenInclude(sc => sc.City)
                        .Select(c => new StreetCheckBoxItemInputModel()
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

        public async Task<IEnumerable<CityDataViewModel>> GetAllCitiesAsync(AllCitiesSearchFilterViewModel inputModel)
        {
            IQueryable<City> allCitiesQuery = this.cityRepository
                .AllAsNoTracking();

            if (!String.IsNullOrWhiteSpace(inputModel.SearchQuery))
            {
                allCitiesQuery = allCitiesQuery
                    .Where(m => m.Name.ToLower().Contains(inputModel.SearchQuery.ToLower()));
            }

            if (inputModel.CurrentPage.HasValue &&
                inputModel.EntitiesPerPage.HasValue)
            {
                allCitiesQuery = allCitiesQuery
                    .Skip(inputModel.EntitiesPerPage.Value * (inputModel.CurrentPage.Value - 1))
                    .Take(inputModel.EntitiesPerPage.Value);
            }

            IEnumerable<CityDataViewModel> model = await allCitiesQuery
               .Select(c => new CityDataViewModel()
               {
                   Id = c.Id.ToString(),
                   Name = c.Name,
                   State = c.State,
                   ZipCode = c.ZipCode,
               })
               .ToArrayAsync();

            return model;
        }

        public async Task<IEnumerable<StreetDataViewModel>> GetAllStreetsInCityAsync(Guid cityId)
        {
            IEnumerable<StreetDataViewModel> model = await streetRepository.AllAsNoTracking()
                .Where(s => s.StreetsCities.Any(sc => sc.CityId == cityId))
                .Select(s => new StreetDataViewModel()
                {
                    Id = s.Id.ToString(),
                    Name = s.Name,
                })
                .ToArrayAsync();

            return model;
        }

        public async Task<CityDetailsViewModel> GetCityDetailsAsync(Guid id)
        {
            CityDetailsViewModel viewModel = null;

            City? city =
               await cityRepository.AllAsNoTracking()
               .Include(c => c.CityStreets)
               .ThenInclude(cs => cs.Street)
               .FirstOrDefaultAsync(c => c.Id == id);

            if (city != null)
            {
                viewModel = new CityDetailsViewModel()
                {
                    Id = city.Id.ToString(),
                    Name = city.Name,
                    State = city.State,
                    ZipCode = city.ZipCode,
                    Streets = city.CityStreets
                    .Where(cs => cs.CityId == id).Select(cs => new StreetDataViewModel()
                    {
                        Id = cs.StreetId.ToString(),
                        Name = cs.Street.Name
                    })
                   .ToArray()
                };
            }

            return viewModel;
        }

        public async Task<CityDetailsViewModel> GetCityDetailsByNameAsync(string name)
        {
            CityDetailsViewModel viewModel = null;

            City? city =
               await cityRepository.AllAsNoTracking()
               .Include(c => c.CityStreets)
               .ThenInclude(cs => cs.Street)
               .FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());

            if (city != null)
            {
                viewModel = new CityDetailsViewModel()
                {
                    Id = city.Id.ToString(),
                    Name = city.Name,
                    State = city.State,
                    ZipCode = city.ZipCode,
                    Streets = city.CityStreets
                    .Where(cs => cs.City.Name.ToLower() == name.ToLower()).Select(cs => new StreetDataViewModel()
                    {
                        Id = cs.StreetId.ToString(),
                        Name = cs.Street.Name
                    })
                   .ToArray()
                };
            }

            return viewModel;
        }

        public async Task<EditCityInputModel> GetCityForEditAsync(Guid id)
        {
            EditCityInputModel editModel = await cityRepository.AllAsNoTracking()
                .Select(c => new EditCityInputModel()
                {
                    Id = c.Id.ToString(),
                    Name = c.Name,
                    State = c.State,
                    ZipCode = c.ZipCode,
                    CityStreets = c.CityStreets,
                    FromCityRoutes = c.FromCityRoutes,
                    ToCityRoutes = c.ToCityRoutes,
                })
                .FirstOrDefaultAsync(s => s.Id.ToLower() == id.ToString().ToLower());

            return editModel;
        }

        public async Task<int> GetCitiesCountByFilterAsync(AllCitiesSearchFilterViewModel inputModel)
        {
            AllCitiesSearchFilterViewModel inputModelCopy = new AllCitiesSearchFilterViewModel()
            {
                CurrentPage = null,
                EntitiesPerPage = null,
                SearchQuery = inputModel.SearchQuery,
            };

            int citiesCount = (await this.GetAllCitiesAsync(inputModelCopy))
                .Count();
            return citiesCount;
        }

        public async Task<IEnumerable<City>> GetAllCitiesForDropDownsAsync()
        {
            IEnumerable<City> model = await cityRepository
                .AllAsNoTracking().ToArrayAsync();

            return model;
        }

    }
}
