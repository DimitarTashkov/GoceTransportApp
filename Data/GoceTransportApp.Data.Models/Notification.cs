namespace GoceTransportApp.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using GoceTransportApp.Data.Common.Models;

    public class Notification : BaseDeletableModel<Guid>
    {
        public Notification()
        {
            this.Id = Guid.NewGuid();
        }

        [Required]
        public string ReceiverId { get; set; } = null!;

        [ForeignKey(nameof(ReceiverId))]
        public ApplicationUser Receiver { get; set; } = null!;

        [Required]
        [MaxLength(500)]
        public string Content { get; set; } = null!;

        [MaxLength(500)]
        public string? Link { get; set; }

        public bool IsRead { get; set; }
    }
}
