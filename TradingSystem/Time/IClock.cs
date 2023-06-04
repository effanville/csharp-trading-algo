using System;

namespace TradingSystem.Time;

/// <summary>
/// An abstraction of a clock to enable retrieval of the current time
/// in both local and Utc normalisation.
/// </summary>
public interface IClock
{
    /// <summary>
    /// Get the current time in the local time.
    /// </summary>
    DateTime Now();

    /// <summary>
    /// Get the current time as in the UTC time zone.
    /// </summary>
    DateTime UtcNow();

    /// <summary>
    /// Get the current time in the time zone desired.
    /// </summary>
    DateTime NowInTimeZone(TimeZoneInfo timeZoneInfo);

    /// <summary>
    /// Start the clock running
    /// </summary>
    void Start();
}