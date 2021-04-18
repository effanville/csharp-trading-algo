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
    public sealed class DownloadLatestCommand : BaseCommand, ICommand
    {
        private readonly IFileSystem fFileSystem;
        private readonly CommandOption<string> fStockFilePathOption;

        /// <inheritdoc/>
        public override string Name
        {
            get
            {
                return "latest";
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DownloadLatestCommand(IConsole console, IReportLogger logger, IFileSystem fileSystem)
            : base(console, logger)
        {
            fFileSystem = fileSystem;
            fStockFilePathOption = new CommandOption<string>("stockFilePath", "FilePath to the stock database to add data to.");
            Options.Add(fStockFilePathOption);
        }

        /// <inheritdoc/>
        public override int Execute(string[] args)
        {
            IStockExchange exchange = new StockExchange();
            exchange.LoadStockExchange(fStockFilePathOption.Value, fFileSystem, fLogger);
            exchange.Download(fLogger);
            exchange.SaveStockExchange(fStockFilePathOption.Value, fFileSystem, fLogger);
            return base.Execute(args);
        }
    }
}
