using FinancialStructures.ReportLogging;
using System;
using TradingConsole.InputParser;
using TradingConsole.Simulation;
using TradingConsole.Statistics;

namespace TradingConsole
{
    class Program
    {
        static void WriteReport(string critical, string type, string location, string message)
        {
            if (critical != "Critical")
            {
                return;
            }

            Console.WriteLine($"{type} - {location} - {message}");
        }
        static void Main(string[] args)
        {
            LogReporter reportLogger = new LogReporter((critical, type, location, message) => WriteReport(critical, type, location, message));
            Console.WriteLine("Trading Console");

            var userInputParser = new UserInputParser(reportLogger);
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
                            var stockDownloader = new StockDownloader(inputOptions, reportLogger);
                            stockDownloader.Download();
                            break;
                        }
                    case ProgramType.Simulate:
                    case ProgramType.Trade:
                        {
                            Console.WriteLine("Simulation Starting");
                            var stats = new TradingStatistics();
                            var tradingSimulation = new TradingSimulation(inputOptions, reportLogger);
                            tradingSimulation.SetupSystemsAndRun(stats);
                            break;
                        }
                    case ProgramType.Help:
                        {
                            Console.WriteLine("User input options are:");
                            foreach (var tokenType in Enum.GetValues(typeof(TextTokenType)))
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

            Console.WriteLine("Program stopping.");
        }
    }
}
