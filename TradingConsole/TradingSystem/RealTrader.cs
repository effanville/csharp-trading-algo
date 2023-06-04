using System;
using System.Collections.Generic;
using System.IO.Abstractions;

using Common.Structure.Reporting;

using FinancialStructures.NamingStructures;
using FinancialStructures.StockStructures;

using TradingSystem.Decisions;
using TradingSystem.Diagnostics;
using TradingSystem.PortfolioStrategies;
using TradingSystem.PriceSystem;
using TradingSystem.Simulator;
using TradingSystem.Trading;

namespace TradingConsole.TradingSystem
{
    internal partial class RealTrader
    {
        private readonly IDecisionSystem DecisionSystem;
        private readonly ITradeSubmitter BuySellSystem;
        private readonly TradeMechanismSettings fTradeMechanismSettings;
        private readonly StockMarketEvolver.Settings fSimulatorSettings;
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

            fPortfolioManager.Portfolio.SavePortfolio(portfolioFilePath, fFileSystem, ReportLogger);
        }

        private void PerformDailyTrades(DateTime time, IStockExchange exchange, IPortfolioManager portfolioManager)
        {
            // Decide which stocks to buy, sell or do nothing with.
            TradeCollection decisions = DecisionSystem.Decide(time, exchange, null);

            var priceService = PriceServiceFactory.Create(
                PriceType.ExchangeFile,
                PriceCalculationSettings.Default(),
                exchange);

            // Exact the buy/Sell decisions.
            List<Trade> sellDecisions = decisions.GetSellDecisions();
            var trades = new TradeCollection(time, time);
            foreach (Trade sell in sellDecisions)
            {
                var actualTrade = BuySellSystem.Trade(time, sell, priceService, 0.0m, ReportLogger);
                if (actualTrade != null)
                {
                    trades.Add(new NameData(actualTrade.Company, actualTrade.Name), actualTrade.TradeType, actualTrade.NumberShares);
                }
            }

            List<Trade> buyDecisions = decisions.GetBuyDecisions();
            foreach (Trade buy in buyDecisions)
            {
                var actualTrade = BuySellSystem.Trade(time, buy, priceService, 0.0m, ReportLogger);
                if (actualTrade != null)
                {
                    trades.Add(new NameData(actualTrade.Company, actualTrade.Name), actualTrade.TradeType, actualTrade.NumberShares);
                }
            }
        }
    }
}
