using FinancialStructures.Database;
using FinancialStructures.GUIFinanceStructures;
using FinancialStructures.ReportingStructures;
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

        public static void SetupSystemsAndRun(UserInputOptions inputOptions, TradingStatistics stats)
        {
            if (inputOptions.funtionType == InputParser.FunctionType.Simulate)
            {
                decisionSystem = new BuyAllDecisionSystem();
                buySellSystem = new SimulationBuySellSystem();
            }
            if (inputOptions.funtionType == InputParser.FunctionType.Trade)
            {
                decisionSystem = new BasicDecisionSystem();
                buySellSystem = new IBClientTradingSystem();
            }

            Run(inputOptions, stats);
        }

        private static void Run(UserInputOptions inputOptions, TradingStatistics stats)
        {
            var exchange = new ExchangeStocks();
            LoadStockDatabase(inputOptions, stats, exchange);
            ParameterGenerators.GenerateSimulationParameters(simulationParameters, inputOptions, exchange);

            var portfolio = new Portfolio();
            LoadStartPortfolio(simulationParameters.StartTime, inputOptions, stats, portfolio);

            DateTime time = simulationParameters.StartTime;

            while (time < simulationParameters.EndTime)
            {
                if ((time.DayOfWeek == DayOfWeek.Saturday) || (time.DayOfWeek == DayOfWeek.Sunday))
                {
                    time += simulationParameters.EvolutionIncrement;
                    continue;
                }

                PerformDailyTrades(time, exchange, portfolio, stats);

                stats.GenerateDayStats();
                time += simulationParameters.EvolutionIncrement;
                Console.WriteLine(time + " - " + portfolio.Value(time));
            }

            stats.GenerateSimulationStats();
        }

        private static void PerformDailyTrades(DateTime day, ExchangeStocks exchange, Portfolio portfolio, TradingStatistics stats)
        {
            // Decide which stocks to buy, sell or do nothing with.
            var status = new DecisionStatus();
            decisionSystem.Decide(day, status, exchange, stats, simulationParameters);
            decisionSystem.AddDailyDecisionStats(stats, day, status.GetBuyDecisionsStockNames(), status.GetSellDecisionsStockNames());

            // Exact the buy/Sell decisions.
            buySellSystem.BuySell(day, status, exchange, portfolio, stats, tradingParameters, simulationParameters);
            stats.AddSnapshot(day, portfolio);
        }

        private static void LoadStockDatabase(UserInputOptions inputOptions, TradingStatistics stats, ExchangeStocks exchange)
        {
            var reports = new ErrorReports();
            string filepath = inputOptions.StockFilePath;
            exchange.LoadExchangeStocks(filepath, reports);
        }

        private static void LoadStartPortfolio(DateTime startTime, UserInputOptions inputOptions, TradingStatistics stats, Portfolio portfolio)
        {
            var reports = new ErrorReports();
            if (inputOptions.PortfolioFilePath != null)
            {
                string filePath = inputOptions.PortfolioFilePath;
                portfolio.LoadPortfolio(filePath, reports);
                stats.StartingCash = portfolio.AllBankAccountsValue(startTime);
            }
            else
            {
                portfolio.TryAddBankAccount(simulationParameters.bankAccData, reports);
                portfolio.TryAddDataToBankAccount(simulationParameters.bankAccData, new DayValue_ChangeLogged(inputOptions.StartDate, inputOptions.StartingCash), reports);
                stats.StartingCash = inputOptions.StartingCash;
            }
        }
    }
}
