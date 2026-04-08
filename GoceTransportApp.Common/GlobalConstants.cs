namespace GoceTransportApp.Common
{
    public static class GlobalConstants
    {
        public const string SystemName = "GoceTransportApp";

        public const string AdministratorRoleName = "Administrator";
        public const string AdministratorArea = "Administration";

        public const string DefaultOrganizationImageUrl = "../../images/no-organization-image";
        public const string DefaultProfileImageUrl = "../../images/no-profile-image";

        public static class TempDataKeys
        {
            // "SuccessMessage" and "FailMessage" are intentionally omitted —
            // controllers use nameof(SuccessMessage) / nameof(FailMessage) from
            // ResultMessages.GeneralMessages, which already resolves to the same key string.
            public const string ErrorMessage = "ErrorMessage";
            public const string UpgradeReason = "UpgradeReason";
            public const string PurchaseFrom = "PurchaseFrom";
            public const string PurchaseTo = "PurchaseTo";
            public const string PurchaseOrg = "PurchaseOrg";
        }

        public static class SignalRMethods
        {
            public const string ReceiveStatusUpdate = "ReceiveStatusUpdate";
            public const string ReceiveNewReview = "ReceiveNewReview";
            public const string ReceiveNotification = "ReceiveNotification";
            public const string ReceiveSystemAlert = "ReceiveSystemAlert";
            public const string ReceiveFavoriteAdded = "ReceiveFavoriteAdded";
            public const string ReceiveDepartureReminder = "ReceiveDepartureReminder";
            public const string ReceivePurchaseConfirmation = "ReceivePurchaseConfirmation";
        }

        public static class RateLimitPolicies
        {
            public const string Login = "login";
            public const string Purchase = "purchase";
        }

        public static class PlanLimits
        {
            // Routes per organization
            public const int FreeRoutes    = 2;
            public const int StarterRoutes = 10;

            // Schedules per organization
            public const int FreeSchedules    = 5;
            public const int StarterSchedules = 30;

            // Pro / Enterprise → int.MaxValue (unlimited)
        }
    }
}
