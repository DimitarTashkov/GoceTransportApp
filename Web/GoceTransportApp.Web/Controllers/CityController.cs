using GoceTransportApp.Services.Data.Streets;
using GoceTransportApp.Web.ViewModels.Streets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using GoceTransportApp.Services.Data.Cities;
using GoceTransportApp.Web.ViewModels.Cities;

using static GoceTransportApp.Common.ResultMessages.CityMessages;
using static GoceTransportApp.Common.GlobalConstants;

using System.Collections.Generic;
using GoceTransportApp.Data.Common.Repositories;
using GoceTransportApp.Data.Models;

namespace GoceTransportApp.Web.Controllers
{
    [Authorize]
    public class CityController : BaseController
    {
        private readonly ICityService cityService;

        public CityController(ICityService cityService, IDeletableEntityRepository<Organization> organizationRepository)
            : base(organizationRepository)
        {
            this.cityService = cityService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(AllCitiesSearchFilterViewModel inputModel)
        {
            IEnumerable<CityDataViewModel> allCities =
                await this.cityService.GetAllCitiesAsync(inputModel);

            int allCitiesCount = await this.cityService.GetCitiesCountByFilterAsync(inputModel);
            AllCitiesSearchFilterViewModel viewModel = new AllCitiesSearchFilterViewModel
            {
                Cities = allCities,
                SearchQuery = inputModel.SearchQuery,
                CurrentPage = inputModel.CurrentPage,
                TotalPages = (int)Math.Ceiling((double)allCitiesCount / inputModel.EntitiesPerPage!.Value)
            };

            return this.View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            CityInputModel model = new CityInputModel();

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
        [Authorize(Roles = AdministratorRoleName)]
        public async Task<IActionResult> Edit(string? id)
        {
            Guid cityGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(id, ref cityGuid);

            if (!isIdValid)
            {
                return this.RedirectToAction(nameof(Index));
            }

            EditCityInputModel? formModel = await this.cityService
                .GetCityForEditAsync(cityGuid);

            if (formModel == null)
            {
                return this.RedirectToAction(nameof(Index));
            }

            return this.View(formModel);
        }

        [HttpPost]
        [Authorize(Roles = AdministratorRoleName)]
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
        [Authorize(Roles = AdministratorRoleName)]
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

                return this.RedirectToAction("Index");
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
                .GetCityDetailsAsync(cityGuid);
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
                .GetCityDetailsByNameAsync(name);

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
                .GetAddStreetToCityModelAsync(cityGuid);

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
                .AddStreetToCityAsync(cityGuid, model);
            if (result == false)
            {
                // TODO: Add temp message and redirect to Details
                return this.RedirectToAction(nameof(Index));
            }

            return this.RedirectToAction(nameof(Details), new {id = model.Id});
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

            IEnumerable<StreetDataViewModel> model = await cityService.GetAllStreetsInCityAsync(cityGuid);

            if (model == null)
            {
                return this.RedirectToAction(nameof(Index));
            }

            return View(model);
        }
    }
}
