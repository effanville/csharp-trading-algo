using System;

using FinancialStructures.NamingStructures;
using FinancialStructures.StockStructures.Implementation;

namespace TradingSystem.PriceSystem
{
    /// <summary>
    /// Represents a service for detailing querying of price and
    /// subscribing to real time price updates.
    /// </summary>
    public interface IPriceService
    {
        decimal GetPrice(DateTime time, string ticker);
        decimal GetPrice(DateTime time, NameData name);
        decimal GetBidPrice(DateTime time, string ticker);
        decimal GetAskPrice(DateTime time, string ticker);
        decimal GetBidPrice(DateTime time, TwoName stock);
        decimal GetAskPrice(DateTime time, TwoName stock);
        StockDay GetCandle(DateTime time, TwoName name);
    }
}
