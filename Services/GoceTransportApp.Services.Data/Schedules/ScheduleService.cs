using GoceTransportApp.Data.Common.Repositories;
using GoceTransportApp.Data.Models;
using GoceTransportApp.Data.Models.Enumerations;
using GoceTransportApp.Services.Data.Base;
using GoceTransportApp.Web.ViewModels.Schedules;
using GoceTransportApp.Web.ViewModels.Vehicles;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GoceTransportApp.Services.Data.Schedules
{
    public class ScheduleService : BaseService, IScheduleService
    {
        private readonly IDeletableEntityRepository<Schedule> scheduleRepository;

        public ScheduleService(IDeletableEntityRepository<Schedule> scheduleRepository)
        {
            this.scheduleRepository = scheduleRepository;
        }

        public async Task CreateAsync(ScheduleInputModel inputModel)
        {
            if (!Enum.TryParse<DayOfWeek>(inputModel.Day, true, out var dayOfWeek))
            {
                throw new ArgumentException("Invalid day format. Please provide a valid day of the week.");
            }

            Schedule schedule = new Schedule()
            {
                Day = dayOfWeek,
                Departure = inputModel.Departure,
                Arrival = inputModel.Arrival,
                OrganizationId = Guid.Parse(inputModel.OrganizationId),
                RouteId = Guid.Parse(inputModel.RouteId),
                VehicleId = Guid.Parse(inputModel.VehicleId),
                CreatedOn = DateTime.UtcNow,
            };

            await scheduleRepository.AddAsync(schedule);
            await scheduleRepository.SaveChangesAsync();
        }

        public async Task<bool> EditScheduleAsync(EditScheduleInputModel inputModel)
        {
            var schedule = await scheduleRepository.GetByIdAsync(Guid.Parse(inputModel.Id));

            if (schedule == null)
            {
                return false;
            }

            if (!Enum.TryParse<DayOfWeek>(inputModel.Day, true, out var dayOfWeek))
            {
                throw new ArgumentException("Invalid day format. Please provide a valid day of the week.");
            }

            schedule.Day = dayOfWeek;
            schedule.Departure = inputModel.Departure;
            schedule.Arrival = inputModel.Arrival;
            schedule.RouteId = Guid.Parse(inputModel.RouteId);
            schedule.VehicleId = Guid.Parse(inputModel.VehicleId);
            schedule.ScheduleTickets = inputModel.ScheduleTickets;
            schedule.OrganizationId = Guid.Parse(inputModel.OrganizationId);
            schedule.LiveStatus = inputModel.LiveStatus;
            schedule.ModifiedOn = DateTime.UtcNow;

            bool result = await scheduleRepository.UpdateAsync(schedule);

            return result;
        }

        private IQueryable<Schedule> BuildFilteredQuery(AllSchedulesSearchFilterViewModel inputModel)
        {
            IQueryable<Schedule> query = scheduleRepository.AllAsNoTracking()
                .Include(r => r.Route).ThenInclude(route => route.FromCity)
                .Include(r => r.Route).ThenInclude(route => route.ToCity);

            // Security filter: limit to user's own organisations (set server-side)
            if (inputModel.OrganizationFilter != null && inputModel.OrganizationFilter.Count > 0)
            {
                query = query.Where(s => inputModel.OrganizationFilter.Contains(s.OrganizationId));
            }

            // User-selected single organisation
            if (!string.IsNullOrEmpty(inputModel.OrganizationId) &&
                Guid.TryParse(inputModel.OrganizationId, out Guid orgGuid))
            {
                query = query.Where(s => s.OrganizationId == orgGuid);
            }

            // City filters
            if (!string.IsNullOrEmpty(inputModel.FromCityId) &&
                Guid.TryParse(inputModel.FromCityId, out Guid fromCityGuid))
            {
                query = query.Where(s => s.Route.FromCity.Id == fromCityGuid);
            }

            if (!string.IsNullOrEmpty(inputModel.ToCityId) &&
                Guid.TryParse(inputModel.ToCityId, out Guid toCityGuid))
            {
                query = query.Where(s => s.Route.ToCity.Id == toCityGuid);
            }

            if (inputModel.DayFilter.HasValue)
            {
                query = query.Where(s => s.Day == inputModel.DayFilter.Value);
            }

            if (inputModel.TimeFilter.HasValue)
            {
                query = query.Where(s =>
                    s.Departure.TimeOfDay == inputModel.TimeFilter.Value ||
                    s.Arrival.TimeOfDay == inputModel.TimeFilter.Value);
            }

            query = inputModel.SortBy switch
            {
                ScheduleSorting.DepartureAscending  => query.OrderBy(s => s.Departure.TimeOfDay),
                ScheduleSorting.DepartureDescending => query.OrderByDescending(s => s.Departure.TimeOfDay),
                ScheduleSorting.ArrivalAscending    => query.OrderBy(s => s.Arrival.TimeOfDay),
                ScheduleSorting.ArrivalDescending   => query.OrderByDescending(s => s.Arrival.TimeOfDay),
                _                                   => query.OrderBy(s => s.Day).ThenBy(s => s.Departure.TimeOfDay),
            };

            return query;
        }

        public async Task<IEnumerable<ScheduleDataViewModel>> GetAllSchedulesAsync(AllSchedulesSearchFilterViewModel inputModel)
        {
            IQueryable<Schedule> query = BuildFilteredQuery(inputModel);

            // Count first, then paginate
            query = query
                .Skip(inputModel.EntitiesPerPage.Value * (inputModel.CurrentPage.Value - 1))
                .Take(inputModel.EntitiesPerPage.Value);

            return await query
                .Select(c => new ScheduleDataViewModel()
                {
                    Id = c.Id.ToString(),
                    Day = c.Day.ToString(),
                    Departing = c.Departure.ToString("HH:mm"),
                    Arriving = c.Arrival.ToString("HH:mm"),
                    FromCity = c.Route.FromCity.Name,
                    ToCity = c.Route.ToCity.Name,
                    OrganizationId = c.OrganizationId.ToString(),
                })
                .ToArrayAsync();
        }

        public async Task<RemoveScheduleViewModel> GetScheduleForDeletionAsync(Guid id)
        {
            RemoveScheduleViewModel deleteModel = await scheduleRepository.AllAsNoTracking()
                .Include(r => r.Route)
                .ThenInclude(route => route.FromCity)
                .Include(r => r.Route)
                .ThenInclude(route => route.ToCity)
                .Select(schedule => new RemoveScheduleViewModel()
                {
                    Id = schedule.Id.ToString(),
                    Day = schedule.Day.ToString(),
                    Departing = schedule.Departure.ToString("HH:mm"),
                    Arriving = schedule.Arrival.ToString("HH:mm"),
                    FromCity = schedule.Route.FromCity.Name,
                    ToCity = schedule.Route.ToCity.Name,
                    OrganizationId = schedule.OrganizationId.ToString(),
                })
                .FirstOrDefaultAsync(s => s.Id.ToLower() == id.ToString().ToLower());

            return deleteModel;
        }

        public async Task<EditScheduleInputModel> GetScheduleForEditAsync(Guid id)
        {
            EditScheduleInputModel editModel = await scheduleRepository.AllAsNoTracking()
              .Select(schedule => new EditScheduleInputModel()
              {
                  Id = schedule.Id.ToString(),
                  Day = schedule.Day.ToString(),
                  Departure = schedule.Departure,
                  Arrival = schedule.Arrival,
                  OrganizationId = schedule.OrganizationId.ToString(),
                  RouteId = schedule.RouteId.ToString(),
                  VehicleId = schedule.VehicleId.ToString(),
                  LiveStatus = schedule.LiveStatus,
                  ScheduleTickets = schedule.ScheduleTickets,
              })
              .FirstOrDefaultAsync(s => s.Id.ToLower() == id.ToString().ToLower());

            return editModel;
        }

        public async Task<bool> RemoveScheduleAsync(RemoveScheduleViewModel inputModel)
        {
            Guid scheduleGuid = Guid.Empty;
            bool isScheduleGuidValid = this.IsGuidValid(inputModel.Id, ref scheduleGuid);

            if (!isScheduleGuidValid)
            {
                return false;
            }

            Schedule schedule = await scheduleRepository
                .FirstOrDefaultAsync(s => s.Id == scheduleGuid);

            if (schedule == null)
            {
                return false;
            }

            await scheduleRepository.DeleteAsync(schedule);

            return true;
        }

        public async Task<ScheduleDetailsViewModel> GetScheduleDetailsAsync(Guid id)
        {
            ScheduleDetailsViewModel viewModel = null;

            Schedule? schedule = await scheduleRepository.AllAsNoTracking()
                .Include(s => s.Vehicle)
                .Include(s => s.Route)
                    .ThenInclude(r => r.FromCity)
                .Include(s => s.Route)
                    .ThenInclude(r => r.ToCity)
                .Include(s => s.Route)
                    .ThenInclude(r => r.FromStreet)
                .Include(s => s.Route)
                    .ThenInclude(r => r.ToStreet)
                .Include(s => s.Organization)
                .FirstOrDefaultAsync(d => d.Id == id);
            if (schedule != null)
            {
                viewModel = new ScheduleDetailsViewModel()
                {
                    Id = schedule.Id.ToString(),
                    Day = schedule.Day.ToString(),
                    Departing = schedule.Departure.TimeOfDay.ToString(),
                    Arriving = schedule.Arrival.TimeOfDay.ToString(),
                    VehicleNumber = schedule.Vehicle.Number,
                    FromCity = schedule.Route.FromCity.Name,
                    ToCity = schedule.Route.ToCity.Name,
                    FromStreet = schedule.Route.FromStreet.Name,
                    ToStreet = schedule.Route.ToStreet.Name,
                    OrganizationId = schedule.OrganizationId.ToString(),
                    OrganizationName = schedule.Organization.Name,
                    LiveStatus = schedule.LiveStatus,
                };
            }

            return viewModel;
        }

        public async Task<IEnumerable<SelectListItem>> GetSchedulesForOrganizationAsync(string organizationId)
        {
            return await scheduleRepository.AllAsNoTracking()
                .Include(s => s.Route)
                    .ThenInclude(r => r.FromCity)
                .Include(s => s.Route)
                    .ThenInclude(r => r.FromStreet)
                .Include(s => s.Route)
                    .ThenInclude(r => r.ToCity)
                .Include(s => s.Route)
                    .ThenInclude(r => r.ToStreet)
                .Where(s => s.OrganizationId == Guid.Parse(organizationId))
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = $"{s.Day} | {s.Departure:HH:mm} - {s.Arrival:HH:mm} | Route: {s.Route.FromCity.Name}, {s.Route.FromStreet.Name} ? {s.Route.ToCity.Name}, {s.Route.ToStreet.Name}"
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<ScheduleDataViewModel>> SearchSchedulesAsync(Guid fromCityId, Guid toCityId, DayOfWeek? dayOfWeek)
        {
            IQueryable<Schedule> query = scheduleRepository.AllAsNoTracking()
                .Include(s => s.Route)
                    .ThenInclude(r => r.FromCity)
                .Include(s => s.Route)
                    .ThenInclude(r => r.ToCity)
                .Include(s => s.Organization)
                .Where(s => s.Route.FromCity.Id == fromCityId && s.Route.ToCity.Id == toCityId);

            if (dayOfWeek.HasValue)
            {
                query = query.Where(s => s.Day == dayOfWeek.Value);
            }

            return await query
                .OrderBy(s => s.Departure)
                .Select(s => new ScheduleDataViewModel()
                {
                    Id = s.Id.ToString(),
                    Day = s.Day.ToString(),
                    Departing = s.Departure.ToString("HH:mm"),
                    Arriving = s.Arrival.ToString("HH:mm"),
                    FromCity = s.Route.FromCity.Name,
                    ToCity = s.Route.ToCity.Name,
                    OrganizationId = s.OrganizationId.ToString(),
                    OrganizationName = s.Organization.Name,
                })
                .ToArrayAsync();
        }

        public async Task<int> GetSchedulesCountByFilterAsync(AllSchedulesSearchFilterViewModel inputModel)
        {
            return await BuildFilteredQuery(inputModel).CountAsync();
        }

        public async Task<IEnumerable<NextDepartureViewModel>> GetNextDeparturesAsync(Guid fromCityId, int limit = 5)
        {
            var now = DateTime.UtcNow;
            var tomorrow = now.Date.AddDays(2); // include today + tomorrow

            var upcoming = await scheduleRepository
                .AllAsNoTracking()
                .Include(s => s.Route)
                    .ThenInclude(r => r.FromCity)
                .Include(s => s.Route)
                    .ThenInclude(r => r.ToCity)
                .Include(s => s.Organization)
                .Where(s => s.Route.FromCityId == fromCityId && s.Departure >= now && s.Departure < tomorrow)
                .OrderBy(s => s.Departure)
                .Take(limit)
                .ToListAsync();

            return upcoming.Select(s =>
            {
                var minutesUntil = (int)(s.Departure - now).TotalMinutes;
                string relative;
                if (minutesUntil < 60)
                    relative = $"in {minutesUntil} min";
                else
                {
                    int h = minutesUntil / 60;
                    int m = minutesUntil % 60;
                    relative = m > 0 ? $"in {h}h {m}m" : $"in {h}h";
                }

                return new NextDepartureViewModel
                {
                    ScheduleId = s.Id.ToString(),
                    FromCity = s.Route.FromCity.Name,
                    ToCity = s.Route.ToCity.Name,
                    OrganizationName = s.Organization.Name,
                    OrganizationId = s.OrganizationId.ToString(),
                    Departure = s.Departure,
                    DepartureTime = s.Departure.ToString("HH:mm"),
                    MinutesUntil = relative,
                    IsToday = s.Departure.Date == now.Date,
                };
            }).ToList();
        }
    }
}
