namespace GoceTransportApp.Services.Data.Notifications
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using GoceTransportApp.Web.ViewModels.Notifications;

    public interface INotificationService
    {
        Task CreateAsync(string receiverId, string content, string? link = null);

        Task<IEnumerable<NotificationViewModel>> GetUnreadAsync(string userId);

        Task MarkAsReadAsync(string id);
    }
}
