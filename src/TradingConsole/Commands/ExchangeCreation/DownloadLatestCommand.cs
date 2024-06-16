using System.Collections.Generic;
using System.IO.Abstractions;

using Effanville.Common.Console;
using Effanville.Common.Console.Commands;
using Effanville.Common.Console.Options;
using Effanville.Common.Structure.Reporting;
using Effanville.FinancialStructures.Stocks;
using Effanville.FinancialStructures.Stocks.Persistence;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Effanville.TradingConsole.Commands.ExchangeCreation
{
    /// <summary>
    /// Contains logic for the download of stock data.
    /// </summary>
    public sealed class DownloadLatestCommand : ICommand
    {
        private readonly IFileSystem _fileSystem;
        private readonly ILogger _logger;
        private readonly IReportLogger _reportLogger;
        private readonly CommandOption<string> _stockFilePathOption;

        /// <inheritdoc/>
        public string Name => "latest";

        /// <inheritdoc/>
        public IList<CommandOption> Options { get; } = new List<CommandOption>();

        /// <inheritdoc/>
        public IList<ICommand> SubCommands { get; } = new List<ICommand>();

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DownloadLatestCommand(IFileSystem fileSystem, ILogger<DownloadLatestCommand> logger, IReportLogger reportLogger)
        {
            _fileSystem = fileSystem;
            _logger = logger;
            _reportLogger = reportLogger;
            _stockFilePathOption = new CommandOption<string>("stockFilePath", "FilePath to the stock database to add data to.");
            Options.Add(_stockFilePathOption);
        }

        /// <inheritdoc/>
        public void WriteHelp(IConsole console) => this.WriteHelp(console, _logger);

        /// <inheritdoc/>
        public bool Validate(IConsole console, IConfiguration config) => this.Validate(config, console, _logger);

        /// <inheritdoc/>
        public int Execute(IConsole console, IConfiguration config)
        {
            var persistence = new ExchangePersistence();
            var settings = ExchangePersistence.CreateOptions(_stockFilePathOption.Value, _fileSystem);
            IStockExchange exchange = persistence.Load(settings, _reportLogger);
            exchange.Download(_reportLogger).Wait();
            persistence.Save(exchange, settings, _reportLogger);
            return 0;
        }
    }
}
