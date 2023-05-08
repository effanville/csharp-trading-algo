using System;
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
    internal sealed class DownloadAllCommand : ICommand
    {
        private readonly IFileSystem fFileSystem;
        private readonly CommandOption<string> fStockFilePathOption;
        private readonly CommandOption<DateTime> fStartDateOption;
        private readonly CommandOption<DateTime> fEndDateOption;

        /// <inheritdoc/>
        public string Name => "all";

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
        public DownloadAllCommand(IFileSystem fileSystem)
        {
            fFileSystem = fileSystem;
            fStockFilePathOption = new CommandOption<string>("stockFilePath", "FilePath to the stock database to add data to.");
            Options.Add(fStockFilePathOption);

            fStartDateOption = new CommandOption<DateTime>("start", "The start date to add data from");
            Options.Add(fStartDateOption);

            fEndDateOption = new CommandOption<DateTime>("end", "The end date to add data to.");
            Options.Add(fEndDateOption);
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
            exchange.Download(fStartDateOption.Value, fEndDateOption.Value, logger).Wait();
            exchange.SaveStockExchange(fStockFilePathOption.Value, fFileSystem, logger);
            return 0;
        }
    }
}
