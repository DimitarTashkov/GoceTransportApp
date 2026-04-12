namespace GoceTransportApp.Services.Messaging
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class NullMessageSender : IEmailSender
    {
        public Task SendEmailAsync(
            string from,
            string fromName,
            string to,
            string subject,
            string htmlContent,
            IEnumerable<EmailAttachment> attachments = null)
        {
            Console.WriteLine("=====================================================");
            Console.WriteLine("[NullMessageSender] Email not sent (no SMTP configured).");
            Console.WriteLine($"  To:      {to}");
            Console.WriteLine($"  Subject: {subject}");
            Console.WriteLine($"  Body:    {htmlContent}");
            Console.WriteLine("=====================================================");

            return Task.CompletedTask;
        }
    }
}
