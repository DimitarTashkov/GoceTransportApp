namespace GoceTransportApp.Web.Controllers
{
    using System.Diagnostics;
    using GoceTransportApp.Common;
    using GoceTransportApp.Web.ViewModels;

    using Microsoft.AspNetCore.Mvc;

    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return this.View();
        }

        public IActionResult Privacy()
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
