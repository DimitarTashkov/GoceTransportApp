using GoceTransportApp.Services.Data.Organizations;
using GoceTransportApp.Services.Data.Vehicles;
using GoceTransportApp.Web.ViewModels.Vehicles;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using GoceTransportApp.Web.ViewModels.Organizations;

using static GoceTransportApp.Common.ErrorMessages.OrganizationMessages;

namespace GoceTransportApp.Web.Controllers
{
    public class OrganizationController : BaseController
    {
        private readonly IOrganizationService organizationService;

        public OrganizationController(IOrganizationService organizationService)
        {
            this.organizationService = organizationService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await organizationService.GetAllOrganizationsAsync();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            OrganizationInputModel model = new OrganizationInputModel();

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
            if (!ModelState.IsValid)
            {
                return this.View(formModel);
            }

            bool isUpdated = await this.organizationService
                .EditOrganizationAsync(formModel);

            if (!isUpdated)
            {
                ModelState.AddModelError(nameof(OrganizationEditFailed), OrganizationEditFailed);

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
            if (!ModelState.IsValid)
            {
                return this.View(formModel);
            }

            bool isDeleted = await this.organizationService
                .RemoveOrganizationAsync(formModel);

            if (!isDeleted)
            {
                ModelState.AddModelError(nameof(OrganizationDeleteFailed), OrganizationDeleteFailed);

                return this.View(formModel);
            }

            return this.RedirectToAction(nameof(Index));
        }

        [HttpGet]
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
                TempData[nameof(InvalidOrganizationDetails)] = InvalidOrganizationDetails;

                return this.RedirectToAction(nameof(Index));
            }

            return View(model);
        }
    }
}
