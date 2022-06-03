using System;
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

using TradingSystem;
using TradingSystem.Trading.System;

namespace TradingConsole.Commands.Execution
{
    /// <summary>
    /// Command pertaining to running a simulation of the stock market for
    /// a specific decision system.
    /// </summary>
    internal sealed class SimulationCommand : ICommand
    {
        private readonly IReportLogger fLogger;
        private readonly IFileSystem fFileSystem;
        private readonly CommandOption<string> fStockFilePath;
        private readonly CommandOption<string> fPortfolioFilePath;
        private readonly CommandOption<decimal> fStartingCash;

        private readonly CommandOption<DateTime> fStartDate;
        private readonly CommandOption<DateTime> fEndDate;
        private readonly CommandOption<TimeSpan> fTradingGap;
        private readonly CommandOption<DecisionSystem.DecisionSystem> fDecisionType;
        private readonly CommandOption<List<StockStatisticType>> fDecisionSystemStats;
        private readonly CommandOption<decimal> fFractionInvest;


        /// <inheritdoc/>
        public string Name => "simulate";

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
        public SimulationCommand(IReportLogger logger, IFileSystem fileSystem)
        {
            fLogger = logger;
            fFileSystem = fileSystem;

            // Portfolio Setup options
            fPortfolioFilePath = new CommandOption<string>("portfolioFilePath", "The path at which to locate the starting portfolio");
            Options.Add(fPortfolioFilePath);
            fStartingCash = new CommandOption<decimal>("startCash", "The starting amount of cash to create the simulation with.");
            Options.Add(fStartingCash);
            fStartDate = new CommandOption<DateTime>("start", "The date to start on.");

            // Simulation run options.
            fStockFilePath = new CommandOption<string>("stockFilePath", "The path at which to locate the Stock Exchange data.");
            Options.Add(fStockFilePath);
            Options.Add(fStartDate);
            fEndDate = new CommandOption<DateTime>("end", "The date to end on.");
            Options.Add(fEndDate);
            fTradingGap = new CommandOption<TimeSpan>("gap", "The interval between evaluations.");
            Options.Add(fTradingGap);

            // Decision system options.
            fDecisionType = new CommandOption<DecisionSystem.DecisionSystem>("decision", "The type of decision system to use.");
            Options.Add(fDecisionType);
            fDecisionSystemStats = new CommandOption<List<StockStatisticType>>("", "");
            Options.Add(fDecisionSystemStats);
            fFractionInvest = new CommandOption<decimal>("invFrac", "The maximum fraction of available cash to put in any purchase.");
            Options.Add(fFractionInvest);
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
            using (new Timer(fLogger, "TotalTime"))
            {
                var portfolioStartSettings = new PortfolioStartSettings(fPortfolioFilePath.Value, fStartDate.Value, fStartingCash.Value);
                var decisionParameters = new DecisionSystemSetupSettings(fDecisionType.Value, fDecisionSystemStats.Value);
                var traderOptions = new TradeMechanismTraderOptions(fFractionInvest.Value);

                var output = TradeSystem.SetupAndSimulate(
                    fStockFilePath.Value,
                    fStartDate.Value,
                    fEndDate.Value,
                    fTradingGap.Value,
                    portfolioStartSettings,
                    decisionParameters,
                    traderOptions,
                    TradeMechanismType.SellAllThenBuy,
                    fFileSystem,
                    fLogger);

                if (output.Portfolio == null)
                {
                    return 1;
                }

                return 0;
            }
        }
    }
}
