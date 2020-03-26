using FinancialStructures.ReportLogging;
using System;
using TradingConsole.InputParser;
using TradingConsole.Simulation;
using TradingConsole.Statistics;

namespace TradingConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            LogReporter reportLogger = new LogReporter((critical, type, location, message) => Console.WriteLine($"{type} - {location} - {message}"));
            Console.WriteLine("Trading Console");
            UserInputOptions inputOptions = UIOGenerator.ParseUserInput(args, reportLogger);

            switch (inputOptions.funtionType)
            {
                case ProgramType.DownloadAll:
                case ProgramType.DownloadLatest:
                case ProgramType.Configure:
                    Console.WriteLine("Downloading:");
                    DownloadStocks.Download(inputOptions, reportLogger);
                    break;
                case ProgramType.Simulate:
                case ProgramType.Trade:
                    Console.WriteLine("Simulation Starting");
                    var stats = new TradingStatistics();
                    TradingSimulation.SetupSystemsAndRun(inputOptions, stats, reportLogger);
                    break;
                default:
                    Console.WriteLine("No admissible input selected");
                    break;
            }

            Console.WriteLine("Program stopping.");
        }
    }
}
