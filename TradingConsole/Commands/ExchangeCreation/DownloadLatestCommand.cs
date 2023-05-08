using System.Collections.Generic;
using System.IO.Abstractions;

using Common.Console;
using Common.Console.Commands;
using Common.Console.Options;
using Common.Structure.Reporting;

using FinancialStructures.StockStructures;
using FinancialStructures.StockStructures.Implementation;


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
        public void WriteHelp(IConsole console)
        {
            CommandExtensions.WriteHelp(this, console);
        }

        /// <inheritdoc/>
        public bool Validate(IConsole console, string[] args)
        {
            return Validate(console, null, args);
        }

        /// <inheritdoc/>
        public bool Validate(IConsole console, IReportLogger logger, string[] args)
        {
            return CommandExtensions.Validate(this, args, console, logger);
        }

        /// <inheritdoc/>
        public int Execute(IConsole console, string[] args = null)
        {
            return Execute(console, null, args);
        }

        /// <inheritdoc/>
        public int Execute(IConsole console, IReportLogger logger, string[] args)
        {
            IStockExchange exchange = new StockExchange();
            exchange.LoadStockExchange(fStockFilePathOption.Value, fFileSystem, logger);
            exchange.Download(logger).Wait();
            exchange.SaveStockExchange(fStockFilePathOption.Value, fFileSystem, logger);
            return CommandExtensions.Execute(this, console, args);
        }
    }
}
