﻿namespace GoceTransportApp.Common
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
            public const string CityNameRequiredMessage = "Street name is required";

            public const string CityEditFailed = "Unexpected error occurred while updating the city! Please contact administrator!";
            public const string CityDeleteFailed = "Unexpected error occurred while deleting the city! Please contact administrator!";
        }
        public static class RouteMessages
        {
            public const string RouteDistanceRequiredMessage = "Route distance is required";
            public const string RouteDurationRequiredMessage = "Route duration is required";


            public const string RouteEditFailed = "Unexpected error occurred while updating the route! Please contact administrator!";
            public const string RouteDeleteFailed = "Unexpected error occurred while deleting the route! Please contact administrator!";
        }
    }
}
