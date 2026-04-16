using System;
using System.Collections.Generic;


using Effanville.Common.Structure.MathLibrary.ParameterEstimation;
using Effanville.FinancialStructures.Stocks;
using Effanville.FinancialStructures.Stocks.Statistics;
using Effanville.TradingStructures.Common.Trading;

using Microsoft.Extensions.Logging;

namespace Effanville.TradingStructures.Strategies.Decision.Implementation
{
    internal sealed class NewFiveDayStatsDecisionSystem : ICalibratedDecisionSystem
    {
        private readonly ArbitraryStatsDecisionSystem _innerSystem;

        public int MinBurnInPeriod => 5 * 25;

        public Estimator.Result? Result => _innerSystem.Result;

        public NewFiveDayStatsDecisionSystem(DecisionSystemFactory.Settings settings, ILoggerFactory loggerFactory)
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
            _innerSystem = new ArbitraryStatsDecisionSystem(newSettings, loggerFactory.CreateLogger<ArbitraryStatsDecisionSystem>());
        }

        public void Calibrate(DecisionSystemSettings settings)
            => _innerSystem.Calibrate(settings);

        public TradeCollection? Decide(DateTime day, IStockExchange stockExchange)
            => _innerSystem.Decide(day, stockExchange);
    }
}