using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoceTransportApp.Web.ViewModels.Users
{
    public class AllUsersViewModel
    {
        public string Id { get; set; } = null!;

        public string? Username { get; set; }

        public string? Email { get; set; }

        public IEnumerable<string> Roles { get; set; } = null!;
    }
}
