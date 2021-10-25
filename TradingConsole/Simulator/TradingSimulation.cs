using System;
using System.IO.Abstractions;
using FinancialStructures.Database;
using FinancialStructures.StockStructures;
using Common.Structure.DataStructures;
using Common.Structure.Reporting;
using TradingConsole.BuySellSystem;
using TradingConsole.DecisionSystem;
using FinancialStructures.NamingStructures;
using TradingConsole.DecisionSystem.Models;
using TradingConsole.BuySellSystem.Models;

namespace TradingConsole.Simulator
{
    /// <summary>
    /// Contains the logic for the creation of user specified options for a simulation of
    /// a stock market trading algorithm.
    /// </summary>
    public static class TradingSimulation
    {
        /// <summary>
        /// Performs the setup of user specified inputs to the simulation.
        /// </summary>
        /// <param name="stockFilePath">FilePath to the stock database</param>
        /// <param name="startTime">Start time of the simulation</param>
        /// <param name="endTime">End time of the simulation</param>
        /// <param name="evolutionIncrement">The gap between trade times.</param>
        /// <param name="startSettings">The settings for the starting portfolio.</param>
        /// <param name="decisionParameters">Parameters to specify the decision system.</param>
        /// <param name="traderOptions">The settings for how a trader should trade.</param>
        /// <param name="buySellType">The type of the system used to buy and sell.</param>
        /// <param name="fileSystem">The file system to use.</param>
        /// <param name="reportLogger">A logging mechanism</param>
        /// <returns>The final portfolio, and the records of the decisions and trades.</returns>
        public static (IPortfolio, DecisionRecord, TradeRecord) SetupAndSimulate(
            string stockFilePath,
            DateTime startTime,
            DateTime endTime,
            TimeSpan evolutionIncrement,
            PortfolioStartSettings startSettings,
            DecisionSystemSetupSettings decisionParameters,
            TradeMechanismTraderOptions traderOptions,
            TradeMechanismType buySellType,
            IFileSystem fileSystem,
            IReportLogger reportLogger)
        {
            var tradeMechanismSettings = new TradeMechanismSettings();
            IStockExchange exchange;
            SimulatorSettings simulatorSettings;
            IPortfolio portfolio;
            IDecisionSystem decisionSystem;
            ITradeMechanism tradeMechanism;

            using (new Timer(reportLogger, "Setup"))
            {
                using (new Timer(reportLogger, "Loading Exchange"))
                {
                    exchange = StockExchangeFactory.Create(stockFilePath, fileSystem, reportLogger);
                    if (!exchange.CheckValidity())
                    {
                        _ = reportLogger.Log(ReportSeverity.Critical, ReportType.Error, ReportLocation.Loading, "Stock input data not suitable.");
                        return (null, null, null);
                    }

                    simulatorSettings = new SimulatorSettings(
                        startTime,
                        endTime,
                        evolutionIncrement,
                        exchange);
                }

                using (new Timer(reportLogger, "Calibrating"))
                {
                    decisionSystem = DecisionSystemFactory.Create(decisionParameters.DecisionSystemType, reportLogger);
                    decisionSystem.Calibrate(decisionParameters, simulatorSettings);
                }

                tradeMechanism = TradeMechanismFactory.Create(buySellType, reportLogger);

                using (new Timer(reportLogger, "Loading Portfolio"))
                {
                    portfolio = LoadStartPortfolio(startSettings, fileSystem, reportLogger);
                }
            }

            bool isCalcTimeValid(DateTime time) => (time.DayOfWeek != DayOfWeek.Saturday) || (time.DayOfWeek != DayOfWeek.Sunday);
            void reportCallback(DateTime time, string message)
            {
                if (time.Day == 1)
                {
                    _ = reportLogger.Log(ReportSeverity.Critical, ReportType.Warning, ReportLocation.DatabaseAccess, message);
                }
            }

            void startEndReportCallback(string message)
            {
                _ = reportLogger.Log(ReportSeverity.Critical, ReportType.Warning, ReportLocation.DatabaseAccess, message);

            }

            return StockMarketHistorySimulator.Simulate(simulatorSettings,
                tradeMechanismSettings,
                isCalcTimeValid,
                startEndReportCallback,
                reportCallback,
                startEndReportCallback,
                portfolio.Copy(),
                (time, port, func1, func2) => PerformDailyTrades(simulatorSettings, time, port, decisionSystem, tradeMechanism, traderOptions, func1, func2, reportLogger),
                reportLogger);
        }

        /// <summary>
        /// Loads the starting portfolio, either from file or with the cash specified in the settings.
        /// </summary>
        private static IPortfolio LoadStartPortfolio(PortfolioStartSettings settings, IFileSystem fileSystem, IReportLogger logger)
        {
            IPortfolio portfolio = PortfolioFactory.GenerateEmpty();
            if (settings.PortfolioFilePath != null)
            {
                portfolio.LoadPortfolio(settings.PortfolioFilePath, fileSystem, logger);
            }
            else
            {
                _ = portfolio.TryAdd(Account.BankAccount, new NameData(settings.DefaultBankAccName.Company, settings.DefaultBankAccName.Name), logger);
                var data = new DailyValuation(settings.StartTime.AddDays(-1), settings.StartingCash);
                _ = portfolio.TryAddOrEditData(Account.BankAccount, settings.DefaultBankAccName, data, data, logger);
            }

            return portfolio;
        }

        /// <summary>
        /// Enacts the trades on a given day. This is the simplest form of
        /// running through all stocks and deciding on what to do, and
        /// then enacting all the trades.
        /// </summary>
        /// <remarks>
        /// TODO: Enable this to be more sophisticated.
        /// </remarks>
        private static (TradeStatus, DecisionStatus) PerformDailyTrades(
            SimulatorSettings settings,
            DateTime day,
            IPortfolio portfolio,
            IDecisionSystem decisionSystem,
            ITradeMechanism tradeMechanism,
            TradeMechanismTraderOptions traderOptions,
            Func<DateTime, TwoName, double> buyCalc,
            Func<DateTime, TwoName, double> sellCalc,
            IReportLogger logger)
        {
            // Decide which stocks to buy, sell or do nothing with.
            DecisionStatus status = decisionSystem.Decide(day, settings, logger: null);

            // Exact the buy/Sell decisions.
            TradeStatus trades = tradeMechanism.EnactAllTrades(
                day,
                status,
                (date, name) => buyCalc(date, name),
                (date, name) => sellCalc(date, name),
                portfolio,
                traderOptions);
            return (trades, status);
        }
    }
}
