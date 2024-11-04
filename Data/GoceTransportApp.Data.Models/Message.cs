using System;
using GoceTransportApp.Data.Common.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using static GoceTransportApp.Common.EntityValidationConstants.MessageConstants;

namespace GoceTransportApp.Data.Models
{
    public class Message : BaseModel<Guid>
    {
        public Message()
        {
            this.Id = Guid.NewGuid();
        }

        [Required]
        [MaxLength(MaxTitleLength)]
        public string Title { get; set; } = null!;

        [Required]
        [MaxLength(MaxContentLength)]
        public string Content { get; set; } = null!;

        [Required]
        public string SenderId { get; set; }

        [ForeignKey(nameof(SenderId))]
        public ApplicationUser Sender { get; set; }

        [Required]
        public Guid OrganizationId { get; set; }

        [ForeignKey(nameof(OrganizationId))]
        public Organization Organization { get; set; }
    }
}
