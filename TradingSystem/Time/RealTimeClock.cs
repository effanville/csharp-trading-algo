using System;

using Effanville.TradingStructures.Common.Time;

namespace TradingSystem.Time;

public sealed class RealTimeClock : IClock
{
    public RealTimeClock()
    {
    }

    /// <inheritdoc/>
    public DateTime Now() => DateTime.Now;

    /// <inheritdoc/>
    public DateTime UtcNow() => DateTime.UtcNow;

    /// <inheritdoc/>
    public DateTime NowInTimeZone(TimeZoneInfo timeZoneInfo)
    {
        var utcTime = DateTime.UtcNow;
        return TimeZoneInfo.ConvertTimeFromUtc(utcTime, timeZoneInfo);
    }

    /// <inheritdoc/>
    public void Start()
    {
    }
}