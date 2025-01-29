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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

        public async Task<IEnumerable<TicketDataViewModel>> GetAllTicketsAsync(AllTicketsSearchFilterViewModel inputModel)
        {
            IQueryable<Ticket> query = ticketRepository
                .AllAsNoTracking()
                .Include(r => r.Route)
                .ThenInclude(r => r.FromCity)
                .Include(r => r.Route)
                .ThenInclude(r => r.ToCity)
                .Include(t => t.TimeTable)
                ;

            if (!string.IsNullOrEmpty(inputModel.SearchQuery))
            {
                query = query.Where(t => t.Route.FromCity.Name.Contains(inputModel.SearchQuery) ||
                                         t.Route.ToCity.Name.Contains(inputModel.SearchQuery));
            }

            if (inputModel.PriceFrom.HasValue)
            {
                query = query.Where(t => t.Price >= inputModel.PriceFrom.Value);
            }

            if (inputModel.PriceTo.HasValue)
            {
                query = query.Where(t => t.Price <= inputModel.PriceTo.Value);
            }

            query = query
                    .Skip(inputModel.EntitiesPerPage.Value * (inputModel.CurrentPage.Value - 1))
                    .Take(inputModel.EntitiesPerPage.Value);

            IEnumerable<TicketDataViewModel> model = await query
              .Select(t => new TicketDataViewModel()
              {
                  Id = t.Id.ToString(),
                  IssuedDate = t.IssuedDate.ToString("yyyy-MM-dd"),
                  ExpiryDate = t.ExpiryDate.ToString("yyyy-MM-dd"),
                  Price = t.Price.ToString(),
                  FromCity = t.Route.FromCity.Name,
                  ToCity = t.Route.ToCity.Name,
                  OrganizationId = t.OrganizationId.ToString()
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
            .Include(t => t.TimeTable)
            .Include(t => t.Route)
                .ThenInclude(r => r.FromCity)
            .Include(t => t.Route)
                .ThenInclude(r => r.ToCity)
            .Include(t => t.Route)
                .ThenInclude(r => r.FromStreet)
            .Include(t => t.Route)
                .ThenInclude(r => r.ToStreet)
            .Include(t => t.Organization)
            .FirstOrDefaultAsync(d => d.Id == id);

            if (ticket != null)
            {
                viewModel = new TicketDetailsViewModel()
                {
                    Id = ticket.Id.ToString(),
                    IssuedDate = ticket.IssuedDate.ToString(),
                    ExpiryDate = ticket.ExpiryDate.ToString(),
                    Price = ticket.Price.ToString(),
                    Day = ticket.TimeTable.Day.ToString(),
                    ArrivingTime = ticket.TimeTable.Arrival.TimeOfDay.ToString(),
                    DepartingTime = ticket.TimeTable.Departure.TimeOfDay.ToString(),
                    FromCity = ticket.Route.FromCity.Name,
                    ToCity = ticket.Route.ToCity.Name,
                    FromStreet = ticket.Route.FromStreet.Name,
                    ToStreet = ticket.Route.ToStreet.Name,
                    OrganizationId = ticket.OrganizationId.ToString(),
                    OrganizationName = ticket.Organization.Name,
                };
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

        public async Task<int> GetTicketsCountByFilterAsync(AllTicketsSearchFilterViewModel inputModel)
        {
            IQueryable<Ticket> query = ticketRepository
               .AllAsNoTracking()
               .Include(r => r.Route)
               .ThenInclude(r => r.FromCity)
               .Include(r => r.Route)
               .ThenInclude(r => r.ToCity)
               .Include(t => t.TimeTable)
               ;

            if (!string.IsNullOrEmpty(inputModel.SearchQuery))
            {
                query = query.Where(t => t.Route.FromCity.Name.Contains(inputModel.SearchQuery) ||
                                         t.Route.ToCity.Name.Contains(inputModel.SearchQuery));
            }

            if (inputModel.PriceFrom.HasValue)
            {
                query = query.Where(t => t.Price >= inputModel.PriceFrom.Value);
            }

            if (inputModel.PriceTo.HasValue)
            {
                query = query.Where(t => t.Price <= inputModel.PriceTo.Value);
            }

            return await query.CountAsync();
        }
    }
}
