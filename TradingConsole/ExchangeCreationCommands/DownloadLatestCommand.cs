using System.IO.Abstractions;
using Common.Console;
using Common.Console.Commands;
using Common.Console.Options;
using FinancialStructures.StockStructures;
using FinancialStructures.StockStructures.Implementation;
using Common.Structure.Reporting;
using System.Collections.Generic;

namespace TradingConsole.ExchangeCreationCommands
{
    /// <summary>
    /// Contains logic for the download of stock data.
    /// </summary>
    internal sealed class DownloadLatestCommand : ICommand
    {
        private readonly IReportLogger fLogger;
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
        public DownloadLatestCommand(IReportLogger logger, IFileSystem fileSystem)
        {
            fLogger = logger;
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
            return CommandExtensions.Validate(this, args, console);
        }

        /// <inheritdoc/>
        public int Execute(IConsole console, string[] args = null)
        {
            IStockExchange exchange = new StockExchange();
            exchange.LoadStockExchange(fStockFilePathOption.Value, fFileSystem, fLogger);
            exchange.Download(fLogger);
            exchange.SaveStockExchange(fStockFilePathOption.Value, fFileSystem, fLogger);
            return CommandExtensions.Execute(this, console, args);
        }
    }
}
