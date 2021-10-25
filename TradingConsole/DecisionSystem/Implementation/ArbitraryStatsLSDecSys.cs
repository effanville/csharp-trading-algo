using System;
using System.Collections.Generic;
using System.Linq;
using FinancialStructures.StockStructures;
using Common.Structure.MathLibrary.ParameterEstimation;
using TradingConsole.Simulator;
using TradingConsole.DecisionSystem.Models;
using FinancialStructures.StockStructures.Statistics;
using Common.Structure.Reporting;

namespace TradingConsole.DecisionSystem.Implementation
{
    /// <summary>
    /// A decision system consisting of arbitrary statistics. It has a least
    /// squares regression estimator to obtain the best fit to these
    /// statistics.
    /// </summary>
    public class ArbitraryStatsLSDecisionSystem : IDecisionSystem
    {
        private IEstimator Estimator;

        /// <summary>
        /// Construct an instance.
        /// </summary>
        public ArbitraryStatsLSDecisionSystem()
        {
        }

        /// <inheritdoc/>
        public void Calibrate(DecisionSystemSetupSettings decisionParameters, SimulatorSettings settings)
        {
            List<IStockStatistic> stockStatistics = new List<IStockStatistic>();
            foreach (StockStatisticType statistic in decisionParameters.Statistics)
            {
                stockStatistics.Add(StockStatisticFactory.Create(statistic));
            }

            TimeSpan simulationLength = settings.EndTime - settings.StartTime;
            DateTime burnInLength = settings.StartTime + settings.EvolutionIncrement * (long)(simulationLength / (2 * settings.EvolutionIncrement));

            int delayTime = stockStatistics.Max(stock => stock.BurnInTime) + 2;
            int numberEntries = ((burnInLength - settings.StartTime).Days - 5) * 5 / 7;
            int numberStatistics = stockStatistics.Count;

            double[,] X = new double[settings.Exchange.Stocks.Count * numberEntries, numberStatistics];
            double[] Y = new double[settings.Exchange.Stocks.Count * numberEntries];
            for (int entryIndex = 0; entryIndex < numberEntries; entryIndex++)
            {
                for (int stockIndex = 0; stockIndex < settings.Exchange.Stocks.Count; stockIndex++)
                {
                    for (int statisticIndex = 0; statisticIndex < numberStatistics; statisticIndex++)
                    {
                        X[entryIndex * settings.Exchange.Stocks.Count + stockIndex, statisticIndex] = stockStatistics[statisticIndex].Calculate(settings.StartTime.AddDays(delayTime + entryIndex), settings.Exchange.Stocks[stockIndex]);
                    }

                    Y[entryIndex * settings.Exchange.Stocks.Count + stockIndex] = settings.Exchange.Stocks[stockIndex].Values(burnInLength.AddDays(delayTime + entryIndex), 0, 1, StockDataStream.Open).Last() / 100;
                }
            }

            Estimator = new LSEstimator(X, Y);
            settings.UpdateStartTime(burnInLength);
        }

        /// <inheritdoc/>
        public DecisionStatus Decide(DateTime day, SimulatorSettings settings, IReportLogger logger)
        {
            var decisions = new DecisionStatus();
            foreach (IStock stock in settings.Exchange.Stocks)
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
