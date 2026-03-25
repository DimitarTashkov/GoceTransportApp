namespace GoceTransportApp.Web.Controllers
{
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using GoceTransportApp.Data.Common.Repositories;
    using GoceTransportApp.Data.Models;
    using GoceTransportApp.Services.Data.Organizations;
    using GoceTransportApp.Services.Data.Tickets;
    using GoceTransportApp.Web.ViewModels.Dashboard;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class DashboardController : BaseController
    {
        private readonly ITicketService ticketService;
        private readonly IOrganizationService organizationService;
        private readonly UserManager<ApplicationUser> userManager;

        public DashboardController(
            ITicketService ticketService,
            IOrganizationService organizationService,
            UserManager<ApplicationUser> userManager,
            IDeletableEntityRepository<Organization> organizationRepository)
            : base(organizationRepository)
        {
            this.ticketService = ticketService;
            this.organizationService = organizationService;
            this.userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await this.userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return this.RedirectToAction("Index", "Home");
            }

            var tickets = await this.ticketService.GetMyTicketsAsync(userId);
            var favorites = await this.organizationService.GetFavoriteOrganizationsByUserIdAsync(userId);
            var userOrgs = await this.organizationService.GetUserOrganizationsAsync(userId);

            var model = new UserDashboardViewModel
            {
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ProfilePictureUrl = user.ProfilePictureUrl,
                MembershipTier = user.MembershipTier,
                UpcomingTrips = tickets.Upcoming,
                PastTrips = tickets.Past,
                TotalUpcomingTrips = tickets.Upcoming.Count(),
                TotalPastTrips = tickets.Past.Count(),
                FavoriteOrganizations = favorites,
                TotalFavorites = favorites.Count(),
                TotalOrganizations = userOrgs.Count(),
            };

            return this.View(model);
        }
    }
}
