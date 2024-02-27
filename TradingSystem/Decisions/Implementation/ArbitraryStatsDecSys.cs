using System;
using System.Collections.Generic;
using System.Linq;

using Effanville.Common.Structure.MathLibrary.ParameterEstimation;
using Effanville.Common.Structure.Reporting;

using Effanville.FinancialStructures.DataStructures;
using Effanville.FinancialStructures.Stocks;
using Effanville.FinancialStructures.Stocks.Statistics;
using Effanville.TradingStructures.Common.Trading;

using TradingSystem.MarketEvolvers;
using TradingSystem.Trading;

namespace TradingSystem.Decisions.Implementation
{
    /// <summary>
    /// A decision system consisting of arbitrary statistics. It has a least
    /// squares regression estimator to obtain the best fit to these
    /// statistics.
    /// </summary>
    internal sealed class ArbitraryStatsDecisionSystem : IDecisionSystem
    {
        private readonly DecisionSystemFactory.Settings fSettings;
        private readonly IReadOnlyList<IStockStatistic> fStockStatistics;
        private Estimator.Result EstimatorResult;

        /// <summary>
        /// Construct an instance.
        /// </summary>
        public ArbitraryStatsDecisionSystem(DecisionSystemFactory.Settings decisionParameters)
        {
            List<IStockStatistic> stockStatistics = new List<IStockStatistic>();
            foreach (StockStatisticType statistic in decisionParameters.Statistics)
            {
                stockStatistics.Add(StockStatisticFactory.Create(statistic));
            }
            fSettings = decisionParameters;
            fStockStatistics = stockStatistics;
        }

        /// <inheritdoc/>
        public void Calibrate(DecisionSystemSettings settings, IReportLogger logger)
        {
            DateTime burnInLength = settings.BurnInEnd;

            int delayTime = fStockStatistics.Max(stock => stock.BurnInTime) + 2;
            int numberEntries = ((burnInLength - settings.StartTime).Days - 5) * 5 / 7;
            int numberStatistics = fStockStatistics.Count;

            double[,] X = new double[settings.NumberStocks * numberEntries, numberStatistics];
            double[] Y = new double[settings.NumberStocks * numberEntries];
            for (int entryIndex = 0; entryIndex < numberEntries; entryIndex++)
            {
                for (int stockIndex = 0; stockIndex < settings.NumberStocks; stockIndex++)
                {
                    for (int statisticIndex = 0; statisticIndex < numberStatistics; statisticIndex++)
                    {
                        X[entryIndex * settings.Exchange.Stocks.Count + stockIndex, statisticIndex] = fStockStatistics[statisticIndex].Calculate(settings.StartTime.AddDays(delayTime + entryIndex), settings.Exchange.Stocks[stockIndex]);
                    }

                    Y[entryIndex * settings.Exchange.Stocks.Count + stockIndex] = Convert.ToDouble(settings.Exchange.Stocks[stockIndex].Values(burnInLength.AddDays(delayTime + entryIndex), 0, fSettings.DayAfterPredictor, StockDataStream.Open).Last() / 100m);
                }
            }

            var estimatorType = TypeHelpers.ConvertFrom(fSettings.DecisionSystemType);
            if (!estimatorType.Success)
            {
                EstimatorResult = Estimator.Fit(estimatorType.Data, X, Y);
            }
            else
            {
                _ = logger.Log(ReportSeverity.Critical, ReportType.Error, ReportLocation.Unknown, $"Created ArbitraryStats system without correct type.");
            }

            _ = logger.Log(ReportSeverity.Critical, ReportType.Warning, ReportLocation.Unknown, $"Estimator Weights are {string.Join(",", EstimatorResult.Estimator)}");
        }

        /// <inheritdoc/>
        public TradeCollection Decide(DateTime day, IStockExchange stockExchange, IReportLogger logger)
        {
            var decisions = new TradeCollection(day, day);
            foreach (IStock stock in stockExchange.Stocks)
            {
                TradeType decision = TradeType.Unknown;
                double[] values = stock.Values(day, 5, 0, StockDataStream.Open).Select(value => Convert.ToDouble(value)).ToArray();
                double value = EstimatorResult.Evaluate(values);

                if (value > fSettings.BuyThreshold)
                {
                    decision = TradeType.Buy;
                }
                else if (value < fSettings.SellThreshold)
                {
                    decision = TradeType.Sell;
                }

                decisions.Add(stock.Name, decision);
            }

            return decisions;
        }
    }
}
