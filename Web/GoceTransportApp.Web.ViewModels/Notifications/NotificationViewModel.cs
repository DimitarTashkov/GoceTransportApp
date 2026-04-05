namespace GoceTransportApp.Web.ViewModels.Notifications
{
    using System;

    public class NotificationViewModel
    {
        public string Id { get; set; } = null!;

        public string Content { get; set; } = null!;

        public string? Link { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
