using GoceTransportApp.Data.Common.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoceTransportApp.Data.Models
{
    public class Report : BaseDeletableModel<Guid>
    {
        public Report()
        {
            this.Id = Guid.NewGuid();
        }

        [Required]
        public DayOfWeek Day { get; set; }

        [Required]
        public int TicketsSold { get; set; }

        [Required]
        public int CoursesCompleted { get; set; }

        [Required]
        public decimal Revenue { get; set; }

        public decimal? Expenses { get; set; }

        [Required]
        public Guid OrganizationId { get; set; }

        [ForeignKey(nameof(OrganizationId))]
        public Organization Organization { get; set; }
    }
}
