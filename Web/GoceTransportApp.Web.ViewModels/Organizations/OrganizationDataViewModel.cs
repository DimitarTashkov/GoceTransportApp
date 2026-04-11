using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoceTransportApp.Data.Models.Enumerations;

namespace GoceTransportApp.Web.ViewModels.Organizations
{
    public class OrganizationDataViewModel
    {
        public string Id { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string Address { get; set; }

        public string FounderId { get; set; } = null!;

        public string Founder { get; set; } = null!;

        public string? ImageUrl { get; set; }

        public MembershipTier FounderTier { get; set; } = MembershipTier.Free;
    }
}
