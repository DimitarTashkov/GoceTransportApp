using GoceTransportApp.Services.Data.Cities;
using GoceTransportApp.Services.Data.Routes;
using GoceTransportApp.Web.ViewModels.Cities;
using GoceTransportApp.Web.ViewModels.Routes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

using static GoceTransportApp.Common.ErrorMessages.RouteMessages;

namespace GoceTransportApp.Web.Controllers
{
    [Authorize]
    public class RouteController : BaseController
    {
        private readonly IRouteService routeService;

        public RouteController(IRouteService routeService)
        {
            this.routeService = routeService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await routeService.GetAllRoutes();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            RouteInputModel model = new RouteInputModel();
            // TODO: Check if the user has permissions to create routes

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(RouteInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await routeService.CreateAsync(model);

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

            EditRouteInputModel? formModel = await this.routeService
                .GetRouteForEdit(cityGuid);

            if (formModel == null)
            {
                return this.RedirectToAction(nameof(Index));
            }

            return this.View(formModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditRouteInputModel formModel)
        {
            if (!ModelState.IsValid)
            {
                return this.View(formModel);
            }

            bool isUpdated = await this.routeService
                .EditRouteAsync(formModel);

            if (!isUpdated)
            {
                ModelState.AddModelError(nameof(RouteEditFailed), RouteEditFailed);

                return this.View(formModel);
            }

            return this.RedirectToAction(nameof(Index));
        }
    }
}
