using System.IO;
using FinancialStructures.StockStructures;
using StructureCommon.Reporting;
using TradingConsole.InputParser;

namespace TradingConsole
{
    public class StockDownloader
    {
        private readonly UserInputOptions InputOptions;
        private readonly IReportLogger ReportLogger;
        public StockDownloader(UserInputOptions inputOptions, IReportLogger reportLogger)
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
            ExchangeStocks exchange = new ExchangeStocks();
            exchange.Configure(InputOptions.StockFilePath);
            string filePath = Path.ChangeExtension(InputOptions.StockFilePath, "xml");
            exchange.SaveExchangeStocks(filePath, ReportLogger);
        }

        private void Download(DownloadType downloadType)
        {
            ExchangeStocks exchange = new ExchangeStocks();
            exchange.LoadExchangeStocks(InputOptions.StockFilePath, ReportLogger);
            exchange.Download(downloadType, InputOptions.StartDate, InputOptions.EndDate, ReportLogger);
            exchange.SaveExchangeStocks(InputOptions.StockFilePath, ReportLogger);
        }
    }
}
