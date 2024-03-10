using Effanville.FinancialStructures.DataStructures;
using Effanville.TradingStructures.Common.Trading;

namespace Effanville.TradingStructures.Trading;

/// <summary>
/// EventArgs for submitting a new trade to be enacted.
/// </summary>
public class TradeCompletedEventArgs
{
    public bool TradeSuccessful { get; }
    
    /// <summary>
    /// The trade that is requested.
    /// </summary>
    public Trade RequestedTrade { get; }
    
    public SecurityTrade? ConfirmedTrade { get; }
    public TradeCompletedEventArgs(Trade requestedTrade, SecurityTrade? confirmedTrade, bool tradeSuccessful)
    {
        RequestedTrade = requestedTrade;
        ConfirmedTrade = confirmedTrade;
        TradeSuccessful = tradeSuccessful;
    }
}