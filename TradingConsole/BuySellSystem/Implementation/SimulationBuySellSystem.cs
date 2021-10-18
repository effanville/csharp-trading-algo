using System;
using System.Collections.Generic;

using Common.Structure.DataStructures;
using Common.Structure.Reporting;

using FinancialStructures.Database;
using FinancialStructures.DataStructures;
using FinancialStructures.FinanceStructures;
using FinancialStructures.NamingStructures;
using FinancialStructures.StockStructures;
using TradingConsole.DecisionSystem;
using TradingConsole.DecisionSystem.Models;

namespace TradingConsole.BuySellSystem.Implementation
{
    /// <summary>
    /// Trading system for use in simulation systems.
    /// </summary>
    internal class SimulationBuySellSystem : ITradeMechanism
    {
        private IReportLogger ReportLogger
        {
            get;
        }

        /// <summary>
        /// Create an instance.
        /// </summary>
        internal SimulationBuySellSystem(IReportLogger reportLogger)
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
            double openPrice = exchange.GetValue(buy.StockName, time, StockDataStream.Open);

            // we modify the price we buy at from the opening price, to simulate market movement.
            double upDown = settings.RandomNumbers.Next(0, 100) > 100 * settings.UpTickProbability ? 1 : -1;
            double valueModifier = 1 + settings.UpTickSize * upDown;
            double priceToBuy = openPrice * valueModifier;

            double cashAvailable = portfolio.TotalValue(Totals.BankAccount, time);
            if (openPrice != 0 && cashAvailable > priceToBuy)
            {
                int numShares = 0;
                while (numShares * priceToBuy < traderOptions.FractionInvest * cashAvailable)
                {
                    numShares++;
                }
                numShares--;

                if (numShares != 0)
                {
                    var tradeDetails = new SecurityTrade(TradeType.Buy, buy.StockName, time, numShares, priceToBuy, traderOptions.TradeCost);
                    if (cashAvailable > tradeDetails.TotalCost)
                    {
                        if (!portfolio.Exists(Account.Security, buy.StockName))
                        {
                            _ = portfolio.TryAdd(Account.Security, new NameData(buy.StockName.Company, buy.StockName.Name, "GBP", buy.StockName.Url, new HashSet<string>()), ReportLogger);
                        }

                        // "Buy" the shares by adding the number of shares in the security desired. First must ensure we know the number of shares held.
                        _ = portfolio.TryGetAccount(Account.Security, buy.StockName, out var desired);
                        ISecurity security = desired as ISecurity;

                        double existingShares = security.Shares.ValueOnOrBefore(time)?.Value ?? 0.0;

                        _ = portfolio.TryAddOrEditDataToSecurity(buy.StockName, time, time, numShares + existingShares, priceToBuy, 1, tradeDetails, ReportLogger);

                        // Remove the cash used to buy the shares from the portfolio.
                        var value = new DailyValuation(time, cashAvailable - tradeDetails.TotalCost);
                        _ = portfolio.TryAddOrEditData(Account.BankAccount, settings.BankAccData, value, value, ReportLogger);

                        _ = ReportLogger.Log(ReportSeverity.Critical, ReportType.Warning, ReportLocation.Execution, $"Date {time} bought {buy.StockName} Cost {tradeDetails.TotalCost:C2} price");
                        return true;
                    }
                }
            }

            return false;
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
            if (sell.BuySell != TradeDecision.Sell)
            {
                return false;
            }

            _ = portfolio.TryGetAccount(Account.Security, sell.StockName, out var desired);
            ISecurity security = desired as ISecurity;

            double numShares = security.UnitPrice.ValueOnOrBefore(time)?.Value ?? 0.0;

            // One can only sell if one already owns some of the security.
            if (portfolio.Exists(Account.Security, sell.StockName) && numShares > 0)
            {
                // First calculate price that one sells at.
                // This is the open price of the stock, with a combat multiplier.
                double upDown = settings.RandomNumbers.Next(0, 100) > 100 * settings.UpTickProbability ? 1 : -1;
                double valueModifier = 1 + settings.UpTickSize * upDown;
                double price = exchange.GetValue(sell.StockName, time, StockDataStream.Open) * valueModifier;


                var tradeDetails = new SecurityTrade(TradeType.Sell, sell.StockName, time, numShares, price, traderOptions.TradeCost);

                // Now perform selling. This consists of removing the security at the specific value in our portfolio.
                _ = portfolio.TryAddOrEditDataToSecurity(sell.StockName, time, time, 0.0, price, 1, tradeDetails, ReportLogger);

                // Now increase the amount in the bank account, i.e. free cash, held in the portfolio, to free it up to be used on other securities.
                double cashAvailable = portfolio.TotalValue(Totals.BankAccount, time);
                var value = new DailyValuation(time, cashAvailable + tradeDetails.TotalCost);
                _ = portfolio.TryAddOrEditData(Account.BankAccount, settings.BankAccData, value, value, ReportLogger);

                _ = ReportLogger.Log(ReportSeverity.Critical, ReportType.Warning, ReportLocation.Execution, $"Date {time} sold {sell.StockName} for {value:C2}");
                return true;
            }

            return false;
        }
    }
}
