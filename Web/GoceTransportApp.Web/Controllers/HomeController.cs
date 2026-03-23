namespace GoceTransportApp.Web.Controllers
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using GoceTransportApp.Common;
    using GoceTransportApp.Services.Data.Cities;
    using GoceTransportApp.Web.ViewModels;
    using GoceTransportApp.Web.ViewModels.Schedules;

    using Microsoft.AspNetCore.Mvc;

    public class HomeController : Controller
    {
        private readonly ICityService cityService;

        public HomeController(ICityService cityService)
        {
            this.cityService = cityService;
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
