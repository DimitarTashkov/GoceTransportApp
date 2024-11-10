using GoceTransportApp.Web.ViewModels.Cities;
using GoceTransportApp.Web.ViewModels.Streets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoceTransportApp.Services.Data.Cities
{
    public class CityService : ICityService
    {
        public Task<bool> AddStreetToCity(Guid cityId, Guid streetId)
        {
            throw new NotImplementedException();
        }

        public Task CreateAsync(CitiesInputModel inputModel)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteCityAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> EditCityAsync(EditCitiesInputModel inputModel)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CitiesDataViewModel>> GetAllCities()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<StreetsDataViewModel>> GetAllStreetsInCity(Guid cityId)
        {
            throw new NotImplementedException();
        }

        public Task<CitiesDataViewModel> GetCityDetails(Guid? id)
        {
            throw new NotImplementedException();
        }

        public Task<CitiesDataViewModel> GetCityDetailsByName(string name)
        {
            throw new NotImplementedException();
        }

        public Task<EditCitiesInputModel> GetCityForEdit(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveStreetFromCity(Guid cityId, Guid streetId)
        {
            throw new NotImplementedException();
        }
    }
}
