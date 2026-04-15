using System;

using Effanville.TradingStructures.Common.Services;

namespace Effanville.TradingStructures.Trading;

public interface IStockMarketAdapter : IService
{
    /// <summary>
    /// Request a trade on the stock market.
    /// </summary>
    void OnTradeRequested(object? obj, TradeRequestEventArgs eventArgs);

    /// <summary>
    /// Callback detailing that a trade has been completed.
    /// </summary>
    event EventHandler<TradeResponseEventArgs>? TradeCompleted;

}