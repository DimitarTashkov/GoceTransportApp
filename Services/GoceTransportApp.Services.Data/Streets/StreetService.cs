﻿using GoceTransportApp.Data.Common.Repositories;
using GoceTransportApp.Data.Models;
using GoceTransportApp.Web.ViewModels.Streets;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoceTransportApp.Services.Data.Streets
{
    public class StreetService : IStreetService
    {
        // TODO: Add all collections so they do not disappear

        private readonly IRepository<Street> streetRepository;

        public StreetService(IRepository<Street> streetRepository)
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

            await streetRepository.DeleteAsync(street);
            return true;
        }

        public async Task<bool> EditStreetAsync(EditStreetInputModel model)
        {
            Street street = new Street()
            {
                Name = model.Street,
                StreetsCities = model.StreetsCities,
                FromStreetRoutes = model.FromStreetRoutes,
                ToStreetRoutes = model.ToStreetRoutes,
                ModifiedOn = DateTime.UtcNow
            };

            bool result = await streetRepository.UpdateAsync(street);

            return result;
        }

        public async Task<IEnumerable<StreetDataViewModel>> GetAllStreets()
        {
            IEnumerable<StreetDataViewModel> model = await streetRepository.GetAllAttached()
                .Select(s => new StreetDataViewModel()
                {
                    Id = s.Id.ToString(),
                    Name = s.Name
                })
                .AsNoTracking()
                .ToArrayAsync();

            return model;
        }

        public async Task<EditStreetInputModel> GetStreetForEdit(Guid id)
        {
            EditStreetInputModel editModel = await streetRepository.GetAllAttached()
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
    }
}
