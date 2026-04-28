using System.Collections.Generic;
using System.IO.Abstractions;

using Effanville.Common.Console.Commands;
using Effanville.Common.Console.Options;
using Effanville.FinancialStructures.Persistence;
using Effanville.FinancialStructures.Stocks;
using Effanville.FinancialStructures.Stocks.Persistence;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Effanville.TradingConsole.Commands.ExchangeCreation;

/// <summary>
/// Configures a stock exchange from a list of Stocks to include.
/// </summary>
public sealed class ConfigureCommand : ICommand
{
    private readonly IFileSystem _fileSystem;
    private readonly ILogger _logger;
    private readonly IConfiguration _config;
    private readonly IPersistence<IStockExchange> _persistence;
    private readonly IStockExchangeFactory _stockExchangeFactory;
    private readonly CommandOption<string> _stockFilePathOption;

    /// <inheritdoc/>
    public string Name => "configure";

    /// <inheritdoc/>
    public IList<CommandOption> Options { get; } = new List<CommandOption>();

    /// <inheritdoc/>
    public IList<ICommand> SubCommands { get; } = new List<ICommand>();

    /// <summary>
    /// Default constructor.
    /// </summary>
    public ConfigureCommand(
        IFileSystem fileSystem,
        ILogger<ConfigureCommand> logger,
        IConfiguration config,
        IPersistence<IStockExchange> persistence,
        IStockExchangeFactory stockExchangeFactory)
    {
        _fileSystem = fileSystem;
        _logger = logger;
        _config = config;
        _persistence = persistence;
        _stockExchangeFactory = stockExchangeFactory;
        _stockFilePathOption = new CommandOption<string>("stockFilePath", "FilePath to the stock database to add data to.", inputString => !string.IsNullOrWhiteSpace(inputString));
        Options.Add(_stockFilePathOption);
    }

    /// <inheritdoc/>
    public void WriteHelp() => this.WriteHelp(_logger);

    /// <inheritdoc/>
    public int Execute()
    {
        IStockExchange exchange = _stockExchangeFactory.Create();
        string inputPath = _stockFilePathOption.Value;
        _stockExchangeFactory.Configure(exchange, new(inputPath));
        string filePath = _fileSystem.Path.ChangeExtension(inputPath, "xml");

        var settings = ExchangePersistence.CreateOptions(filePath, _fileSystem);
        _persistence.Save(exchange, settings);
        return 0;
    }

    /// <inheritdoc/>
    public bool Validate() => this.Validate(_config, _logger);
}
