namespace GoceTransportApp.Common
{
    public static class ErrorMessages
    {
        public static class StreetMessages
        {
            public const string StreetNameRequiredMessage = "Street name is required";

            public const string StreetEditFailed = "Unexpected error occurred while updating the street! Please contact administrator!";
            public const string StreetDeleteFailed = "Unexpected error occurred while deleting the street! Please contact administrator!";
        }

        public static class CityMessages
        {
            public const string CityNameRequiredMessage = "City name is required";
            public const string CityStateRequiredMessage = "City state is required";
            public const string CityZipRequiredMessage = "City zip is required";


            public const string CityEditFailed = "Unexpected error occurred while updating the city! Please contact administrator!";
            public const string CityDeleteFailed = "Unexpected error occurred while deleting the city! Please contact administrator!";
        }

        public static class RouteMessages
        {
            public const string RouteDistanceRequiredMessage = "Route distance is required";
            public const string RouteDurationRequiredMessage = "Route duration is required";

            public const string InvalidRouteDetails = "Unexpected error occurred while extracting route details! Please contact administrator!";
            public const string RouteEditFailed = "Unexpected error occurred while updating the route! Please contact administrator!";
            public const string RouteDeleteFailed = "Unexpected error occurred while deleting the route! Please contact administrator!";
            public const string RouteArchivationFailed = "Unexpected error occurred while archiving the route! Please contact administrator!";
        }

        public static class DriverMessages
        {
            public const string DriverFirstNameRequired = "Driver first name is required";
            public const string DriverLastNameRequired = "Driver last name is required";
            public const string DriverExperienceRequired = "Driver experience is required";
            public const string InvalidDriverDetails = "Unexpected error occurred while extracting driver details! Please contact administrator!";

            public const string InvalidDrivingExperience = "Invalid driving experience value provided.";
            public const string DriverEditFailed = "Unexpected error occurred while updating the driver! Please contact administrator!";
            public const string DriverDeleteFailed = "Unexpected error occurred while deleting the driver! Please contact administrator!";
        }

        public static class VehicleMessages
        {
            public const string VehicleNumberRequired = "Vehicle number is required";
            public const string VehicleTypeRequired = "Vehicle type is required";
            public const string VehicleManufacturerRequired = "Vehicle manufacturer is required";
            public const string VehicleModelRequired = "Vehicle model is required";
            public const string VehicleCapacityRequired = "Vehicle capacity is required";

            public const string InvalidVehicleDetails = "Unexpected error occurred while extracting vehicle details! Please contact administrator!";

            public const string InvalidVehicleStatus = "Invalid vehicle status value provided.";
            public const string VehicleEditFailed = "Unexpected error occurred while updating the vehicle! Please contact administrator!";
            public const string VehicleDeleteFailed = "Unexpected error occurred while deleting the vehicle! Please contact administrator!";
        }

        public static class ScheduleMessages
        {
            public const string InvalidScheduleDetails = "Unexpected error occurred while extracting schedule details! Please contact administrator!";
            public const string ScheduleEditFailed = "Unexpected error occurred while updating the schedule! Please contact administrator!";
            public const string ScheduleDeleteFailed = "Unexpected error occurred while deleting the schedule! Please contact administrator!";
        }

        public static class TicketMessages
        {
            public const string InvalidTicketDetails = "Unexpected error occurred while extracting ticket details! Please contact administrator!";
            public const string TicketEditFailed = "Unexpected error occurred while updating the ticket! Please contact administrator!";
            public const string TicketDeleteFailed = "Unexpected error occurred while deleting the ticket! Please contact administrator!";
            public const string TicketPurchaseFailed = "There was an error processing your ticket purchase!";
            public const string TicketPurchaseSuccess = "Your ticket(s) have been purchased successfully!";
        }

        public static class OrganizationMessages
        {
            public const string OrganizationNameRequired = "Organization name is required!";

            public const string InvalidOrganizationDetails = "Unexpected error occurred while extracting organization details! Please contact administrator!";
            public const string OrganizationEditFailed = "Unexpected error occurred while updating the organization! Please contact administrator!";
            public const string OrganizationDeleteFailed = "Unexpected error occurred while deleting the organization! Please contact administrator!";
            public const string OrganizationPurchaseFailed = "There was an error processing your ticket purchase!";
            public const string OrganizationPurchaseSuccess = "Your ticket(s) have been purchased successfully!";
        }
    }
}
