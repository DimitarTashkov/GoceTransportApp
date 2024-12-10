using GoceTransportApp.Services.Data.Drivers;
using GoceTransportApp.Services.Data.Routes;
using GoceTransportApp.Web.ViewModels.Routes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using GoceTransportApp.Web.ViewModels.Drivers;

using static GoceTransportApp.Common.ErrorMessages.DriverMessages;
using GoceTransportApp.Data.Common.Repositories;
using GoceTransportApp.Data.Models;
using System.Security.Claims;

namespace GoceTransportApp.Web.Controllers
{
    [Authorize]
    public class DriverController : BaseController
    {
        private IDriverService driverService;

        public DriverController(IDriverService driverService, IDeletableEntityRepository<Organization> organizationRepository)
            : base(organizationRepository)
        {
            this.driverService = driverService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await driverService.GetAllDriversAsync();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create(string organizationId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!await this.HasUserCreatedOrganizationAsync(userId, organizationId))
            {
                return RedirectToAction("Drivers", "Organization", new { id = organizationId });
            }

            DriverInputModel model = new DriverInputModel();
            model.OrganizationId = organizationId;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(DriverInputModel model)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!await this.HasUserCreatedOrganizationAsync(userId, model.OrganizationId))
            {
                return RedirectToAction("Drivers", "Organization", new { id = model.OrganizationId });
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await driverService.CreateAsync(model);

            return RedirectToAction("Drivers", "Organization", new { id = model.OrganizationId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string? id, string organizationId)
        {

            Guid driverGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(id, ref driverGuid);

            if (!isIdValid)
            {
                return RedirectToAction("Drivers", "Organization", new { id = organizationId });
            }

            EditDriverInputModel? formModel = await this.driverService
                .GetDriverForEditAsync(driverGuid);

            if (formModel == null)
            {
                return RedirectToAction("Drivers", "Organization", new { id = formModel.OrganizationId });

            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!await this.HasUserCreatedOrganizationAsync(userId, formModel.OrganizationId))
            {
                return RedirectToAction("Drivers", "Organization", new { id = formModel.OrganizationId });
            }

            return this.View(formModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditDriverInputModel formModel)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!await this.HasUserCreatedOrganizationAsync(userId, formModel.OrganizationId))
            {
                return RedirectToAction("Drivers", "Organization", new { id = formModel.OrganizationId });
            }

            if (!ModelState.IsValid)
            {
                return this.View(formModel);
            }

            bool isUpdated = await this.driverService
                .EditDriverAsync(formModel);

            if (!isUpdated)
            {
                ModelState.AddModelError(nameof(DriverEditFailed), DriverEditFailed);

                return this.View(formModel);
            }

            return RedirectToAction("Drivers", "Organization", new { id = formModel.OrganizationId });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string? id, string organizationId)
        {
            Guid driverGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(id, ref driverGuid);
            if (!isIdValid)
            {
                return this.RedirectToAction(nameof(Index));
            }

            RemoveDriverViewModel? model = await driverService
                .GetDriverForDeletionAsync(driverGuid);

            if (model == null)
            {
                return RedirectToAction("Drivers", "Organization", new { id = organizationId });
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!await this.HasUserCreatedOrganizationAsync(userId, model.OrganizationId))
            {
                return RedirectToAction("Drivers", "Organization", new { id = model.OrganizationId });
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(RemoveDriverViewModel formModel)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!await this.HasUserCreatedOrganizationAsync(userId, formModel.OrganizationId))
            {
                return RedirectToAction("Drivers", "Organization", new { id = formModel.OrganizationId });
            }

            if (!ModelState.IsValid)
            {
                return this.View(formModel);
            }

            bool isDeleted = await this.driverService
                .RemoveDriverAsync(formModel);

            if (!isDeleted)
            {
                ModelState.AddModelError(nameof(DriverDeleteFailed), DriverDeleteFailed);

                return this.View(formModel);
            }

            return RedirectToAction("Drivers", "Organization", new { id = formModel.OrganizationId });
        }

        [HttpGet]
        public async Task<IActionResult> Details(string? id, string organizationId)
        {
            Guid driverGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(id, ref driverGuid);
            if (!isIdValid)
            {
                return RedirectToAction("Drivers", "Organization", new { id = organizationId });
            }

            DriverDetailsViewModel? model = await driverService
                .GetDriverDetailsAsync(driverGuid);

            if (model == null)
            {
                TempData[nameof(InvalidDriverDetails)] = InvalidDriverDetails;

                return RedirectToAction("Drivers", "Organization", new { id = organizationId });
            }

            return View(model);
        }
    }
}
