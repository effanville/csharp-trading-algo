using System;

namespace TradingSystem.Time;

/// <summary>
/// Event args for when a time increment has occurred.
/// </summary>
public class TimeIncrementEventArgs : EventArgs
{
    public DateTime Time
    {
        get; set;
    }
    public TimeIncrementEventArgs(DateTime time)
        : base()
    {
        Time = time;
    }
}