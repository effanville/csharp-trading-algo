using System.IO.Abstractions;
using Common.Console;
using Common.Console.Commands;
using Common.Console.Options;
using FinancialStructures.StockStructures;
using FinancialStructures.StockStructures.Implementation;
using System.Collections.Generic;
using Common.Structure.Reporting;

namespace TradingConsole.ExchangeCreation
{
    /// <summary>
    /// Configures a stock exchange from a list of Stocks to include.
    /// </summary>
    internal sealed class ConfigureCommand : ICommand
    {
        private readonly IFileSystem fFileSystem;
        private readonly IReportLogger fLogger;
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
        public ConfigureCommand(IReportLogger reportLogger, IFileSystem fileSystem)
        {
            fLogger = reportLogger;
            fFileSystem = fileSystem;
            fStockFilePathOption = new CommandOption<string>("stockFilePath", "FilePath to the stock database to add data to.", inputString => !string.IsNullOrWhiteSpace(inputString));
            Options.Add(fStockFilePathOption);
        }

        /// <inheritdoc/>
        public void WriteHelp(IConsole console)
        {
            CommandExtensions.WriteHelp(this, console);
        }

        /// <inheritdoc/>
        public int Execute(IConsole console, string[] args)
        {
            IStockExchange exchange = new StockExchange();
            string inputPath = fStockFilePathOption.Value;
            exchange.Configure(inputPath);
            string filePath = fFileSystem.Path.ChangeExtension(inputPath, "xml");
            exchange.SaveStockExchange(filePath, fLogger);
            return CommandExtensions.Execute(this, console, args);
        }

        /// <inheritdoc/>
        public bool Validate(IConsole console, string[] args)
        {
            return CommandExtensions.Validate(this, args, console);
        }
    }
}
