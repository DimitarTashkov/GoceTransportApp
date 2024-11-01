namespace GoceTransportApp.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using GoceTransportApp.Data.Common.Models;

    public class Organization : BaseDeletableModel<Guid>
    {
        public Organization()
        {
            this.Id = Guid.NewGuid();
        }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = null!;

        [MaxLength(50)]
        public string Address { get; set; }

        [Required]
        [MaxLength(50)]
        public string Founder { get; set; } = null!;

        [MaxLength(20)]
        public string Phone { get; set; }

        public HashSet<Driver> Drivers { get; set; } = new HashSet<Driver>();

        public HashSet<Ticket> Tickets { get; set; } = new HashSet<Ticket>();

        public List<Schedule> Schedules { get; set; } = new List<Schedule>();
        public List<Report> Reports { get; set; } = new List<Report>();
    }
}
