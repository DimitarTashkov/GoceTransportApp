namespace GoceTransportApp.Services.Data.Notifications
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using GoceTransportApp.Data.Common.Repositories;
    using GoceTransportApp.Data.Models;
    using GoceTransportApp.Web.ViewModels.Notifications;

    using Microsoft.EntityFrameworkCore;

    public class NotificationService : INotificationService
    {
        private readonly IDeletableEntityRepository<Notification> notificationRepository;

        public NotificationService(IDeletableEntityRepository<Notification> notificationRepository)
        {
            this.notificationRepository = notificationRepository;
        }

        public async Task CreateAsync(string receiverId, string content, string? link = null)
        {
            var notification = new Notification
            {
                ReceiverId = receiverId,
                Content = content,
                Link = link,
                IsRead = false,
            };

            await this.notificationRepository.AddAsync(notification);
            await this.notificationRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<NotificationViewModel>> GetUnreadAsync(string userId)
        {
            return await this.notificationRepository
                .GetAllAttached()
                .Where(n => n.ReceiverId == userId && !n.IsRead)
                .OrderByDescending(n => n.CreatedOn)
                .Select(n => new NotificationViewModel
                {
                    Id = n.Id.ToString(),
                    Content = n.Content,
                    Link = n.Link,
                    CreatedOn = n.CreatedOn,
                })
                .ToListAsync();
        }

        public async Task MarkAsReadAsync(string id)
        {
            if (!Guid.TryParse(id, out Guid guid))
            {
                return;
            }

            var notification = await this.notificationRepository.GetByIdAsync(guid);
            if (notification == null)
            {
                return;
            }

            notification.IsRead = true;
            await this.notificationRepository.UpdateAsync(notification);
            await this.notificationRepository.SaveChangesAsync();
        }
    }
}
