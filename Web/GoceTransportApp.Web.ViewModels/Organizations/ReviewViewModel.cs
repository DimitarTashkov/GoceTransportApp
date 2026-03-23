namespace GoceTransportApp.Web.ViewModels.Organizations
{
    using System;

    public class ReviewViewModel
    {
        public string PassengerName { get; set; } = null!;

        public int Rating { get; set; }

        public string? Comment { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
