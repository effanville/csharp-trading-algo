using Effanville.FinancialStructures.Stocks;
using Effanville.TradingStructures.Common.Scheduling;

namespace Effanville.TradingStructures.MarketData;

public interface IPriceServiceFactory
{
    IPriceService Create(PriceCalculationSettings settings, IStockExchange exchange, IScheduler? scheduler);
}