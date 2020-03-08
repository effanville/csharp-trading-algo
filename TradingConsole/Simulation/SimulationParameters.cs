using FinancialStructures.GUIFinanceStructures;
using System;
using TradingConsole.InputParser;
using TradingConsole.StockStructures;

namespace TradingConsole.Simulation
{
    public static class ParameterGenerators
    {
        public static void GenerateSimulationParameters(SimulationParameters parameters, UserInputOptions inputOptions, ExchangeStocks exchange)
        {
            parameters.StartTime = inputOptions.StartDate != null ? inputOptions.StartDate > exchange.EarliestDate() ? inputOptions.StartDate : exchange.EarliestDate() : exchange.EarliestDate();
            parameters.EndTime = inputOptions.EndDate != null ? inputOptions.EndDate > exchange.LastDate() ? exchange.LastDate() : inputOptions.EndDate : exchange.LastDate();
            parameters.EvolutionIncrement = inputOptions.TradingGap.Seconds != 0 ? inputOptions.TradingGap : new TimeSpan(1, 0, 0, 0);
        }
    }

    public class SimulationParameters
    {
        public DateTime StartTime;
        public DateTime EndTime;
        public TimeSpan EvolutionIncrement;

        public double UpTickProbability;

        public double tradeCost = 6;

        public NameData bankAccData = new NameData("Cash", "Portfolio");
    }
}
