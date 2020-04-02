using FinancialStructures.Mathematics;
using FinancialStructures.ReportLogging;
using FinancialStructures.StockStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using TradingConsole.InputParser;
using TradingConsole.Simulation;
using TradingConsole.Statistics;

namespace TradingConsole.DecisionSystem
{
    public class ArbitraryStatsLSDecisionSystem : IDecisionSystem
    {
        public LogReporter ReportLogger { get; }

        public IEstimator Estimator;

        public List<IStockStatistic> stockStatistics;


        public ArbitraryStatsLSDecisionSystem(LogReporter reportLogger)
        {
            ReportLogger = reportLogger;
        }

        public void Calibrate(UserInputOptions inputOptions, ExchangeStocks exchange, SimulationParameters simulationParameters)
        {
            foreach (var statistic in inputOptions.decisionSystemStats)
            {
                stockStatistics.Add(StockStatisticGenerator.Generate(statistic));
            }

            TimeSpan simulationLength = simulationParameters.EndTime - simulationParameters.StartTime;
            var burnInLength = simulationParameters.StartTime + simulationLength / 2;

            int numberEntries = ((burnInLength - simulationParameters.StartTime).Days - 5) * 5 / 7;
            int numberStatistics = stockStatistics.Count;

            double[,] X = new double[exchange.Stocks.Count * numberEntries, numberStatistics];
            double[] Y = new double[exchange.Stocks.Count * numberEntries];
            for (int entryIndex = 0; entryIndex < numberEntries; entryIndex++)
            {
                for (int stockIndex = 0; stockIndex < exchange.Stocks.Count; stockIndex++)
                {
                    for (int statisticIndex = 0; statisticIndex < numberStatistics; statisticIndex++)
                    {
                        X[entryIndex + stockIndex, statisticIndex] = stockStatistics[statisticIndex].Calculate(simulationParameters.StartTime.AddDays(entryIndex), exchange.Stocks[stockIndex]);
                    }

                    Y[entryIndex + stockIndex] = exchange.Stocks[entryIndex].Values(burnInLength.AddDays(entryIndex), 0, 1, DataStream.Open).Last() / 100;
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
