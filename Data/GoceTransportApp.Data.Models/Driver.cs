namespace GoceTransportApp.Data.Models
{
    using GoceTransportApp.Data.Common.Models;
    using GoceTransportApp.Data.Models.Enumerations;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using static GoceTransportApp.Common.EntityValidationConstants.DriverConstants;

    public class Driver : BaseModel<Guid>
    {
        public Driver()
        {
            this.Id = Guid.NewGuid();
        }

        [Required]
        [MaxLength(MaxNameLength)]
        public string FirstName { get; set; } = null!;

        [Required]
        [MaxLength(MaxNameLength)]
        public string LastName { get; set; } = null!;

        [Range(MinAgeLength, MaxAgeLength)]
        public int? Age { get; set; }

        [Required]
        public DriverExperience Experience { get; set; }

        [Required]
        public Guid OrganizationId { get; set; }

        [ForeignKey(nameof(OrganizationId))]
        public Organization Organization { get; set; } = null!;
    }
}
