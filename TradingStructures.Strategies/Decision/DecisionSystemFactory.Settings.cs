using System.Collections.Generic;

using Effanville.FinancialStructures.Stocks.Statistics;
using Effanville.TradingStructures.Strategies.Decision;

namespace Effanville.TradingStructures.Strategies.Decision
{
    public static partial class DecisionSystemFactory
    {
        public sealed class Settings
        {
            public DecisionSystem DecisionSystemType
            {
                get;
            }

            public IReadOnlyList<StockStatisticType> Statistics
            {
                get;
            }

            public int DayAfterPredictor
            {
                get;
            }

            public double BuyThreshold
            {
                get;
            }

            public double SellThreshold
            {
                get;
            }

            public Settings(
                DecisionSystem decisionSystemType,
                IReadOnlyList<StockStatisticType> statistics,
                double buyThreshold,
                double sellThreshold,
                int dayAfterPredictor)
                 : this(decisionSystemType)
            {
                Statistics = statistics;
                BuyThreshold = buyThreshold;
                SellThreshold = sellThreshold;
                DayAfterPredictor = dayAfterPredictor;
            }

            public Settings(DecisionSystem decisionSystemType)
            {
                DecisionSystemType = decisionSystemType;
            }

            /// <summary>
            /// Do the settings require the dates to use a burn in period.
            /// </summary>
            public bool IsBurnInRequired() => DecisionSystemType == DecisionSystem.BuyAll;
        }
    }
}
