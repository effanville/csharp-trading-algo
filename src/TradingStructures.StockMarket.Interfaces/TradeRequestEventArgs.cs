using System;

using Effanville.TradingStructures.Common.Trading;

namespace Effanville.TradingStructures.StockMarket;

/// <summary>
/// EventArgs for submitting a new trade to be enacted.
/// </summary>
/// <param name="Id"></param>
/// <param name="RequestedTrade"> The trade that is requested. </param>
public sealed record TradeRequestEventArgs(Guid Id, Trade RequestedTrade);
