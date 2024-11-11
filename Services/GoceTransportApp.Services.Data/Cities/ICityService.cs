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

        Task<CitiesDetailsViewModel> GetCityDetails(Guid id);

        Task<CitiesDetailsViewModel> GetCityDetailsByName(string name);

        Task<IEnumerable<StreetsDataViewModel>> GetAllStreetsInCity(Guid cityId);

        Task<CitiesAddStreetInputModel?> GetAddMovieToCinemaInputModelByIdAsync(Guid id);

        Task<bool> AddStreetToCity(Guid streetId, CitiesAddStreetInputModel model);

        Task CreateAsync(CitiesInputModel inputModel);

        Task<EditCitiesInputModel> GetCityForEdit(Guid id);

        Task<bool> EditCityAsync(EditCitiesInputModel inputModel);

        Task<bool> DeleteCityAsync(Guid id);
    }
}
