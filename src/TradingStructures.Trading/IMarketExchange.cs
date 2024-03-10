using System;

using Effanville.Common.Structure.Reporting;
using Effanville.FinancialStructures.DataStructures;
using Effanville.TradingStructures.Common.Trading;
using Effanville.TradingStructures.Pricing;

namespace Effanville.TradingStructures.Trading;

public interface IMarketExchange
{
    void OnTradeRequested(object? obj, TradeSubmittedEventArgs eventArgs);

    event EventHandler<TradeCompletedEventArgs>? TradeCompleted;
    
    /// <summary>
    /// Enact a trade which was submitted at a given time.
    /// </summary>
    SecurityTrade? Trade(
        DateTime time,
        Trade trade,
        IPriceService priceService,
        decimal availableFunds,
        IReportLogger reportLogger);
}