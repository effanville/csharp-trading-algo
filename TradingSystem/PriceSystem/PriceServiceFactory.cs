using System;

using FinancialStructures.StockStructures;

using TradingSystem.PriceSystem.Implementation;

namespace TradingSystem.PriceSystem
{
    public static class PriceServiceFactory
    {
        public static IPriceService Create(PriceType priceType, PriceCalculationSettings settings, IStockExchange exchange)
        {
            return priceType switch
            {
                PriceType.ExchangeFile => new ExchangeFilePriceService(exchange),
                PriceType.RandomWobble => new RandomWobblePriceCalculator(settings, exchange),
                _ => throw new ArgumentOutOfRangeException($"PriceType {priceType} not accepted."),
            };
        }
    }
}