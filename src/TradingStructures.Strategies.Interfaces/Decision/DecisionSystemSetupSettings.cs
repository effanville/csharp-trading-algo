using System.Collections.Generic;

using Effanville.FinancialStructures.Stocks.Statistics;

namespace Effanville.TradingStructures.Strategies.Decision
{
    public sealed class DecisionSystemSetupSettings
    {
        public DecisionSystem DecisionSystemType { get; }

        public IReadOnlyList<StockStatisticSettings>? StatisticSettings { get; }

        public int DayAfterPredictor { get; }

        public double BuyThreshold { get; }

        public double SellThreshold { get; }

        public DecisionSystemSetupSettings(
            DecisionSystem decisionSystemType,
            IReadOnlyList<StockStatisticSettings>? statisticSettings,
            double buyThreshold,
            double sellThreshold,
            int dayAfterPredictor)
            : this(decisionSystemType)
        {
            StatisticSettings = statisticSettings;
            BuyThreshold = buyThreshold;
            SellThreshold = sellThreshold;
            DayAfterPredictor = dayAfterPredictor;
        }

        public DecisionSystemSetupSettings(DecisionSystem decisionSystemType)
        {
            DecisionSystemType = decisionSystemType;
        }

        public DecisionSystemSetupSettings() : this(DecisionSystem.FiveDayStatsLeastSquares) { }

        /// <summary>
        /// Do the settings require the dates to use a burn in period.
        /// </summary>
        public bool IsBurnInRequired() => DecisionSystemType == DecisionSystem.BuyAll;
    }
}