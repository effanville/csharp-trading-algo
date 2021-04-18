using System.IO.Abstractions;
using ConsoleCommon;
using ConsoleCommon.Commands;
using StructureCommon.Reporting;

namespace TradingConsole.ExchangeCreation
{
    /// <summary>
    /// Command that controls the downloading of stock data.
    /// </summary>
    public sealed class DownloadCommand : BaseCommand, ICommand
    {
        /// <inheritdoc/>
        public override string Name
        {
            get
            {
                return "download";
            }
        }

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public DownloadCommand(IConsole console, IReportLogger logger, IFileSystem fileSystem)
            : base(console, logger)
        {
            SubCommands.Add(new DownloadAllCommand(console, logger, fileSystem));
            SubCommands.Add(new DownloadLatestCommand(console, logger, fileSystem));
        }
    }
}
