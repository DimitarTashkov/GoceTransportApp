using GoceTransportApp.Services.Data.Cities;
using GoceTransportApp.Services.Data.Schedules;
using GoceTransportApp.Web.Hubs;
using GoceTransportApp.Web.ViewModels.Vehicles;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System;

using static GoceTransportApp.Common.ResultMessages.GeneralMessages;
using static GoceTransportApp.Common.GlobalConstants;
using GoceTransportApp.Web.ViewModels.Schedules;
using GoceTransportApp.Data.Common.Repositories;
using GoceTransportApp.Data.Models;
using GoceTransportApp.Data.Models.Enumerations;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Net.Mail;
using GoceTransportApp.Services.Data.Vehicles;
using GoceTransportApp.Services.Data.Routes;
using GoceTransportApp.Services.Data.Tickets;
using GoceTransportApp.Web.ViewModels.Tickets;
using System.Collections.Generic;
using System.Linq;
using GoceTransportApp.Web.ViewModels.Organizations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
        private readonly IDeletableEntityRepository<Organization> organizationRepository;
        private readonly IHubContext<NotificationHub> hubContext;
        private readonly UserManager<ApplicationUser> userManager;

        public ScheduleController(
            IScheduleService scheduleService,
            IDeletableEntityRepository<Organization> organizationRepository,
            IVehicleService vehicleService,
            IRouteService routeService,
            ICityService cityService,
            ITicketService ticketService,
            IHubContext<NotificationHub> hubContext,
            UserManager<ApplicationUser> userManager)
            : base(organizationRepository)
        {
            this.scheduleService = scheduleService;
            this.organizationRepository = organizationRepository;
            this.vehicleService = vehicleService;
            this.routeService = routeService;
            this.cityService = cityService;
            this.ticketService = ticketService;
            this.hubContext = hubContext;
            this.userManager = userManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index(AllSchedulesSearchFilterViewModel inputModel)
        {
            string? userId = User.Identity.IsAuthenticated
                ? User.FindFirstValue(ClaimTypes.NameIdentifier)
                : null;

            // Security: non-admin users see only their organisations
            if (userId != null && !User.IsInRole(AdministratorRoleName))
            {
                inputModel.OrganizationFilter = await this.GetUserOrganizationIdsAsync(userId);
            }

            var schedules = await scheduleService.GetAllSchedulesAsync(inputModel);
            var schedulesCount = await scheduleService.GetSchedulesCountByFilterAsync(inputModel);

            // Load dropdown data for the filter panel
            var cities = await cityService.GetAllCitiesForDropDownsAsync();

            IEnumerable<OrganizationDataViewModel> availableOrgs;
            if (User.IsInRole(AdministratorRoleName))
            {
                availableOrgs = await organizationRepository.AllAsNoTracking()
                    .Select(o => new OrganizationDataViewModel { Id = o.Id.ToString(), Name = o.Name })
                    .ToListAsync();
            }
            else if (userId != null)
            {
                availableOrgs = await organizationRepository.AllAsNoTracking()
                    .Where(o => o.FounderId == userId)
                    .Select(o => new OrganizationDataViewModel { Id = o.Id.ToString(), Name = o.Name })
                    .ToListAsync();
            }
            else
            {
                availableOrgs = Array.Empty<OrganizationDataViewModel>();
            }

            inputModel.Schedules = schedules;
            inputModel.TotalPages = (int)Math.Ceiling((double)schedulesCount / inputModel.EntitiesPerPage.Value);
            inputModel.AvailableCities = cities;
            inputModel.AvailableOrganizations = availableOrgs;

            return this.View(inputModel);
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
                return RedirectToAction("Details", "Organization", new { id = model.OrganizationId });
            }

            if (!ModelState.IsValid)
            {
                model.Vehicles = await vehicleService.GetVehiclesForOrganizationAsync(model.OrganizationId);
                model.Routes = await routeService.GetRoutesForOrganizationAsync(model.OrganizationId);
                return View(model);
            }

            await scheduleService.CreateAsync(model);
            TempData[nameof(SuccessMessage)] = SuccessMessage;

            return RedirectToAction("Details", "Organization", new { id = model.OrganizationId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string? id, string organizationId)
        {

            Guid scheduleGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(id, ref scheduleGuid);

            if (!isIdValid)
            {
                return RedirectToAction("Details", "Organization", new { id = organizationId });
            }

            EditScheduleInputModel? formModel = await this.scheduleService
                .GetScheduleForEditAsync(scheduleGuid);

            if (formModel == null)
            {
                TempData[nameof(FailMessage)] = FailMessage;

                return RedirectToAction("Details", "Organization", new { id = organizationId });
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!await this.HasUserCreatedOrganizationAsync(userId, formModel.OrganizationId) && !User.IsInRole(AdministratorRoleName))
            {
                return RedirectToAction("Details", "Organization", new { id = formModel.OrganizationId });
            }

            formModel.Vehicles = await vehicleService.GetVehiclesForOrganizationAsync(organizationId);
            formModel.Routes = await routeService.GetRoutesForOrganizationAsync(organizationId);

            var currentUser = await this.userManager.FindByIdAsync(userId);
            ViewBag.IsPremiumUser = User.IsInRole(AdministratorRoleName) ||
                (currentUser != null && currentUser.MembershipTier != MembershipTier.Free);

            return this.View(formModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditScheduleInputModel formModel)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!await this.HasUserCreatedOrganizationAsync(userId, formModel.OrganizationId) && !User.IsInRole(AdministratorRoleName))
            {
                return RedirectToAction("Details", "Organization", new { id = formModel.OrganizationId });
            }

            if (!ModelState.IsValid)
            {
                formModel.Vehicles = await vehicleService.GetVehiclesForOrganizationAsync(formModel.OrganizationId);
                formModel.Routes = await routeService.GetRoutesForOrganizationAsync(formModel.OrganizationId);
                return this.View(formModel);
            }

            // Gate LiveStatus for Free-tier users
            if (!User.IsInRole(AdministratorRoleName))
            {
                var currentUser = await this.userManager.FindByIdAsync(userId);
                if (currentUser != null && currentUser.MembershipTier == MembershipTier.Free)
                {
                    formModel.LiveStatus = null;
                }
            }

            bool isUpdated = await this.scheduleService
                .EditScheduleAsync(formModel);

            if (!isUpdated)
            {
                ModelState.AddModelError(nameof(FailMessage), FailMessage);

                return this.View(formModel);
            }

            if (!string.IsNullOrWhiteSpace(formModel.LiveStatus))
            {
                await this.hubContext.Clients.All
                    .SendAsync(SignalRMethods.ReceiveStatusUpdate, formModel.Id, formModel.LiveStatus);
            }

            TempData[nameof(SuccessMessage)] = SuccessMessage;

            return RedirectToAction("Details", "Organization", new { id = formModel.OrganizationId });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string? id, string organizationId)
        {
            Guid scheduleGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(id, ref scheduleGuid);
            if (!isIdValid)
            {
                return RedirectToAction("Details", "Organization", new { id = organizationId });
            }

            RemoveScheduleViewModel? model = await scheduleService
                .GetScheduleForDeletionAsync(scheduleGuid);

            if (model == null)
            {
                TempData[nameof(FailMessage)] = FailMessage;

                return RedirectToAction("Details", "Organization", new { id = organizationId });
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!await this.HasUserCreatedOrganizationAsync(userId, model.OrganizationId) && !User.IsInRole(AdministratorRoleName))
            {
                return RedirectToAction("Details", "Organization", new { id = model.OrganizationId });
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(RemoveScheduleViewModel formModel)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!await this.HasUserCreatedOrganizationAsync(userId, formModel.OrganizationId) && !User.IsInRole(AdministratorRoleName))
            {
                return RedirectToAction("Details", "Organization", new { id = formModel.OrganizationId });
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

            return RedirectToAction("Details", "Organization", new { id = formModel.OrganizationId });
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
                return RedirectToAction("Details", "Organization", new { id = organizationId });

            }

            ScheduleDetailsViewModel? model = await scheduleService
                .GetScheduleDetailsAsync(scheduleGuid);

            if (model == null)
            {
                TempData[nameof(FailMessage)] = FailMessage;

                return RedirectToAction("Details", "Organization", new { id = model.OrganizationId });
            }

            return View(model);
        }
    }
}
