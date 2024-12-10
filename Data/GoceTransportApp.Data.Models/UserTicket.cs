using GoceTransportApp.Data.Common.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoceTransportApp.Data.Models
{
    [PrimaryKey(nameof(CustomerId), nameof(TicketId))]
    public class UserTicket : IDeletableEntity
    {
        [Required]
        public string CustomerId { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public ApplicationUser Customer { get; set; }

        [Required]
        public Guid TicketId { get; set; }

        [ForeignKey(nameof(TicketId))]
        public Ticket Ticket { get; set; }

        [Required]
        public int AvailableTickets { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}
