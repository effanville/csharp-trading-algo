﻿using System;
using System.Timers;

namespace TradingSystem.Time;

/// <summary>
/// Implementation of a <see cref="IClock"/> for simulation purposes.
/// </summary>
public sealed class SimulationClock : IClock
{
    private bool _started = false;
    private long _ticks;
    private readonly long _increment;
    private readonly Timer _timer;

    /// <summary>
    /// Construct an instance
    /// </summary>
    public SimulationClock(DateTime startTime, int timerDelay = 50, long increment = 4320000000)
    {
        _ticks = startTime.Ticks;
        _increment = increment;
        _timer = new Timer(timerDelay);
        _timer.Elapsed += OnTimedEvent;
    }

    private void OnTimedEvent(object source, ElapsedEventArgs e) => _ticks += _increment;

    /// <inheritdoc/>
    public DateTime Now() => new DateTime(_ticks, DateTimeKind.Local);
    public DateTime UtcNow() => new DateTime(_ticks, DateTimeKind.Utc);

    /// <inheritdoc/>
    public DateTime NowInTimeZone(TimeZoneInfo timeZoneInfo)
    {
        var utcTime = new DateTime(_ticks, DateTimeKind.Utc);
        return TimeZoneInfo.ConvertTimeFromUtc(utcTime, timeZoneInfo);
    }

    /// <inheritdoc/>
    public void Start()
    {
        _started = true;
        _timer.Enabled = true;
    }
}