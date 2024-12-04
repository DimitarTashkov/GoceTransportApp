using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoceTransportApp.Web.ViewModels.Schedules
{
    public class ScheduleDataViewModel
    {
        public string Id { get; set; } = null!;

        public string Day { get; set; } = null!;

        public string Departing { get; set; } = null!;

        public string Arriving { get; set; } = null!;

        public string FromCity { get; set; } = null!;

        public string ToCity { get; set; } = null!;

        public string OrganizationId { get; set; } = null!;
    }
}
