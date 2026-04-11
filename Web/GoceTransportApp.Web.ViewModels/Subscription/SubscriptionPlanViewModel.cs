namespace GoceTransportApp.Web.ViewModels.Subscription
{
    using System;

    using GoceTransportApp.Data.Models.Enumerations;

    public class SubscriptionPlanViewModel
    {
        public MembershipTier CurrentTier { get; set; }

        public string? StripeSubscriptionId { get; set; }

        /// <summary>Status from Stripe: "active", "canceled", "past_due", etc.</summary>
        public string? StripeStatus { get; set; }

        /// <summary>End of current paid period — user keeps access until this date.</summary>
        public DateTime? CurrentPeriodEnd { get; set; }

        /// <summary>True when the user has already requested cancellation but access continues until <see cref="CurrentPeriodEnd"/>.</summary>
        public bool CancelAtPeriodEnd { get; set; }

        public bool HasActiveSubscription =>
            !string.IsNullOrEmpty(this.StripeSubscriptionId) && this.CurrentTier != MembershipTier.Free;
    }
}
