using System;
using System.Collections.Generic;

using Effanville.Common.Structure.Reporting;

using Effanville.FinancialStructures.Stocks;
using Effanville.FinancialStructures.Stocks.Statistics;

using TradingSystem.MarketEvolvers;
using TradingSystem.Trading;

namespace TradingSystem.Decisions.Implementation
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

        public void Calibrate(TimeIncrementEvolverSettings settings, IReportLogger logger)
        {
            InnerSystem.Calibrate(settings, logger);
        }

        public TradeCollection Decide(DateTime day, IStockExchange stockExchange, IReportLogger logger)
        {
            return InnerSystem.Decide(day, stockExchange, logger);
        }
    }
}
