using System;

namespace Effanville.TradingStructures.Trading;

public interface IMarketExchange
{
    void OnTradeRequested(object obj, TradeSubmittedEventArgs eventArgs);

    event EventHandler<TradeCompletedEventArgs> TradeCompleted;
}