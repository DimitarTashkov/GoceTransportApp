namespace GoceTransportApp.Common
{
    public static class EntityValidationConstants
    {

        public static class UserConstants
        {
            public const int MinNameLength = 4;
            public const int MaxNameLength = 50;
        }

        public static class CityConstants
        {
            public const int MinNameLength = 4;
            public const int MaxNameLength = 50;
            public const int MinStateLength = 4;
            public const int MaxStateLength = 50;
            public const int MinCountryLength = 4;
            public const int MaxCountryLength = 50;
            public const int MinZipCodeLength = 4;
            public const int MaxZipCodeLength = 20;
        }

        public static class StreetConstants
        {
            public const int MinStreetLength = 4;
            public const int MaxStreetLength = 50;
        }

        public static class DriverConstants
        {
            public const int MinNameLength = 3;
            public const int MaxNameLength = 50;
            public const int MinAgeLength = 1;
            public const int MaxAgeLength = 100;
        }

        public static class VehicleConstants
        {
            public const int MinNumberLength = 4;
            public const int MaxNumberLength = 10;
            public const int MinTypeLength = 3;
            public const int MaxTypeLength = 20;
            public const int MinManufacturerLength = 3;
            public const int MaxManufacturerLength = 50;
            public const int MinModelLength = 3;
            public const int MaxModelLength = 50;
            public const int MinCapacityLength = 1;
            public const int MaxCapacityLength = 100;
            public const int MinFuelConsumptionLength = 3;
            public const int MaxFuelConsumptionLength = 50;
        }

        public static class OrganizationConstants
        {
            public const int MinNameLength = 5;
            public const int MaxNameLength = 50;
            public const int MinAddressLength = 5;
            public const int MaxAddressLength = 50;
            public const int MinPhoneNumberLength = 7;
            public const int MaxPhoneNumberLength = 15;
        }

        public static class MessageConstants
        {
            public const int MinTitleLength = 5;
            public const int MaxTitleLength = 50;
            public const int MinContentLength = 10;
            public const int MaxContentLength = 200;
        }
    }
}
