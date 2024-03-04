using System;
using System.IO.Abstractions;

using Effanville.Common.Structure.Reporting;
using Effanville.FinancialStructures.Stocks;
using Effanville.TradingStructures.Common.Diagnostics;
using Effanville.TradingStructures.Pricing;
using Effanville.TradingStructures.Strategies.Decision;
using Effanville.TradingStructures.Strategies.Portfolio;
using Effanville.TradingStructures.Trading;
using Effanville.TradingStructures.Trading.Implementation;
using Effanville.TradingSystem.MarketEvolvers;

using DecisionSystemFactory = Effanville.TradingStructures.Strategies.Decision.DecisionSystemFactory;

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
        public static EvolverResult SetupAndSimulate(
            string stockFilePath,
            DateTime startTime,
            DateTime endTime,
            TimeSpan evolutionIncrement,
            PortfolioStartSettings startSettings,
            PortfolioConstructionSettings constructionSettings,
            DecisionSystemFactory.Settings decisionParameters,
            TradeMechanismSettings traderOptions,
            IFileSystem fileSystem,
            IReportLogger reportLogger)
        {
            IStockExchange exchange;
            TimeIncrementEvolverSettings simulatorSettings;
            IPortfolioManager portfolioManager;
            IDecisionSystem decisionSystem;
            IMarketExchange tradeMechanism;

            using (new Timer(reportLogger, "Setup"))
            {
                using (new Timer(reportLogger, "Loading Exchange"))
                {
                    exchange = StockExchangeFactory.Create(stockFilePath, fileSystem, reportLogger);
                    if (exchange == null)
                    {
                        return EvolverResult.NoResult();
                    }
                }

                simulatorSettings = new TimeIncrementEvolverSettings(
                    startTime,
                    endTime,
                    evolutionIncrement,
                    exchange);

                using (new Timer(reportLogger, "Calibrating"))
                {
                    var decisionSettings = new DecisionSystemSettings(
                        simulatorSettings.StartTime,
                        simulatorSettings.BurnInEnd,
                        simulatorSettings.Exchange.Stocks.Count,
                        simulatorSettings.Exchange);
                    decisionSystem = DecisionSystemFactory.CreateAndCalibrate(decisionParameters, decisionSettings, reportLogger);
                    if (decisionSettings.BurnInEnd == decisionSettings.StartTime)
                    {
                        simulatorSettings.DoesntRequireBurnIn();
                    }
                }

                tradeMechanism = new SimulationExchange(traderOptions, reportLogger);

                using (new Timer(reportLogger, "Loading Portfolio"))
                {
                    portfolioManager = PortfolioManager.LoadFromFile(fileSystem, startSettings, constructionSettings, reportLogger);
                }
            }

            var randomWobblePriceCalculator = PriceServiceFactory.Create(PriceType.RandomWobble, PriceCalculationSettings.Default(), exchange, null);

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

            return TimeIncrementEvolver.Simulate(simulatorSettings,
                randomWobblePriceCalculator,
                portfolioManager,
                decisionSystem,
                tradeMechanism,
                startEndReportCallback,
                FirstOfTheMonthReport,
                startEndReportCallback,
                reportLogger);
        }
    }
}
