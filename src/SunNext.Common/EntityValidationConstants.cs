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
        public static class MarketTrade
        {
            public const int StrategyTagMaxLength = 100;

            public const int HourMin = 0;
            public const int HourMax = 23;

            public const decimal AmountMin = 0;
            public const decimal PriceMin = 0;
            public const decimal ProfitMin = 0;
        }
        public static class DailyPVTradingPosition
        {
            public const decimal EnergyUsedMin = 0;
            public const decimal AvgPriceMin = 0;
            public const decimal ProfitMin = 0;
        }

        public static class Battery
        {
            public const int TypeMaxLength = 50;

            public const double MinCapacityKWh = 0.1;
            public const double MaxCapacityKWh = 10000;
            
            public const int MinLocationLength = 5;
            public const int MaxLocationLength = 20;

            public const int MinModelLength = 2;
            public const int MaxModelLength = 50;
        }

    }
}