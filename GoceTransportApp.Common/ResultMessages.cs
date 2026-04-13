namespace GoceTransportApp.Common
{
    public static class ResultMessages
    {
        public static class StreetMessages
        {
            public const string StreetNameRequiredMessage = "Street name is required";
            public const string CityIdRequired = "City ID cannot be null or empty.";
            public const string InvalidCityId = "Invalid City ID.";
        }

        public static class CityMessages
        {
            public const string CityNameRequiredMessage = "City name is required";
            public const string CityStateRequiredMessage = "City state is required";
            public const string CityZipRequiredMessage = "City zip is required";
        }

        public static class RouteMessages
        {
            public const string RouteDistanceRequiredMessage = "Route distance is required";
            public const string RouteDurationRequiredMessage = "Route duration is required";
        }

        public static class DriverMessages
        {
            public const string DriverFirstNameRequired = "Driver first name is required";
            public const string DriverLastNameRequired = "Driver last name is required";
            public const string DriverExperienceRequired = "Driver experience is required";
            public const string InvalidDrivingExperience = "Invalid driving experience value provided.";
        }

        public static class VehicleMessages
        {
            public const string VehicleNumberRequired = "Vehicle number is required";
            public const string VehicleTypeRequired = "Vehicle type is required";
            public const string VehicleManufacturerRequired = "Vehicle manufacturer is required";
            public const string VehicleModelRequired = "Vehicle model is required";
            public const string VehicleCapacityRequired = "Vehicle capacity is required";
            public const string VehicleNumberExists = "Vehicle with such number already exists";
            public const string InvalidVehicleStatus = "Such vehicle status is not supported!";

        }

        public static class ScheduleMessages
        {
            public const string InvalidScheduleDepartureTime = "Departure time must be earlier than arrival time!";
        }

        public static class TicketMessages
        {
            public const string TicketPurchaseSuccess = "Your ticket(s) have been purchased successfully!";
            public const string TicketPurchaseFail = "Ticket could not be purchased.";
            public const string TicketPurchaseNotification = "Закупихте билет: {0} → {1}";
            public const string TicketCancelSuccess = "Your ticket has been successfully cancelled.";
            public const string TicketCancelFail = "Ticket cannot be cancelled \u2013 departure is less than 24 hours away.";
            public const string InvalidTicketIssueTime = "Ticket issued date be earlier than expiry date!";
            public const string ScheduleCapacityExceeded = "Cannot create ticket: the vehicle for this schedule is fully booked!";
        }

        public static class OrganizationMessages
        {
            public const string OrganizationNameRequired = "Organization name is required!";
            public const string ImageTooLarge = "Image must be under 5 MB.";
            public const string InvalidImageFormat = "Only image files are allowed (.jpg, .png, .gif, .webp).";
            public const string ReviewSuccess = "Thank you for your review!";
            public const string ReviewFail = "You can only leave a review if you have traveled with this carrier, and you haven't already reviewed them.";
            public const string NewReviewNotification = "Имате нов отзив за \"{0}\"!";
            public const string FavoriteAddedNotification = "Добавихте \"{0}\" в любимите си.";
        }

        public static class ContactFormMessages
        {
            public const string ContactFormWasSumbitted = "Your form has been successfully sent to the staff.";
            public const string NewContactFormAlert = "Нова контактна форма е изпратена!";
            public const string ContactFormReplySuccess = "Your reply has been sent successfully.";
        }

        public static class GeneralMessages
        {
            public const string SuccessMessage = "The action has been successfully completed!";
            public const string FailMessage = "Unexpected error occurred while processing your request! Please contact administrator!";
        }
    }
}
