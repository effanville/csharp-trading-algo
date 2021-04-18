using System;
using System.IO.Abstractions;
using ConsoleCommon;
using ConsoleCommon.Commands;
using ConsoleCommon.Options;
using FinancialStructures.StockStructures;
using FinancialStructures.StockStructures.Implementation;
using StructureCommon.Reporting;

namespace TradingConsole.ExchangeCreation
{
    /// <summary>
    /// Contains logic for the download of stock data.
    /// </summary>
    public sealed class DownloadAllCommand : BaseCommand, ICommand
    {
        private readonly IFileSystem fFileSystem;
        private readonly CommandOption<string> fStockFilePathOption;
        private readonly CommandOption<DateTime> fStartDateOption;
        private readonly CommandOption<DateTime> fEndDateOption;

        /// <inheritdoc/>
        public override string Name
        {
            get
            {
                return "all";
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DownloadAllCommand(IConsole console, IReportLogger logger, IFileSystem fileSystem)
            : base(console, logger)
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
        public override int Execute(string[] args)
        {
            IStockExchange exchange = new StockExchange();
            exchange.LoadStockExchange(fStockFilePathOption.Value, fFileSystem, fLogger);
            exchange.Download(fStartDateOption.Value, fEndDateOption.Value, fLogger);
            exchange.SaveStockExchange(fStockFilePathOption.Value, fFileSystem, fLogger);
            return base.Execute(args);
        }
    }
}
