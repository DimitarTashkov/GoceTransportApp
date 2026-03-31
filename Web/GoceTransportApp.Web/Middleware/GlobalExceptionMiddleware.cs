namespace GoceTransportApp.Web.Middleware
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<GlobalExceptionMiddleware> logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await this.next(context);
            }
            catch (Exception ex)
            {
                this.logger.LogError(
                    ex,
                    "Unhandled exception for {Method} {Path} from {IP}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Connection.RemoteIpAddress);

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                if (context.Response.HasStarted)
                {
                    this.logger.LogWarning("Response already started, cannot redirect to error page.");
                    return;
                }

                if (context.Request.Headers.Accept.ToString().Contains("application/json"))
                {
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(new { error = "Възникна грешка. Моля, опитайте отново." });
                }
                else
                {
                    context.Response.Redirect("/Home/Error");
                }
            }
        }
    }
}
