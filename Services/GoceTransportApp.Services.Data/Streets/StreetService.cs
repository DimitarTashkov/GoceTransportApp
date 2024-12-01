using GoceTransportApp.Data.Common.Repositories;
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

        public async Task<IEnumerable<StreetDataViewModel>> GetAllStreets()
        {
            IEnumerable<StreetDataViewModel> model = await streetRepository.AllAsNoTracking()
                .Select(s => new StreetDataViewModel()
                {
                    Id = s.Id.ToString(),
                    Name = s.Name
                })
                .ToArrayAsync();

            return model;
        }

        public async Task<EditStreetInputModel> GetStreetForEdit(Guid id)
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
    }
}
