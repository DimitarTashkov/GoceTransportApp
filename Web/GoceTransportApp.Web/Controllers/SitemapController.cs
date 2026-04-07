namespace GoceTransportApp.Web.Controllers
{
    using System;
    using System.Text;
    using System.Threading.Tasks;

    using GoceTransportApp.Services.Data.Cities;
    using GoceTransportApp.Services.Data.Routes;
    using GoceTransportApp.Web.ViewModels.Routes;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [AllowAnonymous]
    public class SitemapController : Controller
    {
        private readonly IRouteService routeService;
        private readonly ICityService cityService;

        public SitemapController(IRouteService routeService, ICityService cityService)
        {
            this.routeService = routeService;
            this.cityService = cityService;
        }

        [Route("sitemap.xml")]
        [ResponseCache(Duration = 43200, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> Index()
        {
            var host = $"{this.Request.Scheme}://{this.Request.Host}";
            var sb = new StringBuilder();

            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");

            // Static pages
            AddUrl(sb, host, "/", "daily", "1.0");
            AddUrl(sb, host, "/home/aboutus", "monthly", "0.5");
            AddUrl(sb, host, "/home/privacy", "monthly", "0.3");
            AddUrl(sb, host, "/home/terms", "monthly", "0.3");
            AddUrl(sb, host, "/contactform/create", "monthly", "0.4");

            // Key discovery pages
            AddUrl(sb, host, "/schedule/search", "always", "0.9");
            AddUrl(sb, host, "/schedule/index", "hourly", "0.8");
            AddUrl(sb, host, "/route/index", "daily", "0.7");
            AddUrl(sb, host, "/city/index", "weekly", "0.6");
            AddUrl(sb, host, "/organization/index", "daily", "0.8");
            AddUrl(sb, host, "/organization/upgrade", "monthly", "0.6");

            // Auth pages
            AddUrl(sb, host, "/identity/account/login", "monthly", "0.5");
            AddUrl(sb, host, "/identity/account/register", "monthly", "0.5");

            // Dynamic: all routes
            var routeFilter = new AllRoutesSearchFilterViewModel
            {
                CurrentPage = 1,
                EntitiesPerPage = 10000,
            };

            var routes = await this.routeService.GetAllRoutesAsync(routeFilter);
            foreach (var route in routes)
            {
                var routeUrl = $"/route/details?id={route.Id.ToLower()}&organizationid={route.OrganizationId.ToLower()}";
                AddUrl(sb, host, routeUrl, "weekly", "0.7");
            }

            // Dynamic: all cities
            var cities = await this.cityService.GetAllCitiesForDropDownsAsync();
            foreach (var city in cities)
            {
                var cityUrl = $"/city/details?id={city.Id.ToString().ToLower()}";
                AddUrl(sb, host, cityUrl, "weekly", "0.6");
            }

            sb.AppendLine("</urlset>");

            return this.Content(sb.ToString(), "application/xml", Encoding.UTF8);
        }

        private static void AddUrl(StringBuilder sb, string host, string path, string changefreq, string priority)
        {
            sb.AppendLine("  <url>");
            sb.AppendLine($"    <loc>{host}{path}</loc>");
            sb.AppendLine($"    <changefreq>{changefreq}</changefreq>");
            sb.AppendLine($"    <priority>{priority}</priority>");
            sb.AppendLine("  </url>");
        }
    }
}
