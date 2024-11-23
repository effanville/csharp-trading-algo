using System;
using System.Collections.Generic;
using System.IO.Abstractions;

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
    public sealed class DownloadAllCommand : ICommand
    {
        private readonly IFileSystem _fileSystem;
        private readonly ILogger _logger;
        private readonly IReportLogger _reportLogger;
        private readonly CommandOption<string> _stockFilePathOption;
        private readonly CommandOption<DateTime> _startDateOption;
        private readonly CommandOption<DateTime> _endDateOption;

        /// <inheritdoc/>
        public string Name => "all";

        /// <inheritdoc/>
        public IList<CommandOption> Options { get; } = new List<CommandOption>();

        /// <inheritdoc/>
        public IList<ICommand> SubCommands { get; } = new List<ICommand>();

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DownloadAllCommand(IFileSystem fileSystem, ILogger<DownloadAllCommand> logger, IReportLogger reportLogger)
        {
            _fileSystem = fileSystem;
            _logger = logger;
            _reportLogger = reportLogger;
            _stockFilePathOption = new CommandOption<string>("stockFilePath", "FilePath to the stock database to add data to.");
            Options.Add(_stockFilePathOption);

            _startDateOption = new CommandOption<DateTime>("start", "The start date to add data from");
            Options.Add(_startDateOption);

            _endDateOption = new CommandOption<DateTime>("end", "The end date to add data to.");
            Options.Add(_endDateOption);
        }

        /// <inheritdoc/>
        public void WriteHelp() => this.WriteHelp(_logger);

        /// <inheritdoc/>
        public bool Validate(IConfiguration config) => this.Validate(config, _logger);

        /// <inheritdoc/>
        public int Execute(IConfiguration config)
        {
            var persistence = new ExchangePersistence();
            var settings = ExchangePersistence.CreateOptions(_stockFilePathOption.Value, _fileSystem);
            IStockExchange exchange = persistence.Load(settings, _reportLogger);
            exchange.Download(_startDateOption.Value, _endDateOption.Value, _reportLogger).Wait();
            persistence.Save(exchange, settings, _reportLogger);
            return 0;
        }
    }
}
