using System;
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
    /// Command pertaining to running a simulation of the stock market for
    /// a specific decision system.
    /// </summary>
    public sealed class SimulationCommand : BaseCommand, ICommand
    {
        private readonly IFileSystem fFileSystem;
        private readonly CommandOption<string> fStockFilePath;
        private readonly CommandOption<string> fPortfolioFilePath;
        private readonly CommandOption<double> fStartingCash;

        private readonly CommandOption<DateTime> fStartDate;
        private readonly CommandOption<DateTime> fEndDate;
        private readonly CommandOption<TimeSpan> fTradingGap;
        private readonly CommandOption<DecisionSystem.DecisionSystem> fDecisionType;
        private readonly CommandOption<List<StatisticType>> fDecisionSystemStats;

        /// <inheritdoc/>
        public override string Name
        {
            get
            {
                return "simulate";
            }
        }

        /// <summary>
        /// Construct an instance.
        /// </summary>
        public SimulationCommand(IConsole console, IReportLogger logger, IFileSystem fileSystem)
            : base(console, logger)
        {
            fFileSystem = fileSystem;
            fStockFilePath = new CommandOption<string>("stockFilePath", "The path at which to locate the Stock Exchange data.");
            fPortfolioFilePath = new CommandOption<string>("portfolioFilePath", "The path at which to locate the starting portfolio");
            fStartingCash = new CommandOption<double>("", "");
            fStartDate = new CommandOption<DateTime>("", "");
            fEndDate = new CommandOption<DateTime>("", "");
            fTradingGap = new CommandOption<TimeSpan>("", "");
            fDecisionType = new CommandOption<DecisionSystem.DecisionSystem>("", "");
            fDecisionSystemStats = new CommandOption<List<StatisticType>>("", "");
        }

        /// <inheritdoc/>
        public override int Execute(string[] args)
        {
            var simulationParameters = new SimulationParameters(fStartDate.Value, fEndDate.Value, fTradingGap.Value, fStartingCash.Value);
            var decisionParameters = new DecisionSystemParameters(fDecisionType.Value, fDecisionSystemStats.Value);
            var system = new TradingSimulation(fStockFilePath.Value, fPortfolioFilePath.Value, simulationParameters, decisionParameters, BuySellType.Simulate, fFileSystem, fLogger, fConsole);

            var stats = new TradingStatistics();
            system.SimulateRun(stats);
            return 0;
        }
    }
}
