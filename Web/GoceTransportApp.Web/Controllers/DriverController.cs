using GoceTransportApp.Services.Data.Drivers;
using GoceTransportApp.Services.Data.Routes;
using GoceTransportApp.Web.ViewModels.Routes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using GoceTransportApp.Web.ViewModels.Drivers;

using static GoceTransportApp.Common.ErrorMessages.DriverMessages;

namespace GoceTransportApp.Web.Controllers
{
    [Authorize]
    public class DriverController : BaseController
    {
        private IDriverService driverService;

        public DriverController(IDriverService driverService)
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
        public async Task<IActionResult> Create()
        {
            DriverInputModel model = new DriverInputModel();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(DriverInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await driverService.CreateAsync(model);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string? id)
        {

            Guid driverGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(id, ref driverGuid);

            if (!isIdValid)
            {
                return this.RedirectToAction(nameof(Index));
            }

            EditDriverInputModel? formModel = await this.driverService
                .GetDriverForEditAsync(driverGuid);

            if (formModel == null)
            {
                return this.RedirectToAction(nameof(Index));
            }

            return this.View(formModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditDriverInputModel formModel)
        {
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

            return this.RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string? id)
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
                return this.RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(RemoveDriverViewModel formModel)
        {
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

            return this.RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(string? id)
        {
            Guid driverGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(id, ref driverGuid);
            if (!isIdValid)
            {
                return this.RedirectToAction(nameof(Index));
            }

            DriverDetailsViewModel? model = await driverService
                .GetDriverDetailsAsync(driverGuid);

            if (model == null)
            {
                TempData[nameof(InvalidDriverDetails)] = InvalidDriverDetails;

                return this.RedirectToAction("Index");
            }

            return View(model);
        }
    }
}
