using System.Collections.Generic;
using System.IO.Abstractions;

using Common.Console;
using Common.Console.Commands;
using Common.Console.Options;
using Common.Structure.Reporting;

using FinancialStructures.StockStructures.Statistics;

using TradingConsole.BuySellSystem;
using TradingConsole.DecisionSystem;
using TradingConsole.TradingSystem;

namespace TradingConsole.Commands.Execution
{
    /// <summary>
    /// A command for trading with IB.
    /// </summary>
    public sealed class TradingCommand : ICommand
    {
        private readonly IReportLogger fLogger;
        private readonly IFileSystem fFileSystem;
        private readonly CommandOption<string> fStockFilePath;
        private readonly CommandOption<string> fPortfolioFilePath;
        private readonly CommandOption<DecisionSystem.DecisionSystem> fDecisionType;
        private readonly CommandOption<List<StockStatisticType>> fDecisionSystemStats;

        /// <inheritdoc/>
        public string Name => "trade";

        /// <inheritdoc/>
        public IList<CommandOption> Options
        {
            get;
        } = new List<CommandOption>();
        /// <inheritdoc/>
        public IList<ICommand> SubCommands
        {
            get;
        } = new List<ICommand>();

        /// <summary>
        /// Construct an instance.
        /// </summary>
        public TradingCommand(IReportLogger logger, IFileSystem fileSystem)
        {
            fLogger = logger;
            fFileSystem = fileSystem;
            fStockFilePath = new CommandOption<string>("stockFilePath", "The path at which to locate the Stock Exchange data.");
            fPortfolioFilePath = new CommandOption<string>("portfolioFilePath", "The path at which to locate the starting portfolio");
        }

        /// <inheritdoc/>
        public void WriteHelp(IConsole console)
        {
            CommandExtensions.WriteHelp(this, console);
        }

        /// <inheritdoc/>
        public bool Validate(IConsole console, string[] args)
        {
            return CommandExtensions.Validate(this, args, console);
        }

        /// <inheritdoc/>
        public int Execute(IConsole console, string[] args = null)
        {
            var decisionParameters = new DecisionSystemFactory.Settings(fDecisionType.Value, fDecisionSystemStats.Value, 1.05, 1.0, 1);
            var startSettings = new PortfolioStartSettings(fPortfolioFilePath.Value, default, 0.0m);

            var system = new RealTrader(fStockFilePath.Value, startSettings, decisionParameters, TradeMechanismType.IB, fFileSystem, fLogger);
            system.Run(fPortfolioFilePath.Value);
            return 0;
        }
    }
}
