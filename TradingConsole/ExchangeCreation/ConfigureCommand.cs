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
    public sealed class ConfigureCommand : BaseCommand, ICommand
    {
        private readonly IFileSystem fFileSystem;
        private readonly CommandOption<string> fStockFilePathOption;

        /// <inheritdoc/>
        public override string Name
        {
            get
            {
                return "configure";
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ConfigureCommand(IConsole console, IReportLogger logger, IFileSystem fileSystem)
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
            string inputPath = fStockFilePathOption.Value;
            exchange.Configure(inputPath);
            string filePath = fFileSystem.Path.ChangeExtension(inputPath, "xml");
            exchange.SaveStockExchange(filePath, fLogger);
            return base.Execute(args);
        }
    }
}
