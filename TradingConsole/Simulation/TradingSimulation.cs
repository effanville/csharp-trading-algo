using FinancialStructures.Database;
using FinancialStructures.GUIFinanceStructures;
using FinancialStructures.ReportingStructures;
using System;
using TradingConsole.BuySellSystem;
using TradingConsole.DecisionSystem;
using TradingConsole.InputParser;
using TradingConsole.Statistics;
using TradingConsole.StockStructures;

namespace TradingConsole
{
    internal static class TradingSimulation
    {
        private static IDecisionSystem decisionSystem;
        private static IBuySellSystem buySellSystem;
        private static BuySellParams parameters;

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

            var portfolio = new Portfolio();
            LoadStartPortfolio(inputOptions, stats, portfolio);

            int time = inputOptions.StartTime;

            while (time < inputOptions.EndTime)
            {
                var day = new DateTime();
                PerformDailyTrades(day, exchange, portfolio, stats);

                stats.GenerateDayStats();
                time++;
            }

            stats.GenerateSimulationStats();
        }

        private static void PerformDailyTrades(DateTime day, ExchangeStocks exchange, Portfolio portfolio,  TradingStatistics stats)
        {
            //decide which stocks to buy, sell or do nothing with.
            var status = new DecisionStatus();
            decisionSystem.Decide(day, status, exchange, stats);
            buySellSystem.BuySell(day, status, exchange, portfolio, stats, parameters);
        }

        private static void LoadStockDatabase(UserInputOptions inputOptions, TradingStatistics stats, ExchangeStocks exchange)
        {
            var reports = new ErrorReports();
            string filepath = inputOptions.StockFilePath;
            exchange.LoadExchangeStocks(filepath, reports);

        }

        private static void LoadStartPortfolio(UserInputOptions inputOptions, TradingStatistics stats, Portfolio portfolio)
        {
            var reports = new ErrorReports();
            if (inputOptions.PortfolioFilePath != null)
            {
                string filePath = inputOptions.PortfolioFilePath;
                portfolio.LoadPortfolio(filePath, reports);
            }
            else
            {
                var name = new NameData("Cash", "Portfolio");
                portfolio.TryAddBankAccount(name, reports);
                portfolio.TryAddDataToBankAccount(name, new DayValue_ChangeLogged(inputOptions.StartDate, inputOptions.StartingCash), reports);
                stats.StartingCash = inputOptions.StartingCash;
            }
        }

        public static void DisplayStatistics(TradingStatistics stats)
        {
        }
    }
}
