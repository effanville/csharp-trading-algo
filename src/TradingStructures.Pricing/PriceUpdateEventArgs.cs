using System;

using Effanville.FinancialStructures.NamingStructures;
using Effanville.FinancialStructures.Stocks.Implementation;

namespace Effanville.TradingStructures.Pricing;

/// <summary>
/// Event args for events when a price has been updated.
/// </summary>
public class PriceUpdateEventArgs : EventArgs
{
    public NameData Instrument { get; }

    public DateTime Time { get; }
    public decimal Price { get; }

    public StockDay Candle { get; }

    public PriceUpdateEventArgs(DateTime time, NameData instrument, decimal price, StockDay candle)
    {
        Time = time;
        Instrument = instrument;
        Price = price;
        Candle = candle;
    }
}