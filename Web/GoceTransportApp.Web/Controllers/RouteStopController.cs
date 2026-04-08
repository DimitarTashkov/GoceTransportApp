namespace GoceTransportApp.Web.Controllers
{
    using GoceTransportApp.Data.Common.Repositories;
    using GoceTransportApp.Data.Models;
    using GoceTransportApp.Services.Data.RouteStops;
    using GoceTransportApp.Web.ViewModels.Routes;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using static GoceTransportApp.Common.GlobalConstants;
    using static GoceTransportApp.Common.ResultMessages.GeneralMessages;

    [Authorize]
    public class RouteStopController : BaseController
    {
        private readonly IRouteStopService routeStopService;

        public RouteStopController(
            IRouteStopService routeStopService,
            IDeletableEntityRepository<Organization> organizationRepository)
            : base(organizationRepository)
        {
            this.routeStopService = routeStopService;
        }

        [HttpGet]
        public async Task<IActionResult> Manage(string routeId, string organizationId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!await this.HasUserCreatedOrganizationAsync(userId, organizationId)
                && !User.IsInRole(AdministratorRoleName))
            {
                return RedirectToAction("Details", "Organization", new { id = organizationId });
            }

            Guid routeGuid = Guid.Empty;
            if (!this.IsGuidValid(routeId, ref routeGuid))
            {
                return RedirectToAction("Details", "Organization", new { id = organizationId });
            }

            var model = new RouteStopInputModel
            {
                RouteId = routeId,
                OrganizationId = organizationId,
                ExistingStops = await this.routeStopService.GetStopsForRouteAsync(routeGuid),
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(RouteStopInputModel model)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!await this.HasUserCreatedOrganizationAsync(userId, model.OrganizationId)
                && !User.IsInRole(AdministratorRoleName))
            {
                return RedirectToAction("Details", "Organization", new { id = model.OrganizationId });
            }

            if (!ModelState.IsValid)
            {
                Guid routeGuid = Guid.Parse(model.RouteId);
                model.ExistingStops = await this.routeStopService.GetStopsForRouteAsync(routeGuid);
                return View("Manage", model);
            }

            await this.routeStopService.AddStopAsync(model);
            TempData[nameof(SuccessMessage)] = SuccessMessage;

            return RedirectToAction(nameof(Manage), new { routeId = model.RouteId, organizationId = model.OrganizationId });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string stopId, string routeId, string organizationId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!await this.HasUserCreatedOrganizationAsync(userId, organizationId)
                && !User.IsInRole(AdministratorRoleName))
            {
                return RedirectToAction("Details", "Organization", new { id = organizationId });
            }

            Guid stopGuid = Guid.Empty;
            if (!this.IsGuidValid(stopId, ref stopGuid))
            {
                TempData[nameof(FailMessage)] = FailMessage;
                return RedirectToAction(nameof(Manage), new { routeId, organizationId });
            }

            await this.routeStopService.RemoveStopAsync(stopGuid);
            TempData[nameof(SuccessMessage)] = SuccessMessage;

            return RedirectToAction(nameof(Manage), new { routeId, organizationId });
        }
    }
}
