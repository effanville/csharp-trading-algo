using System;
using System.IO;
using System.IO.Abstractions;
using Common.Structure.Reporting;
using TradingConsole.Simulation;
using Common.Console;
using System.Collections.Generic;
using Common.Console.Commands;
using TradingConsole.ExchangeCreation;

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
            var memoryStream = new MemoryStream();
            void reportAction(ReportSeverity severity, ReportType reportType, ReportLocation location, string text)
            {
                reports.AddErrorReport(severity, reportType, location, text);
                console.WriteLine($"{DateTime.Now}({reportType}) {text}\n");
            }
            IReportLogger logger = new LogReporter(reportAction);

            // Define the acceptable commands for this program.
            var validCommands = new List<ICommand>()
            {
                new ConfigureCommand(logger, fileSystem),
                new DownloadCommand(logger, fileSystem),
                new SimulationCommand(logger, fileSystem),
                new TradingCommand(logger, fileSystem)
            };

            // Generate the context, validate the arguments and execute.
            ConsoleContext.SetAndExecute(args, console, logger, validCommands);
            return;
        }
    }
}
