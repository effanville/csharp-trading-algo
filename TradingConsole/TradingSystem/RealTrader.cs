using System;
using System.IO.Abstractions;

using Common.Structure.DataStructures;
using Common.Structure.Reporting;

using FinancialStructures.Database;
using FinancialStructures.Database.Extensions;
using FinancialStructures.NamingStructures;
using FinancialStructures.StockStructures;

using TradingConsole.BuySellSystem;
using TradingConsole.DecisionSystem;

using TradingSystem;
using TradingSystem.DecideThenTradeSystem;
using TradingSystem.Simulator;
using TradingSystem.Simulator.Trading.Decisions;

namespace TradingConsole.TradingSystem
{
    internal partial class RealTrader
    {
        private readonly IDecisionSystem DecisionSystem;
        private readonly ITradeMechanism BuySellSystem;
        private readonly TradeMechanismTraderOptions fTraderOptions;
        private readonly StockMarketEvolver.Settings fSimulatorSettings;
        private readonly IPortfolio fPortfolio;

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
            DecisionSystemFactory.Settings decisionParameters,
            TradeMechanismType buySellType,
            IFileSystem fileSystem,
            IReportLogger reportLogger)
        {
            ReportLogger = reportLogger;
            fFileSystem = fileSystem;

            fTraderOptions = new TradeMechanismTraderOptions();

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

                BuySellSystem = TradeMechanismFactory.Create(buySellType);

                using (new Timer(reportLogger, "Loading Portfolio"))
                {
                    fPortfolio = LoadStartPortfolio(startSettings, reportLogger);
                }
            }
        }

        public void Run(string portfolioFilePath)
        {
            PerformDailyTrades(DateTime.Today, fSimulatorSettings.Exchange, fPortfolio);

            fPortfolio.SavePortfolio(portfolioFilePath, fFileSystem, ReportLogger);
        }

        private void PerformDailyTrades(DateTime day, IStockExchange exchange, IPortfolio portfolio)
        {
            // Decide which stocks to buy, sell or do nothing with.
            DecisionStatus status = DecisionSystem.Decide(day, exchange, null);

            decimal CalculatePurchasePrice(DateTime time, TwoName stock)
            {
                return exchange.GetValue(stock, time);
            }

            decimal CalculateSellPrice(DateTime time, TwoName stock)
            {
                return exchange.GetValue(stock, time);
            }

            // Exact the buy/Sell decisions.
            _ = BuySellSystem.EnactAllTrades(
                day,
                status,
                (date, name) => CalculatePurchasePrice(date, name),
                (date, name) => CalculateSellPrice(date, name),
                portfolio,
                fTraderOptions,
                ReportLogger);
        }

        private IPortfolio LoadStartPortfolio(PortfolioStartSettings settings, IReportLogger logger)
        {
            IPortfolio portfolio = PortfolioFactory.GenerateEmpty();
            if (settings.PortfolioFilePath != null)
            {
                portfolio.LoadPortfolio(settings.PortfolioFilePath, fFileSystem, ReportLogger);
            }
            else
            {
                _ = portfolio.TryAdd(Account.BankAccount, new NameData(settings.DefaultBankAccName.Company, settings.DefaultBankAccName.Name), logger);
                var data = new DailyValuation(settings.StartTime.AddDays(-1), settings.StartingCash);
                _ = portfolio.TryAddOrEditData(Account.BankAccount, settings.DefaultBankAccName, data, data, logger);
            }

            return portfolio;
        }
    }
}
