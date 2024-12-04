﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoceTransportApp.Web.ViewModels.Tickets
{
    public class TicketDataViewModel
    {
        public string Id { get; set; } = null!;

        public string DepartingTime { get; set; } = null!;

        public string ArrivingTime { get; set; } = null!;

        public string Price { get; set; } = null!;

        public string FromCity { get; set; } = null!;

        public string ToCity { get; set; } = null!;

        public string OrganizationId { get; set; } = null!;

    }
}
