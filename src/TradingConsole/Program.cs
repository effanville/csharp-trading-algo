using System;
using System.Collections.Generic;
using System.IO.Abstractions;

using Effanville.Common.Console;
using Effanville.Common.Console.Commands;
using Effanville.Common.Structure.Reporting;

using TradingConsole.Commands.ExchangeCreation;
using TradingConsole.Commands.Execution;

namespace TradingConsole
{
    internal class Program
    {
        internal static int Main(string[] args)
        {
            var fileSystem = new FileSystem();

            // Create the Console to write output.
            void writeLine(string text) => Console.WriteLine(text);
            void writeError(string text)
            {
                var color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{DateTime.Now:yyyyMMdd-hh:mm:ss}]: {text}");
                Console.ForegroundColor = color;
            }
            IConsole console = new ConsoleInstance(writeError, writeLine);

            // Create the logger.
            var reports = new ErrorReports();
            void reportAction(ReportSeverity severity, ReportType reportType, string location, string text)
            {
                reports.AddErrorReport(severity, reportType, location, text);
                var color = Console.ForegroundColor;
                Console.ForegroundColor = reportType == ReportType.Error
                    ? ConsoleColor.Red
                    : reportType == ReportType.Warning
                        ? ConsoleColor.Yellow
                        : color;
                console.WriteLine($"[{DateTime.Now:yyyyMMdd-hh:mm:ss}]: ({reportType}): {text}");

                Console.ForegroundColor = color;
            }
            IReportLogger logger = new LogReporter(reportAction);
            return InternalMain(args, fileSystem, console, logger);
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
