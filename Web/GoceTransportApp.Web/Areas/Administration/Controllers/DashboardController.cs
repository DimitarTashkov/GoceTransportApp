namespace GoceTransportApp.Web.Areas.Administration.Controllers
{
    using GoceTransportApp.Common;
    using GoceTransportApp.Services.Data;
    using GoceTransportApp.Web.ViewModels.Administration.Dashboard;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
    [Area(GlobalConstants.AdministratorArea)]
    public class DashboardController : Controller
    {
        public DashboardController()
        {

        }

        [HttpGet]
        public IActionResult Index()
        {
            return this.View();
        }
    }
}
