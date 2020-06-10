using System;
using FinancialStructures.Database;
using FinancialStructures.PortfolioAPI;
using FinancialStructures.StockStructures;
using StructureCommon.DataStructures;
using StructureCommon.Reporting;
using TradingConsole.BuySellSystem;
using TradingConsole.DecisionSystem;
using TradingConsole.InputParser;
using TradingConsole.Statistics;
using TradingConsole.UserInputs;

namespace TradingConsole.Simulation
{
    internal class TradingSimulation
    {
        private IDecisionSystem DecisionSystem;
        private IBuySellSystem BuySellSystem;
        private readonly BuySellParams TradingParameters = new BuySellParams();
        private SimulationParameters SimulationParameters;

        private readonly ExchangeStocks Exchange = new ExchangeStocks();

        private readonly UserInputOptions InputOptions;
        private readonly IReportLogger ReportLogger;
        private readonly ConsoleStreamWriter ConsoleWriter;

        public TradingSimulation(UserInputOptions inputOptions, IReportLogger reportLogger, ConsoleStreamWriter consoleWriter)
        {
            InputOptions = inputOptions;
            ReportLogger = reportLogger;
            ConsoleWriter = consoleWriter;
        }

        public void SetupSystemsAndRun(TradingStatistics stats)
        {
            switch (InputOptions.DecisionType)
            {
                case DecisionSystemType.BuyAll:
                    DecisionSystem = new BuyAllDecisionSystem(ReportLogger);
                    break;
                case DecisionSystemType.LeastSquares:
                case DecisionSystemType.Lasso:
                case DecisionSystemType.Ridge:
                default:
                    if (InputOptions.StatsChoice == DecisionStatsChoice.FiveDayStats)
                    {
                        DecisionSystem = new FiveDayStatsDecisionSystem(ReportLogger);
                    }
                    else
                    {
                        DecisionSystem = new ArbitraryStatsLSDecisionSystem(ReportLogger);
                    }
                    break;
            }
            switch (InputOptions.BuyingSellingType)
            {
                case BuySellType.IB:
                    BuySellSystem = new IBClientTradingSystem(ReportLogger);
                    break;
                case BuySellType.Simulate:
                default:
                    BuySellSystem = new SimulationBuySellSystem(ReportLogger);
                    break;
            }

            Exchange.LoadExchangeStocks(InputOptions.StockFilePath, ReportLogger);
            if (!Exchange.CheckValidity())
            {
                _ = ReportLogger.Log(ReportSeverity.Critical, ReportType.Error, ReportLocation.Loading, "Stock input data not suitable.");
                return;
            }

            SimulationParameters = new SimulationParameters(InputOptions, Exchange);

            DecisionSystem.Calibrate(InputOptions, Exchange, SimulationParameters);

            if (InputOptions.funtionType == ProgramType.Simulate)
            {
                SimulateRun(stats);
            }
            if (InputOptions.funtionType == ProgramType.Trade)
            {
                Run(stats);
            }
        }

        private void SimulateRun(TradingStatistics stats)
        {
            Portfolio portfolio = new Portfolio();
            LoadStartPortfolio(SimulationParameters.StartTime, stats, portfolio);

            DateTime time = SimulationParameters.StartTime;

            while (time < SimulationParameters.EndTime)
            {
                if ((time.DayOfWeek == DayOfWeek.Saturday) || (time.DayOfWeek == DayOfWeek.Sunday))
                {
                    time += SimulationParameters.EvolutionIncrement;
                    continue;
                }

                PerformDailyTrades(time, Exchange, portfolio, stats, ReportLogger);

                stats.GenerateDayStats();
                time += SimulationParameters.EvolutionIncrement;
                ConsoleWriter.Write(time + " - " + portfolio.Value(time));
            }

            stats.GenerateSimulationStats();
        }

        private void Run(TradingStatistics stats)
        {
            Portfolio portfolio = new Portfolio();
            LoadStartPortfolio(SimulationParameters.StartTime, stats, portfolio);
            PerformDailyTrades(DateTime.Today, Exchange, portfolio, stats, ReportLogger);

            stats.GenerateDayStats();
            stats.GenerateSimulationStats();
            portfolio.SavePortfolio(InputOptions.PortfolioFilePath, ReportLogger);
        }

        private void PerformDailyTrades(DateTime day, ExchangeStocks exchange, Portfolio portfolio, TradingStatistics stats, IReportLogger reportLogger)
        {
            // Decide which stocks to buy, sell or do nothing with.
            DecisionStatus status = new DecisionStatus();
            DecisionSystem.Decide(day, status, exchange, stats, SimulationParameters);
            /*var buys = status.GetBuyDecisions().Select(dec => dec.StockName);
            if (buys.Any())
            {
                ConsoleWriter.Write($"{day} - Buys: {string.Join(',', buys)}");
            }
            var sells = status.GetSellDecisions().Select(dec => dec.StockName);
            if (sells.Any())
            {
                //ConsoleWriter.Write($"{day} - Sells: {string.Join(',', sells)}");
            }*/
            DecisionSystem.AddDailyDecisionStats(stats, day, status.GetBuyDecisionsStockNames(), status.GetSellDecisionsStockNames());

            // Exact the buy/Sell decisions.
            BuySellSystem.BuySell(day, status, exchange, portfolio, stats, TradingParameters, SimulationParameters);
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
                _ = portfolio.TryAdd(AccountType.BankAccount, SimulationParameters.bankAccData, ReportLogger);
                _ = portfolio.TryAddData(AccountType.BankAccount, SimulationParameters.bankAccData, new DailyValuation(InputOptions.StartDate, InputOptions.StartingCash), ReportLogger);
                stats.StartingCash = InputOptions.StartingCash;
            }
        }
    }
}
