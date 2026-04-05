namespace GoceTransportApp.Web.Controllers
{
    using System.Security.Claims;
    using System.Threading.Tasks;

    using GoceTransportApp.Data.Common.Repositories;
    using GoceTransportApp.Data.Models;
    using GoceTransportApp.Services.Data.Notifications;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class NotificationController : BaseController
    {
        private readonly INotificationService notificationService;

        public NotificationController(
            INotificationService notificationService,
            IDeletableEntityRepository<Organization> organizationRepository)
            : base(organizationRepository)
        {
            this.notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyNotifications()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var notifications = await this.notificationService.GetUnreadAsync(userId);
            return Json(notifications);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> MarkAsRead(string id)
        {
            await this.notificationService.MarkAsReadAsync(id);
            return Json(new { success = true });
        }
    }
}
