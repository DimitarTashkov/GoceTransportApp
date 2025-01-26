using GoceTransportApp.Services.Data.Organizations;
using GoceTransportApp.Services.Data.Vehicles;
using GoceTransportApp.Web.ViewModels.Vehicles;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using GoceTransportApp.Web.ViewModels.Organizations;

using static GoceTransportApp.Common.ResultMessages.GeneralMessages;
using static GoceTransportApp.Common.GlobalConstants;
using GoceTransportApp.Data.Common.Repositories;
using GoceTransportApp.Data.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using GoceTransportApp.Web.ViewModels.Routes;
using GoceTransportApp.Web.ViewModels.Schedules;
using GoceTransportApp.Web.ViewModels.Tickets;
using GoceTransportApp.Web.ViewModels.Drivers;
using System.Net.Mail;

namespace GoceTransportApp.Web.Controllers
{
    [Authorize]
    public class OrganizationController : BaseController
    {
        private readonly IOrganizationService organizationService;

        public OrganizationController(IOrganizationService organizationService, IDeletableEntityRepository<Organization> organizationRepository)
            : base(organizationRepository)
        {
            this.organizationService = organizationService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var model = await organizationService.GetAllOrganizationsAsync();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> UserOrganizations()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return this.RedirectToAction(nameof(Index));
            }

            var userOrganizations = await organizationService.GetUserOrganizationsAsync(userId);

            return View(userOrganizations);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            OrganizationInputModel model = new OrganizationInputModel();
            model.FounderId = userId;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrganizationInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await organizationService.CreateAsync(model);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string? id)
        {

            Guid organizationGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(id, ref organizationGuid);

            if (!isIdValid)
            {
                return this.RedirectToAction(nameof(Index));
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!await this.HasUserCreatedOrganizationAsync(userId, id) && !User.IsInRole(AdministratorRoleName))
            {
                return RedirectToAction(nameof(Index));
            }

            EditOrganizationInputModel? formModel = await this.organizationService
                .GetOrganizationForEditAsync(organizationGuid);

            if (formModel == null)
            {
                return this.RedirectToAction(nameof(Index));
            }

            return this.View(formModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditOrganizationInputModel formModel)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!await this.HasUserCreatedOrganizationAsync(userId, formModel.Id) && !User.IsInRole(AdministratorRoleName))
            {
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                return this.View(formModel);
            }

            bool isUpdated = await this.organizationService
                .EditOrganizationAsync(formModel);

            if (!isUpdated)
            {
                ModelState.AddModelError(nameof(FailMessage), FailMessage);

                return this.View(formModel);
            }

            return this.RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string? id)
        {
            Guid organizationGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(id, ref organizationGuid);
            if (!isIdValid)
            {
                return this.RedirectToAction(nameof(Index));
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!await this.HasUserCreatedOrganizationAsync(userId, id) && !User.IsInRole(AdministratorRoleName))
            {
                return RedirectToAction(nameof(Index));
            }

            RemoveOrganizationViewModel? model = await organizationService
                .GetOrganizationForDeletionAsync(organizationGuid);

            if (model == null)
            {
                return this.RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(RemoveOrganizationViewModel formModel)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!await this.HasUserCreatedOrganizationAsync(userId, formModel.Id) && !User.IsInRole(AdministratorRoleName))
            {
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                return this.View(formModel);
            }

            bool isDeleted = await this.organizationService
                .RemoveOrganizationAsync(formModel);

            if (!isDeleted)
            {
                ModelState.AddModelError(nameof(FailMessage), FailMessage);

                return this.View(formModel);
            }

            return this.RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(string? id)
        {
            Guid organizationGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(id, ref organizationGuid);
            if (!isIdValid)
            {
                return this.RedirectToAction(nameof(Index));
            }

            OrganizationDetailsViewModel? model = await organizationService
                .GetOrganizationDetailsAsync(organizationGuid);

            if (model == null)
            {
                TempData[nameof(FailMessage)] = FailMessage;

                return this.RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Routes(string organizationId)
        {
            Guid organizationGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(organizationId, ref organizationGuid);
            if (!isIdValid)
            {
                return this.RedirectToAction(nameof(Index));
            }

            var routes = await organizationService.GetRoutesByOrganizationId(organizationGuid);

            var viewModel = new RoutesForOrganizationViewModel
            {
                OrganizationId = organizationId,
                Routes = routes
            };

            return View(viewModel);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Drivers(string organizationId)
        {
            Guid organizationGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(organizationId, ref organizationGuid);
            if (!isIdValid)
            {
                return this.RedirectToAction(nameof(Index));
            }

            var drivers = await organizationService.GetDriversByOrganizationId(organizationGuid);

            var viewModel = new DriversForOrganizationViewModel
            {
                OrganizationId = organizationId,
                Drivers = drivers
            };

            return View(viewModel);
        }

        [HttpGet]
        [AllowAnonymous]

        public async Task<IActionResult> Vehicles(string organizationId)
        {
            Guid organizationGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(organizationId, ref organizationGuid);
            if (!isIdValid)
            {
                return this.RedirectToAction(nameof(Index));
            }

            var vehicles = await organizationService.GetVehiclesByOrganizationId(organizationGuid);

            var viewModel = new VehiclesForOrganizationViewModel
            {
                OrganizationId = organizationId,
                Vehicles = vehicles
            };

            return View(viewModel);
        }

        [HttpGet]
        [AllowAnonymous]

        public async Task<IActionResult> Tickets(string organizationId)
        {
            Guid organizationGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(organizationId, ref organizationGuid);
            if (!isIdValid)
            {
                return this.RedirectToAction(nameof(Index));
            }

            var tickets = await organizationService.GetTicketsByOrganizationId(organizationGuid);
            var viewModel = new TicketsForOrganizationViewModel
            {
                OrganizationId = organizationId,
                Tickets = tickets
            };

            return View(viewModel);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Schedules(string organizationId)
        {
            Guid organizationGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(organizationId, ref organizationGuid);
            if (!isIdValid)
            {
                return this.RedirectToAction(nameof(Index));
            }

            var schedules = await organizationService.GetSchedulesByOrganizationId(organizationGuid);
            var viewModel = new SchedulesForOrganizationViewModel
            {
                OrganizationId = organizationId,
                Schedules = schedules
            };

            return View(viewModel);
        }
    }
}
