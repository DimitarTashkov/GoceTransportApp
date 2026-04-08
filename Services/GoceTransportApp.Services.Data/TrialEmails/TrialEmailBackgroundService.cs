namespace GoceTransportApp.Services.Data.TrialEmails
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using GoceTransportApp.Data;
    using GoceTransportApp.Services.Messaging;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Runs once every 24 hours and sends three automated email campaigns
    /// to organization founders whose Pro trial is expiring:
    ///   – 7 days left  → urgency nudge
    ///   – 2 days left  → FOMO nudge
    ///   – just expired → "what you lost" summary
    ///
    /// Flags on Organization (TrialDay7EmailSent / TrialDay2EmailSent /
    /// TrialExpiredEmailSent) prevent duplicate sends on service restarts.
    /// </summary>
    public class TrialEmailBackgroundService : BackgroundService
    {
        private static readonly TimeSpan Interval = TimeSpan.FromHours(24);

        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<TrialEmailBackgroundService> logger;

        public TrialEmailBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<TrialEmailBackgroundService> logger)
        {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Run once immediately on startup, then every 24 hours
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessTrialEmailsAsync();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "TrialEmailBackgroundService encountered an error.");
                }

                await Task.Delay(Interval, stoppingToken);
            }
        }

        private async Task ProcessTrialEmailsAsync()
        {
            using var scope = serviceProvider.CreateScope();
            var db            = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var emailSender   = scope.ServiceProvider.GetRequiredService<IEmailSender>();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            string senderEmail = configuration["SendGrid:SenderEmail"] ?? "noreply@gocetransport.app";
            string senderName  = configuration["SendGrid:SenderName"]  ?? "GoceTransport";

            var now = DateTime.UtcNow;

            var trialOrgs = await db.Organizations
                .Include(o => o.Founder)
                .Where(o => !o.IsDeleted && o.IsOnTrial && o.TrialExpiresOn.HasValue)
                .ToListAsync();

            foreach (var org in trialOrgs)
            {
                var expiresOn   = org.TrialExpiresOn!.Value;
                double daysLeft = (expiresOn - now).TotalDays;

                // ── 7-day warning ──────────────────────────────────────────
                if (!org.TrialDay7EmailSent && daysLeft is > 6 and <= 7.5)
                {
                    await TrySendAsync(emailSender, senderEmail, senderName,
                        org.Founder.Email!,
                        $"Вашият Pro trial изтича след 7 дни — {org.Name}",
                        BuildDay7Html(org.Name, org.Founder.FirstName ?? org.Founder.UserName!, 7));

                    org.TrialDay7EmailSent = true;
                    logger.LogInformation("Trial Day-7 email sent for org {OrgId}", org.Id);
                }

                // ── 2-day warning ──────────────────────────────────────────
                else if (!org.TrialDay2EmailSent && daysLeft is > 1.5 and <= 2.5)
                {
                    await TrySendAsync(emailSender, senderEmail, senderName,
                        org.Founder.Email!,
                        $"Остават 2 дни от вашия Pro trial — {org.Name}",
                        BuildDay2Html(org.Name, org.Founder.FirstName ?? org.Founder.UserName!, 2));

                    org.TrialDay2EmailSent = true;
                    logger.LogInformation("Trial Day-2 email sent for org {OrgId}", org.Id);
                }

                // ── Expiry notification ────────────────────────────────────
                else if (!org.TrialExpiredEmailSent && daysLeft <= 0 && daysLeft > -1)
                {
                    await TrySendAsync(emailSender, senderEmail, senderName,
                        org.Founder.Email!,
                        $"Вашият Pro trial изтече — {org.Name}",
                        BuildExpiredHtml(org.Name, org.Founder.FirstName ?? org.Founder.UserName!));

                    org.TrialExpiredEmailSent = true;
                    logger.LogInformation("Trial Expired email sent for org {OrgId}", org.Id);
                }
            }

            await db.SaveChangesAsync();
        }

        private static async Task TrySendAsync(
            IEmailSender sender,
            string from, string fromName,
            string to, string subject, string html)
        {
            try
            {
                await sender.SendEmailAsync(from, fromName, to, subject, html);
            }
            catch
            {
                // Email failure must never break the background loop
            }
        }

        // ── Email templates ───────────────────────────────────────────────

        private static string BuildDay7Html(string orgName, string firstName, int daysLeft) => $@"
<!DOCTYPE html>
<html lang=""bg"">
<head><meta charset=""UTF-8""/></head>
<body style=""font-family:Arial,sans-serif;background:#f8f9fa;margin:0;padding:0;"">
  <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background:#f8f9fa;"">
    <tr><td align=""center"" style=""padding:40px 20px;"">
      <table width=""600"" cellpadding=""0"" cellspacing=""0"" style=""background:#fff;border-radius:12px;overflow:hidden;box-shadow:0 2px 8px rgba(0,0,0,.08);"">
        <tr><td style=""background:#0d6efd;padding:32px;text-align:center;"">
          <h1 style=""color:#fff;margin:0;font-size:24px;"">⏰ Вашият Pro Trial изтича скоро</h1>
        </td></tr>
        <tr><td style=""padding:32px;"">
          <p style=""font-size:16px;color:#212529;"">Здравейте, <strong>{firstName}</strong>,</p>
          <p style=""font-size:15px;color:#495057;"">
            Вашият 14-дневен Pro trial за организацията <strong>{orgName}</strong> изтича
            след <strong>{daysLeft} дни</strong>.
          </p>
          <p style=""font-size:15px;color:#495057;"">С Pro плана имате достъп до:</p>
          <ul style=""font-size:15px;color:#495057;line-height:1.8;"">
            <li>✅ Неограничен брой маршрути и разписания</li>
            <li>✅ Разширено аналитично табло с графики</li>
            <li>✅ Известия в реално време за пасажерите</li>
            <li>✅ Google Maps координати за спирките</li>
            <li>✅ Приоритетна поддръжка (24ч)</li>
          </ul>
          <div style=""text-align:center;margin:32px 0;"">
            <a href=""https://gocetransport.app/organization/upgrade""
               style=""background:#0d6efd;color:#fff;padding:14px 32px;border-radius:50px;text-decoration:none;font-size:16px;font-weight:bold;"">
              Надградете сега
            </a>
          </div>
          <p style=""font-size:13px;color:#6c757d;text-align:center;"">
            GoceTransport е доброволчески проект. Приходите покриват сървърните разходи (~€20/мес).
          </p>
        </td></tr>
      </table>
    </td></tr>
  </table>
</body>
</html>";

        private static string BuildDay2Html(string orgName, string firstName, int daysLeft) => $@"
<!DOCTYPE html>
<html lang=""bg"">
<head><meta charset=""UTF-8""/></head>
<body style=""font-family:Arial,sans-serif;background:#f8f9fa;margin:0;padding:0;"">
  <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background:#f8f9fa;"">
    <tr><td align=""center"" style=""padding:40px 20px;"">
      <table width=""600"" cellpadding=""0"" cellspacing=""0"" style=""background:#fff;border-radius:12px;overflow:hidden;box-shadow:0 2px 8px rgba(0,0,0,.08);"">
        <tr><td style=""background:#dc3545;padding:32px;text-align:center;"">
          <h1 style=""color:#fff;margin:0;font-size:24px;"">🚨 Остават само {daysLeft} дни!</h1>
        </td></tr>
        <tr><td style=""padding:32px;"">
          <p style=""font-size:16px;color:#212529;"">Здравейте, <strong>{firstName}</strong>,</p>
          <p style=""font-size:15px;color:#495057;"">
            Вашият Pro trial за <strong>{orgName}</strong> изтича след <strong>{daysLeft} дни</strong>.
          </p>
          <p style=""font-size:15px;color:#212529;font-weight:bold;"">
            След изтичането ще преминете на Free плана и ще загубите:
          </p>
          <ul style=""font-size:15px;color:#dc3545;line-height:1.8;"">
            <li>❌ Маршрутите над лимита (Free: максимум 2)</li>
            <li>❌ Разписанията над лимита (Free: максимум 5)</li>
            <li>❌ Аналитичните графики</li>
            <li>❌ Известията в реално време</li>
          </ul>
          <div style=""text-align:center;margin:32px 0;"">
            <a href=""https://gocetransport.app/organization/upgrade""
               style=""background:#dc3545;color:#fff;padding:14px 32px;border-radius:50px;text-decoration:none;font-size:16px;font-weight:bold;"">
              Запазете Pro функциите
            </a>
          </div>
        </td></tr>
      </table>
    </td></tr>
  </table>
</body>
</html>";

        private static string BuildExpiredHtml(string orgName, string firstName) => $@"
<!DOCTYPE html>
<html lang=""bg"">
<head><meta charset=""UTF-8""/></head>
<body style=""font-family:Arial,sans-serif;background:#f8f9fa;margin:0;padding:0;"">
  <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background:#f8f9fa;"">
    <tr><td align=""center"" style=""padding:40px 20px;"">
      <table width=""600"" cellpadding=""0"" cellspacing=""0"" style=""background:#fff;border-radius:12px;overflow:hidden;box-shadow:0 2px 8px rgba(0,0,0,.08);"">
        <tr><td style=""background:#6c757d;padding:32px;text-align:center;"">
          <h1 style=""color:#fff;margin:0;font-size:24px;"">Pro Trial-ът ви изтече</h1>
        </td></tr>
        <tr><td style=""padding:32px;"">
          <p style=""font-size:16px;color:#212529;"">Здравейте, <strong>{firstName}</strong>,</p>
          <p style=""font-size:15px;color:#495057;"">
            14-дневният Pro trial за организацията <strong>{orgName}</strong> приключи.
            Профилът ви вече е на Free плана.
          </p>
          <p style=""font-size:15px;color:#495057;"">
            Данните ви са запазени. Надградете по всяко време за да върнете достъпа до:
          </p>
          <ul style=""font-size:15px;color:#495057;line-height:1.8;"">
            <li>🔒 Неограничени маршрути и разписания</li>
            <li>🔒 Пълно аналитично табло с графики</li>
            <li>🔒 Реални известия за пасажерите</li>
            <li>🔒 Google Maps координати</li>
          </ul>
          <div style=""text-align:center;margin:32px 0;"">
            <a href=""https://gocetransport.app/organization/upgrade""
               style=""background:#198754;color:#fff;padding:14px 32px;border-radius:50px;text-decoration:none;font-size:16px;font-weight:bold;"">
              Надградете плана си
            </a>
          </div>
          <p style=""font-size:13px;color:#6c757d;text-align:center;"">
            Starter от €9.99/мес &bull; Pro от €24.99/мес &bull; Enterprise от €49.99/мес
          </p>
        </td></tr>
      </table>
    </td></tr>
  </table>
</body>
</html>";
    }
}
