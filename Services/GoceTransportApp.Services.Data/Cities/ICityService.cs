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
        Task<IEnumerable<StreetsDataViewModel>> GetAllCities();

        Task<StreetsDataViewModel> GetCityDetails(Guid? id);

        Task<StreetsDataViewModel> GetCityDetailsByName(string? name);

        Task<IEnumerable<StreetsDataViewModel>> GetAllStreetsInCity(Guid cityId);

        Task<IEnumerable<StreetsDataViewModel>> AddStreetToCity(Guid cityId, Guid streetId);

        Task<IEnumerable<StreetsDataViewModel>> RemoveStreetFromCity(Guid cityId, Guid streetId);

        Task CreateAsync(StreetsInputModel inputModel);

        Task<EditStreetInputModel> GetCityForEdit(Guid id);

        Task<bool> EditCityAsync(EditStreetInputModel inputModel);

        Task<bool> DeleteCityAsync(Guid id);
    }
}
