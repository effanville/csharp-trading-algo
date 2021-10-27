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
    /// <summary>
    /// Trading system for use in simulation systems.
    /// </summary>
    internal class SimulationBuySellSystem : ITradeMechanism
    {

        /// <summary>
        /// Create an instance.
        /// </summary>
        internal SimulationBuySellSystem()
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
            // If not a buy then stop.
            if (buy.BuySell != TradeDecision.Buy)
            {
                return false;
            }

            // If not enough money to buy then exit.
            double priceToBuy = calculateBuyPrice(time, buy.StockName);
            if (priceToBuy.Equals(double.NaN))
            {
            }
            double cashAvailable = portfolio.TotalValue(Totals.BankAccount, time);
            if (priceToBuy == 0.0 || cashAvailable <= priceToBuy)
            {
                return false;
            }

            int numShares = 0;
            while (numShares * priceToBuy < traderOptions.FractionInvest * cashAvailable)
            {
                numShares++;
            }
            numShares--;

            if (numShares <= 0)
            {
                return false;
            }

            // If not enough money to deal with the total cost then exit.
            var tradeDetails = new SecurityTrade(TradeType.Buy, buy.StockName, time, numShares, priceToBuy, traderOptions.TradeCost);
            if (cashAvailable <= tradeDetails.TotalCost)
            {
                return false;
            }

            if (!portfolio.Exists(Account.Security, buy.StockName))
            {
                _ = portfolio.TryAdd(Account.Security, new NameData(buy.StockName.Company, buy.StockName.Name, "GBP", buy.StockName.Url, new HashSet<string>()), reportLogger);
            }

            // "Buy" the shares by adding the number of shares in the security desired. First must ensure we know the number of shares held.
            _ = portfolio.TryGetAccount(Account.Security, buy.StockName, out var desired);
            ISecurity security = desired as ISecurity;

            double existingShares = security.Shares.ValueOnOrBefore(time)?.Value ?? 0.0;

            _ = portfolio.TryAddOrEditDataToSecurity(buy.StockName, time, time, numShares + existingShares, priceToBuy, 1, tradeDetails, reportLogger: null);

            // Remove the cash used to buy the shares from the portfolio.
            var value = new DailyValuation(time, cashAvailable - tradeDetails.TotalCost);
            _ = portfolio.TryAddOrEditData(Account.BankAccount, traderOptions.BankAccData, value, value, reportLogger: null);

            _ = reportLogger.Log(ReportSeverity.Critical, ReportType.Warning, ReportLocation.Execution, $"Date {time} bought {buy.StockName} Cost {tradeDetails.TotalCost:C2} price");
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
            if (sell.BuySell != TradeDecision.Sell)
            {
                return false;
            }

            // One can only sell if one already owns some of the security.
            if (!portfolio.Exists(Account.Security, sell.StockName))
            {
                return false;
            }

            _ = portfolio.TryGetAccount(Account.Security, sell.StockName, out var desired);
            ISecurity security = desired as ISecurity;
            double numShares = security.Shares.ValueOnOrBefore(time)?.Value ?? 0.0;
            if (numShares <= 0)
            {
                return false;
            }

            double price = calculateSellPrice(time, sell.StockName);

            // some error with price data (or shouldnt be evaluating on this date) so ignore trade.
            if (price.Equals(double.NaN))
            {
                return false;
            }

            var tradeDetails = new SecurityTrade(TradeType.Sell, sell.StockName, time, numShares, price, traderOptions.TradeCost);

            // Now perform selling. This consists of removing the security at the specific value in our portfolio.
            _ = portfolio.TryAddOrEditDataToSecurity(sell.StockName, time, time, 0.0, price, 1, tradeDetails, reportLogger: null);

            // Now increase the amount in the bank account, i.e. free cash, held in the portfolio, to free it up to be used on other securities.
            double cashAvailable = portfolio.TotalValue(Totals.BankAccount, time);
            var value = new DailyValuation(time, cashAvailable + tradeDetails.TotalCost);
            _ = portfolio.TryAddOrEditData(Account.BankAccount, traderOptions.BankAccData, value, value, reportLogger: null);

            _ = reportLogger.Log(ReportSeverity.Critical, ReportType.Warning, ReportLocation.Execution, $"Date {time} sold {sell.StockName} for {tradeDetails.TotalCost:C2}");
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
