namespace GoceTransportApp.Data.Models
{
    using System;

    public class UserFavoriteOrganization
    {
        public string UserId { get; set; } = null!;

        public ApplicationUser User { get; set; } = null!;

        public Guid OrganizationId { get; set; }

        public Organization Organization { get; set; } = null!;

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}
