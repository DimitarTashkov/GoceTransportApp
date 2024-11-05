using GoceTransportApp.Web.ViewModels.Streets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoceTransportApp.Services.Data.Streets
{
    public interface IStreetService
    {
        Task<IEnumerable<StreetsDataViewModel>> GetAllStreetsInCity(Guid cityId);
        Task<IEnumerable<StreetsDataViewModel>> GetAllStreetFromRoutes(Guid streetId);
        Task<IEnumerable<StreetsDataViewModel>> GetAllStreetToRoutes(Guid streetId);
        Task CreateAsync(StreetsInputModel inputModel);
        Task<StreetsInputModel> GetStreetForEdit(Guid id);
        Task<bool> EditStreetAsync(StreetsInputModel inputModel);
        Task DeleteStreetAsync(Guid id);

    }
}
