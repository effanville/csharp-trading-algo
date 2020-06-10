using System;
using System.Diagnostics;
using System.IO;
using StructureCommon.Extensions;
using StructureCommon.Reporting;
using TradingConsole.InputParser;
using TradingConsole.Simulation;
using TradingConsole.Statistics;

namespace TradingConsole
{
    internal class Program
    {
        /// <summary>
        /// Enables writing to console and to a log file.
        /// </summary>
        public static ConsoleStreamWriter ConsoleWriter;

        internal static void WriteHelp()
        {
            ConsoleWriter.Write("");
            ConsoleWriter.Write("Syntax for query:");
            ConsoleWriter.Write("TradingConsole.exe ProgramType --<<optionName>> <<parameter>>");
            ConsoleWriter.Write("");
            ConsoleWriter.Write("ProgramType   - The type of the program");
            ConsoleWriter.Write("");
            ConsoleWriter.Write("Possible Commands:");
            foreach (object command in Enum.GetValues(typeof(ProgramType)))
            {
                ConsoleWriter.Write(command.ToString());
            }
            ConsoleWriter.Write("");
            ConsoleWriter.Write("optionName    - An optional argument to add.");
            ConsoleWriter.Write("");
            ConsoleWriter.Write("Possible Options");
            foreach (object tokenType in Enum.GetValues(typeof(TextTokenType)))
            {
                ConsoleWriter.Write(tokenType.ToString());
            }
            ConsoleWriter.Write("");
            ConsoleWriter.Write("parameters   - An optionName is followed by the parameter value for this option.");
            ConsoleWriter.Write("             - Each option can be specified once.");
            ConsoleWriter.Write("             - Differing options require different parameters.");

        }

        internal static void Main(string[] args)
        {
            Stopwatch stopWatch = new Stopwatch();
            try
            {
                string filepath;
                MemoryStream errorStream = new MemoryStream();
                ConsoleWriter = new ConsoleStreamWriter(errorStream);
                ConsoleWriter.Write("Input arguments:");
                ConsoleWriter.Write(string.Join(' ', args));
                void WriteReport(ReportSeverity critical, ReportType type, ReportLocation location, string message)
                {
                    if (critical != ReportSeverity.Critical)
                    {
                        return;
                    }

                    ConsoleWriter.Write($"{type} - {location} - {message}");
                }

                IReportLogger reportLogger = new LogReporter((critical, type, location, message) => WriteReport(critical, type, location, message));
                ConsoleWriter.Write("Trading Console");

                if (args.Length == 0)
                {
                    WriteHelp();
                }

                stopWatch.Start();

                UserInputParser userInputParser = new UserInputParser(reportLogger);
                UserInputOptions inputOptions = userInputParser.ParseUserInput(args);
                bool optionsOK = userInputParser.EnsureInputsSuitable(inputOptions);
                filepath = inputOptions.StockFilePath;
                if (!optionsOK)
                {
                    ConsoleWriter.Write("User Inputs not suitable");
                }
                else
                {
                    switch (inputOptions.funtionType)
                    {
                        case ProgramType.DownloadAll:
                        case ProgramType.DownloadLatest:
                        case ProgramType.Configure:
                        {
                            ConsoleWriter.Write("Downloading:");
                            StockDownloader stockDownloader = new StockDownloader(inputOptions, reportLogger);
                            stockDownloader.Download();
                            break;
                        }
                        case ProgramType.Simulate:
                        case ProgramType.Trade:
                        {
                            ConsoleWriter.Write("Simulation Starting");
                            TradingStatistics stats = new TradingStatistics();
                            TradingSimulation tradingSimulation = new TradingSimulation(inputOptions, reportLogger, ConsoleWriter);
                            tradingSimulation.SetupSystemsAndRun(stats);
                            stats.ExportToFile(Path.GetDirectoryName(filepath) + "\\" + DateTime.Now.FileSuitableDateTimeValue() + "-" + Path.GetFileNameWithoutExtension(filepath) + "-RunStats.log");
                            break;
                        }
                        case ProgramType.Help:
                        {
                            WriteHelp();
                            break;
                        }
                        default:
                        {
                            ConsoleWriter.Write("No admissible input selected");
                            break;
                        }
                    }
                }

                stopWatch.Stop();
                ConsoleWriter.filePath = Path.GetDirectoryName(filepath) + "\\" + DateTime.Now.FileSuitableDateTimeValue() + "-" + Path.GetFileNameWithoutExtension(filepath) + "-output.log";

            }
            catch (Exception exept)
            {
                ConsoleWriter.Write(exept.Message);
            }
            finally
            {
                TimeSpan lengthOfTime = stopWatch.Elapsed;
                ConsoleWriter.Write("Program finished");
                ConsoleWriter.Write($"{lengthOfTime.TotalSeconds}s");
                ConsoleWriter.SaveToFile();
            }
        }
    }
}
