using GoceTransportApp.Data.Models;
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
        Task<IEnumerable<CityDataViewModel>> GetAllCitiesAsync(AllCitiesSearchFilterViewModel inputModel);

        Task<CityDetailsViewModel> GetCityDetailsAsync(Guid id);

        Task<CityDetailsViewModel> GetCityDetailsByNameAsync(string name);

        Task<IEnumerable<StreetDataViewModel>> GetAllStreetsInCityAsync(Guid cityId);

        Task<CityAddStreetInputModel?> GetAddStreetToCityModelAsync(Guid id);

        Task<bool> AddStreetToCityAsync(Guid cityId, CityAddStreetInputModel model);

        Task CreateAsync(CityInputModel inputModel);

        Task<EditCityInputModel> GetCityForEditAsync(Guid id);

        Task<bool> EditCityAsync(EditCityInputModel inputModel);

        Task<bool> DeleteCityAsync(Guid id);

        Task<int> GetCitiesCountByFilterAsync(AllCitiesSearchFilterViewModel inputModel);

        Task<IEnumerable<City>> GetAllCitiesForDropDownsAsync();
    }
}
