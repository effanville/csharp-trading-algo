using System;
using System.Collections.Generic;
using System.Text;
using TradingConsole.Statistics;

namespace TradingConsole.DecisionSystem
{
    public sealed class DecisionSystemParameters
    {
        public DecisionSystem DecisionSystemType
        {
            get;
        }

        public IReadOnlyList<StatisticType> Statistics
        {
            get;
        }

        public DecisionSystemParameters(DecisionSystem decisionSystemType, IReadOnlyList<StatisticType> statistics)
        {
            DecisionSystemType = decisionSystemType;
            Statistics = statistics;
        }
    }
}
