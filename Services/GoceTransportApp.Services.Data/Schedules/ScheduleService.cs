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

            if (!DateTime.TryParse(inputModel.Departure, out var departureTime))
            {
                throw new ArgumentException("Invalid departure date and time format.");
            }

            if (!DateTime.TryParse(inputModel.Arrival, out var arrivalTime))
            {
                throw new ArgumentException("Invalid arrival date and time format.");
            }

            Schedule schedule = new Schedule()
            {
                Day = dayOfWeek,
                Departure = departureTime,
                Arrival = arrivalTime,
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

            if (!DateTime.TryParse(inputModel.Departure, out var departureTime))
            {
                throw new ArgumentException("Invalid departure date and time format.");
            }

            if (!DateTime.TryParse(inputModel.Arrival, out var arrivalTime))
            {
                throw new ArgumentException("Invalid arrival date and time format.");
            }

            schedule.Day = dayOfWeek;
            schedule.Departure = departureTime;
            schedule.Arrival = arrivalTime;
            schedule.RouteId = Guid.Parse(inputModel.RouteId);
            schedule.VehicleId = Guid.Parse(inputModel.VehicleId);
            schedule.ScheduleTickets = inputModel.ScheduleTickets;
            schedule.OrganizationId = Guid.Parse(inputModel.OrganizationId);
            schedule.ModifiedOn = DateTime.UtcNow;

            bool result = await scheduleRepository.UpdateAsync(schedule);

            return result;
        }

        public async Task<IEnumerable<ScheduleDataViewModel>> GetAllSchedules()
        {
            IEnumerable<ScheduleDataViewModel> model = await scheduleRepository.AllAsNoTracking()
                .Include(r => r.Route)
                .ThenInclude(route => route.FromCity)
                .Include(r => r.Route)
                .ThenInclude(route => route.ToCity)
              .Select(c => new ScheduleDataViewModel()
              {
                  Id = c.Id.ToString(),
                  Day = c.Day.ToString(),
                  Departing = c.Departure.ToString(),
                  Arriving = c.Arrival.ToString(),
                  FromCity = c.Route.FromCity.Name,
                  ToCity = c.Route.ToCity.Name,
                  OrganizationId = c.OrganizationId.ToString(),
              })
              .ToArrayAsync();

            return model;
        }

        public async Task<RemoveScheduleViewModel> GetScheduleForDeletion(Guid id)
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
                    Departing = schedule.Departure.ToString(),
                    Arriving = schedule.Arrival.ToString(),
                    FromCity = schedule.Route.FromCity.Name,
                    ToCity = schedule.Route.ToCity.Name,
                    OrganizationId = schedule.OrganizationId.ToString(),
                })
                .FirstOrDefaultAsync(s => s.Id.ToLower() == id.ToString().ToLower());

            return deleteModel;
        }

        public async Task<EditScheduleInputModel> GetScheduleForEdit(Guid id)
        {
            EditScheduleInputModel editModel = await scheduleRepository.AllAsNoTracking()
              .Select(schedule => new EditScheduleInputModel()
              {
                  Id = schedule.Id.ToString(),
                  Day = schedule.Day.ToString(),
                  Departure = schedule.Departure.ToString(),
                  Arrival = schedule.Arrival.ToString(),
                  OrganizationId = schedule.OrganizationId.ToString(),
                  RouteId = schedule.RouteId.ToString(),
                  VehicleId = schedule.VehicleId.ToString(),
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

        public async Task<ScheduleDetailsViewModel> ScheduleDetails(Guid id)
        {
            ScheduleDetailsViewModel viewModel = null;

            Schedule? schedule = await scheduleRepository.AllAsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);
            if (schedule != null)
            {
                viewModel.Id = schedule.Id.ToString();
                viewModel.Day = schedule.Day.ToString();
                viewModel.Departing = schedule.Departure.ToString();
                viewModel.Arriving = schedule.Arrival.ToString();
                viewModel.VehicleNumber = schedule.Vehicle.Number;
                viewModel.FromCity = schedule.Route.FromCity.Name;
                viewModel.ToCity = schedule.Route.ToCity.Name;
                viewModel.FromStreet = schedule.Route.FromStreet.Name;
                viewModel.ToStreet = schedule.Route.ToStreet.Name;
                viewModel.OrganizationId = schedule.OrganizationId.ToString();
                viewModel.OrganizationName = schedule.Organization.Name;
            }

            return viewModel;
        }
    }
}
