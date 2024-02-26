using Effanville.TradingStructures.Common.Trading;

namespace TradingSystem.Trading;

/// <summary>
/// EventArgs for submitting a new trade to be enacted.
/// </summary>
public class TradeSubmittedEventArgs
{
    /// <summary>
    /// The trade that is requested.
    /// </summary>
    public Trade RequestedTrade;
    public TradeSubmittedEventArgs(Trade requestedTrade)
    {
        RequestedTrade = requestedTrade;
    }
}