using GoceTransportApp.Data.Models;
using GoceTransportApp.Web.ViewModels.Organizations;
using System;
using System.Collections.Generic;

namespace GoceTransportApp.Web.ViewModels.Schedules
{
    public class AllSchedulesSearchFilterViewModel
    {
        // ── Results ──────────────────────────────────────────────────────────
        public IEnumerable<ScheduleDataViewModel> Schedules { get; set; }

        // ── Filters ──────────────────────────────────────────────────────────
        public string? FromCityId { get; set; }
        public string? ToCityId { get; set; }
        public DayOfWeek? DayFilter { get; set; }
        public TimeSpan? TimeFilter { get; set; }

        /// <summary>Single organisation chosen by the user from the filter panel.</summary>
        public string? OrganizationId { get; set; }

        // ── Sorting ───────────────────────────────────────────────────────────
        public ScheduleSorting SortBy { get; set; } = ScheduleSorting.Default;

        // ── Pagination ────────────────────────────────────────────────────────
        public int? CurrentPage { get; set; } = 1;
        public int? EntitiesPerPage { get; set; } = 10;
        public int? TotalPages { get; set; }

        // ── Security filter (set server-side, not from user input) ─────────────
        public List<Guid> OrganizationFilter { get; set; } = new List<Guid>();

        // ── Dropdown data ─────────────────────────────────────────────────────
        public IEnumerable<City> AvailableCities { get; set; } = Array.Empty<City>();
        public IEnumerable<OrganizationDataViewModel> AvailableOrganizations { get; set; } = Array.Empty<OrganizationDataViewModel>();
    }
}
