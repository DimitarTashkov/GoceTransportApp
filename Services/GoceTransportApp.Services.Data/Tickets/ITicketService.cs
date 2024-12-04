using GoceTransportApp.Web.ViewModels.Schedules;
using GoceTransportApp.Web.ViewModels.Tickets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoceTransportApp.Services.Data.Tickets
{
    public interface ITicketService
    {
        Task<IEnumerable<TicketDataViewModel>> GetAllTickets();

        Task CreateAsync(TicketInputModel inputModel);

        Task<EditTicketInputModel> GetTicketForEdit(Guid id);

        Task<bool> EditTicketAsync(EditTicketInputModel inputModel);

        Task<RemoveTicketViewModel> GetTicketForDeletion(Guid id);

        Task<bool> RemoveTicketAsync(RemoveTicketViewModel inputModel);

        Task<TicketDetailsViewModel> TicketDetails(Guid id);
    }
}
