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

        public async Task InvokeAsync(HttpContext context)
        {
            var headers = context.Response.Headers;

            headers["X-Content-Type-Options"] = "nosniff";
            headers["X-Frame-Options"] = "DENY";
            headers["X-XSS-Protection"] = "1; mode=block";
            headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
            headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=(self)";
            headers["Content-Security-Policy"] =
                "default-src 'self'; " +
                "script-src 'self' 'unsafe-inline' https://cdnjs.cloudflare.com https://cdn.jsdelivr.net; " +
                "style-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net https://cdnjs.cloudflare.com; " +
                "img-src 'self' data: https:; " +
                "font-src 'self' https://cdnjs.cloudflare.com https://cdn.jsdelivr.net; " +
                "connect-src 'self'; " +
                "frame-ancestors 'none';";

            await this.next(context);
        }
    }
}
