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

        public DecisionSystemSetupSettings(
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
    }
}
