using System;

using Effanville.TradingStructures.Common.Services;
using Effanville.TradingStructures.Trading;

namespace Effanville.TradingStructures.OrderManagement;

public interface IOrderManagementService : IService
{
    /// <summary>
    /// Request a trade on the stock market.
    /// </summary>
    void OnTradeRequested(object? obj, TradeSubmittedEventArgs eventArgs);

    /// <summary>
    /// Callback detailing that a trade has been completed.
    /// </summary>
    event EventHandler<TradeCompletedEventArgs>? TradeCompleted;
}
