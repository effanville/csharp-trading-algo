using System.Collections.Generic;

using Effanville.FinancialStructures.Stocks.Statistics;

namespace Effanville.TradingStructures.Strategies.Decision
{
    public partial class DecisionSystemFactory
    {
        public sealed class Settings
        {
            public const string OptionsName = "DecisionSystemSettings";

            public DecisionSystem DecisionSystemType { get; set; }

            public IReadOnlyList<StockStatisticType>? Statistics { get; set; }

            public int DayAfterPredictor { get; set; }

            public double BuyThreshold { get; set; }

            public double SellThreshold { get; set; }
        }
    }
}
