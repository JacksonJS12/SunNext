namespace SunNext.Common
{
    public class EntityValidationConstants
    {
        public static class SolarAsset
        {
            public const int NameMaxLength = 100;
            public const int TypeMaxLength = 50;
            public const int InstallerNameMaxLength = 100;
            public const int InstallerEmailMaxLength = 100;
            public const int InstallerPhoneMaxLength = 30;
            public const int TimeZoneMaxLength = 100;
            public const int AddressMaxLength = 200;
            public const int ImageUrlMaxLength = 300;

            public const double PowerMin = 0;
            public const double CapacityMin = 0;
            public const double EfficiencyMin = 0;
            public const double EfficiencyMax = 100;

            public const double EnergyMin = 0;
        }
    }
}