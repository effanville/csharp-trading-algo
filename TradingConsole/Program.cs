using System;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using StructureCommon.Extensions;
using StructureCommon.Reporting;
using TradingConsole.InputParser;
using TradingConsole.Simulation;
using TradingConsole.Statistics;

namespace TradingConsole
{
    internal class Program
    {
        internal static void Main(string[] args)
        {
            Stopwatch stopWatch = new Stopwatch();
            var consoleWriter = new ConsoleStreamWriter(new MemoryStream());
            var fileSystem = new FileSystem();

            try
            {
                string filepath;

                consoleWriter.Write("Input arguments:");
                consoleWriter.Write(string.Join(' ', args));
                void WriteReport(ReportSeverity critical, ReportType type, ReportLocation location, string message)
                {
                    if (critical != ReportSeverity.Critical)
                    {
                        return;
                    }

                    consoleWriter.Write($"{type} - {location} - {message}");
                }

                IReportLogger reportLogger = new LogReporter((critical, type, location, message) => WriteReport(critical, type, location, message));
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
                        case ProgramType.DownloadAll:
                        case ProgramType.DownloadLatest:
                        case ProgramType.Configure:
                        {
                            consoleWriter.Write("Downloading:");
                            StockDownloader stockDownloader = new StockDownloader(inputOptions, fileSystem, reportLogger);
                            stockDownloader.Download();
                            break;
                        }
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
                        case ProgramType.Help:
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
