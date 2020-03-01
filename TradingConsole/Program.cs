using FinancialStructures.ReportingStructures;
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
            var reports = new ErrorReports();
            Console.WriteLine("Trading Console");
            UserInputOptions inputOptions = UIOGenerator.ParseUserInput(args, reports);
            DateTime now = DateTime.Now;
            var then = now.AddSeconds(-1582907959);
            var thenagain = new DateTime(2020, 2, 27).AddSeconds(-1582790400);

            switch (inputOptions.funtionType)
            {
                case FunctionType.DownloadAll:
                case FunctionType.DownloadLatest:
                case FunctionType.Configure:
                    Console.WriteLine("Downloading:");
                    DownloadStocks.Download(inputOptions);
                    break;
                case FunctionType.Simulate:
                case FunctionType.Trade:
                    Console.WriteLine("Simulation Starting");
                    var stats = new TradingStatistics();
                    TradingSimulation.SetupSystemsAndRun(inputOptions, stats);
                    break;
                default:
                    Console.WriteLine("No admissible input selected");
                    break;
            }

            Console.WriteLine("Program stopping.");
        }
    }
}
