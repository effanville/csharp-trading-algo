using System;
using System.IO;
using System.IO.Abstractions;
using StructureCommon.Extensions;
using StructureCommon.Reporting;
using TradingConsole.InputParser;
using TradingConsole.Simulation;
using ConsoleCommon;
using System.Collections.Generic;
using ConsoleCommon.Commands;
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
                string line = DateTime.Now + "(" + reportType.ToString() + ")" + text;
                console.WriteLine(line);
            }
            IReportLogger logger = new LogReporter(reportAction);

            // Define the acceptable commands for this program.
            var validCommands = new List<ICommand>()
            {
                new ConfigureCommand(console, logger, fileSystem),
                new DownloadCommand(console, logger, fileSystem),
                new SimulationCommand(console, logger, fileSystem)
            };

            // Generate the context, validate the arguments and execute. 
            ConsoleContext.SetAndExecute(args, console, logger, validCommands);
            return;
        }
    }
}
