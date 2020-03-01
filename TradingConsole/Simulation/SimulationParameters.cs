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
            parameters = new SimulationParameters();
            parameters.StartTime = inputOptions.StartDate != null ? inputOptions.StartDate : exchange.EarliestDate();
            parameters.EndTime = inputOptions.EndDate != null ? inputOptions.EndDate : exchange.LastDate();
            parameters.EvolutionIncrement = inputOptions.TradingGap;
        }
    }

    public class SimulationParameters
    {
        public DateTime StartTime;
        public DateTime EndTime;
        public TimeSpan EvolutionIncrement;

        public double UpTickProbability;

        public double tradeCost;

        public NameData bankAccData = new NameData("Cash", "Portfolio");
    }
}
