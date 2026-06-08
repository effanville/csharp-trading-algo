using System;

using Effanville.FinancialStructures.DataStructures;
using Effanville.TradingStructures.Common.Trading;

namespace Effanville.TradingStructures.StockMarket;

/// <summary>
/// EventArgs for submitting a new trade to be enacted.
/// </summary>
public class TradeResponseEventArgs
{
    public Guid Id { get; }
    public bool TradeSuccessful { get; }

    /// <summary>
    /// The trade that is requested.
    /// </summary>
    public Trade RequestedTrade { get; }

    public SecurityTrade? ConfirmedTrade { get; }
    public TradeResponseEventArgs(Guid guid, Trade requestedTrade, SecurityTrade? confirmedTrade, bool tradeSuccessful)
    {
        Id = guid;
        RequestedTrade = requestedTrade;
        ConfirmedTrade = confirmedTrade;
        TradeSuccessful = tradeSuccessful;
    }
}