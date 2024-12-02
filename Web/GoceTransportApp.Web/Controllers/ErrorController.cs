using GoceTransportApp.Common;
using GoceTransportApp.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;

namespace GoceTransportApp.Web.Controllers
{
    public class ErrorController : BaseController
    {
        [Route("/Error/500")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult InternalServerError()
        {
            var errorViewModel = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier,
                StatusCode = StatusCodes.InternalServerError
            };

            return this.View(errorViewModel);
        }

        [Route("/Error/404")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult NotFoundError()
        {
            var errorViewModel = new ErrorViewModel();

            errorViewModel.RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier;
            errorViewModel.StatusCode = StatusCodes.NotFound;

            return this.View(errorViewModel);
        }
    }
}
