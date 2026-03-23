namespace GoceTransportApp.Services.Messaging
{
    using System;

    public static class EmailTemplates
    {
        private const string BrandColor = "#2563EB";
        private const string BrandName = "GoceTransport";

        public static string GetTicketConfirmationEmail(
            string recipientName,
            string fromCity,
            string toCity,
            string departureDate,
            string departureTime,
            string arrivalTime,
            string organizationName,
            string ticketId,
            string price)
        {
            return $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""UTF-8"" />
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
  <title>Ticket Confirmation</title>
</head>
<body style=""margin:0;padding:0;background-color:#f3f4f6;font-family:'Segoe UI',Arial,sans-serif;"">
  <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#f3f4f6;padding:40px 0;"">
    <tr>
      <td align=""center"">
        <table width=""600"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#ffffff;border-radius:12px;overflow:hidden;box-shadow:0 4px 24px rgba(0,0,0,0.08);"">

          <!-- Header -->
          <tr>
            <td style=""background-color:{BrandColor};padding:32px 40px;text-align:center;"">
              <h1 style=""color:#ffffff;margin:0;font-size:26px;font-weight:700;letter-spacing:-0.5px;"">
                &#9993; {BrandName}
              </h1>
              <p style=""color:#bfdbfe;margin:8px 0 0;font-size:14px;"">Your travel confirmation</p>
            </td>
          </tr>

          <!-- Greeting -->
          <tr>
            <td style=""padding:32px 40px 0;"">
              <p style=""margin:0;font-size:16px;color:#374151;"">Hello, <strong>{EscapeHtml(recipientName)}</strong>!</p>
              <p style=""margin:12px 0 0;font-size:15px;color:#6b7280;"">
                Your ticket has been successfully issued. Here are your travel details:
              </p>
            </td>
          </tr>

          <!-- Route Banner -->
          <tr>
            <td style=""padding:24px 40px;"">
              <table width=""100%"" cellpadding=""0"" cellspacing=""0""
                     style=""background:linear-gradient(135deg,#eff6ff,#dbeafe);border-radius:10px;padding:24px;"">
                <tr>
                  <td style=""text-align:center;"">
                    <p style=""margin:0;font-size:13px;color:#6b7280;text-transform:uppercase;letter-spacing:1px;"">Departure</p>
                    <p style=""margin:6px 0 0;font-size:28px;font-weight:700;color:#1e40af;"">{EscapeHtml(fromCity)}</p>
                  </td>
                  <td style=""text-align:center;padding:0 16px;"">
                    <span style=""font-size:28px;color:{BrandColor};"">&#8594;</span>
                  </td>
                  <td style=""text-align:center;"">
                    <p style=""margin:0;font-size:13px;color:#6b7280;text-transform:uppercase;letter-spacing:1px;"">Arrival</p>
                    <p style=""margin:6px 0 0;font-size:28px;font-weight:700;color:#1e40af;"">{EscapeHtml(toCity)}</p>
                  </td>
                </tr>
              </table>
            </td>
          </tr>

          <!-- Details Grid -->
          <tr>
            <td style=""padding:0 40px 24px;"">
              <table width=""100%"" cellpadding=""0"" cellspacing=""0"">
                <tr>
                  <td style=""width:50%;padding:0 8px 16px 0;"">
                    {DetailBlock("&#128197;", "Date", EscapeHtml(departureDate))}
                  </td>
                  <td style=""width:50%;padding:0 0 16px 8px;"">
                    {DetailBlock("&#8987;", "Departure", EscapeHtml(departureTime))}
                  </td>
                </tr>
                <tr>
                  <td style=""padding:0 8px 16px 0;"">
                    {DetailBlock("&#128652;", "Carrier", EscapeHtml(organizationName))}
                  </td>
                  <td style=""padding:0 0 16px 8px;"">
                    {DetailBlock("&#128179;", "Price", EscapeHtml(price) + " BGN")}
                  </td>
                </tr>
              </table>
            </td>
          </tr>

          <!-- Ticket ID -->
          <tr>
            <td style=""padding:0 40px 24px;"">
              <div style=""border:1px dashed #d1d5db;border-radius:8px;padding:16px;text-align:center;"">
                <p style=""margin:0;font-size:12px;color:#9ca3af;text-transform:uppercase;letter-spacing:1px;"">Booking Reference</p>
                <p style=""margin:6px 0 0;font-size:14px;font-weight:600;color:#374151;font-family:monospace;"">{EscapeHtml(ticketId)}</p>
              </div>
            </td>
          </tr>

          <!-- Footer -->
          <tr>
            <td style=""background-color:#f9fafb;padding:24px 40px;text-align:center;border-top:1px solid #e5e7eb;"">
              <p style=""margin:0;font-size:13px;color:#9ca3af;"">
                Thank you for choosing <strong style=""color:{BrandColor}"">{BrandName}</strong>.<br/>
                This is an automated message — please do not reply.
              </p>
            </td>
          </tr>

        </table>
      </td>
    </tr>
  </table>
</body>
</html>";
        }

        public static string GetTicketCancellationEmail(
            string recipientName,
            string fromCity,
            string toCity,
            string departureDate,
            string organizationName,
            string ticketId)
        {
            return $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""UTF-8"" />
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
  <title>Ticket Cancellation</title>
</head>
<body style=""margin:0;padding:0;background-color:#f3f4f6;font-family:'Segoe UI',Arial,sans-serif;"">
  <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#f3f4f6;padding:40px 0;"">
    <tr>
      <td align=""center"">
        <table width=""600"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#ffffff;border-radius:12px;overflow:hidden;box-shadow:0 4px 24px rgba(0,0,0,0.08);"">

          <!-- Header -->
          <tr>
            <td style=""background-color:#dc2626;padding:32px 40px;text-align:center;"">
              <h1 style=""color:#ffffff;margin:0;font-size:26px;font-weight:700;letter-spacing:-0.5px;"">
                &#10060; Ticket Cancelled
              </h1>
              <p style=""color:#fecaca;margin:8px 0 0;font-size:14px;"">{BrandName} — Cancellation Notice</p>
            </td>
          </tr>

          <!-- Greeting -->
          <tr>
            <td style=""padding:32px 40px 24px;"">
              <p style=""margin:0;font-size:16px;color:#374151;"">Hello, <strong>{EscapeHtml(recipientName)}</strong>!</p>
              <p style=""margin:12px 0 0;font-size:15px;color:#6b7280;"">
                Your ticket has been <strong style=""color:#dc2626;"">successfully cancelled</strong>.
                A refund will be processed according to our cancellation policy.
              </p>
            </td>
          </tr>

          <!-- Cancelled Route -->
          <tr>
            <td style=""padding:0 40px 24px;"">
              <table width=""100%"" cellpadding=""0"" cellspacing=""0""
                     style=""background-color:#fef2f2;border:1px solid #fecaca;border-radius:10px;padding:24px;"">
                <tr>
                  <td style=""text-align:center;"">
                    <p style=""margin:0;font-size:13px;color:#9ca3af;text-transform:uppercase;letter-spacing:1px;"">Cancelled Route</p>
                    <p style=""margin:10px 0 0;font-size:22px;font-weight:700;color:#7f1d1d;"">
                      {EscapeHtml(fromCity)} &#8594; {EscapeHtml(toCity)}
                    </p>
                    <p style=""margin:6px 0 0;font-size:14px;color:#6b7280;"">
                      {EscapeHtml(departureDate)} &nbsp;|&nbsp; {EscapeHtml(organizationName)}
                    </p>
                  </td>
                </tr>
              </table>
            </td>
          </tr>

          <!-- Ticket Ref -->
          <tr>
            <td style=""padding:0 40px 24px;"">
              <div style=""border:1px dashed #d1d5db;border-radius:8px;padding:16px;text-align:center;"">
                <p style=""margin:0;font-size:12px;color:#9ca3af;text-transform:uppercase;letter-spacing:1px;"">Cancelled Booking Reference</p>
                <p style=""margin:6px 0 0;font-size:14px;font-weight:600;color:#374151;font-family:monospace;"">{EscapeHtml(ticketId)}</p>
              </div>
            </td>
          </tr>

          <!-- Footer -->
          <tr>
            <td style=""background-color:#f9fafb;padding:24px 40px;text-align:center;border-top:1px solid #e5e7eb;"">
              <p style=""margin:0;font-size:13px;color:#9ca3af;"">
                We hope to see you again on <strong style=""color:{BrandColor}"">{BrandName}</strong>.<br/>
                This is an automated message — please do not reply.
              </p>
            </td>
          </tr>

        </table>
      </td>
    </tr>
  </table>
</body>
</html>";
        }

        private static string DetailBlock(string icon, string label, string value)
        {
            return $@"
<div style=""background-color:#f9fafb;border-radius:8px;padding:14px 16px;"">
  <p style=""margin:0;font-size:12px;color:#9ca3af;text-transform:uppercase;letter-spacing:1px;"">{icon} {label}</p>
  <p style=""margin:4px 0 0;font-size:15px;font-weight:600;color:#111827;"">{value}</p>
</div>";
        }

        private static string EscapeHtml(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            return input
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;");
        }
    }
}
