using GoceTransportApp.Web.ViewModels.Organizations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoceTransportApp.Web.ViewModels.Schedules
{
    public class AllSchedulesSearchFilterViewModel
    {
        public IEnumerable<ScheduleDataViewModel> Schedules { get; set; }

        public DayOfWeek? DayFilter { get; set; }

        public TimeSpan? TimeFilter { get; set; }

        public int? CurrentPage { get; set; } = 1;

        public int? EntitiesPerPage { get; set; } = 20;

        public int? TotalPages { get; set; }
    }
}
