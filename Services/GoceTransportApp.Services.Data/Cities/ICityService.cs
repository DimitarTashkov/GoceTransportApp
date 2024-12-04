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
        Task<IEnumerable<CityDataViewModel>> GetAllCities();

        Task<CityDetailsViewModel> GetCityDetails(Guid id);

        Task<CityDetailsViewModel> GetCityDetailsByName(string name);

        Task<IEnumerable<StreetDataViewModel>> GetAllStreetsInCity(Guid cityId);

        Task<CityAddStreetInputModel?> GetAddStreetToCityModel(Guid id);

        Task<bool> AddStreetToCity(Guid cityId, CityAddStreetInputModel model);

        Task CreateAsync(CityInputModel inputModel);

        Task<EditCityInputModel> GetCityForEdit(Guid id);

        Task<bool> EditCityAsync(EditCityInputModel inputModel);

        Task<bool> DeleteCityAsync(Guid id);
    }
}
