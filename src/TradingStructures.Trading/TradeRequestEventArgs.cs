using System;

using Effanville.TradingStructures.Common.Trading;

namespace Effanville.TradingStructures.Trading;

/// <summary>
/// EventArgs for submitting a new trade to be enacted.
/// </summary>
public sealed class TradeRequestEventArgs
{
    public Guid Id { get; set; }

    /// <summary>
    /// The trade that is requested.
    /// </summary>
    public Trade RequestedTrade { get; set; }

    public TradeRequestEventArgs(Guid id, Trade requestedTrade)
    {
        Id = id;
        RequestedTrade = requestedTrade;
    }
}