using GoceTransportApp.Data.Models;
using GoceTransportApp.Data.Models.Enumerations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

                switch (stripeEvent.Type)
                {
                    case "checkout.session.completed":
                        await this.HandleCheckoutSessionCompletedAsync(stripeEvent);
                        break;

                    case "invoice.payment_succeeded":
                        await this.HandleInvoicePaymentSucceededAsync(stripeEvent);
                        break;

                    case "invoice.payment_failed":
                        await this.HandleInvoicePaymentFailedAsync(stripeEvent);
                        break;

                    case "customer.subscription.deleted":
                        await this.HandleSubscriptionDeletedAsync(stripeEvent);
                        break;
                }

                return this.Ok();
            }
            catch (StripeException e)
            {
                return this.BadRequest(new { error = e.Message });
            }
        }

        // Fired once when user completes the Stripe Checkout flow.
        // Stores the SubscriptionId and CustomerId on the user and upgrades their tier.
        private async Task HandleCheckoutSessionCompletedAsync(Event stripeEvent)
        {
            var session = stripeEvent.Data.Object as Stripe.Checkout.Session;
            if (session == null) return;

            var userId = session.ClientReferenceId;
            if (string.IsNullOrEmpty(userId)) return;

            session.Metadata.TryGetValue("planType", out var planType);
            var tier = PlanTypeToTier(planType);

            var user = await this.userManager.FindByIdAsync(userId);
            if (user == null) return;

            user.MembershipTier       = tier;
            user.StripeCustomerId     = session.CustomerId;
            user.StripeSubscriptionId = session.SubscriptionId;
            await this.userManager.UpdateAsync(user);
        }

        // Fired every month when the recurring payment succeeds.
        // Ensures the user stays on their current paid tier.
        private async Task HandleInvoicePaymentSucceededAsync(Event stripeEvent)
        {
            var invoice = stripeEvent.Data.Object as Invoice;
            if (invoice?.CustomerId == null) return;

            var user = await this.FindUserByCustomerIdAsync(invoice.CustomerId);
            if (user == null) return;

            // Tier is already set — nothing to change.
            // Edge case: if somehow reverted to Free, restore via subscription metadata.
            if (user.MembershipTier == MembershipTier.Free && user.StripeSubscriptionId != null)
            {
                var subscriptionService = new SubscriptionService();
                var subscription = await subscriptionService.GetAsync(user.StripeSubscriptionId);
                subscription.Metadata.TryGetValue("planType", out var planType);
                user.MembershipTier = PlanTypeToTier(planType);
                await this.userManager.UpdateAsync(user);
            }
        }

        // Fired when a monthly payment fails (e.g. expired card).
        // Downgrades the user to Free immediately.
        private async Task HandleInvoicePaymentFailedAsync(Event stripeEvent)
        {
            var invoice = stripeEvent.Data.Object as Invoice;
            if (invoice?.CustomerId == null) return;

            var user = await this.FindUserByCustomerIdAsync(invoice.CustomerId);
            if (user == null) return;

            user.MembershipTier = MembershipTier.Free;
            await this.userManager.UpdateAsync(user);
        }

        // Fired when a subscription is cancelled or expires.
        // Reverts the user to the Free tier and clears Stripe IDs.
        private async Task HandleSubscriptionDeletedAsync(Event stripeEvent)
        {
            var subscription = stripeEvent.Data.Object as Subscription;
            if (subscription == null) return;

            var user = await this.FindUserBySubscriptionIdAsync(subscription.Id);
            if (user == null) return;

            user.MembershipTier       = MembershipTier.Free;
            user.StripeSubscriptionId = null;
            await this.userManager.UpdateAsync(user);
        }

        private async Task<ApplicationUser?> FindUserBySubscriptionIdAsync(string subscriptionId)
        {
            return await this.userManager.Users
                .FirstOrDefaultAsync(u => u.StripeSubscriptionId == subscriptionId);
        }

        private async Task<ApplicationUser?> FindUserByCustomerIdAsync(string customerId)
        {
            return await this.userManager.Users
                .FirstOrDefaultAsync(u => u.StripeCustomerId == customerId);
        }

        private static MembershipTier PlanTypeToTier(string? planType) => planType switch
        {
            "Starter"    => MembershipTier.Starter,
            "Pro"        => MembershipTier.Pro,
            "Enterprise" => MembershipTier.Enterprise,
            _            => MembershipTier.Free,
        };
    }
}
