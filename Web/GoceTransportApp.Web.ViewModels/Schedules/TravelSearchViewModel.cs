using GoceTransportApp.Data.Models;
using System;
using System.Collections.Generic;

namespace GoceTransportApp.Web.ViewModels.Schedules
{
    public class TravelSearchViewModel
    {
        public string? FromCityId { get; set; }

        public string? ToCityId { get; set; }

        public DateTime? DepartureDate { get; set; }

        public IEnumerable<City> Cities { get; set; } = new List<City>();

        public IEnumerable<ScheduleDataViewModel> Results { get; set; } = new List<ScheduleDataViewModel>();
    }
}
