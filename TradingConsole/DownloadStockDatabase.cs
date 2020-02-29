using FinancialStructures.ReportingStructures;
using System.IO;
using TradingConsole.InputParser;
using TradingConsole.StockStructures;

namespace TradingConsole
{
    public static class DownloadStocks
    {
        public static void Download(UserInputOptions inputOptions)
        {
            switch (inputOptions.funtionType)
            {
                case FunctionType.DownloadAll:
                    DownloadAll(inputOptions);
                    break;
                case FunctionType.DownloadLatest:
                    DownloadLatest(inputOptions);
                    break;
                case FunctionType.Configure:
                    Configure(inputOptions);
                    break;
                default:
                    break;
            }

            return;
        }

        private static void Configure(UserInputOptions inputOptions)
        {
            var reports = new ErrorReports();
            var exchange = new ExchangeStocks();
            exchange.Configure(inputOptions);
            string filePath = Path.ChangeExtension(inputOptions.StockFilePath, "xml");
            exchange.SaveExchangeStocks(filePath, reports);
        }

        private static void DownloadLatest(UserInputOptions inputOptions)
        {
            var reports = new ErrorReports();
            var exchange = new ExchangeStocks();
            exchange.LoadExchangeStocks(inputOptions.StockFilePath, reports);
            exchange.DownloadLatest();
        }

        private static void DownloadAll(UserInputOptions inputOptions)
        {
            var reports = new ErrorReports();
            var exchange = new ExchangeStocks();
            exchange.LoadExchangeStocks(inputOptions.StockFilePath, reports);
            exchange.DownloadAll();
        }
    }
}
