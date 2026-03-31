using System;

namespace GoceTransportApp.Web.ViewModels.Schedules
{
    public class NextDepartureViewModel
    {
        public string ScheduleId { get; set; } = null!;
        public string FromCity { get; set; } = null!;
        public string ToCity { get; set; } = null!;
        public string OrganizationName { get; set; } = null!;
        public string OrganizationId { get; set; } = null!;
        public DateTime Departure { get; set; }
        public string DepartureTime { get; set; } = null!;
        public string MinutesUntil { get; set; } = null!;
        public bool IsToday { get; set; }
    }
}
