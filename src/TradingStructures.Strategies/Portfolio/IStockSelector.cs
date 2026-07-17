using Effanville.FinancialStructures.NamingStructures;

namespace Effanville.TradingStructures.Strategies.Portfolio;

internal interface IStockSelector
{
    bool IsStockInUniverse(NameData instrument);
}
