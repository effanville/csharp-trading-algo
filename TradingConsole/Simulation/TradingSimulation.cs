using FinancialStructures.Database;
using FinancialStructures.DataStructures;
using FinancialStructures.PortfolioAPI;
using FinancialStructures.ReportLogging;
using FinancialStructures.StockStructures;
using System;
using TradingConsole.BuySellSystem;
using TradingConsole.DecisionSystem;
using TradingConsole.InputParser;
using TradingConsole.Statistics;

namespace TradingConsole.Simulation
{
    internal class TradingSimulation
    {
        private IDecisionSystem decisionSystem;
        private IBuySellSystem buySellSystem;
        private BuySellParams tradingParameters = new BuySellParams();
        private SimulationParameters simulationParameters = new SimulationParameters();

        private UserInputOptions InputOptions;
        private LogReporter ReportLogger;

        public TradingSimulation(UserInputOptions inputOptions, LogReporter reportLogger)
        {
            InputOptions = inputOptions;
            ReportLogger = reportLogger;
        }

        public void SetupSystemsAndRun(TradingStatistics stats)
        {
            if (InputOptions.funtionType == ProgramType.Simulate)
            {
                decisionSystem = new BuyAllDecisionSystem(ReportLogger);
                buySellSystem = new SimulationBuySellSystem(ReportLogger);
                SimulateRun(stats);
            }
            if (InputOptions.funtionType == ProgramType.Trade)
            {
                decisionSystem = new BasicDecisionSystem(ReportLogger);
                buySellSystem = new IBClientTradingSystem(ReportLogger);
                Run(stats);
            }
        }

        private void SimulateRun(TradingStatistics stats)
        {
            var exchange = new ExchangeStocks();
            exchange.LoadExchangeStocks(InputOptions.StockFilePath, ReportLogger);
            ParameterGenerators.GenerateSimulationParameters(simulationParameters, InputOptions, exchange);

            var portfolio = new Portfolio();
            LoadStartPortfolio(simulationParameters.StartTime, stats, portfolio);

            DateTime time = simulationParameters.StartTime;

            while (time < simulationParameters.EndTime)
            {
                if ((time.DayOfWeek == DayOfWeek.Saturday) || (time.DayOfWeek == DayOfWeek.Sunday))
                {
                    time += simulationParameters.EvolutionIncrement;
                    continue;
                }

                PerformDailyTrades(time, exchange, portfolio, stats, ReportLogger);

                stats.GenerateDayStats();
                time += simulationParameters.EvolutionIncrement;
                Console.WriteLine(time + " - " + portfolio.Value(time));
            }

            stats.GenerateSimulationStats();
        }

        private void Run(TradingStatistics stats)
        {
            var exchange = new ExchangeStocks();
            exchange.LoadExchangeStocks(InputOptions.StockFilePath, ReportLogger);
            ParameterGenerators.GenerateSimulationParameters(simulationParameters, InputOptions, exchange);

            var portfolio = new Portfolio();
            LoadStartPortfolio(simulationParameters.StartTime, stats, portfolio);
            PerformDailyTrades(DateTime.Today, exchange, portfolio, stats, ReportLogger);

            stats.GenerateDayStats();
            stats.GenerateSimulationStats();
            portfolio.SavePortfolio(InputOptions.PortfolioFilePath, ReportLogger);
        }

        private void PerformDailyTrades(DateTime day, ExchangeStocks exchange, Portfolio portfolio, TradingStatistics stats, LogReporter reportLogger)
        {
            // Decide which stocks to buy, sell or do nothing with.
            var status = new DecisionStatus();
            decisionSystem.Decide(day, status, exchange, stats, simulationParameters);
            decisionSystem.AddDailyDecisionStats(stats, day, status.GetBuyDecisionsStockNames(), status.GetSellDecisionsStockNames());

            // Exact the buy/Sell decisions.
            buySellSystem.BuySell(day, status, exchange, portfolio, stats, tradingParameters, simulationParameters);
            stats.AddSnapshot(day, portfolio);
        }

        private void LoadStartPortfolio(DateTime startTime, TradingStatistics stats, Portfolio portfolio)
        {
            if (InputOptions.PortfolioFilePath != null)
            {
                portfolio.LoadPortfolio(InputOptions.PortfolioFilePath, ReportLogger);
                stats.StartingCash = portfolio.TotalValue(AccountType.BankAccount, startTime);
            }
            else
            {
                portfolio.TryAdd(AccountType.BankAccount, simulationParameters.bankAccData, ReportLogger);
                portfolio.TryAddData(AccountType.BankAccount, simulationParameters.bankAccData, new DayValue_ChangeLogged(InputOptions.StartDate, InputOptions.StartingCash), ReportLogger);
                stats.StartingCash = InputOptions.StartingCash;
            }
        }
    }
}
