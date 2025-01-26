namespace GoceTransportApp.Common
{
    public static class ResultMessages
    {
        public static class StreetMessages
        {
            public const string StreetNameRequiredMessage = "Street name is required";

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
            public const string InvalidTicketIssueTime = "Ticket issued date be earlier than expiry date!";
        }

        public static class OrganizationMessages
        {
            public const string OrganizationNameRequired = "Organization name is required!";
        }

        public static class ContactFormMessages
        {
            public const string ContactFormWasSumbitted = "Your form has been successfully sent to the staff.";
        }

        public static class GeneralMessages
        {
            public const string SuccessMessage = "The action has been successfully completed!";
            public const string FailMessage = "Unexpected error occurred while processing your request! Please contact administrator!";

        }
    }
}
