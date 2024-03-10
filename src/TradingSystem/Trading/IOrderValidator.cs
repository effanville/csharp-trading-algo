using System;

using Effanville.TradingStructures.Common.Services;
using Effanville.TradingStructures.Trading;

namespace Effanville.TradingSystem.Trading;

public interface IOrderListener : IService
{
    event EventHandler<TradeSubmittedEventArgs> SubmitTrade; 
    void OnTradeRequested(object? obj, TradeSubmittedEventArgs eventArgs);
    void OnTradeConfirmed(object? obj, TradeCompletedEventArgs eventArgs);
}