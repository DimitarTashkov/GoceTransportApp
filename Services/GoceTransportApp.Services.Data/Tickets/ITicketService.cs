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
        Task<IEnumerable<TicketDataViewModel>> GetAllTicketsAsync(AllTicketsSearchFilterViewModel inputModel);

        Task<bool> CreateAsync(TicketInputModel inputModel);

        Task<EditTicketInputModel> GetTicketForEditAsync(Guid id);

        Task<bool> EditTicketAsync(EditTicketInputModel inputModel);

        Task<RemoveTicketViewModel> GetTicketForDeletionAsync(Guid id);

        Task<bool> RemoveTicketAsync(RemoveTicketViewModel inputModel);

        Task<TicketDetailsViewModel> GetTicketDetailsAsync(Guid id);
        Task<bool> BuyTicketsAsync(Guid ticketId, int quantity);
        Task<bool> PurchaseTicketAsync(string userId, Guid ticketId);
        Task<int> GetTicketsCountByFilterAsync(AllTicketsSearchFilterViewModel inputModel);
        Task<MyTicketsViewModel> GetMyTicketsAsync(string userId);
        Task<bool> CancelTicketAsync(string userId, Guid ticketId);
        Task<IEnumerable<PassengerViewModel>> GetPassengersForScheduleAsync(Guid scheduleId);
        Task<bool> BoardPassengerAsync(string customerId, Guid ticketId);
        Task<DateTime?> GetTicketDepartureDateTimeAsync(Guid ticketId);
    }
}
