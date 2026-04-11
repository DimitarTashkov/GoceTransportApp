namespace GoceTransportApp.Web.Middleware
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;

    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public Task InvokeAsync(HttpContext context)
        {
            context.Response.OnStarting(() =>
            {
                var path = context.Request.Path.Value ?? string.Empty;
                var headers = context.Response.Headers;

                headers["X-Content-Type-Options"] = "nosniff";
                headers["X-XSS-Protection"] = "1; mode=block";
                headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
                headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=(self)";

                // OAuth callbacks (signin-google, signin-facebook, etc.) perform top-level
                // redirects issued by the identity provider; DENY breaks them on some flows.
                var isOAuthCallback = path.StartsWith("/signin-", System.StringComparison.OrdinalIgnoreCase)
                    || path.StartsWith("/signout-", System.StringComparison.OrdinalIgnoreCase);

                if (!isOAuthCallback)
                {
                    headers["X-Frame-Options"] = "DENY";
                    headers["Content-Security-Policy"] =
                        "default-src 'self'; " +
                        "script-src 'self' 'unsafe-inline' https://cdnjs.cloudflare.com https://cdn.jsdelivr.net https://accounts.google.com https://www.gstatic.com; " +
                        "style-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net https://cdnjs.cloudflare.com https://accounts.google.com https://www.gstatic.com; " +
                        "img-src 'self' data: https:; " +
                        "font-src 'self' https://cdnjs.cloudflare.com https://cdn.jsdelivr.net https://fonts.gstatic.com; " +
                        "connect-src 'self' https://accounts.google.com; " +
                        "frame-src https://accounts.google.com; " +
                        "frame-ancestors 'none';";
                }

                return Task.CompletedTask;
            });

            return this.next(context);
        }
    }
}
