using System;
using System.Collections.Generic;
using System.Linq;

using Common.Structure.MathLibrary.ParameterEstimation;
using Common.Structure.Reporting;

using FinancialStructures.StockStructures;
using FinancialStructures.StockStructures.Statistics;

using TradingSystem.Decisions.Models;
using TradingSystem.Decisions.System;
using TradingSystem.Simulator;

namespace TradingConsole.DecisionSystem.Implementation
{
    /// <summary>
    /// A decision system consisting of arbitrary statistics. It has a least
    /// squares regression estimator to obtain the best fit to these
    /// statistics.
    /// </summary>
    public class ArbitraryStatsLSDecisionSystem : IDecisionSystem
    {
        private readonly IReadOnlyList<IStockStatistic> fStockStatistics;
        private IEstimator Estimator;

        /// <summary>
        /// Construct an instance.
        /// </summary>
        public ArbitraryStatsLSDecisionSystem(DecisionSystemSetupSettings decisionParameters)
        {
            List<IStockStatistic> stockStatistics = new List<IStockStatistic>();
            foreach (StockStatisticType statistic in decisionParameters.Statistics)
            {
                stockStatistics.Add(StockStatisticFactory.Create(statistic));
            }

            fStockStatistics = stockStatistics;
        }

        /// <inheritdoc/>
        public void Calibrate(SimulatorSettings settings, IReportLogger logger)
        {
            TimeSpan simulationLength = settings.EndTime - settings.StartTime;
            DateTime burnInLength = settings.BurnInEnd;

            int delayTime = fStockStatistics.Max(stock => stock.BurnInTime) + 2;
            int numberEntries = ((burnInLength - settings.StartTime).Days - 5) * 5 / 7;
            int numberStatistics = fStockStatistics.Count;

            double[,] X = new double[settings.Exchange.Stocks.Count * numberEntries, numberStatistics];
            double[] Y = new double[settings.Exchange.Stocks.Count * numberEntries];
            for (int entryIndex = 0; entryIndex < numberEntries; entryIndex++)
            {
                for (int stockIndex = 0; stockIndex < settings.Exchange.Stocks.Count; stockIndex++)
                {
                    for (int statisticIndex = 0; statisticIndex < numberStatistics; statisticIndex++)
                    {
                        X[entryIndex * settings.Exchange.Stocks.Count + stockIndex, statisticIndex] = fStockStatistics[statisticIndex].Calculate(settings.StartTime.AddDays(delayTime + entryIndex), settings.Exchange.Stocks[stockIndex]);
                    }

                    Y[entryIndex * settings.Exchange.Stocks.Count + stockIndex] = settings.Exchange.Stocks[stockIndex].Values(burnInLength.AddDays(delayTime + entryIndex), 0, 1, StockDataStream.Open).Last() / 100;
                }
            }

            Estimator = new LSEstimator(X, Y);
            _ = logger.Log(ReportSeverity.Critical, ReportType.Warning, ReportLocation.Unknown, $"Estimator Weights are {string.Join(",", Estimator.Estimator)}");
        }

        /// <inheritdoc/>
        public DecisionStatus Decide(DateTime day, IStockExchange stockExchange, IReportLogger logger)
        {
            var decisions = new DecisionStatus();
            foreach (IStock stock in stockExchange.Stocks)
            {
                TradeDecision decision;
                double[] values = stock.Values(day, 5, 0, StockDataStream.Open).ToArray();
                double value = Estimator.Evaluate(values);

                if (value > 1.05)
                {
                    decision = TradeDecision.Buy;
                }
                else if (value < 1)
                {
                    decision = TradeDecision.Sell;
                }
                else
                {
                    decision = TradeDecision.Hold;
                }

                decisions.AddDecision(stock.Name, decision);
            }

            return decisions;
        }
    }
}
