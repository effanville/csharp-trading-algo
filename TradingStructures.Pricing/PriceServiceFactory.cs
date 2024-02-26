using System;

using Effanville.FinancialStructures.Stocks;
using Effanville.TradingStructures.Common.Scheduling;
using Effanville.TradingStructures.Pricing.Implementation;

namespace Effanville.TradingStructures.Pricing
{
    public static class PriceServiceFactory
    {
        public static IPriceService Create(PriceType priceType, PriceCalculationSettings settings, IStockExchange exchange, IScheduler scheduler)
        {
            return priceType switch
            {
                PriceType.ExchangeFile => new ExchangeFilePriceService(exchange, scheduler),
                PriceType.RandomWobble => new RandomWobblePriceCalculator(settings, exchange, scheduler),
                _ => throw new ArgumentOutOfRangeException($"PriceType {priceType} not accepted."),
            };
        }
    }
}