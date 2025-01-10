using System;
using System.Collections.Generic;
using System.IO.Abstractions;

using Effanville.Common.Console.Commands;
using Effanville.Common.Console.Options;
using Effanville.Common.Structure.Reporting;
using Effanville.FinancialStructures.Stocks.Statistics;
using Effanville.TradingStructures.Common.Diagnostics;
using Effanville.TradingStructures.Strategies.Decision;
using Effanville.TradingSystem;
using Effanville.TradingSystem.DependencyInjection;
using Effanville.TradingSystem.MarketEvolvers;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Effanville.TradingConsole.Commands.Execution
{
    /// <summary>
    /// Command pertaining to running a simulation of the stock market for
    /// a specific decision system.
    /// </summary>
    public sealed partial class SimulationCommand : ICommand
    {
        private readonly IFileSystem _fileSystem;
        private readonly ILogger _logger;
        private readonly IReportLogger _reportLogger;

        private const string StartDateName = "start";
        private const string EndDateName = "end";
        private const string FractionInvestName = "invFrac";
        private const string DecisionSystemName = "decision";
        private const string PortfolioFilePathName = "portfolioFilePath";
        private const string IncrementName = "gap";
        private const string StockFilePathName = "stockFilePath";
        private const string StartingCashName = "startCash";
        private const string DecisionSystemStatsName = "decidingStats";

        /// <inheritdoc/>
        public string Name => "simulate";

        /// <inheritdoc/>
        public IList<CommandOption> Options { get; } = new List<CommandOption>();
        /// <inheritdoc/>
        public IList<ICommand> SubCommands { get; } = new List<ICommand>();

        /// <summary>
        /// Construct an instance.
        /// </summary>
        public SimulationCommand(IFileSystem fileSystem, ILogger<SimulationCommand> logger, IReportLogger reportLogger)
        {
            _fileSystem = fileSystem;
            _logger = logger;
            _reportLogger = reportLogger;

            Options.Add(new CommandOption<string>("jsonSettingsPath", "The path to the json file containing the options for this execution."));

            // Portfolio Setup options
            Options.Add(new CommandOption<string>(PortfolioFilePathName, "The path at which to locate the starting portfolio"));
            Options.Add(new CommandOption<decimal>(StartingCashName, "The starting amount of cash to create the simulation with."));

            // Simulation run options.
            Options.Add(new CommandOption<string>(StockFilePathName, "The path at which to locate the Stock Exchange data."));
            Options.Add(new CommandOption<DateTime>(StartDateName, "The date to start on."));
            Options.Add(new CommandOption<DateTime>(EndDateName, "The date to end on."));
            Options.Add(new CommandOption<TimeSpan>(IncrementName, "The interval between evaluations."));

            // Decision system options.
            Options.Add(new CommandOption<DecisionSystem>(DecisionSystemName, "The type of decision system to use."));
            Options.Add(new CommandOption<List<StockStatisticType>>(DecisionSystemStatsName, ""));
            Options.Add(new CommandOption<decimal>(FractionInvestName, "The maximum fraction of available cash to put in any purchase."));
        }

        /// <inheritdoc/>
        public void WriteHelp() => this.WriteHelp(_logger);

        /// <inheritdoc/>
        public bool Validate(IConfiguration config) => this.Validate(config, _logger);
        
        public int Execute(IConfiguration config)
        {
            using (new Timer(_reportLogger, "TotalTime"))
            {
                Settings? settings = Settings.CreateSettings(Options, _fileSystem);
                if (settings == null)
                {
                    return 1;
                }

                _logger.Log(LogLevel.Information, settings.StockFilePath);
                
                var builder = new HostApplicationBuilder();
                builder.Logging.RegisterLogging(_reportLogger);
                builder.Services.RegisterTradingServices(
                    settings.StockFilePath,
                    settings.StartTime,
                    settings.EndTime,
                    settings.EvolutionIncrement,
                    settings.PortfolioSettings,
                    settings.PortfolioConstructionSettings,
                    settings.DecisionSystemSettings,
                    _fileSystem);
                builder.Build().RunAsync();

                return 0;
            }
        }
    }
}
