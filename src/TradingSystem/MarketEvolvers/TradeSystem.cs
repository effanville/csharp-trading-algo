using System;
using System.IO.Abstractions;
using System.Threading.Tasks;

using Effanville.Common.Structure.Reporting;
using Effanville.FinancialStructures.Stocks;
using Effanville.TradingStructures.Common.Diagnostics;
using Effanville.TradingStructures.Strategies;
using Effanville.TradingStructures.Strategies.Decision;
using Effanville.TradingStructures.Strategies.Execution;
using Effanville.TradingStructures.Strategies.Portfolio;

namespace Effanville.TradingSystem.MarketEvolvers
{
    /// <summary>
    /// Contains the logic for the creation of user specified options for a simulation of
    /// a stock market trading algorithm.
    /// </summary>
    public static class TradeSystem
    {
        /// <summary>
        /// Performs the setup of user specified inputs to the simulation.
        /// </summary>
        /// <param name="stockFilePath">FilePath to the stock database</param>
        /// <param name="startTime">Start time of the simulation</param>
        /// <param name="endTime">End time of the simulation</param>
        /// <param name="evolutionIncrement">The gap between trade times.</param>
        /// <param name="startSettings">The settings for the starting portfolio.</param>
        /// <param name="constructionSettings">The settings detailing how to construct the portfolio.</param>
        /// <param name="decisionParameters">Parameters to specify the decision system.</param>
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
            IFileSystem fileSystem,
            IReportLogger reportLogger)
        {
            TimeIncrementEvolverSettings simulatorSettings;
            IPortfolioManager portfolioManager;
            IDecisionSystem decisionSystem;

            using (new Timer(reportLogger, "Setup"))
            {
                IStockExchange exchange;
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
                        simulatorSettings.BurnInStart,
                        simulatorSettings.StartTime,
                        simulatorSettings.Exchange.Stocks.Count,
                        simulatorSettings.Exchange);
                    decisionSystem = DecisionSystemFactory.CreateAndCalibrate(decisionParameters, decisionSettings, reportLogger);
                    if (decisionSettings.BurnInEnd == decisionSettings.StartTime)
                    {
                        simulatorSettings.DoesntRequireBurnIn();
                    }
                }

                using (new Timer(reportLogger, "Loading Portfolio"))
                {
                    portfolioManager = PortfolioManager.LoadFromFile(fileSystem, startSettings, constructionSettings, reportLogger);
                }
            }
            
            var executionStrategy = ExecutionStrategyFactory.Create(StrategyType.TimeIncrementExecution, reportLogger, simulatorSettings.Exchange, decisionSystem);
            var strategy = new Strategy(decisionSystem, executionStrategy, portfolioManager, reportLogger);
            var evolver = new EventEvolver(simulatorSettings, simulatorSettings.Exchange, strategy, reportLogger);
            
            using (new Timer(reportLogger, "Execution"))
            {
                evolver.Initialise();
                evolver.Start();
                while (evolver.IsActive)
                {
                    _ = Task.Delay(100);
                }
            }
            return evolver.Result;
        }
    }
}
