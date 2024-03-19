using System;

using Effanville.TradingStructures.Common.Trading;

namespace Effanville.TradingStructures.Trading;

/// <summary>
/// EventArgs for submitting a new trade to be enacted.
/// </summary>
public sealed class TradeSubmittedEventArgs
{
    public DateTime Time { get; set; }
    public decimal AvailableFunds { get; set; }
    
    /// <summary>
    /// The trade that is requested.
    /// </summary>
    public Trade RequestedTrade { get; set; }
    
    public TradeSubmittedEventArgs(Trade requestedTrade)
    {
        RequestedTrade = requestedTrade;
    }
    
    public TradeSubmittedEventArgs(Trade requestedTrade, decimal availableFunds)
    {
        RequestedTrade = requestedTrade;
        AvailableFunds = availableFunds;
    }
}