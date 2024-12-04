using GoceTransportApp.Web.ViewModels.Schedules;
using GoceTransportApp.Web.ViewModels.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoceTransportApp.Services.Data.Schedules
{
    public interface IScheduleService
    {
        Task<IEnumerable<ScheduleDataViewModel>> GetAllSchedulesAsync();

        Task CreateAsync(ScheduleInputModel inputModel);

        Task<EditScheduleInputModel> GetScheduleForEditAsync(Guid id);

        Task<bool> EditScheduleAsync(EditScheduleInputModel inputModel);

        Task<RemoveScheduleViewModel> GetScheduleForDeletionAsync(Guid id);

        Task<bool> RemoveScheduleAsync(RemoveScheduleViewModel inputModel);

        Task<ScheduleDetailsViewModel> GetScheduleDetailsAsync(Guid id);
    }
}
