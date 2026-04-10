# Stripe Integration Task: Subscriptions Checkout

## Context
Project: Goce Transport (`gocetransport.app`)
Goal: Integrate Stripe Checkout to handle recurring monthly subscriptions for Transport Organizations (Starter, Pro, Enterprise plans). The user has already created the products in the Stripe Dashboard and has the `price_` IDs.

## Task 1: Install Dependencies
**Action:** Install the official Stripe SDK via NuGet.
```bash
dotnet add package Stripe.net
Task 2: Configuration
File Target: appsettings.json (and appsettings.Development.json)
Action: Add the Stripe configuration section. (The user will replace the dummy values with their actual keys).

JSON
  "Stripe": {
    "PublishableKey": "pk_test_YOUR_PUBLISHABLE_KEY",
    "SecretKey": "sk_test_YOUR_SECRET_KEY",
    "StarterPriceId": "price_YOUR_STARTER_ID",
    "ProPriceId": "price_YOUR_PRO_ID",
    "EnterprisePriceId": "price_YOUR_ENTERPRISE_ID"
  }
File Target: Program.cs
Action: Initialize the Stripe configuration using the Secret Key. Place this before var app = builder.Build();.

C#
using Stripe;

StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];
Task 3: Create Subscription Controller
File Target: Controllers/SubscriptionController.cs (Create new)
Action: Implement an MVC Controller that creates a Stripe Checkout Session for a subscription.

Implementation constraints:

Use [Authorize] (only logged-in organizations can subscribe).

Method: [HttpPost("create-checkout-session")]

It must accept a planType parameter (e.g., "Starter", "Pro", "Enterprise") to select the correct Price ID from configuration.

SessionCreateOptions.Mode MUST be "subscription".

Set SuccessUrl and CancelUrl to appropriate callback routes (you can mock the views for now).

Code Structure Example:

C#
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Stripe.Checkout;
using System.Collections.Generic;

namespace GoceTransportApp.Web.Controllers
{
    [Authorize]
    [Route("Subscription")]
    public class SubscriptionController : Controller
    {
        private readonly IConfiguration _configuration;

        public SubscriptionController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("create-checkout-session")]
        public IActionResult CreateCheckoutSession(string planType)
        {
            string priceId = planType switch
            {
                "Starter" => _configuration["Stripe:StarterPriceId"],
                "Pro" => _configuration["Stripe:ProPriceId"],
                "Enterprise" => _configuration["Stripe:EnterprisePriceId"],
                _ => null
            };

            if (priceId == null) return BadRequest("Invalid plan selected.");

            var domain = $"{Request.Scheme}://{Request.Host}";
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        Price = priceId,
                        Quantity = 1,
                    },
                },
                Mode = "subscription",
                SuccessUrl = domain + "/Subscription/Success?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = domain + "/Subscription/Cancel",
                // IMPORTANT: Pass the organization/user ID to match the payment later
                // ClientReferenceId = userId 
            };

            var service = new SessionService();
            Session session = service.Create(options);

            return Redirect(session.Url);
        }

        [HttpGet("Success")]
        public IActionResult Success() => View(); // Agent: Create a simple dummy view for this

        [HttpGet("Cancel")]
        public IActionResult Cancel() => View(); // Agent: Create a simple dummy view for this
    }
}
Agent Completion Checklist:
[ ] Stripe.net package installed.

[ ] appsettings.json configured (waiting for user secrets).

[ ] Program.cs configured with StripeConfiguration.ApiKey.

[ ] SubscriptionController created with correct subscription mode.

[ ] Dummy Success and Cancel views generated.

[ ] NO large commits; commit this feature separately.