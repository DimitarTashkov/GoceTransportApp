using GoceTransportApp.Services.Data.Drivers;
using GoceTransportApp.Web.ViewModels.Drivers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using GoceTransportApp.Services.Data.Vehicles;
using GoceTransportApp.Web.ViewModels.Vehicles;

using static GoceTransportApp.Common.ErrorMessages.VehicleMessages;


namespace GoceTransportApp.Web.Controllers
{
    public class VehicleController : BaseController
    {
        private readonly IVehicleService vehicleService;

        public VehicleController(IVehicleService vehicleService)
        {
            this.vehicleService = vehicleService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await vehicleService.GetAllVehicles();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            VehicleInputModel model = new VehicleInputModel();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(VehicleInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await vehicleService.CreateAsync(model);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string? id)
        {

            Guid vehicleGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(id, ref vehicleGuid);

            if (!isIdValid)
            {
                return this.RedirectToAction(nameof(Index));
            }

            EditVehicleInputModel? formModel = await this.vehicleService
                .GetVehicleForEdit(vehicleGuid);

            if (formModel == null)
            {
                return this.RedirectToAction(nameof(Index));
            }

            return this.View(formModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditVehicleInputModel formModel)
        {
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

            return this.RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string? id)
        {
            Guid vehicleGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(id, ref vehicleGuid);
            if (!isIdValid)
            {
                return this.RedirectToAction(nameof(Index));
            }

            RemoveVehicleViewModel? model = await vehicleService
                .GetVehicleForDeletion(vehicleGuid);

            if (model == null)
            {
                return this.RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(RemoveVehicleViewModel formModel)
        {
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

            return this.RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(string? id)
        {
            Guid vehicleGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(id, ref vehicleGuid);
            if (!isIdValid)
            {
                return this.RedirectToAction(nameof(Index));
            }

            VehicleDetailsViewModel? model = await vehicleService
                .VehicleDetails(vehicleGuid);

            if (model == null)
            {
                TempData[nameof(InvalidVehicleDetails)] = InvalidVehicleDetails;

                return this.RedirectToAction(nameof(Index));
            }

            return View(model);
        }
    }
}
