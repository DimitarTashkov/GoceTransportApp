// ReSharper disable VirtualMemberCallInConstructor
namespace GoceTransportApp.Data.Models
{
    using GoceTransportApp.Data.Common.Models;
    using Microsoft.AspNetCore.Identity;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using static GoceTransportApp.Common.EntityValidationConstants.UserConstants;
    using static GoceTransportApp.Common.GlobalConstants;

    public class ApplicationUser : IdentityUser<string>, IAuditInfo, IDeletableEntity
    {
        public ApplicationUser()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Roles = new HashSet<IdentityUserRole<string>>();
            this.Claims = new HashSet<IdentityUserClaim<string>>();
            this.Logins = new HashSet<IdentityUserLogin<string>>();
        }

        // Audit info
        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        // Deletable entity
        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }

        public virtual ICollection<IdentityUserRole<string>> Roles { get; set; }

        public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; }

        public virtual ICollection<IdentityUserLogin<string>> Logins { get; set; }

        // Additional information
        [Required]
        [MaxLength(MaxNameLength)]
        public string FirstName { get; set; } = null!;

        [Required]
        [MaxLength(MaxNameLength)]
        public string LastName { get; set; } = null!;

        [Required]
        [DefaultValue(DefaultProfileImageUrl)]
        public string ProfilePictureUrl { get; set; } = null!;

        // TODO: Get user city via geolocation and set it then from enumeration
        [Required]
        public string City { get; set; }

        public HashSet<Ticket> UserTickets { get; set; }
        = new HashSet<Ticket>();

        public HashSet<Organization> UserOrganizations { get; set; }
        = new HashSet<Organization>();

        public HashSet<Message> UserMessages { get; set; }
        = new HashSet<Message>();
    }
}
