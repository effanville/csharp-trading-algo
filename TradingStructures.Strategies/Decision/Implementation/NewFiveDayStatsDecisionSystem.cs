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
        private readonly ArbitraryStatsDecisionSystem InnerSystem;

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
            InnerSystem = new ArbitraryStatsDecisionSystem(newSettings);
        }

        public void Calibrate(DecisionSystemSettings settings, IReportLogger logger)
        {
            InnerSystem.Calibrate(settings, logger);
        }

        public TradeCollection Decide(DateTime day, IStockExchange stockExchange, IReportLogger logger)
        {
            return InnerSystem.Decide(day, stockExchange, logger);
        }
    }
}
