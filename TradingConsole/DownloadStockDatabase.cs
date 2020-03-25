using FinancialStructures.ReportLogging;
using System.IO;
using TradingConsole.InputParser;
using TradingConsole.StockStructures;

namespace TradingConsole
{
    public static class DownloadStocks
    {
        public static void Download(UserInputOptions inputOptions, LogReporter reportLogger)
        {
            switch (inputOptions.funtionType)
            {
                case FunctionType.DownloadAll:
                    Download(DownloadType.All, inputOptions, reportLogger);
                    break;
                case FunctionType.DownloadLatest:
                    Download(DownloadType.Latest, inputOptions, reportLogger);
                    break;
                case FunctionType.Configure:
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
            exchange.Configure(inputOptions);
            string filePath = Path.ChangeExtension(inputOptions.StockFilePath, "xml");
            exchange.SaveExchangeStocks(filePath, reportLogger);
        }

        private static void Download(DownloadType downloadType, UserInputOptions inputOptions, LogReporter reportLogger)
        {
            var exchange = new ExchangeStocks();
            exchange.LoadExchangeStocks(inputOptions.StockFilePath, reportLogger);
            exchange.Download(downloadType, inputOptions, reportLogger);
            exchange.SaveExchangeStocks(inputOptions.StockFilePath, reportLogger);
        }
    }
}
