using System;

using Effanville.Common.Structure.Reporting;
using Effanville.FinancialStructures.DataStructures;
using Effanville.TradingStructures.Common.Services;
using Effanville.TradingStructures.Common.Trading;
using Effanville.TradingStructures.Pricing;

namespace Effanville.TradingStructures.Trading;

public interface IStockMarketAdapter : IService
{
    void OnTradeRequested(object? obj, TradeSubmittedEventArgs eventArgs);

    event EventHandler<TradeCompletedEventArgs>? TradeCompleted;
}