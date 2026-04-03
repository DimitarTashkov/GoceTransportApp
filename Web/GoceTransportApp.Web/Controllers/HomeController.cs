namespace GoceTransportApp.Web.Controllers
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using GoceTransportApp.Common;
    using GoceTransportApp.Services.Data.Cities;
    using GoceTransportApp.Services.Data.Schedules;
    using GoceTransportApp.Web.ViewModels;
    using GoceTransportApp.Web.ViewModels.Schedules;

    using Microsoft.AspNetCore.Mvc;

    public class HomeController : Controller
    {
        private readonly ICityService cityService;
        private readonly IScheduleService scheduleService;

        public HomeController(ICityService cityService, IScheduleService scheduleService)
        {
            this.cityService = cityService;
            this.scheduleService = scheduleService;
        }

        public async Task<IActionResult> Index()
        {
            var cities = await this.cityService.GetAllCitiesForDropDownsAsync();
            var model = new TravelSearchViewModel { Cities = cities };
            return this.View(model);
        }

        public IActionResult Privacy()
        {
            return this.View();
        }

        public IActionResult AboutUs()
        {
            return this.View();
        }

        public IActionResult Terms()
        {
            return this.View();
        }

        [HttpGet]
        public async Task<IActionResult> NextDepartures(string fromCityId)
        {
            if (!Guid.TryParse(fromCityId, out Guid cityGuid))
            {
                return PartialView("_NextDeparturesPartial", Array.Empty<NextDepartureViewModel>());
            }

            var departures = await this.scheduleService.GetNextDeparturesAsync(cityGuid, limit: 5);
            return PartialView("_NextDeparturesPartial", departures);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? statusCode = null)
        {
            if (statusCode == StatusCodes.InternalServerError)
            {
                return this.Redirect($"/Error/{StatusCodes.InternalServerError}");
            }

            return this.Redirect($"/Error/{StatusCodes.NotFound}");
        }
    }
}
