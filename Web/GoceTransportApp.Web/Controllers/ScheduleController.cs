using GoceTransportApp.Services.Data.Schedules;
using GoceTransportApp.Web.ViewModels.Vehicles;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;

using static GoceTransportApp.Common.ErrorMessages.ScheduleMessages;
using GoceTransportApp.Web.ViewModels.Schedules;

namespace GoceTransportApp.Web.Controllers
{
    public class ScheduleController : BaseController
    {
        private readonly IScheduleService scheduleService;

        public ScheduleController(IScheduleService scheduleService)
        {
            this.scheduleService = scheduleService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await scheduleService.GetAllSchedulesAsync();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ScheduleInputModel model = new ScheduleInputModel();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ScheduleInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await scheduleService.CreateAsync(model);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string? id)
        {

            Guid scheduleGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(id, ref scheduleGuid);

            if (!isIdValid)
            {
                return this.RedirectToAction(nameof(Index));
            }

            EditScheduleInputModel? formModel = await this.scheduleService
                .GetScheduleForEditAsync(scheduleGuid);

            if (formModel == null)
            {
                return this.RedirectToAction(nameof(Index));
            }

            return this.View(formModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditScheduleInputModel formModel)
        {
            if (!ModelState.IsValid)
            {
                return this.View(formModel);
            }

            bool isUpdated = await this.scheduleService
                .EditScheduleAsync(formModel);

            if (!isUpdated)
            {
                ModelState.AddModelError(nameof(ScheduleEditFailed), ScheduleEditFailed);

                return this.View(formModel);
            }

            return this.RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string? id)
        {
            Guid scheduleGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(id, ref scheduleGuid);
            if (!isIdValid)
            {
                return this.RedirectToAction(nameof(Index));
            }

            RemoveScheduleViewModel? model = await scheduleService
                .GetScheduleForDeletionAsync(scheduleGuid);

            if (model == null)
            {
                return this.RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(RemoveScheduleViewModel formModel)
        {
            if (!ModelState.IsValid)
            {
                return this.View(formModel);
            }

            bool isDeleted = await this.scheduleService
                .RemoveScheduleAsync(formModel);

            if (!isDeleted)
            {
                ModelState.AddModelError(nameof(ScheduleDeleteFailed), ScheduleDeleteFailed);

                return this.View(formModel);
            }

            return this.RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(string? id)
        {
            Guid scheduleGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(id, ref scheduleGuid);
            if (!isIdValid)
            {
                return this.RedirectToAction(nameof(Index));
            }

            ScheduleDetailsViewModel? model = await scheduleService
                .GetScheduleDetailsAsync(scheduleGuid);

            if (model == null)
            {
                TempData[nameof(InvalidScheduleDetails)] = InvalidScheduleDetails;

                return this.RedirectToAction(nameof(Index));
            }

            return View(model);
        }
    }
}
