﻿using System;

using Effanville.FinancialStructures.Stocks;
using Effanville.TradingStructures.Common.Scheduling;
using Effanville.TradingStructures.Pricing.Implementation;

namespace Effanville.TradingStructures.Pricing;

public class PriceServiceFactory : IPriceServiceFactory
{
    public IPriceService Create(PriceCalculationSettings settings, IStockExchange exchange, IScheduler? scheduler)
        => settings.PriceType switch
        {
            PriceType.ExchangeFile => new ExchangeFilePriceService(exchange, scheduler),
            PriceType.RandomWobble => new RandomWobblePriceCalculator(settings, exchange, scheduler),
            _ => throw new ArgumentOutOfRangeException($"PriceType {settings.PriceType} not accepted."),
        };
}

