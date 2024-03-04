using System;

using Effanville.TradingStructures.Common.Time;

namespace TradingSystem.Time;

/// <summary>
/// Implementation of a <see cref="IClock"/> for simulation purposes.
/// </summary>
public sealed class SimulationEventBasedClock : IClock
{
    public DateTime NextEventTime
    {
        get;
        set;
    }

    /// <summary>
    /// Construct an instance
    /// </summary>
    public SimulationEventBasedClock(DateTime startTime)
    {
        NextEventTime = startTime;
    }

    /// <inheritdoc/>
    public DateTime Now() => NextEventTime.ToLocalTime();
    public DateTime UtcNow() => NextEventTime.ToUniversalTime();

    /// <inheritdoc/>
    public DateTime NowInTimeZone(TimeZoneInfo timeZoneInfo)
    {
        var utcTime = NextEventTime.ToUniversalTime();
        return TimeZoneInfo.ConvertTimeFromUtc(utcTime, timeZoneInfo);
    }

    /// <inheritdoc/>
    public void Start() { }
}