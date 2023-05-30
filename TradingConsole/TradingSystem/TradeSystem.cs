using System;
using System.IO.Abstractions;

using Common.Structure.Reporting;

using FinancialStructures.StockStructures;

using TradingSystem.Diagnostics;
using TradingSystem.Decisions;
using TradingSystem.Simulator;
using TradingSystem.Trading;
using TradingSystem.PriceSystem;
using TradingSystem.PortfolioStrategies;

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
        public static StockMarketEvolver.Result SetupAndSimulate(
            string stockFilePath,
            DateTime startTime,
            DateTime endTime,
            TimeSpan evolutionIncrement,
            PortfolioStartSettings startSettings,
            PortfolioConstructionSettings constructionSettings,
            DecisionSystemFactory.Settings decisionParameters,
            TradeMechanismSettings traderOptions,
            TradeMechanismType buySellType,
            IFileSystem fileSystem,
            IReportLogger reportLogger)
        {
            IStockExchange exchange;
            StockMarketEvolver.Settings simulatorSettings;
            IPortfolioManager portfolioManager;
            IDecisionSystem decisionSystem;
            ITradeMechanism tradeMechanism;

            using (new Timer(reportLogger, "Setup"))
            {
                using (new Timer(reportLogger, "Loading Exchange"))
                {
                    exchange = StockExchangeFactory.Create(stockFilePath, fileSystem, reportLogger);
                    if (exchange == null)
                    {
                        return StockMarketEvolver.Result.NoResult();
                    }
                }

                simulatorSettings = new StockMarketEvolver.Settings(
                    startTime,
                    endTime,
                    evolutionIncrement,
                    exchange);

                using (new Timer(reportLogger, "Calibrating"))
                {
                    decisionSystem = DecisionSystemFactory.CreateAndCalibrate(decisionParameters, simulatorSettings, reportLogger);
                }

                tradeMechanism = TradeMechanismFactory.Create(buySellType);

                using (new Timer(reportLogger, "Loading Portfolio"))
                {
                    portfolioManager = PortfolioManager.LoadFromFile(fileSystem, startSettings, constructionSettings, reportLogger);
                }
            }

            var randomWobblePriceCalculator = PriceServiceFactory.Create(PriceType.RandomWobble, PriceCalculationSettings.Default(), exchange);

            void FirstOfTheMonthReport(DateTime time, string message)
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

            var callbacks = new StockMarketEvolver.Reporting(startEndReportCallback, FirstOfTheMonthReport, startEndReportCallback, reportLogger);
            DecideThenTradeEnactor tradeEnactor = new DecideThenTradeEnactor(decisionSystem, tradeMechanism, traderOptions);

            return StockMarketEvolver.Simulate(simulatorSettings,
                randomWobblePriceCalculator,
                portfolioManager,
                tradeEnactor,
                callbacks);
        }
    }
}
