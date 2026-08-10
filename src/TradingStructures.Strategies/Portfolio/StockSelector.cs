using System.Collections.Generic;
using System.Linq;

using Effanville.FinancialStructures.NamingStructures;

using Microsoft.Extensions.Options;

namespace Effanville.TradingStructures.Strategies.Portfolio;

internal class StockSelector(IOptionsSnapshot<StockSelectorSettings> options) : IStockSelector
{
    private readonly HashSet<string>? _applicableUniverse = options.Value.StockTickers?.ToHashSet();

    public bool IsStockInUniverse(NameData instrument)
        => _applicableUniverse == null || _applicableUniverse.Contains(instrument.Ticker);
}
