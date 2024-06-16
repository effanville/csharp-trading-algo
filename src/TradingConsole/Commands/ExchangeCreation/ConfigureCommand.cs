using System.Collections.Generic;
using System.IO.Abstractions;

using Effanville.Common.Console;
using Effanville.Common.Console.Commands;
using Effanville.Common.Console.Options;
using Effanville.Common.Structure.Reporting;
using Effanville.FinancialStructures.Stocks;
using Effanville.FinancialStructures.Stocks.Implementation;
using Effanville.FinancialStructures.Stocks.Persistence;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Effanville.TradingConsole.Commands.ExchangeCreation
{
    /// <summary>
    /// Configures a stock exchange from a list of Stocks to include.
    /// </summary>
    public sealed class ConfigureCommand : ICommand
    {
        private readonly IFileSystem _fileSystem;
        private readonly ILogger _logger;
        private readonly IReportLogger _reportLogger;
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
        public ConfigureCommand(IFileSystem fileSystem, ILogger<ConfigureCommand> logger, IReportLogger reportLogger)
        {
            _fileSystem = fileSystem;
            _logger = logger;
            _reportLogger = reportLogger;
            _stockFilePathOption = new CommandOption<string>("stockFilePath", "FilePath to the stock database to add data to.", inputString => !string.IsNullOrWhiteSpace(inputString));
            Options.Add(_stockFilePathOption);
        }

        /// <inheritdoc/>
        public void WriteHelp(IConsole console) => this.WriteHelp(console, _logger);
        
        /// <inheritdoc/>
        public int Execute(IConsole console, IConfiguration config)
        {
            IStockExchange exchange = new StockExchange();
            string inputPath = _stockFilePathOption.Value;
            exchange.Configure(inputPath, _fileSystem, _reportLogger);
            string filePath = _fileSystem.Path.ChangeExtension(inputPath, "xml");
            
            var persistence = new ExchangePersistence();
            var settings = ExchangePersistence.CreateOptions(filePath, _fileSystem);
            persistence.Save(exchange, settings, _reportLogger);
            return 0;
        }

        /// <inheritdoc/>
        public bool Validate(IConsole console, IConfiguration config) => this.Validate(config, console, _logger);
    }
}
