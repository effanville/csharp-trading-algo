using System;

using Effanville.FinancialStructures.Stocks;

using TradingSystem.PriceSystem.Implementation;

namespace TradingSystem.PriceSystem
{
    public static class PriceServiceFactory
    {
        public static IPriceService Create(PriceType priceType, PriceCalculationSettings settings, IStockExchange exchange, Scheduler scheduler)
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