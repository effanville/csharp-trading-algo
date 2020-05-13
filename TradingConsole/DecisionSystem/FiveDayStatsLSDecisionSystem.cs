using StructureCommon.Reporting;
using FinancialStructures.StockStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using TradingConsole.InputParser;
using TradingConsole.Simulation;
using TradingConsole.Statistics;
using StructureCommon.MathLibrary.ParameterEstimation;

namespace TradingConsole.DecisionSystem
{
    public class FiveDayStatsLSDecisionSystem : IDecisionSystem
    {
        public LogReporter ReportLogger { get; }

        public IEstimator Estimator;


        public FiveDayStatsLSDecisionSystem(LogReporter reportLogger)
        {
            ReportLogger = reportLogger;
        }

        public void Calibrate(UserInputOptions inputOptions, ExchangeStocks exchange, SimulationParameters simulationParameters)
        {
            TimeSpan simulationLength = simulationParameters.EndTime - simulationParameters.StartTime;
            var burnInLength = simulationParameters.StartTime + simulationLength / 2;

            int numberEntries = ((burnInLength - simulationParameters.StartTime).Days - 5) * 5 / 7;

            double[,] X = new double[exchange.Stocks.Count * numberEntries, 5];
            double[] Y = new double[exchange.Stocks.Count * numberEntries];
            for (int i = 0; i < numberEntries; i++)
            {
                for (int stockIndex = 0; stockIndex < exchange.Stocks.Count; stockIndex++)
                {
                    var values = exchange.Stocks[stockIndex].Values(simulationParameters.StartTime.AddDays(i), 0, 6, DataStream.Open);
                    for (int j = 0; j < 5; j++)
                    {
                        X[i + stockIndex, j] = values[j] / 100;
                    }

                    Y[i + stockIndex] = values.Last() / 100;
                }
            }


            Estimator = new LSEstimator(X, Y);

            simulationParameters.StartTime = burnInLength;
        }

        public void AddDailyDecisionStats(TradingStatistics stats, DateTime day, List<string> buys, List<string> sells)
        {
            stats.AddDailyDecisionStats(day, buys, sells);
        }

        public void Decide(DateTime date, DecisionStatus status, ExchangeStocks exchange, TradingStatistics stats, SimulationParameters simulationParameters)
        {
            foreach (var stock in exchange.Stocks)
            {
                StockTradeDecision decision;
                var values = stock.Values(date, 5, 0, DataStream.Open).ToArray();
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
