using System.Collections.Generic;

namespace GoceTransportApp.Web.ViewModels.Schedules
{
    public class PassengerListViewModel
    {
        public string ScheduleId { get; set; } = null!;

        public string OrganizationId { get; set; } = null!;

        public string FromCity { get; set; } = null!;

        public string ToCity { get; set; } = null!;

        public string Day { get; set; } = null!;

        public string DepartingTime { get; set; } = null!;

        public IEnumerable<PassengerViewModel> Passengers { get; set; } = new List<PassengerViewModel>();
    }
}
