using System.Collections.Generic;
using System.IO.Abstractions;

using Effanville.Common.Console;
using Effanville.Common.Console.Commands;
using Effanville.Common.Console.Options;
using Effanville.Common.Structure.Reporting;
using Effanville.FinancialStructures.Stocks;
using Effanville.FinancialStructures.Stocks.Implementation;
using Effanville.FinancialStructures.Stocks.Persistence;

namespace Effanville.TradingConsole.Commands.ExchangeCreation
{
    /// <summary>
    /// Configures a stock exchange from a list of Stocks to include.
    /// </summary>
    internal sealed class ConfigureCommand : ICommand
    {
        private readonly IFileSystem _fileSystem;
        private readonly CommandOption<string> _stockFilePathOption;

        /// <inheritdoc/>
        public string Name => "configure";
        
        /// <inheritdoc/>
        public IList<CommandOption> Options { get; } = new List<CommandOption>();
        
        /// <inheritdoc/>
        public IList<ICommand> SubCommands { get; } = new List<ICommand>();

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ConfigureCommand(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            _stockFilePathOption = new CommandOption<string>("stockFilePath", "FilePath to the stock database to add data to.", inputString => !string.IsNullOrWhiteSpace(inputString));
            Options.Add(_stockFilePathOption);
        }

        /// <inheritdoc/>
        public void WriteHelp(IConsole console) => CommandExtensions.WriteHelp(this, console);
        
        /// <inheritdoc/>
        public int Execute(IConsole console, string[] args) => Execute(console, null, args);

        /// <inheritdoc/>
        public int Execute(IConsole console, IReportLogger? logger, string[] args)
        {
            IStockExchange exchange = new StockExchange();
            string inputPath = _stockFilePathOption.Value;
            exchange.Configure(inputPath, _fileSystem, logger);
            string filePath = _fileSystem.Path.ChangeExtension(inputPath, "xml");
            
            var persistence = new ExchangePersistence();
            var settings = ExchangePersistence.CreateOptions(filePath, _fileSystem);
            persistence.Save(exchange, settings, logger);
            return 0;
        }

        /// <inheritdoc/>
        public bool Validate(IConsole console, string[] args) => Validate(console, null, args);

        /// <inheritdoc/>
        public bool Validate(IConsole console, IReportLogger? logger, string[] args) 
            => this.Validate(args, console, logger);
    }
}
