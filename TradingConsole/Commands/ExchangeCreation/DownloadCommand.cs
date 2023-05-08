﻿using System.Collections.Generic;
using System.IO.Abstractions;

using Common.Console;
using Common.Console.Commands;
using Common.Console.Options;
using Common.Structure.Reporting;

namespace TradingConsole.Commands.ExchangeCreation
{
    /// <summary>
    /// Command that controls the downloading of stock data.
    /// </summary>
    internal sealed class DownloadCommand : ICommand
    {
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
        public DownloadCommand(IFileSystem fileSystem)
        {
            SubCommands.Add(new DownloadAllCommand(fileSystem));
            SubCommands.Add(new DownloadLatestCommand(fileSystem));
        }

        /// <inheritdoc/>
        public void WriteHelp(IConsole console)
        {
            CommandExtensions.WriteHelp(this, console);
        }

        /// <inheritdoc/>
        public int Execute(IConsole console, string[] args)
        {
            return Execute(console, null, args);
        }

        /// <inheritdoc/>
        public int Execute(IConsole console, IReportLogger logger, string[] args)
        {
            return CommandExtensions.Execute(this, console, logger, args);
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
    }
}
