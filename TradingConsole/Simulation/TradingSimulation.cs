using System;
using System.IO.Abstractions;
using FinancialStructures.Database;
using FinancialStructures.StockStructures;
using FinancialStructures.StockStructures.Implementation;
using Common.Structure.DataStructures;
using Common.Structure.Reporting;
using TradingConsole.BuySellSystem;
using TradingConsole.DecisionSystem;
using TradingConsole.Statistics;

namespace TradingConsole.Simulation
{
    internal partial class TradingSimulation
    {
        private readonly IDecisionSystem DecisionSystem;
        private readonly IBuySellSystem BuySellSystem;
        private readonly BuySellParams TradingParameters = new BuySellParams();
        private readonly SimulationParameters SimulationParameters;

        private readonly IStockExchange Exchange = new StockExchange();
        private readonly IPortfolio fPortfolio;

        private readonly IReportLogger ReportLogger;
        private readonly IFileSystem fFileSystem;

        /// <summary>
        /// Does the setup of the simulation error.
        /// </summary>
        public bool SetupError
        {
            get;
            set;
        }

        public TradingSimulation(string stockFilePath, string portfolioFilePath, SimulationParameters simulationParameters, DecisionSystemParameters decisionParameters, BuySellType buySellType, IFileSystem fileSystem, IReportLogger reportLogger)
        {
            ReportLogger = reportLogger;
            fFileSystem = fileSystem;
            using (new Timer(ReportLogger, "Setup"))
            {
                using (new Timer(ReportLogger, "Loading Exchange"))
                {
                    Exchange.LoadStockExchange(stockFilePath, ReportLogger);
                    if (!Exchange.CheckValidity())
                    {
                        _ = ReportLogger.Log(ReportSeverity.Critical, ReportType.Error, ReportLocation.Loading, "Stock input data not suitable.");
                        SetupError = true;
                        return;
                    }
                }

                SimulationParameters = simulationParameters;
                SimulationParameters.EnsureStartEndConsistent(Exchange);

                using (new Timer(reportLogger, "Calibrating"))
                {
                    DecisionSystem = DecisionSystemGenerator.Generate(decisionParameters.DecisionSystemType, ReportLogger);
                    BuySellSystem = BuySellSystemGenerator.Generate(buySellType, ReportLogger);
                    DecisionSystem.Calibrate(decisionParameters, Exchange, SimulationParameters);
                }
                using (new Timer(reportLogger, "Loading Portfolio"))
                {
                    fPortfolio = LoadStartPortfolio(portfolioFilePath, SimulationParameters.StartingCash);
                }
            }
        }

        public void SimulateRun(TradingStatistics stats)
        {
            using (new Timer(ReportLogger, "Simulation"))
            {
                DateTime time = SimulationParameters.StartTime;

                while (time < SimulationParameters.EndTime)
                {
                    if ((time.DayOfWeek == DayOfWeek.Saturday) || (time.DayOfWeek == DayOfWeek.Sunday))
                    {
                        time += SimulationParameters.EvolutionIncrement;
                        continue;
                    }

                    PerformDailyTrades(time, Exchange, fPortfolio, stats);

                    stats.GenerateDayStats();
                    time += SimulationParameters.EvolutionIncrement;
                    if (time.Day == 1)
                    {
                        _ = ReportLogger.Log(ReportSeverity.Critical, ReportType.Warning, ReportLocation.DatabaseAccess, $"Date {time} total value {fPortfolio.TotalValue(Totals.All, time)}");
                    }
                }
            }
        }

        public void Run(string portfolioFilePath, TradingStatistics stats)
        {
            PerformDailyTrades(DateTime.Today, Exchange, fPortfolio, stats);

            stats.GenerateDayStats();
            fPortfolio.SavePortfolio(portfolioFilePath, fFileSystem, ReportLogger);
        }

        private void PerformDailyTrades(DateTime day, IStockExchange exchange, IPortfolio portfolio, TradingStatistics stats)
        {
            // Decide which stocks to buy, sell or do nothing with.
            DecisionStatus status = new DecisionStatus();
            DecisionSystem.Decide(day, status, exchange, stats, SimulationParameters);
            DecisionSystem.AddDailyDecisionStats(stats, day, status.GetBuyDecisionsStockNames(), status.GetSellDecisionsStockNames());

            // Exact the buy/Sell decisions.
            BuySellSystem.BuySell(day, status, exchange, portfolio, stats, TradingParameters, SimulationParameters);
            stats.AddSnapshot(day, portfolio);
        }

        private IPortfolio LoadStartPortfolio(string portfolioFilePath, double startingCash)
        {
            IPortfolio portfolio = PortfolioFactory.GenerateEmpty();
            if (portfolioFilePath != null)
            {
                portfolio.LoadPortfolio(portfolioFilePath, fFileSystem, ReportLogger);
            }
            else
            {
                _ = portfolio.TryAdd(Account.BankAccount, SimulationParameters.BankAccData, ReportLogger);
                var data = new DailyValuation(SimulationParameters.StartTime, startingCash);
                _ = portfolio.TryAddOrEditData(Account.BankAccount, SimulationParameters.BankAccData, data, data, ReportLogger);
            }

            return portfolio;
        }
    }
}
