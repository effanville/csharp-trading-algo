using System;
using System.Collections.Generic;

using Common.Structure.Reporting;

using FinancialStructures.StockStructures;
using FinancialStructures.StockStructures.Statistics;

using TradingSystem.DecideThenTradeSystem;
using TradingSystem.Simulator;
using TradingSystem.Simulator.Trading.Decisions;

namespace TradingConsole.DecisionSystem.Implementation
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

        public void Calibrate(StockMarketEvolver.Settings settings, IReportLogger logger)
        {
            InnerSystem.Calibrate(settings, logger);
        }

        public DecisionStatus Decide(DateTime day, IStockExchange stockExchange, IReportLogger logger)
        {
            return InnerSystem.Decide(day, stockExchange, logger);
        }
    }
}
