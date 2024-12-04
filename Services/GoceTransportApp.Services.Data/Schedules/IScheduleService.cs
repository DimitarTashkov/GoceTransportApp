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
        Task<IEnumerable<ScheduleDataViewModel>> GetAllSchedules();

        Task CreateAsync(ScheduleInputModel inputModel);

        Task<EditScheduleInputModel> GetScheduleForEdit(Guid id);

        Task<bool> EditScheduleAsync(EditScheduleInputModel inputModel);

        Task<RemoveScheduleViewModel> GetScheduleForDeletion(Guid id);

        Task<bool> RemoveScheduleAsync(RemoveScheduleViewModel inputModel);

        Task<ScheduleDetailsViewModel> ScheduleDetails(Guid id);
    }
}
