using GoceTransportApp.Data.Common.Repositories;
using GoceTransportApp.Data.Models;
using GoceTransportApp.Data.Models.Enumerations;
using GoceTransportApp.Services.Data.Cities;
using GoceTransportApp.Services.Data.Organizations;
using GoceTransportApp.Services.Data.Routes;
using GoceTransportApp.Web.ViewModels.Cities;
using GoceTransportApp.Web.ViewModels.Routes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

using static GoceTransportApp.Common.ResultMessages.RouteMessages;
using static GoceTransportApp.Common.ResultMessages.GeneralMessages;
using static GoceTransportApp.Common.GlobalConstants;

namespace GoceTransportApp.Web.Controllers
{
    [Authorize]
    public class RouteController : BaseController
    {
        private readonly IRouteService routeService;
        private readonly UserManager<ApplicationUser> userManager;

        public RouteController(
            IRouteService routeService,
            IDeletableEntityRepository<Organization> organizationRepository,
            UserManager<ApplicationUser> userManager)
            : base(organizationRepository)
        {
            this.routeService = routeService;
            this.userManager = userManager;
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
                return RedirectToAction("UserOrganizations", "Organization");
            }

            var currentUser = await this.userManager.FindByIdAsync(userId);
            bool isAdmin = User.IsInRole(AdministratorRoleName);
            ViewBag.IsPremiumUser = isAdmin ||
                (currentUser != null && currentUser.MembershipTier != MembershipTier.Free);

            if (!isAdmin && currentUser != null)
            {
                int limit = await GetEffectiveRouteLimitAsync(currentUser.MembershipTier, organizationId);
                int count = await routeService.GetRoutesCountForOrganizationAsync(Guid.Parse(organizationId));
                ViewBag.RouteLimitReached = count >= limit;
                ViewBag.RouteLimit = limit == int.MaxValue ? "∞" : limit.ToString();
                ViewBag.RouteCount = count;
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
                return RedirectToAction("Details", "Organization", new { id = model.OrganizationId });
            }

            // Enforce per-organization route limit
            if (!User.IsInRole(AdministratorRoleName))
            {
                var currentUser = await this.userManager.FindByIdAsync(userId);
                if (currentUser != null)
                {
                    int limit = await GetEffectiveRouteLimitAsync(currentUser.MembershipTier, model.OrganizationId);
                    if (limit < int.MaxValue)
                    {
                        int count = await routeService.GetRoutesCountForOrganizationAsync(Guid.Parse(model.OrganizationId));
                        if (count >= limit)
                        {
                            TempData[TempDataKeys.UpgradeReason] = $"Route limit reached ({count}/{limit}) on the {currentUser.MembershipTier} plan. Upgrade to add more routes.";
                            return RedirectToAction("Upgrade", "Organization");
                        }
                    }

                    // Gate map coordinates for Free-tier users
                    if (currentUser.MembershipTier == MembershipTier.Free)
                    {
                        model.FromLatitude = null;
                        model.FromLongitude = null;
                        model.ToLatitude = null;
                        model.ToLongitude = null;
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await routeService.CreateAsync(model);
            TempData[nameof(SuccessMessage)] = SuccessMessage;

            return RedirectToAction("Details", "Organization", new { id = model.OrganizationId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string? id, string organizationId)
        {

            Guid routeGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(id, ref routeGuid);

            if (!isIdValid)
            {
                return RedirectToAction("Details", "Organization", new { id = organizationId });

            }

            EditRouteInputModel? formModel = await this.routeService
                .GetRouteForEditAsync(routeGuid);


            if (formModel == null)
            {
                return RedirectToAction("Details", "Organization", new { id = formModel.OrganizationId });

            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!await this.HasUserCreatedOrganizationAsync(userId, formModel.OrganizationId) && !User.IsInRole(AdministratorRoleName))
            {
                return RedirectToAction("Details", "Organization", new { id = formModel.OrganizationId });
            }

            var currentUser = await this.userManager.FindByIdAsync(userId);
            ViewBag.IsPremiumUser = User.IsInRole(AdministratorRoleName) ||
                (currentUser != null && currentUser.MembershipTier != MembershipTier.Free);

            return this.View(formModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditRouteInputModel formModel)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!await this.HasUserCreatedOrganizationAsync(userId, formModel.OrganizationId) && !User.IsInRole(AdministratorRoleName))
            {
                return RedirectToAction("Details", "Organization", new { id = formModel.OrganizationId });
            }

            // Gate map coordinates for Free-tier users
            if (!User.IsInRole(AdministratorRoleName))
            {
                var currentUser = await this.userManager.FindByIdAsync(userId);
                if (currentUser != null && currentUser.MembershipTier == MembershipTier.Free)
                {
                    formModel.FromLatitude = null;
                    formModel.FromLongitude = null;
                    formModel.ToLatitude = null;
                    formModel.ToLongitude = null;
                }
            }

            if (!ModelState.IsValid)
            {
                return RedirectToAction("Details", "Organization", new { id = formModel.OrganizationId });
            }

            bool isUpdated = await this.routeService
                .EditRouteAsync(formModel);

            if (!isUpdated)
            {
                ModelState.AddModelError(nameof(FailMessage), FailMessage);

                return this.View(formModel);
            }

            TempData[nameof(SuccessMessage)] = SuccessMessage;

            return RedirectToAction("Details", "Organization", new { id = formModel.OrganizationId });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string? id, string organizationId)
        {
            Guid routeGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(id, ref routeGuid);
            if (!isIdValid)
            {
                return RedirectToAction("Details", "Organization", new { id = organizationId });
            }

            RemoveRouteViewModel? model = await routeService
                .GetRouteForDeletionAsync(routeGuid);

            if (model == null)
            {
                TempData[nameof(FailMessage)] = FailMessage;

                return RedirectToAction("Details", "Organization", new { id = model.OrganizationId });

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
                return RedirectToAction("Details", "Organization", new { id = formModel.OrganizationId });
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

            TempData[nameof(SuccessMessage)] = SuccessMessage;

            return RedirectToAction("Details", "Organization", new { id = formModel.OrganizationId });

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

        //    return RedirectToAction("Details", "Organization", new { id = formModel.OrganizationId });
        //}

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(string? id, string organizationId)
        {
            Guid routeGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(id, ref routeGuid);
            if (!isIdValid)
            {
                return RedirectToAction("Details", "Organization", new { id = organizationId });
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
