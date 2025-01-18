using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoceTransportApp.Web.ViewModels.ContactForms
{
    public class ContactFormDataViewModel
    {
        public Guid Id { get; set; }

        public string Username { get; set; }

        public string Title { get; set; }

        public DateTime DateSubmitted { get; set; }
    }
}
