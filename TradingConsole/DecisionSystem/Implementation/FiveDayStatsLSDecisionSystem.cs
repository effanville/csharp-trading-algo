using System;
using System.Collections.Generic;
using System.Linq;
using FinancialStructures.StockStructures;
using Common.Structure.MathLibrary.ParameterEstimation;
using Common.Structure.Reporting;
using TradingConsole.Simulator;
using TradingConsole.DecisionSystem.Models;

namespace TradingConsole.DecisionSystem.Implementation
{
    /// <summary>
    /// Decision system based upon the 5 previous stock days prices.
    /// </summary>
    public class FiveDayStatsLSDecisionSystem : IDecisionSystem
    {
        private readonly IReportLogger fLogger;
        private IEstimator Estimator;

        /// <summary>
        /// Construct and instance.
        /// </summary>
        /// <param name="reportLogger"></param>
        public FiveDayStatsLSDecisionSystem(IReportLogger reportLogger)
        {
            fLogger = reportLogger;
        }

        /// <inheritdoc />
        public void Calibrate(DecisionSystemSetupSettings decisionParameters, SimulatorSettings settings)
        {
            TimeSpan simulationLength = settings.EndTime - settings.StartTime;
            DateTime burnInLength = settings.StartTime + new TimeSpan((long)(simulationLength.Ticks / 2));

            int numberEntries = ((burnInLength - settings.StartTime).Days - 5) * 5 / 7;

            double[,] X = new double[settings.Exchange.Stocks.Count * numberEntries, 5];
            double[] Y = new double[settings.Exchange.Stocks.Count * numberEntries];
            for (int i = 0; i < numberEntries; i++)
            {
                for (int stockIndex = 0; stockIndex < settings.Exchange.Stocks.Count; stockIndex++)
                {
                    List<double> values = settings.Exchange.Stocks[stockIndex].Values(settings.StartTime.AddDays(i), 0, 6, StockDataStream.Open);
                    for (int j = 0; j < 5; j++)
                    {
                        if (values[j].Equals(double.NaN))
                        {
                            values[j] = values[j + 1];
                        }
                        X[i + stockIndex, j] = values[j] / 100;
                    }

                    if (values.Last().Equals(double.NaN))
                    {
                        values[values.Count - 1] = values[values.Count - 2];
                    }

                    Y[i + stockIndex] = values.Last() / 100;
                }
            }


            Estimator = new LSEstimator(X, Y);

            _ = fLogger.Log(ReportSeverity.Critical, ReportType.Warning, ReportLocation.Unknown, $"Estimator Weights are {string.Join(",", Estimator.Estimator)}");
            settings.UpdateStartTime(burnInLength);
        }
        /// <inheritdoc />
        public DecisionStatus Decide(DateTime day, SimulatorSettings settings, DecisionRecord record)
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

            record.AddForTheRecord(day, decisions);
            return decisions;
        }
    }
}
