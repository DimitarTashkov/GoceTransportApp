namespace GoceTransportApp.Web.ViewModels.Dashboard
{
    using GoceTransportApp.Data.Models.Enumerations;
    using GoceTransportApp.Web.ViewModels.Organizations;
    using GoceTransportApp.Web.ViewModels.Tickets;
    using System.Collections.Generic;

    public class UserDashboardViewModel
    {
        public string UserName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? ProfilePictureUrl { get; set; }

        public MembershipTier MembershipTier { get; set; }

        public int TotalOrganizations { get; set; }

        public int TotalUpcomingTrips { get; set; }

        public int TotalPastTrips { get; set; }

        public int TotalFavorites { get; set; }

        public IEnumerable<MyTicketViewModel> UpcomingTrips { get; set; }
            = new List<MyTicketViewModel>();

        public IEnumerable<OrganizationDataViewModel> FavoriteOrganizations { get; set; }
            = new List<OrganizationDataViewModel>();

        public IEnumerable<MyTicketViewModel> PastTrips { get; set; }
            = new List<MyTicketViewModel>();
    }
}
