using FinancialStructures.ReportLogging;
using FinancialStructures.StockStructures;
using System.IO;
using TradingConsole.InputParser;

namespace TradingConsole
{
    public class StockDownloader
    {
        private readonly UserInputOptions InputOptions;
        private readonly LogReporter ReportLogger;
        public StockDownloader(UserInputOptions inputOptions, LogReporter reportLogger)
        {
            InputOptions = inputOptions;
            ReportLogger = reportLogger;
        }

        public void Download()
        {
            switch (InputOptions.funtionType)
            {
                case ProgramType.DownloadAll:
                    Download(DownloadType.All);
                    break;
                case ProgramType.DownloadLatest:
                    Download(DownloadType.Latest);
                    break;
                case ProgramType.Configure:
                    Configure();
                    break;
                default:
                    break;
            }

            return;
        }

        private void Configure()
        {
            var exchange = new ExchangeStocks();
            exchange.Configure(InputOptions.StockFilePath);
            string filePath = Path.ChangeExtension(InputOptions.StockFilePath, "xml");
            exchange.SaveExchangeStocks(filePath, ReportLogger);
        }

        private void Download(DownloadType downloadType)
        {
            var exchange = new ExchangeStocks();
            exchange.LoadExchangeStocks(InputOptions.StockFilePath, ReportLogger);
            exchange.Download(downloadType, InputOptions.StartDate, InputOptions.EndDate, ReportLogger);
            exchange.SaveExchangeStocks(InputOptions.StockFilePath, ReportLogger);
        }
    }
}
