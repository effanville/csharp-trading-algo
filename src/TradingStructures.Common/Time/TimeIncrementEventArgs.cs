using System;

namespace Effanville.TradingStructures.Common.Time;

/// <summary>
/// Event args for when a time increment has occurred.
/// </summary>
public class TimeIncrementEventArgs : EventArgs
{
    public DateTime Time
    {
        get;
    }
    public TimeIncrementEventArgs(DateTime time)
    {
        Time = time;
    }
}