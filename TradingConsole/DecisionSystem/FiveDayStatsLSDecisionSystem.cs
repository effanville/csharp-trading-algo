using System;
using System.Collections.Generic;
using System.Linq;
using FinancialStructures.StockStructures;
using StructureCommon.MathLibrary.ParameterEstimation;
using StructureCommon.Reporting;
using TradingConsole.InputParser;
using TradingConsole.Simulation;
using TradingConsole.Statistics;

namespace TradingConsole.DecisionSystem
{
    public class FiveDayStatsLSDecisionSystem : IDecisionSystem
    {
        public IReportLogger ReportLogger
        {
            get;
        }

        public IEstimator Estimator;

        public FiveDayStatsDecisionSystem(IReportLogger reportLogger)
        {
            ReportLogger = reportLogger;
        }

        public void Calibrate(UserInputOptions inputOptions, IStockExchange exchange, SimulationParameters simulationParameters)
        {
            TimeSpan simulationLength = simulationParameters.EndTime - simulationParameters.StartTime;
            DateTime burnInLength = simulationParameters.StartTime + simulationLength / 2;

            int numberEntries = ((burnInLength - simulationParameters.StartTime).Days - 5) * 5 / 7;

            double[,] X = new double[exchange.Stocks.Count * numberEntries, 5];
            double[] Y = new double[exchange.Stocks.Count * numberEntries];
            for (int i = 0; i < numberEntries; i++)
            {
                for (int stockIndex = 0; stockIndex < exchange.Stocks.Count; stockIndex++)
                {
                    List<double> values = exchange.Stocks[stockIndex].Values(simulationParameters.StartTime.AddDays(i), 0, 6, StockDataStream.Open);
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

            _ = ReportLogger.Log(ReportSeverity.Critical, ReportType.Report, ReportLocation.Unknown, $"Estimator Weights are {string.Join(',', Estimator.Estimator)}");
            simulationParameters.StartTime = burnInLength;
        }

        public void AddDailyDecisionStats(TradingStatistics stats, DateTime day, List<string> buys, List<string> sells)
        {
            stats.AddDailyDecisionStats(day, buys, sells);
        }

        public void Decide(DateTime date, DecisionStatus status, IStockExchange exchange, TradingStatistics stats, SimulationParameters simulationParameters)
        {
            foreach (Stock stock in exchange.Stocks)
            {
                StockTradeDecision decision;
                double[] values = stock.Values(date, 5, 0, StockDataStream.Open).ToArray();
                double value = Estimator.Evaluate(values);

                if (value > 1.05)
                {
                    decision = StockTradeDecision.Buy;
                }
                else if (value < 1)
                {
                    decision = StockTradeDecision.Sell;
                }
                else
                {
                    decision = StockTradeDecision.Hold;
                }

                status.AddDecision(stock.Name, decision);
            }
        }
    }
}
