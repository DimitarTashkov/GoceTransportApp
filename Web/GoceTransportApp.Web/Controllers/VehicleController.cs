using GoceTransportApp.Services.Data.Drivers;
using GoceTransportApp.Web.ViewModels.Drivers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using GoceTransportApp.Services.Data.Vehicles;
using GoceTransportApp.Web.ViewModels.Vehicles;

using static GoceTransportApp.Common.ErrorMessages.VehicleMessages;
using GoceTransportApp.Web.ViewModels.Streets;
using System.Collections.Generic;
using GoceTransportApp.Data.Common.Repositories;
using GoceTransportApp.Data.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace GoceTransportApp.Web.Controllers
{
    [Authorize]
    public class VehicleController : BaseController
    {
        private readonly IVehicleService vehicleService;

        public VehicleController(IVehicleService vehicleService, IDeletableEntityRepository<Organization> organizationRepository)
            : base(organizationRepository)
        {
            this.vehicleService = vehicleService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(AllVehiclesSearchFilterViewModel inputModel)
        {
            IEnumerable<VehicleDataViewModel> allVehicles =
                await vehicleService.GetAllVehiclesAsync(inputModel);

            int allVehiclesCount = await vehicleService.GetVehiclesCountByFilterAsync(inputModel);
            AllVehiclesSearchFilterViewModel viewModel = new AllVehiclesSearchFilterViewModel
            {
                Vehicles = allVehicles,
                SearchQuery = inputModel.SearchQuery,
                CurrentPage = inputModel.CurrentPage,
                TotalPages = (int)Math.Ceiling((double)allVehiclesCount / inputModel.EntitiesPerPage!.Value)
            };

            return this.View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Create(string organizationId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!await this.HasUserCreatedOrganizationAsync(userId, organizationId))
            {
                return RedirectToAction("Vehicles", "Organization", new { organizationId = organizationId });
            }

            VehicleInputModel model = new VehicleInputModel();
            model.OrganizationId = organizationId;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(VehicleInputModel model)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!await this.HasUserCreatedOrganizationAsync(userId, model.OrganizationId))
            {
                return RedirectToAction("Vehicles", "Organization", new { organizationId = model.OrganizationId });
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await vehicleService.CreateAsync(model);

            return RedirectToAction("Vehicles", "Organization", new { organizationId = model.OrganizationId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string? id, string organizationId)
        {

            Guid vehicleGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(id, ref vehicleGuid);

            if (!isIdValid)
            {
                return RedirectToAction("Vehicles", "Organization", new { organizationId = organizationId });
            }

            EditVehicleInputModel? formModel = await this.vehicleService
                .GetVehicleForEditAsync(vehicleGuid);

            if (formModel == null)
            {
                return RedirectToAction("Vehicles", "Organization", new { organizationId = formModel.OrganizationId });
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!await this.HasUserCreatedOrganizationAsync(userId, formModel.OrganizationId))
            {
                return RedirectToAction("Vehicles", "Organization", new { organizationId = formModel.OrganizationId });
            }

            return this.View(formModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditVehicleInputModel formModel)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!await this.HasUserCreatedOrganizationAsync(userId, formModel.OrganizationId))
            {
                return RedirectToAction("Vehicles", "Organization", new { organizationId = formModel.OrganizationId });
            }

            if (!ModelState.IsValid)
            {
                return this.View(formModel);
            }

            bool isUpdated = await this.vehicleService
                .EditVehicleAsync(formModel);

            if (!isUpdated)
            {
                ModelState.AddModelError(nameof(VehicleEditFailed), VehicleEditFailed);

                return this.View(formModel);
            }

            return RedirectToAction("Vehicles", "Organization", new { organizationId = formModel.OrganizationId });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string? id, string organizationId)
        {
            Guid vehicleGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(id, ref vehicleGuid);
            if (!isIdValid)
            {
                return RedirectToAction("Vehicles", "Organization", new { organizationId = organizationId });
            }

            RemoveVehicleViewModel? model = await vehicleService
                .GetVehicleForDeletionAsync(vehicleGuid);

            if (model == null)
            {
                return RedirectToAction("Vehicles", "Organization", new { organizationId = organizationId });
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!await this.HasUserCreatedOrganizationAsync(userId, model.OrganizationId))
            {
                return RedirectToAction("Vehicles", "Organization", new { organizationId = model.OrganizationId });
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(RemoveVehicleViewModel formModel)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!await this.HasUserCreatedOrganizationAsync(userId, formModel.OrganizationId))
            {
                return RedirectToAction("Vehicles", "Organization", new { organizationId = formModel.OrganizationId });
            }

            if (!ModelState.IsValid)
            {
                return this.View(formModel);
            }

            bool isDeleted = await this.vehicleService
                .RemoveVehicleAsync(formModel);

            if (!isDeleted)
            {
                ModelState.AddModelError(nameof(VehicleDeleteFailed), VehicleDeleteFailed);

                return this.View(formModel);
            }

            return RedirectToAction("Vehicles", "Organization", new { organizationId = formModel.OrganizationId });
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(string? id, string organizationId)
        {
            Guid vehicleGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(id, ref vehicleGuid);
            if (!isIdValid)
            {
                return RedirectToAction("Vehicles", "Organization", new { organizationId = organizationId });
            }

            VehicleDetailsViewModel? model = await vehicleService
                .GetVehicleDetailsAsync(vehicleGuid);

            if (model == null)
            {
                TempData[nameof(InvalidVehicleDetails)] = InvalidVehicleDetails;

                return RedirectToAction("Vehicles", "Organization", new { organizationId = organizationId });
            }

            return View(model);
        }
    }
}
