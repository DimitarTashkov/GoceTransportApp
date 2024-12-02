namespace GoceTransportApp.Web.Areas.Administration.Controllers
{
    using GoceTransportApp.Common;
    using GoceTransportApp.Web.Controllers;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
    [Area("Administration")]
    public class UserAdministrationController : BaseController
    {

    }
}
