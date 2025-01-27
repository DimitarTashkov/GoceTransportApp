using GoceTransportApp.Data.Common.Repositories;
using GoceTransportApp.Data.Models;
using GoceTransportApp.Services.Data.Cities;
using GoceTransportApp.Services.Data.Organizations;
using GoceTransportApp.Services.Data.Routes;
using GoceTransportApp.Web.ViewModels.Cities;
using GoceTransportApp.Web.ViewModels.Routes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

using static GoceTransportApp.Common.ResultMessages.RouteMessages;
using static GoceTransportApp.Common.ResultMessages.GeneralMessages;
using static GoceTransportApp.Common.GlobalConstants;
using System.Net.Mail;

namespace GoceTransportApp.Web.Controllers
{
    [Authorize]
    public class RouteController : BaseController
    {
        private readonly IRouteService routeService;

        public RouteController(IRouteService routeService, IDeletableEntityRepository<Organization> organizationRepository)
            : base(organizationRepository)
        {
            this.routeService = routeService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index(AllRoutesSearchFilterViewModel inputModel)
        {
            IEnumerable<RouteDataViewModel> allRoutes = await routeService.GetAllRoutesAsync(inputModel);

            int allRoutesCount = await routeService.GetRoutesCountByFilterAsync(inputModel);

            AllRoutesSearchFilterViewModel viewModel = new AllRoutesSearchFilterViewModel
            {
                Routes = allRoutes,
                SearchQuery = inputModel.SearchQuery,
                DepartingCityFilter = inputModel.DepartingCityFilter,
                ArrivingCityFilter = inputModel.ArrivingCityFilter,
                CurrentPage = inputModel.CurrentPage,
                EntitiesPerPage = inputModel.EntitiesPerPage,
                TotalPages = (int)Math.Ceiling((double)allRoutesCount / inputModel.EntitiesPerPage.Value)
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Create(string organizationId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!await this.HasUserCreatedOrganizationAsync(userId, organizationId) && !User.IsInRole(AdministratorRoleName))
            {
                return RedirectToAction("Routes", "Organization", new { organizationId = organizationId });
            }

            RouteInputModel model = new RouteInputModel();
            model.OrganizationId = organizationId;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(RouteInputModel model)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!await this.HasUserCreatedOrganizationAsync(userId, model.OrganizationId) && !User.IsInRole(AdministratorRoleName))
            {
                return RedirectToAction("Routes", "Organization", new { organizationId = model.OrganizationId });
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await routeService.CreateAsync(model);

            return RedirectToAction("Routes", "Organization", new { organizationId = model.OrganizationId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string? id, string organizationId)
        {

            Guid routeGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(id, ref routeGuid);

            if (!isIdValid)
            {
                return RedirectToAction("Routes", "Organization", new { organizationId = organizationId });

            }

            EditRouteInputModel? formModel = await this.routeService
                .GetRouteForEditAsync(routeGuid);


            if (formModel == null)
            {
                return RedirectToAction("Routes", "Organization", new { organizationId = formModel.OrganizationId });

            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!await this.HasUserCreatedOrganizationAsync(userId, formModel.OrganizationId) && !User.IsInRole(AdministratorRoleName))
            {
                return RedirectToAction("Routes", "Organization", new { organizationId = formModel.OrganizationId });
            }

            return this.View(formModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditRouteInputModel formModel)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!await this.HasUserCreatedOrganizationAsync(userId, formModel.OrganizationId) && !User.IsInRole(AdministratorRoleName))
            {
                return RedirectToAction("Routes", "Organization", new { organizationId = formModel.OrganizationId });
            }

            if (!ModelState.IsValid)
            {
                return RedirectToAction("Routes", "Organization", new { organizationId = formModel.OrganizationId });
            }

            bool isUpdated = await this.routeService
                .EditRouteAsync(formModel);

            if (!isUpdated)
            {
                ModelState.AddModelError(nameof(FailMessage), FailMessage);

                return this.View(formModel);
            }

            return RedirectToAction("Routes", "Organization", new { organizationId = formModel.OrganizationId });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string? id, string organizationId)
        {
            Guid routeGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(id, ref routeGuid);
            if (!isIdValid)
            {
                return RedirectToAction("Routes", "Organization", new { organizationId = organizationId });
            }

            RemoveRouteViewModel? model = await routeService
                .GetRouteForDeletionAsync(routeGuid);

            if (model == null)
            {
                TempData[nameof(FailMessage)] = FailMessage;

                return RedirectToAction("Routes", "Organization", new { organizationId = model.OrganizationId });

            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!await this.HasUserCreatedOrganizationAsync(userId, model.OrganizationId) && !User.IsInRole(AdministratorRoleName))
            {
                return RedirectToAction("Routes", "Organization",new { organizationId = model.OrganizationId });
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(RemoveRouteViewModel formModel)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!await this.HasUserCreatedOrganizationAsync(userId, formModel.OrganizationId) && !User.IsInRole(AdministratorRoleName))
            {
                return RedirectToAction("Routes", "Organization", new { organizationId = formModel.OrganizationId });
            }

            if (!ModelState.IsValid)
            {
                return this.View(formModel);
            }

            bool isDeleted = await this.routeService
                .DeleteRouteAsync(formModel);

            if (!isDeleted)
            {
                ModelState.AddModelError(nameof(FailMessage), FailMessage);

                return this.View(formModel);
            }

            return RedirectToAction("Routes", "Organization", new { organizationId = formModel.OrganizationId });

        }

        //[HttpPost]
        //public async Task<IActionResult> Archive(RemoveRouteViewModel formModel)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return this.View(formModel);
        //    }

        //    bool isArchived = await this.routeService
        //        .ArchiveRouteAsync(formModel);

        //    if (!isArchived)
        //    {
        //        ModelState.AddModelError(nameof(FailMessage), FailMessage);

        //        return this.View(formModel);
        //    }

        //    return RedirectToAction("Routes", "Organization", new { organizationId = formModel.OrganizationId });
        //}

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(string? id, string organizationId)
        {
            Guid routeGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(id, ref routeGuid);
            if (!isIdValid)
            {
                return RedirectToAction("Routes", "Organization", new { organizationId = organizationId });
            }

            RouteDetailsViewModel? model = await routeService
                .GetRouteInformationAsync(routeGuid);

            if (model == null)
            {
                TempData[nameof(FailMessage)] = FailMessage;

                return RedirectToAction("Routes", "Organization", new { organizationId = organizationId});
            }

            return View(model);
        }
    }
}
