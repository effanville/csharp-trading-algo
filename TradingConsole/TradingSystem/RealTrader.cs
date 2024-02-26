using System;
using System.Collections.Generic;
using System.IO.Abstractions;

using Effanville.Common.Structure.Reporting;
using Effanville.FinancialStructures.Persistence;
using Effanville.FinancialStructures.Stocks;
using Effanville.TradingStructures.Common.Diagnostics;
using Effanville.TradingStructures.Common.Trading;
using Effanville.TradingStructures.Pricing;
using Effanville.TradingStructures.Trading;

using TradingSystem.Decisions;
using TradingSystem.MarketEvolvers;
using TradingSystem.PortfolioStrategies;
using TradingSystem.Trading;

namespace TradingConsole.TradingSystem
{
    internal partial class RealTrader
    {
        private readonly IDecisionSystem DecisionSystem;
        private readonly ITradeSubmitter BuySellSystem;
        private readonly TradeMechanismSettings fTradeMechanismSettings;
        private readonly TimeIncrementEvolverSettings fSimulatorSettings;
        private readonly IPortfolioManager fPortfolioManager;

        private readonly IReportLogger ReportLogger;
        private readonly IFileSystem fFileSystem;

        /// <summary>
        /// Does the setup of the simulation error.
        /// </summary>
        public bool SetupError
        {
            get;
            set;
        }

        public RealTrader(
            string stockFilePath,
            PortfolioStartSettings startSettings,
            PortfolioConstructionSettings constructionSettings,
            DecisionSystemFactory.Settings decisionParameters,
            TradeSubmitterType buySellType,
            IFileSystem fileSystem,
            IReportLogger reportLogger)
        {
            ReportLogger = reportLogger;
            fFileSystem = fileSystem;

            IStockExchange exchange;
            using (new Timer(ReportLogger, "Setup"))
            {
                using (new Timer(ReportLogger, "Loading Exchange"))
                {
                    exchange = StockExchangeFactory.Create(stockFilePath, fFileSystem, ReportLogger);
                    if (!exchange.CheckValidity())
                    {
                        _ = ReportLogger.Log(ReportSeverity.Critical, ReportType.Error, ReportLocation.Loading, "Stock input data not suitable.");
                        SetupError = true;
                        return;
                    }
                }

                using (new Timer(reportLogger, "Calibrating"))
                {
                    DecisionSystem = DecisionSystemFactory.Create(decisionParameters);
                    DecisionSystem.Calibrate(fSimulatorSettings, ReportLogger);
                }

                BuySellSystem = TradeSubmitterFactory.Create(buySellType, TradeMechanismSettings.Default());

                using (new Timer(reportLogger, "Loading Portfolio"))
                {
                    fPortfolioManager = PortfolioManager.LoadFromFile(fFileSystem, startSettings, constructionSettings, reportLogger);
                }
            }
        }

        public void Run(string portfolioFilePath)
        {
            PerformDailyTrades(DateTime.Today, fSimulatorSettings.Exchange, fPortfolioManager);
            var persistence = new PortfolioPersistence();
            var settings = PortfolioPersistence.CreateOptions(portfolioFilePath, fFileSystem);
            persistence.Save(fPortfolioManager.Portfolio, settings, ReportLogger);
        }

        private void PerformDailyTrades(DateTime time, IStockExchange exchange, IPortfolioManager portfolioManager)
        {
            // Decide which stocks to buy, sell or do nothing with.
            TradeCollection decisions = DecisionSystem.Decide(time, exchange, null);
            TradeHistory decisionRecord = new TradeHistory();
            TradeHistory tradeRecord = new TradeHistory();

            var priceService = PriceServiceFactory.Create(
                PriceType.ExchangeFile,
                PriceCalculationSettings.Default(),
                exchange,
                null);

            // Exact the buy/Sell decisions.
            List<Trade> sellDecisions = decisions.GetSellDecisions();
            var trades = new TradeCollection(time, time);
            foreach (Trade sell in sellDecisions)
            {
                TradeSubmitterHelpers.SubmitAndReportTrade(
                    time,
                    sell,
                    priceService,
                    portfolioManager,
                    BuySellSystem,
                    tradeRecord,
                    decisionRecord,
                    ReportLogger);
            }

            List<Trade> buyDecisions = decisions.GetBuyDecisions();
            foreach (Trade buy in buyDecisions)
            {
                TradeSubmitterHelpers.SubmitAndReportTrade(
                    time,
                    buy,
                    priceService,
                    portfolioManager,
                    BuySellSystem,
                    tradeRecord,
                    decisionRecord,
                    ReportLogger);
            }
        }
    }
}
