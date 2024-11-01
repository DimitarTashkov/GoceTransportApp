using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoceTransportApp.Data.Models.Enumerations
{
    public enum VehicleStatus
    {
        Busy = 1,
        Available = 2,
        [Display(Name = "Under Maintenance")]
        UnderMaintenance = 3,
    }
}
