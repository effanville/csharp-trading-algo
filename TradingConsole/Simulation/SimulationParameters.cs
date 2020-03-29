using FinancialStructures.NamingStructures;
using FinancialStructures.StockStructures;
using System;
using System.Linq;
using TradingConsole.DecisionSystem.BasicMLSystem;
using TradingConsole.InputParser;

namespace TradingConsole.Simulation
{
    public class SimulationParameters
    {
        public SimulationParameters(UserInputOptions inputOptions, ExchangeStocks exchange)
        {
            StartTime = inputOptions.StartDate != null ? inputOptions.StartDate > exchange.EarliestDate() ? inputOptions.StartDate : exchange.EarliestDate() : exchange.EarliestDate();
            EndTime = inputOptions.EndDate != null ? inputOptions.EndDate > exchange.LastDate() ? exchange.LastDate() : inputOptions.EndDate : exchange.LastDate();
            EvolutionIncrement = inputOptions.TradingGap.Seconds != 0 ? inputOptions.TradingGap : new TimeSpan(1, 0, 0, 0);
            CalculateEstimatorParameters(inputOptions, exchange);
        }

        public void CalculateEstimatorParameters(UserInputOptions inputOptions, ExchangeStocks exchange)
        {
            TimeSpan simulationLength = EndTime - StartTime;
            var burnInLength = StartTime + simulationLength / 2;

            int numberEntries = ((burnInLength - StartTime).Days - 5) * 5 / 7;

            double[,] X = new double[exchange.Stocks.Count * numberEntries, 5];
            double[] Y = new double[exchange.Stocks.Count * numberEntries];
            for (int i = 0; i < numberEntries; i++)
            {
                for (int stockIndex = 0; stockIndex < exchange.Stocks.Count; stockIndex++)
                {
                    var values = exchange.Stocks[stockIndex].Values(burnInLength.AddDays(i), 0, 6, DataStream.Open);
                    for (int j = 0; j < 5; j++)
                    {
                        X[i + stockIndex, j] = values[j] / 100;
                    }

                    Y[i + stockIndex] = values.Last() / 100;
                }
            }


            Estimator = new LSEstimator(X, Y);

            StartTime = burnInLength;
        }

        public IEstimator Estimator;

        public Random randomNumbers = new Random();
        public DateTime StartTime;
        public DateTime EndTime;
        public TimeSpan EvolutionIncrement;

        public double UpTickProbability = 0.5;
        public double UpTickSize = 0.05;

        public double tradeCost = 6;

        public NameData bankAccData = new NameData("Cash", "Portfolio");
    }
}
