using GoceTransportApp.Services.Data.Streets;
using GoceTransportApp.Web.ViewModels.Streets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System;
using System.Threading.Tasks;

using static GoceTransportApp.Common.ErrorMessages.StreetMessages;

namespace GoceTransportApp.Web.Controllers
{
    [Authorize]
    public class StreetController : BaseController
    {
        private readonly IStreetService streetService;

        public StreetController(IStreetService streetService)
        {
            this.streetService = streetService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await streetService.GetAllStreets();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            StreetInputModel model = new StreetInputModel();
            // TODO: Check if the user has permissions to create streets

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(StreetInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await streetService.CreateAsync(model);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string? id)
        {

            Guid streetGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(id, ref streetGuid);

            if (!isIdValid)
            {
                return this.RedirectToAction(nameof(Index));
            }

            EditStreetInputModel? formModel = await this.streetService
                .GetStreetForEdit(streetGuid);

            if (formModel == null)
            {
                return this.RedirectToAction(nameof(Index));
            }

            return this.View(formModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditStreetInputModel formModel)
        {
            if (!ModelState.IsValid)
            {
                return this.View(formModel);
            }

            bool isUpdated = await this.streetService
                .EditStreetAsync(formModel);

            if (!isUpdated)
            {
                ModelState.AddModelError(nameof(StreetEditFailed), StreetEditFailed);

                return this.View(formModel);
            }

            return this.RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string? id)
        {
            Guid streetGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(id, ref streetGuid);
            if (!isIdValid)
            {
                return this.RedirectToAction(nameof(Index));
            }

            bool result = await streetService
                .DeleteStreetAsync(streetGuid);

            if (result == false)
            {
                TempData[nameof(StreetDeleteFailed)] = StreetDeleteFailed;

                return this.RedirectToAction("Index");
            }

            return this.RedirectToAction(nameof(Index));
        }
    }
}
