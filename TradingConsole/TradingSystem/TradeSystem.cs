using System;
using System.IO.Abstractions;

using Common.Structure.DataStructures;
using Common.Structure.Reporting;

using FinancialStructures.Database;
using FinancialStructures.NamingStructures;
using FinancialStructures.StockStructures;

using TradingConsole.BuySellSystem;
using TradingConsole.DecisionSystem;

using TradingSystem;
using TradingSystem.Decisions.Models;
using TradingSystem.Decisions.System;
using TradingSystem.Simulator;
using TradingSystem.Trading.Models;
using TradingSystem.Trading.System;

namespace TradingConsole.TradingSystem
{
    /// <summary>
    /// Contains the logic for the creation of user specified options for a simulation of
    /// a stock market trading algorithm.
    /// </summary>
    public static partial class TradeSystem
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
        public static (IPortfolio, DecisionHistory, TradeHistory) SetupAndSimulate(
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
            var tradeMechanismSettings = new PriceCalculationSettings();
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
                    decisionSystem = DecisionSystemFactory.Create(decisionParameters);
                    decisionSystem.Calibrate(simulatorSettings, reportLogger);
                }

                tradeMechanism = TradeMechanismFactory.Create(buySellType);

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

            var callbacks = new SimulationCallbacks(startEndReportCallback, reportCallback, startEndReportCallback);
            SimpleTradeEnactor tradeEnactor = new SimpleTradeEnactor(decisionSystem, tradeMechanism, traderOptions);

            return StockMarketHistorySimulator.Simulate(simulatorSettings,
                tradeMechanismSettings,
                isCalcTimeValid,
                callbacks,
                portfolio.Copy(),
                tradeEnactor,
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
    }
}
