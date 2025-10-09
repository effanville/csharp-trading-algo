using System;

using Effanville.FinancialStructures.DataStructures;
using Effanville.TradingStructures.Common.Trading;

namespace Effanville.TradingStructures.OrderManagement;

/// <summary>
/// EventArgs for submitting a new trade to be enacted.
/// </summary>
public class TradeCompletedEventArgs
{
    public Guid Id { get; }
    public DateTime ReceivedTime { get; }
    public bool TradeSuccessful { get; }

    /// <summary>
    /// The trade that is requested.
    /// </summary>
    public Trade RequestedTrade { get; }

    public SecurityTrade? ConfirmedTrade { get; }
    public TradeCompletedEventArgs(Guid guid, DateTime time, Trade requestedTrade, SecurityTrade? confirmedTrade, bool tradeSuccessful)
    {
        Id = guid;
        ReceivedTime = time;
        RequestedTrade = requestedTrade;
        ConfirmedTrade = confirmedTrade;
        TradeSuccessful = tradeSuccessful;
    }
}