using FinancialStructures.Database;
using FinancialStructures.PortfolioAPI;
using StructureCommon.Reporting;
using FinancialStructures.StockStructures;
using System;
using TradingConsole.BuySellSystem;
using TradingConsole.DecisionSystem;
using TradingConsole.InputParser;
using TradingConsole.Statistics;
using StructureCommon.DataStructures;

namespace TradingConsole.Simulation
{
    internal class TradingSimulation
    {
        private IDecisionSystem DecisionSystem;
        private IBuySellSystem BuySellSystem;
        private BuySellParams TradingParameters = new BuySellParams();
        private SimulationParameters SimulationParameters;

        private ExchangeStocks Exchange = new ExchangeStocks();

        private UserInputOptions InputOptions;
        private LogReporter ReportLogger;

        public TradingSimulation(UserInputOptions inputOptions, LogReporter reportLogger)
        {
            InputOptions = inputOptions;
            ReportLogger = reportLogger;
        }

        public void SetupSystemsAndRun(TradingStatistics stats)
        {
            switch (InputOptions.DecisionType)
            {
                case DecisionSystemType.BuyAll:
                    DecisionSystem = new BuyAllDecisionSystem(ReportLogger);
                    break;
                case DecisionSystemType.ArbitraryStatsLeastSquares:
                    DecisionSystem = new ArbitraryStatsLSDecisionSystem(ReportLogger);
                    break;
                case DecisionSystemType.FiveDayStatsLeastSquares:
                default:
                    DecisionSystem = new FiveDayStatsLSDecisionSystem(ReportLogger);
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
                ReportLogger.Log(ReportSeverity.Critical, ReportType.Error, ReportLocation.Loading, "Stock input data not suitable.");
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
            var portfolio = new Portfolio();
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
                Console.WriteLine(time + " - " + portfolio.Value(time));
            }

            stats.GenerateSimulationStats();
        }

        private void Run(TradingStatistics stats)
        {
            var portfolio = new Portfolio();
            LoadStartPortfolio(SimulationParameters.StartTime, stats, portfolio);
            PerformDailyTrades(DateTime.Today, Exchange, portfolio, stats, ReportLogger);

            stats.GenerateDayStats();
            stats.GenerateSimulationStats();
            portfolio.SavePortfolio(InputOptions.PortfolioFilePath, ReportLogger);
        }

        private void PerformDailyTrades(DateTime day, ExchangeStocks exchange, Portfolio portfolio, TradingStatistics stats, LogReporter reportLogger)
        {
            // Decide which stocks to buy, sell or do nothing with.
            var status = new DecisionStatus();
            DecisionSystem.Decide(day, status, exchange, stats, SimulationParameters);
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
                portfolio.TryAdd(AccountType.BankAccount, SimulationParameters.bankAccData, ReportLogger);
                portfolio.TryAddData(AccountType.BankAccount, SimulationParameters.bankAccData, new DayValue_ChangeLogged(InputOptions.StartDate, InputOptions.StartingCash), ReportLogger);
                stats.StartingCash = InputOptions.StartingCash;
            }
        }
    }
}
