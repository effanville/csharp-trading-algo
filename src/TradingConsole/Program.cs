using System;
using System.Collections.Generic;
using System.IO.Abstractions;

using Effanville.Common.Console;
using Effanville.Common.Console.Commands;
using Effanville.Common.Structure.Reporting;
using Effanville.TradingConsole.Commands.ExchangeCreation;

using SimulationCommand = Effanville.TradingConsole.Commands.Execution.SimulationCommand;

namespace Effanville.TradingConsole
{
    internal static class Program
    {
        internal static int Main(string[] args)
        {
            var fileSystem = new FileSystem();

            IConsole console = new ConsoleInstance(WriteError, WriteLine);

            // Create the logger.
            IReportLogger logger = new LogReporter(ReportAction, saveInternally: true);
            return InternalMain(args, fileSystem, console, logger);

            void ReportAction(ReportSeverity severity, ReportType reportType, string location, string text)
            {
                var color = Console.ForegroundColor;
                Console.ForegroundColor = reportType == ReportType.Error
                    ? ConsoleColor.Red
                    : reportType == ReportType.Warning
                        ? ConsoleColor.Yellow
                        : color;
                console.WriteLine($"[{DateTime.Now:yyyyMMdd-hh:mm:ss}]: ({reportType}): {text}");

                Console.ForegroundColor = color;
            }

            void WriteError(string text)
            {
                var color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{DateTime.Now:yyyyMMdd-hh:mm:ss}]: {text}");
                Console.ForegroundColor = color;
            }

            // Create the Console to write output.
            void WriteLine(string text) => Console.WriteLine(text);
        }

        internal static int InternalMain(string[] args, IFileSystem fileSystem, IConsole console, IReportLogger logger)
        {
            var globals = new ConsoleGlobals(fileSystem, console, logger);
            // Define the acceptable commands for this program.
            var validCommands = new List<ICommand>()
            {
                new ConfigureCommand(fileSystem),
                new DownloadCommand(fileSystem),
                new SimulationCommand(fileSystem)
            };

            // Generate the context, validate the arguments and execute.
            int exitCode = ConsoleContext.SetAndExecute(args, globals, validCommands);
            return exitCode;
        }
    }
}
