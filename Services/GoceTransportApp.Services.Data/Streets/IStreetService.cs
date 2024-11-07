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

        Task<IEnumerable<StreetsDataViewModel>> GetAllStreetFromRoutes(Guid routeId);

        Task<IEnumerable<StreetsDataViewModel>> GetAllStreetToRoutes(Guid routeId);

        Task CreateAsync(StreetsInputModel inputModel);

        Task<EditStreetInputModel> GetStreetForEdit(Guid id);

        Task<bool> EditStreetAsync(EditStreetInputModel inputModel);

        Task<bool> DeleteStreetAsync(Guid id);
    }
}
