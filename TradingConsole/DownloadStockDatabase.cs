using FinancialStructures.ReportLogging;
using FinancialStructures.StockStructures;
using System.IO;
using TradingConsole.InputParser;

namespace TradingConsole
{
    public static class DownloadStocks
    {
        public static void Download(UserInputOptions inputOptions, LogReporter reportLogger)
        {
            switch (inputOptions.funtionType)
            {
                case ProgramType.DownloadAll:
                    Download(DownloadType.All, inputOptions, reportLogger);
                    break;
                case ProgramType.DownloadLatest:
                    Download(DownloadType.Latest, inputOptions, reportLogger);
                    break;
                case ProgramType.Configure:
                    Configure(inputOptions, reportLogger);
                    break;
                default:
                    break;
            }

            return;
        }

        private static void Configure(UserInputOptions inputOptions, LogReporter reportLogger)
        {
            var exchange = new ExchangeStocks();
            exchange.Configure(inputOptions.StockFilePath);
            string filePath = Path.ChangeExtension(inputOptions.StockFilePath, "xml");
            exchange.SaveExchangeStocks(filePath, reportLogger);
        }

        private static void Download(DownloadType downloadType, UserInputOptions inputOptions, LogReporter reportLogger)
        {
            var exchange = new ExchangeStocks();
            exchange.LoadExchangeStocks(inputOptions.StockFilePath, reportLogger);
            exchange.Download(downloadType, inputOptions.StartDate, inputOptions.EndDate, reportLogger);
            exchange.SaveExchangeStocks(inputOptions.StockFilePath, reportLogger);
        }
    }
}
