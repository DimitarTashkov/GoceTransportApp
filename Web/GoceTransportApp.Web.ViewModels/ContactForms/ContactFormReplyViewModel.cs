using System;
using System.ComponentModel.DataAnnotations;

namespace GoceTransportApp.Web.ViewModels.ContactForms
{
    public class ContactFormReplyViewModel
    {
        public Guid Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }

        public DateTime DateSubmitted { get; set; }

        [Required]
        [StringLength(2000, MinimumLength = 5)]
        public string ReplyText { get; set; }
    }
}
