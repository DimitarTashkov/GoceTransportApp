using GoceTransportApp.Web.ViewModels.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoceTransportApp.Web.ViewModels.Schedules
{
    public class SchedulesForOrganizationViewModel
    {
        public string OrganizationId { get; set; }
        public IEnumerable<ScheduleDataViewModel> Schedules { get; set; }
    }
}
