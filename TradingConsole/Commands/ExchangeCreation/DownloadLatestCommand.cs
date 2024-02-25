using System.Collections.Generic;
using System.IO.Abstractions;

using Effanville.Common.Console;
using Effanville.Common.Console.Commands;
using Effanville.Common.Console.Options;
using Effanville.Common.Structure.Reporting;
using Effanville.FinancialStructures.Persistence;
using Effanville.FinancialStructures.Stocks;
using Effanville.FinancialStructures.Stocks.Implementation;
using Effanville.FinancialStructures.Stocks.Persistence;


namespace TradingConsole.Commands.ExchangeCreation
{
    /// <summary>
    /// Contains logic for the download of stock data.
    /// </summary>
    internal sealed class DownloadLatestCommand : ICommand
    {
        private readonly IFileSystem fFileSystem;
        private readonly CommandOption<string> fStockFilePathOption;

        /// <inheritdoc/>
        public string Name => "latest";

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
        /// Default constructor.
        /// </summary>
        public DownloadLatestCommand(IFileSystem fileSystem)
        {
            fFileSystem = fileSystem;
            fStockFilePathOption = new CommandOption<string>("stockFilePath", "FilePath to the stock database to add data to.");
            Options.Add(fStockFilePathOption);
        }

        /// <inheritdoc/>
        public void WriteHelp(IConsole console) => CommandExtensions.WriteHelp(this, console);

        /// <inheritdoc/>
        public bool Validate(IConsole console, string[] args) => Validate(console, null, args);

        /// <inheritdoc/>
        public bool Validate(IConsole console, IReportLogger logger, string[] args) => CommandExtensions.Validate(this, args, console, logger);

        /// <inheritdoc/>
        public int Execute(IConsole console, string[] args = null) => Execute(console, null, args);

        /// <inheritdoc/>
        public int Execute(IConsole console, IReportLogger logger, string[] args)
        {
            var persistence = new ExchangePersistence();
            var settings = ExchangePersistence.CreateOptions(fStockFilePathOption.Value, fFileSystem);
            IStockExchange exchange = persistence.Load(settings, logger);
            exchange.Download(logger).Wait();
            persistence.Save(exchange, settings, logger);
            return 0;
        }
    }
}
