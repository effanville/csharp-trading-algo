using System;

namespace Effanville.TradingStructures.Common.Time;

internal sealed class RealTimeClock : IClock
{
    public DateTime NextEventTime { get; set; }

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
    public void Start() { }
    
    /// <inheritdoc/>
    public void Stop() { }
}