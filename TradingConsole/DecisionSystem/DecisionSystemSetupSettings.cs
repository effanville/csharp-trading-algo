using System.Collections.Generic;

using FinancialStructures.StockStructures.Statistics;

namespace TradingConsole.DecisionSystem
{
    public sealed class DecisionSystemSetupSettings
    {
        public DecisionSystem DecisionSystemType
        {
            get;
        }

        public IReadOnlyList<StockStatisticType> Statistics
        {
            get;
        }

        public DecisionSystemSetupSettings(DecisionSystem decisionSystemType, IReadOnlyList<StockStatisticType> statistics)
        {
            DecisionSystemType = decisionSystemType;
            Statistics = statistics;
        }
    }
}
