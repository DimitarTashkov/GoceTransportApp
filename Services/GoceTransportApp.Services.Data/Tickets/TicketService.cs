using GoceTransportApp.Data.Common.Repositories;
using GoceTransportApp.Data.Models;
using GoceTransportApp.Services.Data.Base;
using GoceTransportApp.Web.ViewModels.Schedules;
using GoceTransportApp.Web.ViewModels.Tickets;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoceTransportApp.Services.Data.Tickets
{
    public class TicketService : BaseService, ITicketService
    {
        private readonly IDeletableEntityRepository<Ticket> ticketRepository;
        private readonly IDeletableEntityRepository<UserTicket> userTicketRepository;

        public TicketService(IDeletableEntityRepository<Ticket> ticketRepository, IDeletableEntityRepository<UserTicket> userTicketRepository)
        {
            this.ticketRepository = ticketRepository;
            this.userTicketRepository = userTicketRepository;
        }

        public async Task CreateAsync(TicketInputModel inputModel)
        {
            if (!DateTime.TryParse(inputModel.IssuedDate, out var issuedDate))
            {
                throw new ArgumentException("Invalid departure date and time format.");
            }

            if (!DateTime.TryParse(inputModel.ExpiryDate, out var expiryDate))
            {
                throw new ArgumentException("Invalid arrival date and time format.");
            }

            Ticket ticket = new Ticket()
            {
                IssuedDate = issuedDate,
                ExpiryDate = expiryDate,
                Price = inputModel.Price,
                OrganizationId = Guid.Parse(inputModel.OrganizationId),
                RouteId = Guid.Parse(inputModel.RouteId),
                ScheduleId = Guid.Parse(inputModel.ScheduleId),
                CreatedOn = DateTime.UtcNow,
            };

            await ticketRepository.AddAsync(ticket);
            await ticketRepository.SaveChangesAsync();
        }

        public async Task<bool> EditTicketAsync(EditTicketInputModel inputModel)
        {
            var ticket = await ticketRepository.GetByIdAsync(Guid.Parse(inputModel.Id));

            if (ticket == null)
            {
                return false;
            }

            if (!DateTime.TryParse(inputModel.IssuedDate, out var issuedDate))
            {
                throw new ArgumentException("Invalid departure date and time format.");
            }

            if (!DateTime.TryParse(inputModel.ExpiryDate, out var expiryDate))
            {
                throw new ArgumentException("Invalid arrival date and time format.");
            }

            ticket.IssuedDate = issuedDate;
            ticket.ExpiryDate = expiryDate;
            ticket.Price = inputModel.Price;
            ticket.RouteId = Guid.Parse(inputModel.RouteId);
            ticket.ScheduleId = Guid.Parse(inputModel.ScheduleId);
            ticket.TicketsUsers = inputModel.TicketsUsers;
            ticket.OrganizationId = Guid.Parse(inputModel.OrganizationId);
            ticket.ModifiedOn = DateTime.UtcNow;

            bool result = await ticketRepository.UpdateAsync(ticket);

            return result;
        }

        public async Task<IEnumerable<TicketDataViewModel>> GetAllTicketsAsync()
        {
            IEnumerable<TicketDataViewModel> model = await ticketRepository.AllAsNoTracking()
                .Include(r => r.TimeTable)
              .Select(c => new TicketDataViewModel()
              {
                  Id = c.Id.ToString(),
                  ArrivingTime = c.TimeTable.Arrival.ToString(),
                  DepartingTime = c.TimeTable.Departure.ToString(),
                  Price = c.Price.ToString(),
                  FromCity = c.Route.FromCity.Name,
                  ToCity = c.Route.ToCity.Name,
                  OrganizationId = c.OrganizationId.ToString(),
              })
              .ToArrayAsync();

            return model;
        }

        public async Task<RemoveTicketViewModel> GetTicketForDeletionAsync(Guid id)
        {
            RemoveTicketViewModel deleteModel = await ticketRepository.AllAsNoTracking()
                .Include(r => r.Route)
                .ThenInclude(route => route.FromCity)
                .Include(r => r.Route)
                .ThenInclude(route => route.ToCity)
                .Select(ticket => new RemoveTicketViewModel()
                {
                    Id = ticket.Id.ToString(),
                    IssuedDate = ticket.IssuedDate.ToString(),
                    ExpiryDate = ticket.ExpiryDate.ToString(),
                    OrganizationId = ticket.OrganizationId.ToString()
                })
                .FirstOrDefaultAsync(s => s.Id.ToLower() == id.ToString().ToLower());

            return deleteModel;
        }

        public async Task<EditTicketInputModel> GetTicketForEditAsync(Guid id)
        {
            EditTicketInputModel editModel = await ticketRepository.AllAsNoTracking()
               .Select(ticket => new EditTicketInputModel()
               {
                   Id = ticket.Id.ToString(),
                   IssuedDate = ticket.IssuedDate.ToString(),
                   ExpiryDate = ticket.IssuedDate.ToString(),
                   Price = ticket.Price,
                   OrganizationId = ticket.OrganizationId.ToString(),
                   RouteId = ticket.RouteId.ToString(),
                   ScheduleId = ticket.ScheduleId.ToString(),
                   TicketsUsers = ticket.TicketsUsers,
               })
               .FirstOrDefaultAsync(s => s.Id.ToLower() == id.ToString().ToLower());

            return editModel;
        }

        public async Task<bool> RemoveTicketAsync(RemoveTicketViewModel inputModel)
        {
            Guid ticketGuid = Guid.Empty;
            bool isTicketGuidValid = this.IsGuidValid(inputModel.Id, ref ticketGuid);

            if (!isTicketGuidValid)
            {
                return false;
            }

            Ticket ticket = await ticketRepository
                .FirstOrDefaultAsync(s => s.Id == ticketGuid);

            if (ticket == null)
            {
                return false;
            }

            await ticketRepository.DeleteAsync(ticket);

            return true;
        }

        public async Task<TicketDetailsViewModel> GetTicketDetailsAsync(Guid id)
        {
            TicketDetailsViewModel viewModel = null;

            Ticket? ticket = await ticketRepository.AllAsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);

            UserTicket userTicket = await userTicketRepository.FirstOrDefaultAsync(x => x.TicketId == ticket.Id);

            if (ticket != null)
            {
                viewModel.Id = ticket.Id.ToString();
                viewModel.IssuedDate = ticket.IssuedDate.ToString();
                viewModel.ExpiryDate = ticket.ExpiryDate.ToString();
                viewModel.ArrivingTime = ticket.TimeTable.Arrival.ToString();
                viewModel.DepartingTime = ticket.TimeTable.Departure.ToString();
                viewModel.FromCity = ticket.Route.FromCity.Name;
                viewModel.ToCity = ticket.Route.ToCity.Name;
                viewModel.FromStreet = ticket.Route.FromStreet.Name;
                viewModel.ToStreet = ticket.Route.ToStreet.Name;
                viewModel.OrganizationId = ticket.OrganizationId.ToString();
                viewModel.OrganizationName = ticket.Organization.Name;
                viewModel.AvailableTickets = userTicket.AvailableTickets;
            }

            return viewModel;
        }

        public async Task<bool> BuyTicketsAsync(Guid ticketId, int quantity)
        {
            Ticket ticket = await ticketRepository.FirstOrDefaultAsync(t => t.Id == ticketId);
            if (ticket == null)
            {
                return false;
            }

            UserTicket userTicket = await userTicketRepository
                .FirstOrDefaultAsync(ut => ut.TicketId == ticket.Id);

            if (userTicket == null || userTicket.AvailableTickets < quantity)
            {
                return false;
            }

            userTicket.AvailableTickets -= quantity;
            await userTicketRepository.SaveChangesAsync();

            return true;
        }
    }
}
