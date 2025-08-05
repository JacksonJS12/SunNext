using System;

namespace SunNext.Common
{
    public static class GlobalConstants
    {
        public const string SystemName = "SunNext";

        public const string AdministratorRoleName = "Administrator";
        
        public const int DefaultPage = 1;
        public const int EntitiesPerPage = 3;
        
        public const string dateFormat = "dd/MM/yyyy";
        
        public static TimeZoneInfo bulgariaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. Europe Standard Time"); // Bulgaria timezone
        public static DateTime TodayEESTTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, bulgariaTimeZone).Date;

    }
}
