using System;

using Effanville.FinancialStructures.DataStructures;
using Effanville.TradingStructures.Common.Trading;

namespace Effanville.TradingStructures.OrderManagement;

/// <summary>
/// EventArgs for submitting a new trade to be enacted.
/// </summary>
/// <param name="Id"></param>
/// <param name="ReceivedTime"></param>
/// <param name="RequestedTrade"> The trade that is requested. </param>
/// <param name="ConfirmedTrade"></param>
/// <param name="TradeSuccessful"></param>
public record TradeCompletedEventArgs(
    Guid Id,
    DateTime ReceivedTime,
    Trade RequestedTrade,
    SecurityTrade? ConfirmedTrade,
    bool TradeSuccessful);
