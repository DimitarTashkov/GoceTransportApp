using GoceTransportApp.Web.ViewModels.Cities;
using GoceTransportApp.Web.ViewModels.Streets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoceTransportApp.Services.Data.Cities
{
    public interface ICityService
    {
        Task<IEnumerable<CitiesDataViewModel>> GetAllCities();

        Task<CitiesDataViewModel> GetCityDetails(Guid? id);

        Task<CitiesDataViewModel> GetCityDetailsByName(string? name);

        Task<IEnumerable<StreetsDataViewModel>> GetAllStreetsInCity(Guid cityId);

        Task<bool> AddStreetToCity(Guid cityId, Guid streetId);

        Task<bool> RemoveStreetFromCity(Guid cityId, Guid streetId);

        Task CreateAsync(CitiesInputModel inputModel);

        Task<EditCitiesInputModel> GetCityForEdit(Guid id);

        Task<bool> EditCityAsync(EditCitiesInputModel inputModel);

        Task<bool> DeleteCityAsync(Guid id);
    }
}
