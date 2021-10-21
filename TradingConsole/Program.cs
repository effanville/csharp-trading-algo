using System;
using System.IO.Abstractions;
using Common.Structure.Reporting;
using TradingConsole.ExecutionCommands;
using Common.Console;
using System.Collections.Generic;
using Common.Console.Commands;
using TradingConsole.ExchangeCreationCommands;

namespace TradingConsole
{
    internal class Program
    {
        internal static void Main(string[] args)
        {
            var fileSystem = new FileSystem();

            // Create the Console to write output.
            void writeLine(string text) => Console.WriteLine(text);
            void writeError(string text)
            {
                var color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(text);
                Console.ForegroundColor = color;
            }
            IConsole console = new ConsoleInstance(writeError, writeLine);

            // Create the logger.
            var reports = new ErrorReports();
            void reportAction(ReportSeverity severity, ReportType reportType, ReportLocation location, string text)
            {
                reports.AddErrorReport(severity, reportType, location, text);
                console.WriteLine($"{DateTime.Now}({reportType}) {text}");
            }
            IReportLogger logger = new LogReporter(reportAction);
            InternalMain(args, fileSystem, console, logger);
        }

        internal static void InternalMain(string[] args, IFileSystem fileSystem, IConsole console, IReportLogger logger)
        {
            var globals = new ConsoleGlobals(fileSystem, console, logger);
            // Define the acceptable commands for this program.
            var validCommands = new List<ICommand>()
            {
                new ConfigureCommand(logger, fileSystem),
                new DownloadCommand(logger, fileSystem),
                new SimulationCommand(logger, fileSystem),
                new TradingCommand(logger, fileSystem)
            };

            // Generate the context, validate the arguments and execute.
            ConsoleContext.SetAndExecute(args, globals, validCommands);
            return;
        }
    }
}
