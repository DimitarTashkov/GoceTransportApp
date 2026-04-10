using GoceTransportApp.Data.Models;
using GoceTransportApp.Data.Models.Enumerations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Stripe;
using System.IO;
using System.Threading.Tasks;

namespace GoceTransportApp.Web.Controllers
{
    [ApiController]
    [Route("api/webhook")]
    [IgnoreAntiforgeryToken]
    public class WebhookController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly UserManager<ApplicationUser> userManager;

        public WebhookController(IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            this.configuration = configuration;
            this.userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Index()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var stripeSignature = Request.Headers["Stripe-Signature"];

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    stripeSignature,
                    this.configuration["Stripe:WebhookSecret"]);

                if (stripeEvent.Type == "checkout.session.completed")
                {
                    var session = stripeEvent.Data.Object as Stripe.Checkout.Session;

                    var userId = session.ClientReferenceId;
                    if (string.IsNullOrEmpty(userId))
                    {
                        return this.Ok();
                    }

                    session.Metadata.TryGetValue("planType", out var planType);

                    var tier = planType switch
                    {
                        "Starter"    => MembershipTier.Starter,
                        "Pro"        => MembershipTier.Pro,
                        "Enterprise" => MembershipTier.Enterprise,
                        _            => MembershipTier.Free,
                    };

                    var user = await this.userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        user.MembershipTier = tier;
                        await this.userManager.UpdateAsync(user);
                    }
                }

                return this.Ok();
            }
            catch (StripeException e)
            {
                return this.BadRequest(new { error = e.Message });
            }
        }
    }
}
