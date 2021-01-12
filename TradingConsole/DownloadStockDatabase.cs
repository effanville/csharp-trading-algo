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
                    Download(StockExchangeDownloadMethod.All);
                    break;
                case ProgramType.DownloadLatest:
                    Download(StockExchangeDownloadMethod.Latest);
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
            IStockExchange exchange = new StockExchange();
            exchange.Configure(InputOptions.StockFilePath);
            string filePath = Path.ChangeExtension(InputOptions.StockFilePath, "xml");
            exchange.SaveStockExchange(filePath, ReportLogger);
        }

        private void Download(StockExchangeDownloadMethod downloadType)
        {
            IStockExchange exchange = new StockExchange();
            exchange.LoadStockExchange(InputOptions.StockFilePath, ReportLogger);
            exchange.Download(downloadType, InputOptions.StartDate, InputOptions.EndDate, ReportLogger);
            exchange.SaveStockExchange(InputOptions.StockFilePath, ReportLogger);
        }
    }
}
