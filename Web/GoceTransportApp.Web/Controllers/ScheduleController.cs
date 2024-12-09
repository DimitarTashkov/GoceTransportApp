﻿using GoceTransportApp.Services.Data.Schedules;
using GoceTransportApp.Web.ViewModels.Vehicles;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;

using static GoceTransportApp.Common.ErrorMessages.ScheduleMessages;
using GoceTransportApp.Web.ViewModels.Schedules;
using GoceTransportApp.Data.Common.Repositories;
using GoceTransportApp.Data.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace GoceTransportApp.Web.Controllers
{
    [Authorize]
    public class ScheduleController : BaseController
    {
        private readonly IScheduleService scheduleService;

        public ScheduleController(IScheduleService scheduleService, IDeletableEntityRepository<Organization> organizationRepository)
            : base(organizationRepository)
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
        public async Task<IActionResult> Create(string organizationId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!await this.HasUserCreatedOrganizationAsync(userId, organizationId))
            {
                return RedirectToAction("Schedules", "Organization", new { id = organizationId });
            }

            ScheduleInputModel model = new ScheduleInputModel();
            model.OrganizationId = organizationId;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ScheduleInputModel model)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!await this.HasUserCreatedOrganizationAsync(userId, model.OrganizationId))
            {
                return RedirectToAction("Schedules", "Organization", new { id = model.OrganizationId });
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await scheduleService.CreateAsync(model);

            return RedirectToAction("Schedules", "Organization", new { id = model.OrganizationId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string? id, string organizationId)
        {

            Guid scheduleGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(id, ref scheduleGuid);

            if (!isIdValid)
            {
                return RedirectToAction("Schedules", "Organization", new { id = organizationId });
            }

            EditScheduleInputModel? formModel = await this.scheduleService
                .GetScheduleForEditAsync(scheduleGuid);

            if (formModel == null)
            {
                return RedirectToAction("Schedules", "Organization", new { id = organizationId });
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!await this.HasUserCreatedOrganizationAsync(userId, formModel.OrganizationId))
            {
                return RedirectToAction("Schedules", "Organization", new { id = formModel.OrganizationId });
            }

            return this.View(formModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditScheduleInputModel formModel)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!await this.HasUserCreatedOrganizationAsync(userId, formModel.OrganizationId))
            {
                return RedirectToAction("Schedules", "Organization", new { id = formModel.OrganizationId });
            }

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

            return RedirectToAction("Schedules", "Organization", new { id = formModel.OrganizationId });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string? id, string organizationId)
        {
            Guid scheduleGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(id, ref scheduleGuid);
            if (!isIdValid)
            {
                return RedirectToAction("Schedules", "Organization", new { id = organizationId });
            }

            RemoveScheduleViewModel? model = await scheduleService
                .GetScheduleForDeletionAsync(scheduleGuid);

            if (model == null)
            {
                return RedirectToAction("Schedules", "Organization", new { id = organizationId });
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!await this.HasUserCreatedOrganizationAsync(userId, model.OrganizationId))
            {
                return RedirectToAction("Schedules", "Organization", new { id = model.OrganizationId });
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(RemoveScheduleViewModel formModel)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!await this.HasUserCreatedOrganizationAsync(userId, formModel.OrganizationId))
            {
                return RedirectToAction("Schedules", "Organization", new { id = formModel.OrganizationId });
            }

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

            return RedirectToAction("Schedules", "Organization", new { id = formModel.OrganizationId });
        }

        [HttpGet]
        public async Task<IActionResult> Details(string? id, string organizationId)
        {
            Guid scheduleGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(id, ref scheduleGuid);
            if (!isIdValid)
            {
                return RedirectToAction("Schedules", "Organization", new { id = organizationId });

            }

            ScheduleDetailsViewModel? model = await scheduleService
                .GetScheduleDetailsAsync(scheduleGuid);

            if (model == null)
            {
                TempData[nameof(InvalidScheduleDetails)] = InvalidScheduleDetails;

                return RedirectToAction("Schedules", "Organization", new { id = model.OrganizationId });
            }

            return View(model);
        }
    }
}
