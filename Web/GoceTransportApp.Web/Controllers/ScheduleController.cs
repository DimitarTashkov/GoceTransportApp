﻿using GoceTransportApp.Services.Data.Schedules;
using GoceTransportApp.Web.ViewModels.Vehicles;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;

using static GoceTransportApp.Common.ErrorMessages.ScheduleMessages;
using static GoceTransportApp.Common.GlobalConstants;
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
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var model = await scheduleService.GetAllSchedulesAsync();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create(string organizationId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!await this.HasUserCreatedOrganizationAsync(userId, organizationId) && !User.IsInRole(AdministratorRoleName))
            {
                return RedirectToAction("Schedules", "Organization", new { organizationId = organizationId });
            }

            ScheduleInputModel model = new ScheduleInputModel();
            model.OrganizationId = organizationId;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ScheduleInputModel model)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!await this.HasUserCreatedOrganizationAsync(userId, model.OrganizationId) && !User.IsInRole(AdministratorRoleName))
            {
                return RedirectToAction("Schedules", "Organization", new { organizationId = model.OrganizationId });
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await scheduleService.CreateAsync(model);

            return RedirectToAction("Schedules", "Organization", new { organizationId = model.OrganizationId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string? id, string organizationId)
        {

            Guid scheduleGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(id, ref scheduleGuid);

            if (!isIdValid)
            {
                return RedirectToAction("Schedules", "Organization", new { organizationId = organizationId });
            }

            EditScheduleInputModel? formModel = await this.scheduleService
                .GetScheduleForEditAsync(scheduleGuid);

            if (formModel == null)
            {
                return RedirectToAction("Schedules", "Organization", new { organizationId = organizationId });
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!await this.HasUserCreatedOrganizationAsync(userId, formModel.OrganizationId) && !User.IsInRole(AdministratorRoleName))
            {
                return RedirectToAction("Schedules", "Organization", new { organizationId = formModel.OrganizationId });
            }

            return this.View(formModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditScheduleInputModel formModel)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!await this.HasUserCreatedOrganizationAsync(userId, formModel.OrganizationId) && !User.IsInRole(AdministratorRoleName))
            {
                return RedirectToAction("Schedules", "Organization", new { organizationId = formModel.OrganizationId });
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

            return RedirectToAction("Schedules", "Organization", new { organizationId = formModel.OrganizationId });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string? id, string organizationId)
        {
            Guid scheduleGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(id, ref scheduleGuid);
            if (!isIdValid)
            {
                return RedirectToAction("Schedules", "Organization", new { organizationId = organizationId });
            }

            RemoveScheduleViewModel? model = await scheduleService
                .GetScheduleForDeletionAsync(scheduleGuid);

            if (model == null)
            {
                return RedirectToAction("Schedules", "Organization", new { organizationId = organizationId });
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!await this.HasUserCreatedOrganizationAsync(userId, model.OrganizationId) && !User.IsInRole(AdministratorRoleName))
            {
                return RedirectToAction("Schedules", "Organization", new { organizationId = model.OrganizationId });
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(RemoveScheduleViewModel formModel)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!await this.HasUserCreatedOrganizationAsync(userId, formModel.OrganizationId) && !User.IsInRole(AdministratorRoleName))
            {
                return RedirectToAction("Schedules", "Organization", new { organizationId = formModel.OrganizationId });
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

            return RedirectToAction("Schedules", "Organization", new { organizationId = formModel.OrganizationId });
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(string? id, string organizationId)
        {
            Guid scheduleGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(id, ref scheduleGuid);
            if (!isIdValid)
            {
                return RedirectToAction("Schedules", "Organization", new { organizationId = organizationId });

            }

            ScheduleDetailsViewModel? model = await scheduleService
                .GetScheduleDetailsAsync(scheduleGuid);

            if (model == null)
            {
                TempData[nameof(InvalidScheduleDetails)] = InvalidScheduleDetails;

                return RedirectToAction("Schedules", "Organization", new { organizationId = model.OrganizationId });
            }

            return View(model);
        }
    }
}
