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
    internal sealed class DownloadLatestCommand : ICommand
    {
        private readonly IFileSystem _fileSystem;
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
        public DownloadLatestCommand(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            _stockFilePathOption = new CommandOption<string>("stockFilePath", "FilePath to the stock database to add data to.");
            Options.Add(_stockFilePathOption);
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
            exchange.Download(logger).Wait();
            persistence.Save(exchange, settings, logger);
            return 0;
        }
    }
}
