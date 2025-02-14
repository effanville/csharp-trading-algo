﻿using System;

namespace Effanville.TradingStructures.Common.Time;

/// <summary>
/// An abstraction of a clock to enable retrieval of the current time
/// in both local and Utc normalisation.
/// </summary>
public interface IClock
{
    /// <summary>
    /// The time when the next event is going to fire.
    /// </summary>
    DateTime NextEventTime { get; set; }
    
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
    
    /// <summary>
    /// Stop the clock from running
    /// </summary>
    void Stop();
}