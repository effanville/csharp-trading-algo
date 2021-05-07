using System.Collections.Generic;
using System.IO.Abstractions;
using ConsoleCommon;
using ConsoleCommon.Commands;
using ConsoleCommon.Options;
using StructureCommon.Reporting;
using TradingConsole.BuySellSystem;
using TradingConsole.DecisionSystem;
using TradingConsole.Statistics;

namespace TradingConsole.Simulation
{
    /// <summary>
    /// A command for trading with IB.
    /// </summary>
    public sealed class TradingCommand : BaseCommand, ICommand
    {
        private readonly IFileSystem fFileSystem;
        private readonly CommandOption<string> fStockFilePath;
        private readonly CommandOption<string> fPortfolioFilePath;
        private readonly CommandOption<DecisionSystem.DecisionSystem> fDecisionType;
        private readonly CommandOption<List<StatisticType>> fDecisionSystemStats;

        /// <inheritdoc/>
        public override string Name
        {
            get
            {
                return "trade";
            }
        }

        /// <summary>
        /// Construct an instance.
        /// </summary>
        public TradingCommand(IConsole console, IReportLogger logger, IFileSystem fileSystem)
            : base(console, logger)
        {
            fFileSystem = fileSystem;
            fStockFilePath = new CommandOption<string>("stockFilePath", "The path at which to locate the Stock Exchange data.");
            fPortfolioFilePath = new CommandOption<string>("portfolioFilePath", "The path at which to locate the starting portfolio");
        }

        /// <inheritdoc/>
        public override int Execute(string[] args)
        {
            var simulationParameters = new SimulationParameters(default, default, default, 0.0);
            var decisionParameters = new DecisionSystemParameters(fDecisionType.Value, fDecisionSystemStats.Value);
            var system = new TradingSimulation(fStockFilePath.Value, fPortfolioFilePath.Value, simulationParameters, decisionParameters, BuySellType.IB, fFileSystem, fLogger);
            var stats = new TradingStatistics();
            system.Run(fPortfolioFilePath.Value, stats);
            return 0;
        }
    }
}
