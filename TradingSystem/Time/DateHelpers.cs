using System;

using Nager.Date;

namespace TradingSystem.Time
{
    internal static class DateHelpers
    {
        internal static bool IsCalcTimeValid(DateTime time, CountryCode countryCode)
        {
            return (time.DayOfWeek != DayOfWeek.Saturday)
                && (time.DayOfWeek != DayOfWeek.Sunday)
                && !DateSystem.IsPublicHoliday(time, countryCode);
        }
    }
}
