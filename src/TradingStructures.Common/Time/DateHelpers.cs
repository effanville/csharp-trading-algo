using System;

using Nager.Date;

namespace Effanville.TradingStructures.Common.Time
{
    public static class DateHelpers
    {
        public static bool IsCalcTimeValid(DateTime time, CountryCode countryCode)
            => (time.DayOfWeek != DayOfWeek.Saturday)
               && (time.DayOfWeek != DayOfWeek.Sunday)
               && !DateSystem.IsPublicHoliday(time, countryCode);
    }
}
