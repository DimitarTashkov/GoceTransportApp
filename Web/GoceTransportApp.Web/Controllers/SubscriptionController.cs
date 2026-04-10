using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Stripe.Checkout;
using System.Collections.Generic;
using System.Security.Claims;

namespace GoceTransportApp.Web.Controllers
{
    [Authorize]
    public class SubscriptionController : Controller
    {
        private readonly IConfiguration configuration;

        public SubscriptionController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpPost]
        public IActionResult CreateCheckoutSession(string planType)
        {
            string priceId = planType switch
            {
                "Starter"    => this.configuration["Stripe:StarterPriceId"],
                "Pro"        => this.configuration["Stripe:ProPriceId"],
                "Enterprise" => this.configuration["Stripe:EnterprisePriceId"],
                _            => null,
            };

            if (priceId == null)
            {
                return this.BadRequest("Invalid plan selected.");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        Price    = priceId,
                        Quantity = 1,
                    },
                },
                Mode              = "subscription",
                ClientReferenceId = userId,
                Metadata          = new Dictionary<string, string> { ["planType"] = planType },
                SuccessUrl        = Url.Action(nameof(Success), "Subscription", null, Request.Scheme, Request.Host.Value) + "?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl         = Url.Action(nameof(Cancel), "Subscription", null, Request.Scheme, Request.Host.Value),
            };

            var service = new SessionService();
            Session session = service.Create(options);

            return this.Redirect(session.Url);
        }

        [HttpGet]
        public IActionResult Success() => this.View();

        [HttpGet]
        public IActionResult Cancel() => this.View();
    }
}
