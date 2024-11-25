using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoceTransportApp.Web.ViewModels.Drivers
{
    public class RemoveDriverViewModel
    {
        public string Id { get; set; } = null!;

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string OrganizationId { get; set; } = null!;
    }
}
