using FinancialStructures.StockStructures.Statistics;

using System.Collections.Generic;

namespace TradingConsole.DecisionSystem
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
            {
                DecisionSystemType = decisionSystemType;
                Statistics = statistics;
                BuyThreshold = buyThreshold;
                SellThreshold = sellThreshold;
                DayAfterPredictor = dayAfterPredictor;
            }

            /// <summary>
            /// Do the settings require the dates to use a burn in period.
            /// </summary>
            public bool IsBurnInRequired()
            {
                return DecisionSystemType == DecisionSystem.BuyAll;
            }
        }
    }
}
