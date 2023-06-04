using FinancialStructures.NamingStructures;
using FinancialStructures.StockStructures;

namespace TradingSystem.ExchangeStructures;

/// <summary>
/// Contains any static data about a stock, e.g. name, ticker.
/// Also contains any cached data about the instrument, and any stored
/// financials that are known.
/// </summary>
public sealed class StockInstrument
{
    public string Ticker;
    public NameData Name;
    public StockInstrument(string ticker, NameData name)
    {
        Ticker = ticker;
        Name = name;
    }

    public StockInstrument(string ticker, string company, string name, string currency, string url)
        : this(ticker, new NameData(company.Trim(), name.Trim(), currency.Trim(), url.Trim()))
    {
    }

    public StockInstrument(IStock stock)
        : this(stock.Ticker, stock.Name)
    {
    }
}
