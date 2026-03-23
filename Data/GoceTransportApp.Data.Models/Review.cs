namespace GoceTransportApp.Data.Models
{
    using GoceTransportApp.Data.Common.Models;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Review : BaseModel<Guid>
    {
        public Review()
        {
            this.Id = Guid.NewGuid();
        }

        [Required]
        public Guid OrganizationId { get; set; }

        [ForeignKey(nameof(OrganizationId))]
        public Organization Organization { get; set; } = null!;

        [Required]
        public string PassengerId { get; set; } = null!;

        [ForeignKey(nameof(PassengerId))]
        public ApplicationUser Passenger { get; set; } = null!;

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(500)]
        public string? Comment { get; set; }
    }
}
