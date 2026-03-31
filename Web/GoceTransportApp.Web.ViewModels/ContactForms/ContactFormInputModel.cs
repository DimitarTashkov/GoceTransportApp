using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static GoceTransportApp.Common.EntityValidationConstants.MessageConstants;

namespace GoceTransportApp.Web.ViewModels.ContactForms
{
    public class ContactFormInputModel
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(200)]
        public string Email { get; set; }

        [Required]
        [StringLength(MaxTitleLength, MinimumLength = MinTitleLength)]
        public string Title { get; set; }

        [Required]
        [StringLength(MaxContentLength, MinimumLength = MinContentLength)]
        public string Message { get; set; }
    }
}
