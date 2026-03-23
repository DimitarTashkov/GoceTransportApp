using GoceTransportApp.Services.Data.Cities;
using GoceTransportApp.Services.Data.Schedules;
using GoceTransportApp.Web.ViewModels.Vehicles;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;

using static GoceTransportApp.Common.ResultMessages.GeneralMessages;
using static GoceTransportApp.Common.GlobalConstants;
using GoceTransportApp.Web.ViewModels.Schedules;
using GoceTransportApp.Data.Common.Repositories;
using GoceTransportApp.Data.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Net.Mail;
using GoceTransportApp.Services.Data.Vehicles;
using GoceTransportApp.Services.Data.Routes;
using GoceTransportApp.Services.Data.Tickets;
using GoceTransportApp.Web.ViewModels.Tickets;
using System.Collections.Generic;
using System.Linq;

namespace GoceTransportApp.Web.Controllers
{
    [Authorize]
    public class ScheduleController : BaseController
    {
        private readonly IScheduleService scheduleService;
        private readonly IVehicleService vehicleService;
        private readonly IRouteService routeService;
        private readonly ICityService cityService;
        private readonly ITicketService ticketService;

        public ScheduleController(
            IScheduleService scheduleService,
            IDeletableEntityRepository<Organization> organizationRepository,
            IVehicleService vehicleService,
            IRouteService routeService,
            ICityService cityService,
            ITicketService ticketService)
            : base(organizationRepository)
        {
            this.scheduleService = scheduleService;
            this.vehicleService = vehicleService;
            this.routeService = routeService;
            this.cityService = cityService;
            this.ticketService = ticketService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index(AllSchedulesSearchFilterViewModel inputModel)
        {
            IEnumerable<ScheduleDataViewModel> allSchedules =
            await scheduleService.GetAllSchedulesAsync(inputModel);

            int allTicketsCount = await scheduleService.GetSchedulesCountByFilterAsync(inputModel);

            AllSchedulesSearchFilterViewModel viewModel = new AllSchedulesSearchFilterViewModel
            {
                Schedules = allSchedules,
                DayFilter = inputModel.DayFilter,
                TimeFilter = inputModel.TimeFilter,
                CurrentPage = inputModel.CurrentPage,
                EntitiesPerPage = inputModel.EntitiesPerPage,
                TotalPages = (int)Math.Ceiling((double)allTicketsCount / inputModel.EntitiesPerPage.Value)
            };

            return this.View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Create(string organizationId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!await this.HasUserCreatedOrganizationAsync(userId, organizationId) && !User.IsInRole(AdministratorRoleName))
            {
                return RedirectToAction("UserOrganizations", "Organization");
            }

            ScheduleInputModel model = new ScheduleInputModel();
            model.Vehicles = await vehicleService.GetVehiclesForOrganizationAsync(organizationId);
            model.Routes = await routeService.GetRoutesForOrganizationAsync(organizationId);
            model.OrganizationId = organizationId;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ScheduleInputModel model)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!await this.HasUserCreatedOrganizationAsync(userId, model.OrganizationId) && !User.IsInRole(AdministratorRoleName))
            {
                return RedirectToAction("Schedules", "Organization", new { organizationId = model.OrganizationId });
            }

            if (!ModelState.IsValid)
            {
                model.Vehicles = await vehicleService.GetVehiclesForOrganizationAsync(model.OrganizationId);
                model.Routes = await routeService.GetRoutesForOrganizationAsync(model.OrganizationId);
                return View(model);
            }

            await scheduleService.CreateAsync(model);
            TempData[nameof(SuccessMessage)] = SuccessMessage;

            return RedirectToAction("Schedules", "Organization", new { organizationId = model.OrganizationId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string? id, string organizationId)
        {

            Guid scheduleGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(id, ref scheduleGuid);

            if (!isIdValid)
            {
                return RedirectToAction("Schedules", "Organization", new { organizationId = organizationId });
            }

            EditScheduleInputModel? formModel = await this.scheduleService
                .GetScheduleForEditAsync(scheduleGuid);

            if (formModel == null)
            {
                TempData[nameof(FailMessage)] = FailMessage;

                return RedirectToAction("Schedules", "Organization", new { organizationId = organizationId });
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!await this.HasUserCreatedOrganizationAsync(userId, formModel.OrganizationId) && !User.IsInRole(AdministratorRoleName))
            {
                return RedirectToAction("Schedules", "Organization", new { organizationId = formModel.OrganizationId });
            }

            formModel.Vehicles = await vehicleService.GetVehiclesForOrganizationAsync(organizationId);
            formModel.Routes = await routeService.GetRoutesForOrganizationAsync(organizationId);

            return this.View(formModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditScheduleInputModel formModel)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!await this.HasUserCreatedOrganizationAsync(userId, formModel.OrganizationId) && !User.IsInRole(AdministratorRoleName))
            {
                return RedirectToAction("Schedules", "Organization", new { organizationId = formModel.OrganizationId });
            }

            if (!ModelState.IsValid)
            {
                formModel.Vehicles = await vehicleService.GetVehiclesForOrganizationAsync(formModel.OrganizationId);
                formModel.Routes = await routeService.GetRoutesForOrganizationAsync(formModel.OrganizationId);
                return this.View(formModel);
            }

            bool isUpdated = await this.scheduleService
                .EditScheduleAsync(formModel);

            if (!isUpdated)
            {
                ModelState.AddModelError(nameof(FailMessage), FailMessage);

                return this.View(formModel);
            }

            TempData[nameof(SuccessMessage)] = SuccessMessage;

            return RedirectToAction("Schedules", "Organization", new { organizationId = formModel.OrganizationId });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string? id, string organizationId)
        {
            Guid scheduleGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(id, ref scheduleGuid);
            if (!isIdValid)
            {
                return RedirectToAction("Schedules", "Organization", new { organizationId = organizationId });
            }

            RemoveScheduleViewModel? model = await scheduleService
                .GetScheduleForDeletionAsync(scheduleGuid);

            if (model == null)
            {
                TempData[nameof(FailMessage)] = FailMessage;

                return RedirectToAction("Schedules", "Organization", new { organizationId = organizationId });
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!await this.HasUserCreatedOrganizationAsync(userId, model.OrganizationId) && !User.IsInRole(AdministratorRoleName))
            {
                return RedirectToAction("Schedules", "Organization", new { organizationId = model.OrganizationId });
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(RemoveScheduleViewModel formModel)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!await this.HasUserCreatedOrganizationAsync(userId, formModel.OrganizationId) && !User.IsInRole(AdministratorRoleName))
            {
                return RedirectToAction("Schedules", "Organization", new { organizationId = formModel.OrganizationId });
            }

            if (!ModelState.IsValid)
            {
                return this.View(formModel);
            }

            bool isDeleted = await this.scheduleService
                .RemoveScheduleAsync(formModel);

            if (!isDeleted)
            {
                ModelState.AddModelError(nameof(FailMessage), FailMessage);

                return this.View(formModel);
            }

            TempData[nameof(SuccessMessage)] = SuccessMessage;

            return RedirectToAction("Schedules", "Organization", new { organizationId = formModel.OrganizationId });
        }

        [HttpGet]
        public async Task<IActionResult> Passengers(string? id, string organizationId)
        {
            Guid scheduleGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(id, ref scheduleGuid);
            if (!isIdValid)
            {
                return RedirectToAction("Schedules", "Organization", new { organizationId });
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!await this.HasUserCreatedOrganizationAsync(userId, organizationId) && !User.IsInRole(AdministratorRoleName))
            {
                return RedirectToAction("Schedules", "Organization", new { organizationId });
            }

            ScheduleDetailsViewModel scheduleDetails = await this.scheduleService.GetScheduleDetailsAsync(scheduleGuid);
            if (scheduleDetails == null)
            {
                TempData[nameof(FailMessage)] = FailMessage;
                return RedirectToAction("Schedules", "Organization", new { organizationId });
            }

            var passengers = await this.ticketService.GetPassengersForScheduleAsync(scheduleGuid);

            PassengerListViewModel model = new PassengerListViewModel
            {
                ScheduleId = id,
                OrganizationId = organizationId,
                FromCity = scheduleDetails.FromCity,
                ToCity = scheduleDetails.ToCity,
                Day = scheduleDetails.Day,
                DepartingTime = scheduleDetails.Departing,
                Passengers = passengers,
            };

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Board(string customerId, string ticketId, string scheduleId, string organizationId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!await this.HasUserCreatedOrganizationAsync(userId, organizationId) && !User.IsInRole(AdministratorRoleName))
            {
                return RedirectToAction("Schedules", "Organization", new { organizationId });
            }

            Guid ticketGuid = Guid.Empty;
            bool isValid = IsGuidValid(ticketId, ref ticketGuid);
            if (!isValid)
            {
                TempData[nameof(FailMessage)] = FailMessage;
                return RedirectToAction(nameof(Passengers), new { id = scheduleId, organizationId });
            }

            await this.ticketService.BoardPassengerAsync(customerId, ticketGuid);

            return RedirectToAction(nameof(Passengers), new { id = scheduleId, organizationId });
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Search(TravelSearchViewModel model)
        {
            var cities = await this.cityService.GetAllCitiesForDropDownsAsync();
            model.Cities = cities;

            if (!string.IsNullOrEmpty(model.FromCityId) && !string.IsNullOrEmpty(model.ToCityId))
            {
                DayOfWeek? dayOfWeek = model.DepartureDate.HasValue
                    ? model.DepartureDate.Value.DayOfWeek
                    : (DayOfWeek?)null;

                model.Results = await this.scheduleService.SearchSchedulesAsync(
                    Guid.Parse(model.FromCityId),
                    Guid.Parse(model.ToCityId),
                    dayOfWeek);
            }

            return this.View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(string? id, string organizationId)
        {
            Guid scheduleGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(id, ref scheduleGuid);
            if (!isIdValid)
            {
                return RedirectToAction("Schedules", "Organization", new { organizationId = organizationId });

            }

            ScheduleDetailsViewModel? model = await scheduleService
                .GetScheduleDetailsAsync(scheduleGuid);

            if (model == null)
            {
                TempData[nameof(FailMessage)] = FailMessage;

                return RedirectToAction("Schedules", "Organization", new { organizationId = model.OrganizationId });
            }

            return View(model);
        }
    }
}
