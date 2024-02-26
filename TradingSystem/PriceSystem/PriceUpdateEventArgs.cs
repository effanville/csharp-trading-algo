using System;

using Effanville.FinancialStructures.Stocks.Implementation;
using Effanville.TradingStructures.Exchanges;

namespace TradingSystem.PriceSystem;

/// <summary>
/// Event args for events when a price has been updated.
/// </summary>
public class PriceUpdateEventArgs : EventArgs
{
    public StockInstrument Instrument
    {
        get;
    }

    public DateTime Time
    {
        get;
    }
    public decimal Price
    {
        get;
    }

    public StockDay Candle
    {
        get;
    }

    public PriceUpdateEventArgs(DateTime time, StockInstrument instrument, decimal price, StockDay candle)
    {
        Time = time;
        Instrument = instrument;
        Price = price;
        Candle = candle;
    }
}