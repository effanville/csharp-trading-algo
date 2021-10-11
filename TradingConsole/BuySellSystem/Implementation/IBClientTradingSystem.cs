using System;
using System.Collections.Generic;
using Common.Structure.DataStructures;
using Common.Structure.Reporting;

using FinancialStructures.Database;
using FinancialStructures.DataStructures;
using FinancialStructures.FinanceStructures;
using FinancialStructures.NamingStructures;
using FinancialStructures.StockStructures;

using TradingConsole.DecisionSystem.Models;

namespace TradingConsole.BuySellSystem.Implementation
{
    internal class IBClientTradingSystem : ITradeMechanism
    {
        private IReportLogger ReportLogger
        {
            get;
        }

        internal IBClientTradingSystem(IReportLogger reportLogger)
        {
            ReportLogger = reportLogger;
        }

        /// <inheritdoc/>
        public bool Buy(
            DateTime time,
            Decision buy,
            IStockExchange exchange,
            IPortfolio portfolio,
            TradeMechanismSettings settings,
            TradeMechanismTraderOptions traderOptions)
        {
            double price = exchange.GetValue(buy.StockName, time);
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
                        _ = portfolio.TryAdd(Account.Security, new NameData(buy.StockName.Company, buy.StockName.Name, "GBP", buy.StockName.Url, new HashSet<string>()), ReportLogger);
                    }

                    // "Buy" the shares by adding the number of shares in the security desired. First must ensure we know the number of shares held.
                    _ = portfolio.TryGetAccount(Account.Security, buy.StockName, out var desired);
                    ISecurity security = desired as ISecurity;

                    double existingShares = security.UnitPrice.ValueOnOrBefore(time).Value;

                    _ = portfolio.TryAddOrEditDataToSecurity(buy.StockName, time, time, numShares + existingShares, price, 1, trade, ReportLogger);
                    var data = new DailyValuation(time, cashAvailable - trade.TotalCost);
                    _ = portfolio.TryAddOrEditData(Account.BankAccount, settings.BankAccData, data, data, ReportLogger);
                }
            }

            _ = ReportLogger.Log(ReportSeverity.Critical, ReportType.Warning, ReportLocation.Execution, $"Date {time} bought {buy.StockName}");
            return true;
        }

        /// <inheritdoc/>
        public bool Sell(
            DateTime time,
            Decision sell,
            IStockExchange exchange,
            IPortfolio portfolio,
            TradeMechanismSettings settings,
            TradeMechanismTraderOptions traderOptions)
        {
            double cashAvailable = portfolio.TotalValue(Totals.BankAccount, time);
            double price = exchange.GetValue(sell.StockName, time);
            _ = portfolio.TryGetAccount(Account.Security, sell.StockName, out var desired);
            ISecurity security = desired as ISecurity;

            double numShares = security.UnitPrice.ValueOnOrBefore(time).Value;
            var trade = new SecurityTrade(TradeType.Sell, sell.StockName, time, numShares, price, traderOptions.TradeCost);
            _ = portfolio.TryAddOrEditDataToSecurity(sell.StockName, time, time, 0.0, price, 1.0, trade, ReportLogger);
            var data = new DailyValuation(time, cashAvailable + trade.TotalCost);
            _ = portfolio.TryAddOrEditData(Account.BankAccount, settings.BankAccData, data, data, ReportLogger);
            _ = ReportLogger.Log(ReportSeverity.Critical, ReportType.Warning, ReportLocation.Execution, $"Date {time} sold {sell.StockName}");
            return true;
        }
    }
}

