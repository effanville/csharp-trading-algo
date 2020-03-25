using FinancialStructures.Database;
using FinancialStructures.DataStructures;
using FinancialStructures.PortfolioAPI;
using FinancialStructures.ReportLogging;
using System;
using TradingConsole.BuySellSystem;
using TradingConsole.DecisionSystem;
using TradingConsole.InputParser;
using TradingConsole.Statistics;
using TradingConsole.StockStructures;

namespace TradingConsole.Simulation
{
    internal static class TradingSimulation
    {
        private static IDecisionSystem decisionSystem;
        private static IBuySellSystem buySellSystem;
        private static BuySellParams tradingParameters = new BuySellParams();
        private static SimulationParameters simulationParameters = new SimulationParameters();

        public static void SetupSystemsAndRun(UserInputOptions inputOptions, TradingStatistics stats, LogReporter reportLogger)
        {
            if (inputOptions.funtionType == InputParser.FunctionType.Simulate)
            {
                decisionSystem = new BuyAllDecisionSystem(reportLogger);
                buySellSystem = new SimulationBuySellSystem(reportLogger);
            }
            if (inputOptions.funtionType == InputParser.FunctionType.Trade)
            {
                decisionSystem = new BasicDecisionSystem(reportLogger);
                buySellSystem = new IBClientTradingSystem(reportLogger);
            }

            Run(inputOptions, stats, reportLogger);
        }

        private static void Run(UserInputOptions inputOptions, TradingStatistics stats, LogReporter reportLogger)
        {
            var exchange = new ExchangeStocks();
            LoadStockDatabase(inputOptions, stats, exchange, reportLogger);
            ParameterGenerators.GenerateSimulationParameters(simulationParameters, inputOptions, exchange);

            var portfolio = new Portfolio();
            LoadStartPortfolio(simulationParameters.StartTime, inputOptions, stats, portfolio, reportLogger);

            DateTime time = simulationParameters.StartTime;

            while (time < simulationParameters.EndTime)
            {
                if ((time.DayOfWeek == DayOfWeek.Saturday) || (time.DayOfWeek == DayOfWeek.Sunday))
                {
                    time += simulationParameters.EvolutionIncrement;
                    continue;
                }

                PerformDailyTrades(time, exchange, portfolio, stats, reportLogger);

                stats.GenerateDayStats();
                time += simulationParameters.EvolutionIncrement;
                Console.WriteLine(time + " - " + portfolio.Value(time));
            }

            stats.GenerateSimulationStats();
        }

        private static void PerformDailyTrades(DateTime day, ExchangeStocks exchange, Portfolio portfolio, TradingStatistics stats, LogReporter reportLogger)
        {
            // Decide which stocks to buy, sell or do nothing with.
            var status = new DecisionStatus();
            decisionSystem.Decide(day, status, exchange, stats, simulationParameters);
            decisionSystem.AddDailyDecisionStats(stats, day, status.GetBuyDecisionsStockNames(), status.GetSellDecisionsStockNames());

            // Exact the buy/Sell decisions.
            buySellSystem.BuySell(day, status, exchange, portfolio, stats, tradingParameters, simulationParameters);
            stats.AddSnapshot(day, portfolio);
        }

        private static void LoadStockDatabase(UserInputOptions inputOptions, TradingStatistics stats, ExchangeStocks exchange, LogReporter reportLogger)
        {
            string filepath = inputOptions.StockFilePath;
            exchange.LoadExchangeStocks(filepath, reportLogger);
        }

        private static void LoadStartPortfolio(DateTime startTime, UserInputOptions inputOptions, TradingStatistics stats, Portfolio portfolio, LogReporter reportLogger)
        {
            if (inputOptions.PortfolioFilePath != null)
            {
                string filePath = inputOptions.PortfolioFilePath;
                portfolio.LoadPortfolio(filePath, reportLogger);
                stats.StartingCash = portfolio.TotalValue(AccountType.BankAccount, startTime);
            }
            else
            {
                portfolio.TryAdd(AccountType.BankAccount, simulationParameters.bankAccData, reportLogger);
                portfolio.TryAddData(AccountType.BankAccount, simulationParameters.bankAccData, new DayValue_ChangeLogged(inputOptions.StartDate, inputOptions.StartingCash), reportLogger);
                stats.StartingCash = inputOptions.StartingCash;
            }
        }
    }
}
