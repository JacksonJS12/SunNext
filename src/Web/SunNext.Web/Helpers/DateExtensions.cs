using System;

namespace SunNext.Web.Helpers;

public static class DateExtensions
{
    public static DateTime? ToDateTime(this string? input)
    {
        return DateTime.TryParse(input, out var dt) ? dt : null;
    }
}