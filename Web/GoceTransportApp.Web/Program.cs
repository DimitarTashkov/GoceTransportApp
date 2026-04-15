namespace GoceTransportApp.Web
{
    using System;
    using System.Reflection;
    using System.Threading.RateLimiting;

    using Microsoft.AspNetCore.Diagnostics.HealthChecks;
    using Microsoft.AspNetCore.HttpOverrides;
    using Microsoft.AspNetCore.ResponseCompression;

    using GoceTransportApp.Data;
    using GoceTransportApp.Data.Common;
    using GoceTransportApp.Data.Common.Repositories;
    using GoceTransportApp.Data.Models;
    using GoceTransportApp.Data.Repositories;
    using GoceTransportApp.Data.Seeding;
    using GoceTransportApp.Services;
    using GoceTransportApp.Services.Data;
    using GoceTransportApp.Services.Data.Cities;
    using GoceTransportApp.Services.Data.ContactForms;
    using GoceTransportApp.Services.Data.Drivers;
    using GoceTransportApp.Services.Data.Organizations;
    using GoceTransportApp.Services.Data.Reviews;
    using GoceTransportApp.Services.Data.Routes;
    using GoceTransportApp.Services.Data.Analytics;
    using GoceTransportApp.Services.Data.TrialEmails;
    using GoceTransportApp.Services.Data.RouteStops;
    using GoceTransportApp.Services.Data.Schedules;
    using GoceTransportApp.Services.Data.Streets;
    using GoceTransportApp.Services.Data.Tickets;
    using GoceTransportApp.Services.Data.Users;
    using GoceTransportApp.Services.Data.Vehicles;
    using GoceTransportApp.Services.Mapping;
    using GoceTransportApp.Services.Messaging;
    using GoceTransportApp.Web.Hubs;
    using GoceTransportApp.Web.Middleware;
    using Microsoft.Extensions.Logging;
    using GoceTransportApp.Web.Resources;
    using GoceTransportApp.Web.Services.Identity;
    using GoceTransportApp.Web.ViewModels;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.RateLimiting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Polly;
    using Polly.Extensions.Http;
    using Serilog;

    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    // ВРЕМЕННО отпушваме логовете за аутентикация:
    .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", Serilog.Events.LogEventLevel.Debug)
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
                // ... надолу остава същото
                .WriteTo.Console()
                .WriteTo.File(
                    path: "logs/goce-transport-.log",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            try
            {
                Log.Information("Starting GoceTransportApp web host");
                var builder = WebApplication.CreateBuilder(args);
                builder.Host.UseSerilog();
                ConfigureServices(builder.Services, builder.Configuration);
                Stripe.StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

                builder.Services.Configure<ForwardedHeadersOptions>(options =>
                {
                    options.ForwardedHeaders = ForwardedHeaders.XForwardedProto;
                    options.KnownNetworks.Clear();
                    options.KnownProxies.Clear();
                });

                var app = builder.Build();
                app.UseForwardedHeaders();
                Configure(app);
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services
                .AddDefaultIdentity<ApplicationUser>(IdentityOptionsProvider.GetIdentityOptions)
                .AddRoles<ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddErrorDescriber<LocalizedIdentityErrorDescriber>();

            var keysDirectory = new System.IO.DirectoryInfo(
                System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "GoceTransportApp",
                    "DataProtection-Keys"));
            if (!keysDirectory.Exists)
            {
                keysDirectory.Create();
            }

            services.AddDataProtection()
                .PersistKeysToFileSystem(keysDirectory)
                .SetApplicationName("GoceTransportApp");

            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.ClientId = configuration["Authentication:Google:ClientId"] ?? "dummy-client-id";
                    options.ClientSecret = configuration["Authentication:Google:ClientSecret"] ?? "dummy-client-secret";
                    options.CorrelationCookie.SameSite = SameSiteMode.None;
                    options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
                });

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None;
                options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.None;
                options.Secure = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
            });

            services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10 MB
            });

            services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = true;
            });

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddControllersWithViews(
                options =>
                {
                    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                })
                .AddViewLocalization()
                .AddDataAnnotationsLocalization(options =>
                {
                    options.DataAnnotationLocalizerProvider = (type, factory) =>
                        factory.Create(typeof(SharedResource));
                })
                .AddRazorRuntimeCompilation();
            services.AddRazorPages();

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddMemoryCache();

            services.AddSignalR();

            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
            });

            services.AddHealthChecks()
                .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy());

            services.AddRateLimiter(options =>
            {
                // Global: 100 requests/minute per IP (SignalR hubs are exempt)
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
                    context =>
                    {
                        if (context.Request.Path.StartsWithSegments("/notificationHub"))
                            return RateLimitPartition.GetNoLimiter("signalr");

                        return RateLimitPartition.GetFixedWindowLimiter(
                            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                            _ => new FixedWindowRateLimiterOptions
                            {
                                PermitLimit = 100,
                                Window = TimeSpan.FromMinutes(1),
                            });
                    });

                // Strict limit for login: 10 attempts/5 min per IP
                options.AddPolicy(GoceTransportApp.Common.GlobalConstants.RateLimitPolicies.Login, context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 10,
                            Window = TimeSpan.FromMinutes(5),
                        }));

                // Strict limit for ticket purchase: 20 attempts/min per IP
                options.AddPolicy(GoceTransportApp.Common.GlobalConstants.RateLimitPolicies.Purchase, context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 20,
                            Window = TimeSpan.FromMinutes(1),
                        }));

                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });

            services.AddSingleton(configuration);

            // CORS — locked to production domain in Production, open in Development
            services.AddCors(options =>
            {
                options.AddPolicy("Production", policy =>
                {
                    policy.WithOrigins(
                            "https://gocetransport.app",
                            "https://www.gocetransport.app")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
                options.AddPolicy("Development", policy =>
                {
                    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                });
            });

            // HTTP client for the Transport API (used in Cloud Run: Web → API over HTTP)
            // Retry: 3 attempts with exponential backoff (2s, 4s, 8s) — handles Cloud Run cold starts
            // Circuit Breaker: opens after 5 consecutive failures for 30 seconds
            var retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));

            var circuitBreakerPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 5,
                    durationOfBreak: TimeSpan.FromSeconds(30));

            services.AddHttpClient("TransportApi", client =>
            {
                client.BaseAddress = new Uri(configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5001");
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .AddPolicyHandler(retryPolicy)
            .AddPolicyHandler(circuitBreakerPolicy);

            // Data repositories
            services.AddScoped(typeof(IDeletableEntityRepository<>), typeof(EfDeletableEntityRepository<>));
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddScoped<IDbQueryRunner, DbQueryRunner>();

            // Application services
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<ISettingsService, SettingsService>();
            services.AddScoped<IStreetService, StreetService>();
            services.AddScoped<ICityService, CityService>();
            services.AddScoped<IRouteService, RouteService>();
            services.AddScoped<IRouteStopService, RouteStopService>();
            services.AddScoped<IAnalyticsService, AnalyticsService>();
            services.AddScoped<IVehicleService, VehicleService>();
            services.AddScoped<IDriverService, DriverService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IScheduleService, ScheduleService>();
            services.AddScoped<ITicketService, TicketService>();
            services.AddScoped<IOrganizationService, OrganizationService>();
            services.AddScoped<IContactFormService, ContactFormService>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<GoceTransportApp.Services.Data.Notifications.INotificationService, GoceTransportApp.Services.Data.Notifications.NotificationService>();

            services.AddHostedService<TrialEmailBackgroundService>();
        }

        private static void Configure(WebApplication app)
        {
            // Seed data on application startup
            using (var serviceScope = app.Services.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                dbContext.Database.Migrate();
                new ApplicationDbContextSeeder().SeedAsync(dbContext, serviceScope.ServiceProvider).GetAwaiter().GetResult();
            }

            AutoMapperConfig.RegisterMappings(typeof(ErrorViewModel).GetTypeInfo().Assembly);

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseStatusCodePagesWithReExecute("/Home/Error/{0}");
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseMiddleware<GlobalExceptionMiddleware>();
                app.UseHsts();
            }

            // Security headers — early in pipeline
            app.UseMiddleware<SecurityHeadersMiddleware>();
            app.UseHttpsRedirection();
            app.UseCookiePolicy();

            // Response compression — before static files and routing
            app.UseResponseCompression();

            // Static files with 7-day cache (sw.js excluded — must not be cached)
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    var path = ctx.Context.Request.Path.Value ?? string.Empty;
                    if (path.EndsWith("sw.js", StringComparison.OrdinalIgnoreCase))
                    {
                        ctx.Context.Response.Headers.Append("Cache-Control", "no-cache, no-store, must-revalidate");
                        ctx.Context.Response.Headers.Append("Service-Worker-Allowed", "/");
                    }
                    else
                    {
                        ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=604800");
                    }
                },
            });

            app.UseCookiePolicy();

            app.UseRouting();

            // Localization — after routing, before auth
            var supportedCultures = new[] { "bg-BG", "en-US" };
            var localizationOptions = new Microsoft.AspNetCore.Builder.RequestLocalizationOptions()
                .SetDefaultCulture("bg-BG")
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);
            app.UseRequestLocalization(localizationOptions);

            // CORS — between Routing and Authentication
            app.UseCors(app.Environment.IsDevelopment() ? "Development" : "Production");

            // Rate limiting — after routing (required for endpoint-based rate limiting attributes)
            app.UseRateLimiter();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapHub<NotificationHub>("/notificationHub");
            app.MapControllerRoute("areaRoute", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
            app.MapControllerRoute("Errors", "{controller=Home}/{action=Index}/{statusCode?}");
            app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();
            app.MapHealthChecks("/health");
        }
    }
}
