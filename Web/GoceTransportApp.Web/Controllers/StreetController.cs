using GoceTransportApp.Data.Common.Repositories;
using GoceTransportApp.Data.Models;
using GoceTransportApp.Services.Data.Streets;
using GoceTransportApp.Web.ViewModels.Streets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using static GoceTransportApp.Common.ResultMessages.StreetMessages;
using static GoceTransportApp.Common.GlobalConstants;

namespace GoceTransportApp.Web.Controllers
{
    [Authorize]
    public class StreetController : BaseController
    {
        private readonly IStreetService streetService;

        public StreetController(IStreetService streetService, IDeletableEntityRepository<Organization> organizationRepository)
            : base(organizationRepository)
        {
            this.streetService = streetService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(AllStreetsSearchFilterViewModel inputModel)
        {
            IEnumerable<StreetDataViewModel> allStreets =
                await this.streetService.GetAllStreetsAsync(inputModel);

            int allStreetsCount = await this.streetService.GetStreetsCountByFilterAsync(inputModel);
            AllStreetsSearchFilterViewModel viewModel = new AllStreetsSearchFilterViewModel
            {
                Streets = allStreets,
                SearchQuery = inputModel.SearchQuery,
                CurrentPage = inputModel.CurrentPage,
                TotalPages = (int)Math.Ceiling((double)allStreetsCount / inputModel.EntitiesPerPage!.Value)
            };

            return this.View(viewModel);
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
        [Authorize(Roles = AdministratorRoleName)]

        public async Task<IActionResult> Edit(string? id)
        {

            Guid streetGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(id, ref streetGuid);

            if (!isIdValid)
            {
                return this.RedirectToAction(nameof(Index));
            }

            EditStreetInputModel? formModel = await this.streetService
                .GetStreetForEditAsync(streetGuid);

            if (formModel == null)
            {
                return this.RedirectToAction(nameof(Index));
            }

            return this.View(formModel);
        }

        [HttpPost]
        [Authorize(Roles = AdministratorRoleName)]

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
        [Authorize(Roles = AdministratorRoleName)]

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
