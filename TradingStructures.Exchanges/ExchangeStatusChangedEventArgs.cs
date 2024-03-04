namespace Effanville.TradingStructures.Exchanges;

/// <summary>
/// EventArgs for when an Exchange changes status.
/// </summary>
public sealed class ExchangeStatusChangedEventArgs : EventArgs
{
    public ExchangeSession PreviousSession
    {
        get;
    }

    public ExchangeSession NewSession
    {
        get;
    }

    public ExchangeStatusChangedEventArgs(ExchangeSession previous, ExchangeSession newSession)
    {
        PreviousSession = previous;
        NewSession = newSession;
    }
}