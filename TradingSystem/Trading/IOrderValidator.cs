using Effanville.TradingStructures.Common.Services;
using Effanville.TradingStructures.Trading;

namespace Effanville.TradingSystem.Trading;

public interface IOrderListener : IService
{
    void OnTradeRequested(object obj, TradeSubmittedEventArgs eventArgs);
}