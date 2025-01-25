using GoceTransportApp.Web.ViewModels.ContactForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoceTransportApp.Web.ViewModels.Users
{
    public class AllUsersSearchFilterViewModel
    {
        public IEnumerable<AllUsersViewModel> Users { get; set; }

        public string SearchQuery { get; set; }

        public int? CurrentPage { get; set; } = 1;

        public int? EntitiesPerPage { get; set; } = 20;

        public int? TotalPages { get; set; }
    }
}
