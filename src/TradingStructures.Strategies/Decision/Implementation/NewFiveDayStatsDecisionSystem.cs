using System;
using System.Collections.Generic;

using Effanville.Common.Structure.Reporting;
using Effanville.FinancialStructures.Stocks;
using Effanville.FinancialStructures.Stocks.Statistics;
using Effanville.TradingStructures.Common.Trading;

namespace Effanville.TradingStructures.Strategies.Decision.Implementation
{
    internal sealed class NewFiveDayStatsDecisionSystem : IDecisionSystem
    {
        private readonly ArbitraryStatsDecisionSystem _innerSystem;

        public NewFiveDayStatsDecisionSystem(DecisionSystemSetupSettings settings)
        {
            var newSettings = new DecisionSystemSetupSettings(
                settings.DecisionSystemType,
                new List<StockStatisticSettings>()
                {
                    new StockStatisticSettings("PreviousDayOpen", 1, StockDataStream.Open),
                    new StockStatisticSettings("PreviousNDayValue", 2, StockDataStream.Open),
                    new StockStatisticSettings("PreviousNDayValue", 3, StockDataStream.Open),
                    new StockStatisticSettings("PreviousNDayValue", 4, StockDataStream.Open),
                    new StockStatisticSettings("PreviousNDayValue", 5, StockDataStream.Open)
                },
                settings.BuyThreshold,
                settings.SellThreshold,
                settings.DayAfterPredictor);
            _innerSystem = new ArbitraryStatsDecisionSystem(newSettings);
        }

        public void Calibrate(DecisionSystemSettings settings, IReportLogger? logger)
            => _innerSystem.Calibrate(settings, logger);

        public TradeCollection? Decide(DateTime day, IStockExchange stockExchange, IReportLogger? logger)
            => _innerSystem.Decide(day, stockExchange, logger);
    }
}