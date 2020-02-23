using FinancialStructures.ReportingStructures;
using System;
using TradingConsole.InputParser;
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

            if (inputOptions.funtionType == FunctionType.Download)
            {
                Console.WriteLine("Downloading:");
            }
            if (inputOptions.funtionType == FunctionType.Simulate)
            {
                Console.WriteLine("SimulationStarting");
                var stats = new TradingStatistics();

                TradingSimulation.Run(inputOptions, stats);
            }
        }
    }
}
