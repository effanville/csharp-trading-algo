using System.Collections.Generic;

using Effanville.FinancialStructures.NamingStructures;
using Effanville.FinancialStructures.Stocks;
using Effanville.TradingStructures.Common;

namespace Effanville.TradingStructures.StaticData;

internal sealed class StaticDataService : IStaticDataService
{
    public string Name => nameof(StaticDataService);

    public Dictionary<string, NameData> StockInstruments { get; } = new Dictionary<string, NameData>();

    public StaticDataService(IStockExchange stockExchange)
    {
        foreach (var stock in stockExchange.Stocks)
        {
            string ticker = stock.Name.Ticker;
            StockInstruments.Add(ticker, stock.Name);
        }
    }

    public void Initialize(EvolverSettings settings)
    {

    }
}