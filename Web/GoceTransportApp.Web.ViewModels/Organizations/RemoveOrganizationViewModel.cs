using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoceTransportApp.Web.ViewModels.Organizations
{
    public class RemoveOrganizationViewModel
    {
        public string Id { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string Address { get; set; }

        public string ImageUrl { get; set; } = null!;

        public string FounderId { get; set; } = null!;

        public string FounderName { get; set; } = null!;
    }
}
