using System;
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
            void WriteReport(ReportSeverity critical, ReportType type, ReportLocation location, string message)
            {
                if (critical != ReportSeverity.Critical)
                {
                    return;
                }

                Console.WriteLine($"{type} - {location} - {message}");
            }

            LogReporter reportLogger = new LogReporter((critical, type, location, message) => WriteReport(critical, type, location, message));
            Console.WriteLine("Trading Console");
            try
            {
                UserInputParser userInputParser = new UserInputParser(reportLogger);
                UserInputOptions inputOptions = userInputParser.ParseUserInput(args);
                bool optionsOK = userInputParser.EnsureInputsSuitable(inputOptions);
                if (!optionsOK)
                {
                    Console.WriteLine("User Inputs not suitable");
                }
                else
                {
                    switch (inputOptions.funtionType)
                    {
                        case ProgramType.DownloadAll:
                        case ProgramType.DownloadLatest:
                        case ProgramType.Configure:
                        {
                            Console.WriteLine("Downloading:");
                            StockDownloader stockDownloader = new StockDownloader(inputOptions, reportLogger);
                            stockDownloader.Download();
                            break;
                        }
                        case ProgramType.Simulate:
                        case ProgramType.Trade:
                        {
                            Console.WriteLine("Simulation Starting");
                            TradingStatistics stats = new TradingStatistics();
                            TradingSimulation tradingSimulation = new TradingSimulation(inputOptions, reportLogger);
                            tradingSimulation.SetupSystemsAndRun(stats);
                            break;
                        }
                        case ProgramType.Help:
                        {
                            Console.WriteLine("User input options are:");
                            foreach (object tokenType in Enum.GetValues(typeof(TextTokenType)))
                            {
                                Console.WriteLine(tokenType.ToString());
                            }
                            break;
                        }
                        default:
                        {
                            Console.WriteLine("No admissible input selected");
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("Program stopping.");
        }
    }
}
