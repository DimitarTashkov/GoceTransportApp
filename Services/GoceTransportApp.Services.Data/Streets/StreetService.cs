﻿using GoceTransportApp.Data.Common.Repositories;
using GoceTransportApp.Data.Models;
using GoceTransportApp.Services.Mapping;
using GoceTransportApp.Web.ViewModels.Streets;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GoceTransportApp.Services.Data.Streets
{
    public class StreetService : IStreetService
    {
        private readonly IDeletableEntityRepository<Street> streetRepository;

        public StreetService(IDeletableEntityRepository<Street> streetRepository)
        {
            this.streetRepository = streetRepository;
        }

        public async Task CreateAsync(StreetInputModel inputModel)
        {
            Street street = new Street()
            {
                Name = inputModel.Street,
                CreatedOn = DateTime.UtcNow
            };

           await streetRepository.AddAsync(street);
           await streetRepository.SaveChangesAsync();
        }

        public async Task<bool> DeleteStreetAsync(Guid id)
        {
            Street street = await streetRepository
                .FirstOrDefaultAsync(s => s.Id == id);

            if (street == null)
            {
                return false;
            }

            bool result = await streetRepository.DeleteAsync(street);
            return result;
        }

        public async Task<bool> EditStreetAsync(EditStreetInputModel model)
        {
            var street = await streetRepository.GetByIdAsync(Guid.Parse(model.Id));
            if (street == null)
            {
                return false;
            }

            street.Name = model.Street;
            street.StreetsCities = model.StreetsCities;
            street.FromStreetRoutes = model.FromStreetRoutes;
            street.ToStreetRoutes = model.ToStreetRoutes;
            street.ModifiedOn = DateTime.UtcNow;

            bool result = await streetRepository.UpdateAsync(street);

            return result;
        }

        public async Task<IEnumerable<StreetDataViewModel>> GetAllStreetsAsync(AllStreetsSearchFilterViewModel inputModel)
        {
            IQueryable<Street> allStreetsQuery = this.streetRepository
                .AllAsNoTracking();

            if (!String.IsNullOrWhiteSpace(inputModel.SearchQuery))
            {
                allStreetsQuery = allStreetsQuery
                    .Where(m => m.Name.ToLower().Contains(inputModel.SearchQuery.ToLower()));
            }

            if (inputModel.CurrentPage.HasValue &&
                inputModel.EntitiesPerPage.HasValue)
            {
                allStreetsQuery = allStreetsQuery
                    .Skip(inputModel.EntitiesPerPage.Value * (inputModel.CurrentPage.Value - 1))
                    .Take(inputModel.EntitiesPerPage.Value);
            }

            IEnumerable<StreetDataViewModel> model = await allStreetsQuery
                .Select(s => new StreetDataViewModel()
                {
                    Id = s.Id.ToString()
                    ,
                    Name = s.Name
                })
                .ToArrayAsync();

            return model;
        }

        public async Task<EditStreetInputModel> GetStreetForEditAsync(Guid id)
        {
            EditStreetInputModel editModel = await streetRepository.AllAsNoTracking()
                .Select(s => new EditStreetInputModel()
                {
                    Id = s.Id.ToString(),
                    Street = s.Name,
                    StreetsCities = s.StreetsCities,
                    FromStreetRoutes = s.FromStreetRoutes,
                    ToStreetRoutes = s.ToStreetRoutes

                })
                .FirstOrDefaultAsync(s => s.Id.ToLower() == id.ToString().ToLower());

            return editModel;
        }

        public async Task<int> GetStreetsCountByFilterAsync(AllStreetsSearchFilterViewModel inputModel)
        {
            AllStreetsSearchFilterViewModel inputModelCopy = new AllStreetsSearchFilterViewModel()
            {
                CurrentPage = null,
                EntitiesPerPage = null,
                SearchQuery = inputModel.SearchQuery,
            };

            int streetsCount = (await this.GetAllStreetsAsync(inputModelCopy))
                .Count();
            return streetsCount;
        }
    }
}
