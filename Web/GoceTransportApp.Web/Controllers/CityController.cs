﻿using GoceTransportApp.Services.Data.Streets;
using GoceTransportApp.Web.ViewModels.Streets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using GoceTransportApp.Services.Data.Cities;
using GoceTransportApp.Web.ViewModels.Cities;

using static GoceTransportApp.Common.ErrorMessages.CityMessages;
using System.Collections.Generic;

namespace GoceTransportApp.Web.Controllers
{
    [Authorize]
    public class CityController : BaseController
    {
        private readonly ICityService cityService;

        public CityController(ICityService cityService)
        {
            this.cityService = cityService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await cityService.GetAllCities();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            CityInputModel model = new CityInputModel();
            // TODO: Check if the user has permissions to create streets

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CityInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await cityService.CreateAsync(model);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string? id)
        {

            Guid cityGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(id, ref cityGuid);

            if (!isIdValid)
            {
                return this.RedirectToAction(nameof(Index));
            }

            EditCityInputModel? formModel = await this.cityService
                .GetCityForEdit(cityGuid);

            if (formModel == null)
            {
                return this.RedirectToAction(nameof(Index));
            }

            return this.View(formModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditCityInputModel formModel)
        {
            if (!ModelState.IsValid)
            {
                return this.View(formModel);
            }

            bool isUpdated = await this.cityService
                .EditCityAsync(formModel);

            if (!isUpdated)
            {
                ModelState.AddModelError(nameof(CityEditFailed), CityEditFailed);

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

            bool result = await cityService
                .DeleteCityAsync(streetGuid);

            if (result == false)
            {
                TempData[nameof(CityDeleteFailed)] = CityDeleteFailed;

                return this.RedirectToAction("Index", "Movie");
            }

            return this.RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(string? id)
        {
            Guid cityGuid = Guid.Empty;
            bool isCityValid = this.IsGuidValid(id, ref cityGuid);

            if (!isCityValid)
            {
                return this.RedirectToAction(nameof(Index));
            }

            CityDetailsViewModel? model = await this.cityService
                .GetCityDetails(cityGuid);
            if (model == null)
            {
                return this.RedirectToAction(nameof(Index));
            }

            return this.View(model);
        }

        [HttpGet]
        public async Task<IActionResult> DetailsByName(string? name)
        {
            if (name == null)
            {
                return this.RedirectToAction(nameof(Index));
            }

            CityDetailsViewModel? model = await this.cityService
                .GetCityDetailsByName(name);

            if (model == null)
            {
                return this.RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Details), new { id = model.Id });
        }

        [HttpGet]
        public async Task<IActionResult> AddStreet(string? id)
        {
            Guid cityGuid = Guid.Empty;
            bool isGuidValid = this.IsGuidValid(id, ref cityGuid);
            if (!isGuidValid)
            {
                return this.RedirectToAction(nameof(Index));
            }

            CityAddStreetInputModel? viewModel = await this.cityService
                .GetAddMovieToCinemaInputModelByIdAsync(cityGuid);

            if (viewModel == null)
            {
                return this.RedirectToAction(nameof(Index));
            }

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddStreet(CityAddStreetInputModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            Guid cityGuid = Guid.Empty;
            bool isGuidValid = this.IsGuidValid(model.Id, ref cityGuid);
            if (!isGuidValid)
            {
                return this.RedirectToAction(nameof(Index));
            }

            bool result = await this.cityService
                .AddStreetToCity(cityGuid, model);
            if (result == false)
            {
                return this.RedirectToAction(nameof(Index));
            }

            return this.RedirectToAction(nameof(Index), "Cinema");
        }

        [HttpGet]
        public async Task<IActionResult> StreetsInCity(string? id)
        {
            Guid cityGuid = Guid.Empty;
            bool isGuidValid = this.IsGuidValid(id, ref cityGuid);
            if (!isGuidValid)
            {
                return this.RedirectToAction(nameof(Index));
            }

            IEnumerable<StreetDataViewModel> model = await cityService.GetAllStreetsInCity(cityGuid);

            if (model == null)
            {
                return this.RedirectToAction(nameof(Index));
            }

            return View(model);
        }
    }
}