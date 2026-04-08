using System.Collections.Generic;

using Effanville.FinancialStructures.NamingStructures;
using Effanville.TradingStructures.Common.Services;

namespace Effanville.TradingStructures.StaticData;

public interface IStaticDataService : IService
{
    Dictionary<string, NameData> StockInstruments { get; }
}
