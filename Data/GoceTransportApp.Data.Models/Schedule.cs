namespace GoceTransportApp.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using GoceTransportApp.Data.Common.Models;

    public class Schedule : BaseDeletableModel<Guid>
    {
        public Schedule()
        {
            this.Id = Guid.NewGuid();
        }

        [Required]
        public DayOfWeek Day { get; set; }

        [Required]
        public DateTime Departure { get; set; }

        [Required]
        public DateTime Arrival { get; set; }

        [Required]
        public Guid RouteId { get; set; }

        [ForeignKey(nameof(RouteId))]
        public Route Route { get; set; } = null!;
    }
}
