using System.Collections.Generic;
using System.IO.Abstractions;

using Effanville.Common.Console.Commands;
using Effanville.Common.Console.Options;
using Effanville.Common.Structure.Reporting;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Effanville.TradingConsole.Commands.ExchangeCreation
{
    /// <summary>
    /// Command that controls the downloading of stock data.
    /// </summary>
    public sealed class DownloadCommand : ICommand
    {
        private readonly ILogger _logger;
        
        /// <inheritdoc/>
        public string Name => "download";

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
        /// Default Constructor.
        /// </summary>
        public DownloadCommand(
            IFileSystem fileSystem, 
            ILogger<DownloadCommand> logger,
            ILogger<DownloadAllCommand> downloadAllLogger,
            ILogger<DownloadLatestCommand> downloadLatestLogger,
            IReportLogger reportLogger)
        {
            _logger = logger;
            SubCommands.Add(new DownloadAllCommand(fileSystem, downloadAllLogger, reportLogger));
            SubCommands.Add(new DownloadLatestCommand(fileSystem, downloadLatestLogger, reportLogger));
        }

        /// <inheritdoc/>
        public void WriteHelp() => this.WriteHelp(_logger);

        /// <inheritdoc/>
        public int Execute(IConfiguration config) => this.Execute(config, _logger);

        /// <inheritdoc/>
        public bool Validate(IConfiguration config) => this.Validate(config, _logger);
    }
}
