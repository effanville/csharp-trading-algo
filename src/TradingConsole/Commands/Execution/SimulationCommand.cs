using System;
using System.Collections.Generic;
using System.IO.Abstractions;

using Effanville.Common.Console.Commands;
using Effanville.Common.Console.Options;
using Effanville.Common.Structure.Reporting;
using Effanville.TradingStructures.Common.Diagnostics;
using Effanville.TradingStructures.Strategies;
using Effanville.TradingSystem.DependencyInjection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Effanville.TradingConsole.Commands.Execution;

/// <summary>
/// Command pertaining to running a simulation of the stock market for
/// a specific decision system.
/// </summary>
public sealed partial class SimulationCommand : ICommand
{
    private readonly IFileSystem _fileSystem;
    private readonly ITimerFactory _timerFactory;
    private readonly ILogger _logger;
    private readonly IReportLogger _reportLogger;
    private readonly IConfiguration _config;
    private const string StartDateName = "start";
    private const string EndDateName = "end";
    private const string IncrementName = "gap";
    private const string StockFilePathName = "stockFilePath";

    /// <inheritdoc/>
    public string Name => "simulate";

    /// <inheritdoc/>
    public IList<CommandOption> Options { get; } = new List<CommandOption>();
    /// <inheritdoc/>
    public IList<ICommand> SubCommands { get; } = new List<ICommand>();

    /// <summary>
    /// Construct an instance.
    /// </summary>
    public SimulationCommand(
        IFileSystem fileSystem,
        ITimerFactory timerFactory,
        ILogger<SimulationCommand> logger,
        IReportLogger reportLogger,
        IConfiguration config)
    {
        _fileSystem = fileSystem;
        _timerFactory = timerFactory;
        _logger = logger;
        _reportLogger = reportLogger;
        _config = config;
        Options.Add(new CommandOption<string>("jsonSettingsPath", "The path to the json file containing the options for this execution."));

        // Simulation run options.
        Options.Add(new CommandOption<string>(StockFilePathName, "The path at which to locate the Stock Exchange data."));
        Options.Add(new CommandOption<DateTime>(StartDateName, "The date to start on."));
        Options.Add(new CommandOption<DateTime>(EndDateName, "The date to end on."));
        Options.Add(new CommandOption<TimeSpan>(IncrementName, "The interval between evaluations."));
    }

    /// <inheritdoc/>
    public void WriteHelp() => this.WriteHelp(_logger);

    /// <inheritdoc/>
    public bool Validate() => this.Validate(_config, _logger);

    public int Execute()
    {
        using (_timerFactory.Create("TotalTime"))
        {
            Settings? settings = Settings.CreateSettings(Options, _fileSystem);
            if (settings == null)
            {
                return 1;
            }

            _logger.LogInformation(settings.EvolverSettings.StockFilePath);

            var builder = new HostApplicationBuilder();

            _ = builder.Configuration.AddConfiguration(_config);

            _ = builder.Logging.RegisterLogging(_reportLogger);
            _ = builder.Services.RegisterTradingServices(
                settings.EvolverSettings,
                new StrategySettings([
                    new("Default")]),
                _fileSystem);
            builder.Build().Run();

            return 0;
        }
    }
}
