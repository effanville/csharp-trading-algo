using System;
using System.Collections.Generic;
using System.Linq;

using Common.Structure.MathLibrary.ParameterEstimation;
using Common.Structure.Reporting;

using FinancialStructures.StockStructures;

using TradingSystem.Decisions.Models;
using TradingSystem.Decisions.System;
using TradingSystem.Simulator;

namespace TradingConsole.DecisionSystem.Implementation
{
    /// <summary>
    /// Decision system based upon the 5 previous stock days prices.
    /// </summary>
    public class FiveDayStatsDecisionSystem : IDecisionSystem
    {
        private readonly DecisionSystemSetupSettings fSettings;
        private IEstimator Estimator;
        private readonly int fDayAfterPredictor;

        /// <summary>
        /// Construct and instance.
        /// </summary>
        public FiveDayStatsDecisionSystem(DecisionSystemSetupSettings settings)
        {
            fSettings = settings;
            fDayAfterPredictor = settings.DayAfterPredictor;
        }

        /// <inheritdoc />
        public void Calibrate(SimulatorSettings settings, IReportLogger logger)
        {
            DateTime burnInLength = settings.BurnInEnd;
            int numberEntries = ((burnInLength - settings.StartTime).Days - 5) * 5 / 7;

            double[,] X = new double[settings.Exchange.Stocks.Count * numberEntries, 5];
            double[] Y = new double[settings.Exchange.Stocks.Count * numberEntries];
            for (int i = 0; i < numberEntries; i++)
            {
                for (int stockIndex = 0; stockIndex < settings.Exchange.Stocks.Count; stockIndex++)
                {
                    List<double> values = settings.Exchange.Stocks[stockIndex].Values(settings.StartTime.AddDays(i), 0, 5 + fDayAfterPredictor, StockDataStream.Open).Select(value => Convert.ToDouble(value)).ToList();
                    for (int j = 0; j < 5; j++)
                    {
                        if (values[j].Equals(double.NaN))
                        {
                            values[j] = values[j + 1];
                        }
                        X[i + stockIndex, j] = values[j] / values[0];
                    }

                    if (values.Last().Equals(double.NaN))
                    {
                        values[values.Count - 1] = values[values.Count - 2];
                    }

                    Y[i + stockIndex] = values.Last() / values[0];
                }
            }

            switch (fSettings.DecisionSystemType)
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

        /// <inheritdoc />
        public DecisionStatus Decide(DateTime day, IStockExchange stockExchange, IReportLogger logger)
        {
            var decisions = new DecisionStatus();
            foreach (IStock stock in stockExchange.Stocks)
            {
                TradeDecision decision;
                double[] values = stock.Values(day, 5, 0, StockDataStream.Open).Select(value => Convert.ToDouble(value)).ToArray();
                double normaliseFactor = values[0];
                for (int valueIndex = 0; valueIndex < values.Length; valueIndex++)
                {
                    values[valueIndex] /= normaliseFactor;
                }

                double value = Estimator.Evaluate(values);

                if (value > fSettings.BuyThreshold)
                {
                    decision = TradeDecision.Buy;
                }
                else if (value < fSettings.SellThreshold)
                {
                    decision = TradeDecision.Sell;
                }
                else
                {
                    decision = TradeDecision.Hold;
                }

                _ = logger?.Log(ReportSeverity.Detailed, ReportType.Information, ReportLocation.Execution, $"{stock.Name} - value {value} - decision {decision}.");

                decisions.AddDecision(stock.Name, decision);
            }

            _ = logger?.Log(ReportSeverity.Detailed, ReportType.Information, ReportLocation.Execution, $"Decisions: {decisions}");
            return decisions;
        }
    }
}
