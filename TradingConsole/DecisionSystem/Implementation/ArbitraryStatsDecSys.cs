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
    public class ArbitraryStatsDecisionSystem : IDecisionSystem
    {
        private readonly DecisionSystem fDecisionType;
        private readonly IReadOnlyList<IStockStatistic> fStockStatistics;
        private readonly int fDayAfterPredictor;
        private IEstimator Estimator;
        private double fSellThreshold;
        private double fBuyThreshold;

        /// <summary>
        /// Construct an instance.
        /// </summary>
        public ArbitraryStatsDecisionSystem(DecisionSystemSetupSettings decisionParameters)
        {
            List<IStockStatistic> stockStatistics = new List<IStockStatistic>();
            foreach (StockStatisticType statistic in decisionParameters.Statistics)
            {
                stockStatistics.Add(StockStatisticFactory.Create(statistic));
            }

            fDayAfterPredictor = decisionParameters.DayAfterPredictor;
            fStockStatistics = stockStatistics;
            fDecisionType = decisionParameters.DecisionSystemType;
            fSellThreshold = decisionParameters.SellThreshold;
            fBuyThreshold = decisionParameters.BuyThreshold;
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

                    Y[entryIndex * settings.Exchange.Stocks.Count + stockIndex] = Convert.ToDouble(settings.Exchange.Stocks[stockIndex].Values(burnInLength.AddDays(delayTime + entryIndex), 0, fDayAfterPredictor, StockDataStream.Open).Last() / 100m);
                }
            }
            switch (fDecisionType)
            {
                case DecisionSystem.FiveDayStatsLeastSquares:
                    Estimator = new LSEstimator(X, Y);
                    break;
                case DecisionSystem.FiveDayStatsLasso:
                    Estimator = new LassoRegression(X, Y);
                    break;
                case DecisionSystem.FiveDayStatsRidge:
                    Estimator = new RidgeRegression(X, Y);
                    break;
                default:
                    _ = logger.Log(ReportSeverity.Critical, ReportType.Error, ReportLocation.Unknown, $"Created FiveDayStats system without five day stats type.");
                    break;
            }
            _ = logger.Log(ReportSeverity.Critical, ReportType.Warning, ReportLocation.Unknown, $"Estimator Weights are {string.Join(",", Estimator.Estimator)}");
        }

        /// <inheritdoc/>
        public DecisionStatus Decide(DateTime day, IStockExchange stockExchange, IReportLogger logger)
        {
            var decisions = new DecisionStatus();
            foreach (IStock stock in stockExchange.Stocks)
            {
                TradeDecision decision;
                double[] values = stock.Values(day, 5, 0, StockDataStream.Open).Select(value => Convert.ToDouble(value)).ToArray();
                double value = Estimator.Evaluate(values);

                if (value > fBuyThreshold)
                {
                    decision = TradeDecision.Buy;
                }
                else if (value < fSellThreshold)
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
