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
    /// Configures a stock exchange from a list of Stocks to include.
    /// </summary>
    internal sealed class ConfigureCommand : ICommand
    {
        private readonly IFileSystem fFileSystem;
        private readonly CommandOption<string> fStockFilePathOption;

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

        /// <inheritdoc/>
        public string Name => "configure";

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ConfigureCommand(IFileSystem fileSystem)
        {
            fFileSystem = fileSystem;
            fStockFilePathOption = new CommandOption<string>("stockFilePath", "FilePath to the stock database to add data to.", inputString => !string.IsNullOrWhiteSpace(inputString));
            Options.Add(fStockFilePathOption);
        }

        /// <inheritdoc/>
        public void WriteHelp(IConsole console) => CommandExtensions.WriteHelp(this, console);
        /// <inheritdoc/>
        public int Execute(IConsole console, string[] args) => Execute(console, null, args);

        /// <inheritdoc/>
        public int Execute(IConsole console, IReportLogger logger, string[] args)
        {
            IStockExchange exchange = new StockExchange();
            string inputPath = fStockFilePathOption.Value;
            exchange.Configure(inputPath, fFileSystem, logger);
            string filePath = fFileSystem.Path.ChangeExtension(inputPath, "xml");
            
            var persistence = new XmlExchangePersistence();
            var settings = new  XmlFilePersistenceOptions(filePath, fFileSystem);
            persistence.Save(exchange, settings, logger);
            return 0;
        }

        /// <inheritdoc/>
        public bool Validate(IConsole console, string[] args) => Validate(console, null, args);

        /// <inheritdoc/>
        public bool Validate(IConsole console, IReportLogger logger, string[] args) => CommandExtensions.Validate(this, args, console, logger);
    }
}
