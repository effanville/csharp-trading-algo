using System;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using StructureCommon.Extensions;
using StructureCommon.Reporting;
using TradingConsole.InputParser;
using TradingConsole.Simulation;
using TradingConsole.Statistics;
using ConsoleCommon;
using System.Collections.Generic;
using ConsoleCommon.Commands;
using TradingConsole.ExchangeCreation;

namespace TradingConsole
{
    internal class Program
    {
        internal static void NewMain(string[] args)
        {
            var fileSystem = new FileSystem();

            // Create the logger.
            var reports = new ErrorReports();
            void reportAction(ReportSeverity severity, ReportType reportType, ReportLocation location, string text)
            {
                reports.AddErrorReport(severity, reportType, location, text);
                Console.WriteLine(DateTime.Now + "(" + reportType.ToString() + ")" + text);
            }
            IReportLogger logger = new LogReporter(reportAction);


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

            // Define the acceptable commands for this program.
            var validCommands = new List<ICommand>()
            {
                new ConfigureCommand(console, logger, fileSystem),
                new DownloadCommand(console, logger, fileSystem),
            };

            // Generate the context, validate the arguments and execute. 
            ConsoleContext.SetAndExecute(args, console, logger, validCommands);
            return;
        }

        internal static void Main(string[] args)
        {
            Stopwatch stopWatch = new Stopwatch();
            var consoleWriter = new ConsoleStreamWriter(new MemoryStream());
            var fileSystem = new FileSystem();

            // Create the logger.
            var reports = new ErrorReports();
            void reportAction(ReportSeverity severity, ReportType reportType, ReportLocation location, string text)
            {
                reports.AddErrorReport(severity, reportType, location, text);
                Console.WriteLine(DateTime.Now + "(" + reportType.ToString() + ")" + text);
            }
            IReportLogger reportLogger = new LogReporter(reportAction);

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

            try
            {
                string filepath;

                consoleWriter.Write("Input arguments:");
                consoleWriter.Write(string.Join(' ', args));
                consoleWriter.Write("Trading Console");

                stopWatch.Start();

                UserInputParser userInputParser = new UserInputParser(reportLogger);
                UserInputOptions inputOptions = userInputParser.ParseUserInput(args);
                bool optionsOK = userInputParser.EnsureInputsSuitable(inputOptions);
                filepath = inputOptions.StockFilePath;
                if (!optionsOK)
                {
                    consoleWriter.Write("User Inputs not suitable");
                }
                else
                {
                    switch (inputOptions.FuntionType)
                    {
                        case ProgramType.Simulate:
                        case ProgramType.Trade:
                        {
                            consoleWriter.Write("Simulation Starting");
                            TradingStatistics stats = new TradingStatistics();
                            TradingSimulation tradingSimulation = new TradingSimulation(inputOptions, fileSystem, reportLogger, consoleWriter);
                            tradingSimulation.SetupSystemsAndRun(stats);
                            stats.ExportToFile(Path.GetDirectoryName(filepath) + "\\" + DateTime.Now.FileSuitableDateTimeValue() + "-" + Path.GetFileNameWithoutExtension(filepath) + "-RunStats.log");
                            break;
                        }
                        default:
                        {
                            break;
                        }
                    }
                }

                stopWatch.Stop();
                consoleWriter.filePath = Path.GetDirectoryName(filepath) + "\\" + DateTime.Now.FileSuitableDateTimeValue() + "-" + Path.GetFileNameWithoutExtension(filepath) + "-output.log";

            }
            catch (Exception exept)
            {
                consoleWriter.Write(exept.Message);
            }
            finally
            {
                TimeSpan lengthOfTime = stopWatch.Elapsed;
                consoleWriter.Write("Program finished");
                consoleWriter.Write($"{lengthOfTime.TotalSeconds}s");
                consoleWriter.SaveToFile();
            }
        }
    }
}
