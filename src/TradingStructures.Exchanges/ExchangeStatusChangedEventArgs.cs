using System;

namespace Effanville.TradingStructures.Exchanges;

/// <summary>
/// EventArgs for when an Exchange changes status.
/// </summary>
public sealed class ExchangeStatusChangedEventArgs : EventArgs
{
    public DateTime Time { get; }
    public ExchangeSession PreviousSession { get; }

    public ExchangeSession NewSession { get; }

    public ExchangeStatusChangedEventArgs(DateTime time, ExchangeSession previous, ExchangeSession newSession)
    {
        Time = time;
        PreviousSession = previous;
        NewSession = newSession;
    }
}