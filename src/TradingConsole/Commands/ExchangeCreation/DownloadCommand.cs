using System.Collections.Generic;

using Effanville.Common.Console.Commands;
using Effanville.Common.Console.Options;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Effanville.TradingConsole.Commands.ExchangeCreation;

/// <summary>
/// Command that controls the downloading of stock data.
/// </summary>
public sealed class DownloadCommand : ICommand
{
    private readonly ILogger _logger;
    private readonly IConfiguration _config;

    /// <inheritdoc/>
    public string Name => "download";

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
    /// Default Constructor.
    /// </summary>
    public DownloadCommand(
        ILogger<DownloadCommand> logger,
        DownloadAllCommand downloadAll,
        DownloadLatestCommand downloadLatest,
        IConfiguration config)
    {
        _logger = logger;
        _config = config;
        SubCommands.Add(downloadAll);
        SubCommands.Add(downloadLatest);
    }

    /// <inheritdoc/>
    public void WriteHelp() => this.WriteHelp(_logger);

    /// <inheritdoc/>
    public int Execute() => this.Execute(_config, _logger);

    /// <inheritdoc/>
    public bool Validate() => this.Validate(_config, _logger);
}
