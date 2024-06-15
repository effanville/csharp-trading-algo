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

        public NewFiveDayStatsDecisionSystem(DecisionSystemFactory.Settings settings)
        {
            var newSettings = new DecisionSystemFactory.Settings(
                settings.DecisionSystemType,
                new List<StockStatisticType>()
                {
                    StockStatisticType.PrevDayOpen,
                    StockStatisticType.PrevTwoOpen,
                    StockStatisticType.PrevThreeOpen,
                    StockStatisticType.PrevFourOpen,
                    StockStatisticType.PrevFiveOpen
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