using System;
using System.IO.Abstractions;
using FinancialStructures.Database;
using FinancialStructures.Database.Implementation;
using FinancialStructures.StockStructures;
using FinancialStructures.StockStructures.Implementation;
using StructureCommon.DataStructures;
using StructureCommon.Reporting;
using TradingConsole.BuySellSystem;
using TradingConsole.DecisionSystem;
using TradingConsole.InputParser;
using TradingConsole.Statistics;

namespace TradingConsole.Simulation
{
    internal class TradingSimulation
    {
        private IDecisionSystem DecisionSystem;
        private IBuySellSystem BuySellSystem;
        private readonly BuySellParams TradingParameters = new BuySellParams();
        private SimulationParameters SimulationParameters;

        private readonly IStockExchange Exchange = new StockExchange();

        private readonly UserInputOptions InputOptions;
        private readonly IReportLogger ReportLogger;
        private readonly IFileSystem fFileSystem;
        private readonly ConsoleStreamWriter ConsoleWriter;

        public TradingSimulation(UserInputOptions inputOptions, IFileSystem fileSystem, IReportLogger reportLogger, ConsoleStreamWriter consoleWriter)
        {
            InputOptions = inputOptions;
            ReportLogger = reportLogger;
            fFileSystem = fileSystem;
            ConsoleWriter = consoleWriter;
        }

        public void SetupSystemsAndRun(TradingStatistics stats)
        {

            Exchange.LoadStockExchange(InputOptions.StockFilePath, ReportLogger);
            if (!Exchange.CheckValidity())
            {
                _ = ReportLogger.Log(ReportSeverity.Critical, ReportType.Error, ReportLocation.Loading, "Stock input data not suitable.");
                return;
            }

            SimulationParameters = new SimulationParameters(InputOptions, Exchange);

            DecisionSystem = DecisionSystemGenerator.Generate(InputOptions.DecisionType, ReportLogger);
            BuySellSystem = BuySellSystemGenerator.Generate(InputOptions.BuyingSellingType, ReportLogger);
            DecisionSystem.Calibrate(InputOptions, Exchange, SimulationParameters);

            if (InputOptions.FuntionType == ProgramType.Simulate)
            {
                SimulateRun(stats);
            }
            if (InputOptions.FuntionType == ProgramType.Trade)
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
                if (time.Day == 1)
                {
                    ConsoleWriter.Write(time + " - " + portfolio.TotalValue(Totals.All, time));
                    ;
                }
            }

            stats.GenerateSimulationStats();
        }

        private void Run(TradingStatistics stats)
        {
            IPortfolio portfolio = new Portfolio();
            LoadStartPortfolio(SimulationParameters.StartTime, stats, portfolio);
            PerformDailyTrades(DateTime.Today, Exchange, portfolio, stats, ReportLogger);

            stats.GenerateDayStats();
            stats.GenerateSimulationStats();
            portfolio.SavePortfolio(InputOptions.PortfolioFilePath, fFileSystem, ReportLogger);
        }

        private void PerformDailyTrades(DateTime day, IStockExchange exchange, IPortfolio portfolio, TradingStatistics stats, IReportLogger reportLogger)
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

        private void LoadStartPortfolio(DateTime startTime, TradingStatistics stats, IPortfolio portfolio)
        {
            if (InputOptions.PortfolioFilePath != null)
            {
                portfolio.LoadPortfolio(InputOptions.PortfolioFilePath, fFileSystem, ReportLogger);
                stats.StartingCash = portfolio.TotalValue(Totals.BankAccount, startTime);
            }
            else
            {
                _ = portfolio.TryAdd(Account.BankAccount, SimulationParameters.bankAccData, ReportLogger);
                var data = new DailyValuation(InputOptions.StartDate, InputOptions.StartingCash);
                _ = portfolio.TryAddOrEditData(Account.BankAccount, SimulationParameters.bankAccData, data, data, ReportLogger);
                stats.StartingCash = InputOptions.StartingCash;
            }
        }
    }
}
