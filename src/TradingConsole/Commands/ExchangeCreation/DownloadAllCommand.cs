using System;
using System.Collections.Generic;
using System.IO.Abstractions;

using Effanville.Common.Console;
using Effanville.Common.Console.Commands;
using Effanville.Common.Console.Options;
using Effanville.Common.Structure.Reporting;
using Effanville.FinancialStructures.Stocks;
using Effanville.FinancialStructures.Stocks.Persistence;

namespace Effanville.TradingConsole.Commands.ExchangeCreation
{
    /// <summary>
    /// Contains logic for the download of stock data.
    /// </summary>
    internal sealed class DownloadAllCommand : ICommand
    {
        private readonly IFileSystem _fileSystem;
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
        public DownloadAllCommand(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            _stockFilePathOption = new CommandOption<string>("stockFilePath", "FilePath to the stock database to add data to.");
            Options.Add(_stockFilePathOption);

            _startDateOption = new CommandOption<DateTime>("start", "The start date to add data from");
            Options.Add(_startDateOption);

            _endDateOption = new CommandOption<DateTime>("end", "The end date to add data to.");
            Options.Add(_endDateOption);
        }

        /// <inheritdoc/>
        public void WriteHelp(IConsole console) => CommandExtensions.WriteHelp(this, console);

        /// <inheritdoc/>
        public bool Validate(IConsole console, string[] args) => Validate(console, null, args);

        /// <inheritdoc/>
        public bool Validate(IConsole console, IReportLogger? logger, string[] args) 
            => this.Validate(args, console, logger);


        /// <inheritdoc/>
        public int Execute(IConsole console, string[] args) => Execute(console, null, args);

        /// <inheritdoc/>
        public int Execute(IConsole console, IReportLogger? logger, string[] args)
        {
            var persistence = new ExchangePersistence();
            var settings = ExchangePersistence.CreateOptions(_stockFilePathOption.Value, _fileSystem);
            IStockExchange exchange = persistence.Load(settings, logger);
            exchange.Download(_startDateOption.Value, _endDateOption.Value, logger).Wait();
            persistence.Save(exchange, settings, logger);
            return 0;
        }
    }
}
