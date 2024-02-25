using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.DataStructures;
using TradingSystem.PriceSystem;
using TradingSystem.Trading;
using System;
using Effanville.Common.Structure.DataStructures;
using Effanville.FinancialStructures.Database.Extensions.Values;
using Effanville.Common.Structure.Reporting;
using Effanville.FinancialStructures.NamingStructures;
using System.Collections.Generic;
using Effanville.FinancialStructures.FinanceStructures;
using Effanville.FinancialStructures.Stocks;
using System.IO.Abstractions;

using Effanville.FinancialStructures.Database.Extensions.DataEdit;
using Effanville.FinancialStructures.Persistence;

using TradingSystem.MarketEvolvers;

namespace TradingSystem.PortfolioStrategies
{
    /// <summary>
    /// A basic manager for a portfolio, that just deals with how much money to spend on 
    /// the given trade.
    /// </summary>
    public sealed class PortfolioManager : IPortfolioManager
    {
        private readonly IReportLogger _logger;

        /// <inheritdoc/>
        public PortfolioConstructionSettings PortfolioConstructionSettings
        {
            get;
        }

        /// <inheritdoc/>
        public PortfolioStartSettings StartSettings
        {
            get;
        }

        /// <inheritdoc/>
        public IPortfolio StartPortfolio
        {
            get;
        }

        /// <inheritdoc/>
        public IPortfolio Portfolio
        {
            get;
        }

        public string Name => nameof(PortfolioManager);

        public PortfolioManager(
            IPortfolio portfolio,
            PortfolioStartSettings startSettings,
            PortfolioConstructionSettings constructionSettings,
            IReportLogger logger)
        {
            _logger = logger;
            Portfolio = portfolio;
            StartPortfolio = portfolio.Copy();
            StartSettings = startSettings;
            PortfolioConstructionSettings = constructionSettings;
        }

        public void Initialize(EvolverSettings settings) { }
        public void Restart() { }
        public void Shutdown() { }

        /// <summary>
        /// Create a portfolioManager from a settings object.
        /// </summary>
        public static PortfolioManager LoadFromFile(
            IFileSystem fileSystem,
            PortfolioStartSettings startSettings,
            PortfolioConstructionSettings constructionSettings,
            IReportLogger logger)
        {
            var portfolio = LoadStartPortfolio(startSettings, fileSystem, logger);
            return new PortfolioManager(portfolio, startSettings, constructionSettings, logger);
        }

        private static IPortfolio LoadStartPortfolio(PortfolioStartSettings settings, IFileSystem fileSystem, IReportLogger logger)
        {
            var persistence = new PortfolioPersistence();
            IPortfolio portfolio;
            if (!string.IsNullOrWhiteSpace(settings.PortfolioFilePath))
            {
                portfolio = persistence.Load(PortfolioPersistence.CreateOptions(settings.PortfolioFilePath, fileSystem), logger);
            }
            else
            {
                portfolio = PortfolioFactory.GenerateEmpty();
                _ = portfolio.TryAdd(Account.BankAccount, new NameData(settings.DefaultBankAccName.Company, settings.DefaultBankAccName.Name), logger);
                var data = new DailyValuation(settings.StartTime.AddDays(-1), settings.StartingCash);
                _ = portfolio.TryAddOrEditData(Account.BankAccount, settings.DefaultBankAccName, data, data, logger);
            }

            return portfolio;
        }

        /// <inheritdoc/>
        public Trade ValidateTrade(DateTime time, Trade trade, IPriceService priceService)
        {
            if (trade.BuySell == TradeType.Buy)
            {
                // If not enough money to buy then exit.
                decimal priceToBuy = priceService.GetAskPrice(time, trade.StockName);
                if (priceToBuy.Equals(decimal.MinValue))
                {
                    return null;
                }

                decimal cashAvailable = AvailableFunds(time);

                if (priceToBuy == 0.0m || cashAvailable <= priceToBuy)
                {
                    return null;
                }

                int numShares = 0;
                while (numShares * priceToBuy < PortfolioConstructionSettings.FractionInvest * cashAvailable)
                {
                    numShares++;
                }
                numShares--;

                if (numShares <= 0)
                {
                    return null;
                }

                return new Trade(trade.StockName, trade.BuySell, numShares);
            }
            else if (trade.BuySell == TradeType.Sell)
            {
                // One can only sell if one already owns some of the security.
                var twoName = trade.StockName.ToTwoName();
                if (!Portfolio.Exists(Account.Security, twoName))
                {
                    return null;
                }

                _ = Portfolio.TryGetAccount(Account.Security, trade.StockName, out var desired);
                ISecurity security = desired as ISecurity;
                decimal numShares = security.Shares.ValueOnOrBefore(time)?.Value ?? 0.0m;
                if (numShares <= 0)
                {
                    return null;
                }

                return new Trade(trade.StockName, trade.BuySell, numShares);
            }

            return null;
        }

        /// <inheritdoc/>
        public bool AddTrade(DateTime time, Trade trade, SecurityTrade tradeConfirmation)
        {
            if (tradeConfirmation == null)
            {
                return false;
            }

            if (!Portfolio.Exists(Account.Security, trade.StockName))
            {
                _ = Portfolio.TryAdd(Account.Security, new NameData(trade.StockName.Company, trade.StockName.Name, trade.StockName.Currency, trade.StockName.Url, new HashSet<string>()), _logger);
            }
            _ = Portfolio.TryAddOrEditTradeData(Account.Security, trade.StockName, tradeConfirmation, tradeConfirmation);

            // Remove the cash used to buy the shares from the portfolio.
            decimal cashAvailable = Portfolio.TotalValue(Totals.BankAccount, time);
            decimal afterTradeCashValue = cashAvailable - trade.BuySell.Sign() * tradeConfirmation.TotalCost;
            var value = new DailyValuation(time, afterTradeCashValue);
            _ = Portfolio.TryAddOrEditData(Account.BankAccount, StartSettings.DefaultBankAccName, value, value, reportLogger: null);
            _logger.Log(ReportSeverity.Critical, ReportType.Warning, ReportLocation.Execution.ToString(), $"Date {time} bought {trade.StockName} Cost {tradeConfirmation.TotalCost:C2} price");
            return true;
        }

        public decimal AvailableFunds(DateTime time) => Portfolio.TotalValue(Totals.BankAccount, time);

        public void OnPriceUpdate(object obj, PriceUpdateEventArgs eventArgs)
        {
            TwoName updateName = eventArgs.Instrument.Name;
            if (updateName != null && Portfolio.Exists(Account.Security, updateName))
            {
                var valuation = new DailyValuation(eventArgs.Time, eventArgs.Price);
                _ = Portfolio.TryAddOrEditData(
                    Account.Security,
                    updateName,
                    valuation,
                    valuation,
                    _logger);
            }
        }

        public void ReportStatus(DateTime time)
        {
            if (time.Day != 1)
            {
                return;
            }

            _logger.Log(ReportType.Information, nameof(PortfolioManager), $"Date: {time}. TotalVal: {Portfolio.TotalValue(Totals.All):C2}. TotalCash: {Portfolio.TotalValue(Totals.BankAccount):C2}");
        }

        /// <inheritdoc/>
        public void UpdateData(DateTime day, IStockExchange exchange)
        {
            foreach (var security in Portfolio.Funds)
            {
                decimal value = exchange.GetValue(security.Names.ToTwoName(), day);
                if (!value.Equals(decimal.MinValue))
                {
                    security.SetData(day, value);
                }
            }
        }
    }
}