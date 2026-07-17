using System.Collections.Generic;
using System.Linq;

using Effanville.FinancialStructures.NamingStructures;

namespace Effanville.TradingStructures.Strategies.Portfolio;

internal class StockSelector(StockSelectorSettings settings) : IStockSelector
{
    private readonly HashSet<string>? _applicableUniverse = settings.StockTickers?.ToHashSet();

    public bool IsStockInUniverse(NameData instrument)
        => _applicableUniverse == null || _applicableUniverse.Contains(instrument.Ticker);
}
