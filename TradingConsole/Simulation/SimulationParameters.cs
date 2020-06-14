using System;
using FinancialStructures.NamingStructures;
using FinancialStructures.StockStructures;
using TradingConsole.InputParser;

namespace TradingConsole.Simulation
{
    public class SimulationParameters
    {
        public SimulationParameters(UserInputOptions inputOptions, ExchangeStocks exchange)
        {
            var earliest = exchange.LatestEarliestDate();
            StartTime = inputOptions.StartDate != null ? inputOptions.StartDate > earliest ? inputOptions.StartDate : earliest : earliest;
            EndTime = inputOptions.EndDate != null ? inputOptions.EndDate > exchange.LastDate() ? exchange.LastDate() : inputOptions.EndDate : exchange.LastDate();
            EvolutionIncrement = inputOptions.TradingGap.Seconds != 0 ? inputOptions.TradingGap : new TimeSpan(1, 0, 0, 0);
        }

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
