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
        Task<IEnumerable<StreetDataViewModel>> GetAllStreetsAsync(AllStreetsSearchFilterViewModel inputModel);

        Task CreateAsync(StreetInputModel inputModel);

        Task<EditStreetInputModel> GetStreetForEditAsync(Guid id);

        Task<bool> EditStreetAsync(EditStreetInputModel inputModel);

        Task<bool> DeleteStreetAsync(Guid id);
        Task<int> GetStreetsCountByFilterAsync(AllStreetsSearchFilterViewModel inputModel);
    }
}
