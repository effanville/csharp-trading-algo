using Effanville.FinancialStructures.NamingStructures;
using Effanville.FinancialStructures.Stocks;

namespace Effanville.TradingStructures.Exchanges;

/// <summary>
/// Contains any static data about a stock, e.g. name, ticker.
/// Also contains any cached data about the instrument, and any stored
/// financials that are known.
/// </summary>
public sealed class StockInstrument
{
    public NameData Name { get; private set; }
    public StockInstrument(NameData name)
    {
        Name = name;
    }

    public StockInstrument(string ticker, string company, string name, string currency, string url)
        : this(new NameData(company.Trim(), name.Trim(), currency.Trim(), url.Trim()){Ticker = ticker})
    {
    }

    public StockInstrument(IStock stock)
        : this(stock.Name)
    {
    }
}
