using System;
using System.Collections.Generic;

using Common.Structure.DataStructures;
using Common.Structure.Reporting;

using FinancialStructures.Database;
using FinancialStructures.DataStructures;
using FinancialStructures.FinanceStructures;
using FinancialStructures.NamingStructures;

using TradingSystem.Decisions.Models;
using TradingSystem.Trading.Models;
using TradingSystem.Trading.System;

namespace TradingConsole.BuySellSystem.Implementation
{
    internal class IBClientTradingSystem : ITradeMechanism
    {
        internal IBClientTradingSystem()
        {
        }

        /// <inheritdoc/>
        public bool Buy(
            DateTime time,
            Decision buy,
            Func<DateTime, NameData, double> calculateBuyPrice,
            IPortfolio portfolio,
            TradeMechanismTraderOptions traderOptions,
            IReportLogger reportLogger)
        {
            double price = calculateBuyPrice(time, buy.StockName);
            double cashAvailable = portfolio.TotalValue(Totals.BankAccount, time);
            if (price != 0)
            {
                int numShares = 0;
                while (numShares * price < traderOptions.FractionInvest)
                {
                    numShares++;
                }
                numShares--;

                var trade = new SecurityTrade(TradeType.Buy, buy.StockName, time, numShares, price, traderOptions.TradeCost);
                if (cashAvailable > trade.TotalCost)
                {
                    if (!portfolio.Exists(Account.Security, buy.StockName))
                    {
                        _ = portfolio.TryAdd(Account.Security, new NameData(buy.StockName.Company, buy.StockName.Name, "GBP", buy.StockName.Url, new HashSet<string>()), reportLogger);
                    }

                    // "Buy" the shares by adding the number of shares in the security desired. First must ensure we know the number of shares held.
                    _ = portfolio.TryGetAccount(Account.Security, buy.StockName, out var desired);
                    ISecurity security = desired as ISecurity;

                    double existingShares = security.UnitPrice.ValueOnOrBefore(time).Value;

                    _ = portfolio.TryAddOrEditDataToSecurity(buy.StockName, time, time, numShares + existingShares, price, 1, trade, reportLogger);
                    var data = new DailyValuation(time, cashAvailable - trade.TotalCost);
                    _ = portfolio.TryAddOrEditData(Account.BankAccount, traderOptions.BankAccData, data, data, reportLogger);
                }
            }

            _ = reportLogger.Log(ReportSeverity.Critical, ReportType.Warning, ReportLocation.Execution, $"Date {time} bought {buy.StockName}");
            return true;
        }

        /// <inheritdoc/>
        public bool Sell(
            DateTime time,
            Decision sell,
            Func<DateTime, NameData, double> calculateSellPrice,
            IPortfolio portfolio,
            TradeMechanismTraderOptions traderOptions,
            IReportLogger reportLogger)
        {
            double cashAvailable = portfolio.TotalValue(Totals.BankAccount, time);
            double price = calculateSellPrice(time, sell.StockName);
            _ = portfolio.TryGetAccount(Account.Security, sell.StockName, out var desired);
            ISecurity security = desired as ISecurity;

            double numShares = security.UnitPrice.ValueOnOrBefore(time).Value;
            var trade = new SecurityTrade(TradeType.Sell, sell.StockName, time, numShares, price, traderOptions.TradeCost);
            _ = portfolio.TryAddOrEditDataToSecurity(sell.StockName, time, time, 0.0, price, 1.0, trade, reportLogger);
            var data = new DailyValuation(time, cashAvailable + trade.TotalCost);
            _ = portfolio.TryAddOrEditData(Account.BankAccount, traderOptions.BankAccData, data, data, reportLogger);
            _ = reportLogger.Log(ReportSeverity.Critical, ReportType.Warning, ReportLocation.Execution, $"Date {time} sold {sell.StockName}");
            return true;
        }

        /// <inheritdoc/>
        public TradeStatus EnactAllTrades(
            DateTime time,
            DecisionStatus decisions,
            Func<DateTime, NameData, double> calculateBuyPrice,
            Func<DateTime, NameData, double> calculateSellPrice,
            IPortfolio portfolio,
            TradeMechanismTraderOptions traderOptions,
            IReportLogger reportLogger)
        {
            List<Decision> sellDecisions = decisions.GetSellDecisions();
            int numberSells = 0;
            foreach (Decision sell in sellDecisions)
            {
                if (Sell(time, sell, calculateSellPrice, portfolio, traderOptions, reportLogger))
                {
                    numberSells++;
                }
            }

            int numberBuys = 0;
            List<Decision> buyDecisions = decisions.GetBuyDecisions();
            foreach (Decision buy in buyDecisions)
            {
                if (Buy(time, buy, calculateBuyPrice, portfolio, traderOptions, reportLogger))
                {
                    numberBuys++;
                }
            }

            return new TradeStatus(numberBuys, numberSells);
        }
    }
}

