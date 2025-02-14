﻿using System;

using Effanville.FinancialStructures.NamingStructures;
using Effanville.FinancialStructures.Stocks.Implementation;
using Effanville.TradingStructures.Common.Services;

namespace Effanville.TradingStructures.Pricing
{
    /// <summary>
    /// Represents a service for detailing querying of price and
    /// subscribing to real time price updates.
    /// </summary>
    public interface IPriceService : IService
    {
        event EventHandler<PriceUpdateEventArgs> PriceChanged;
        decimal GetPrice(DateTime time, string ticker);
        decimal GetPrice(DateTime time, NameData name);
        decimal GetBidPrice(DateTime time, string ticker);
        decimal GetAskPrice(DateTime time, string ticker);
        decimal GetBidPrice(DateTime time, TwoName stock);
        decimal GetAskPrice(DateTime time, TwoName stock);
        StockDay GetCandle(DateTime startTime, DateTime endTime, TwoName name);
    }
}
