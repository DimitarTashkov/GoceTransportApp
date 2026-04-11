using GoceTransportApp.Data.Models;
using GoceTransportApp.Web.ViewModels.Subscription;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GoceTransportApp.Web.Controllers
{
    [Authorize]
    public class SubscriptionController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly UserManager<ApplicationUser> userManager;

        public SubscriptionController(IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            this.configuration = configuration;
            this.userManager = userManager;
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
        public async Task<IActionResult> ManagePlan()
        {
            var user = await this.userManager.GetUserAsync(User);
            if (user == null)
            {
                return this.Challenge();
            }

            var vm = new SubscriptionPlanViewModel
            {
                CurrentTier          = user.MembershipTier,
                StripeSubscriptionId = user.StripeSubscriptionId,
            };

            if (!string.IsNullOrEmpty(user.StripeSubscriptionId))
            {
                var subscriptionService = new SubscriptionService();
                Stripe.Subscription subscription = await subscriptionService.GetAsync(
                    user.StripeSubscriptionId,
                    new SubscriptionGetOptions { Expand = new System.Collections.Generic.List<string> { "items" } });

                vm.StripeStatus      = subscription.Status;
                vm.CancelAtPeriodEnd = subscription.CancelAtPeriodEnd;

                // In Stripe.net v47+, CurrentPeriodEnd moved from Subscription to SubscriptionItem
                var firstItem = subscription.Items?.Data?.Count > 0 ? subscription.Items.Data[0] : null;
                vm.CurrentPeriodEnd = firstItem?.CurrentPeriodEnd;
            }

            return this.View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelSubscription()
        {
            var user = await this.userManager.GetUserAsync(User);
            if (user == null)
            {
                return this.Challenge();
            }

            if (string.IsNullOrEmpty(user.StripeSubscriptionId))
            {
                return this.RedirectToAction(nameof(ManagePlan));
            }

            var subscriptionService = new SubscriptionService();
            await subscriptionService.UpdateAsync(user.StripeSubscriptionId, new SubscriptionUpdateOptions
            {
                CancelAtPeriodEnd = true,
            });

            TempData["SuccessMessage"] = "CancelScheduled";
            return this.RedirectToAction(nameof(ManagePlan));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResumeSubscription()
        {
            var user = await this.userManager.GetUserAsync(User);
            if (user == null)
            {
                return this.Challenge();
            }

            if (string.IsNullOrEmpty(user.StripeSubscriptionId))
            {
                return this.RedirectToAction(nameof(ManagePlan));
            }

            var subscriptionService = new SubscriptionService();
            await subscriptionService.UpdateAsync(user.StripeSubscriptionId, new SubscriptionUpdateOptions
            {
                CancelAtPeriodEnd = false,
            });

            TempData["SuccessMessage"] = "ResumeScheduled";
            return this.RedirectToAction(nameof(ManagePlan));
        }

        [HttpGet]
        public async Task<IActionResult> Invoices()
        {
            var user = await this.userManager.GetUserAsync(User);
            if (user == null)
            {
                return this.Challenge();
            }

            var vm = new UserInvoicesListViewModel
            {
                HasStripeCustomer = !string.IsNullOrEmpty(user.StripeCustomerId),
            };

            if (vm.HasStripeCustomer)
            {
                var invoiceService = new InvoiceService();
                var stripeInvoices = await invoiceService.ListAsync(new InvoiceListOptions
                {
                    Customer = user.StripeCustomerId,
                    Limit    = 100,
                });

                vm.Invoices = stripeInvoices.Data
                    .OrderByDescending(i => i.Created)
                    .Select(i => new UserInvoiceViewModel
                    {
                        Number           = i.Number,
                        Created          = i.Created,
                        AmountPaid       = i.AmountPaid,
                        Currency         = i.Currency,
                        Status           = i.Status,
                        InvoicePdfUrl    = i.InvoicePdf,
                        HostedInvoiceUrl = i.HostedInvoiceUrl,
                    })
                    .ToList();
            }

            return this.View(vm);
        }

        [HttpGet]
        public IActionResult Success() => this.View();

        [HttpGet]
        public IActionResult Cancel() => this.View();
    }
}
