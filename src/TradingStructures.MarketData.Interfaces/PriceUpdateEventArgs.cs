using System;

using Effanville.FinancialStructures.NamingStructures;
using Effanville.FinancialStructures.Stocks.Implementation;

namespace Effanville.TradingStructures.MarketData;

/// <summary>
/// Event args for events when a price has been updated.
/// </summary>
public class PriceUpdateEventArgs(
    DateTime time,
    NameData instrument,
    decimal price,
    StockDay candle)
    : EventArgs
{
    public NameData Instrument { get; } = instrument;

    public DateTime Time { get; } = time;
    public decimal Price { get; } = price;

    public StockDay Candle { get; } = candle;
}