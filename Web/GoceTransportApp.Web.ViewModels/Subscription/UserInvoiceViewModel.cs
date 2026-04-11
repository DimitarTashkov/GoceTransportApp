namespace GoceTransportApp.Web.ViewModels.Subscription
{
    using System;
    using System.Collections.Generic;

    public class UserInvoiceViewModel
    {
        public string Number { get; set; } = null!;

        public DateTime Created { get; set; }

        /// <summary>Amount in minor units (cents). Use <see cref="AmountFormatted"/> for display.</summary>
        public long AmountPaid { get; set; }

        public string Currency { get; set; } = null!;

        /// <summary>Stripe status: "paid", "open", "void", "draft", "uncollectible".</summary>
        public string Status { get; set; } = null!;

        /// <summary>Direct PDF download URL from Stripe (no auth needed — signed link).</summary>
        public string? InvoicePdfUrl { get; set; }

        /// <summary>Stripe-hosted payment / view page.</summary>
        public string? HostedInvoiceUrl { get; set; }

        public string AmountFormatted => $"{this.AmountPaid / 100m:0.00} {this.Currency.ToUpperInvariant()}";
    }

    public class UserInvoicesListViewModel
    {
        public IEnumerable<UserInvoiceViewModel> Invoices { get; set; } = new List<UserInvoiceViewModel>();

        public bool HasStripeCustomer { get; set; }
    }
}
