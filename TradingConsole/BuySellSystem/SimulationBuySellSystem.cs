using System;
using System.Collections.Generic;
using FinancialStructures.Database;
using FinancialStructures.DataStructures;
using FinancialStructures.NamingStructures;
using FinancialStructures.StockStructures;
using StructureCommon.DataStructures;
using StructureCommon.Reporting;
using TradingConsole.DecisionSystem;
using TradingConsole.Simulation;
using TradingConsole.Statistics;

namespace TradingConsole.BuySellSystem
{
    /// <summary>
    /// Trading system for use in simulation systems.
    /// </summary>
    internal class SimulationBuySellSystem : BuySellBase
    {
        /// <summary>
        /// Create an instance.
        /// </summary>
        internal SimulationBuySellSystem(IReportLogger reportLogger)
            : base(reportLogger)
        {
        }

        /// <inheritdoc/>
        public override void SellHolding(DateTime day, Decision sell, IStockExchange stocks, IPortfolio portfolio, TradingStatistics stats, BuySellParams parameters, SimulationParameters simulationParameters)
        {
            // One can only sell if one already owns some of the security.
            if (portfolio.Exists(Account.Security, sell.StockName) && portfolio.SecurityPrices(sell.StockName, day, SecurityDataStream.NumberOfShares) > 0)
            {
                // First calculate price that one sells at.
                // This is the open price of the stock, with a combat multiplier.
                double upDown = simulationParameters.RandomNumbers.Next(0, 100) > 100 * simulationParameters.UpTickProbability ? 1 : -1;
                double valueModifier = 1 + simulationParameters.UpTickSize * upDown;
                double price = stocks.GetValue(sell.StockName, day, StockDataStream.Open) * valueModifier;

                double numShares = portfolio.SecurityPrices(sell.StockName, day, SecurityDataStream.NumberOfShares);
                var tradeDetails = new SecurityTrade(TradeType.Sell, sell.StockName, day, numShares, price, simulationParameters.TradeCost);

                // Now perform selling. This consists of removing the security at the specific value in our portfolio.
                // Note that the security in the portfolio does not take into account the cost of the trade
                _ = portfolio.TryAddOrEditDataToSecurity(sell.StockName, day, day, 0.0, price, 1, tradeDetails, ReportLogger);

                // Now increase the amount in the bank account, i.e. free cash, held in the portfolio, to free it up to be used on other securities.
                double cashAvailable = portfolio.TotalValue(Totals.BankAccount, day);
                var value = new DailyValuation(day, cashAvailable + tradeDetails.TotalCost);
                _ = portfolio.TryAddOrEditData(Account.BankAccount, simulationParameters.BankAccData, value, value, ReportLogger);
            }

            base.SellHolding(day, sell, stocks, portfolio, stats, parameters, simulationParameters);
        }

        /// <inheritdoc/>
        public override void BuyHolding(DateTime day, Decision buy, IStockExchange stocks, IPortfolio portfolio, TradingStatistics stats, BuySellParams parameters, SimulationParameters simulationParameters)
        {
            double openPrice = stocks.GetValue(buy.StockName, day, StockDataStream.Open);

            // we modify the price we buy at from the opening price, to simulate market movement.
            double upDown = simulationParameters.RandomNumbers.Next(0, 100) > 100 * simulationParameters.UpTickProbability ? 1 : -1;
            double valueModifier = 1 + simulationParameters.UpTickSize * upDown;
            double priceToBuy = openPrice * valueModifier;

            double cashAvailable = portfolio.TotalValue(Totals.BankAccount, day);
            if (openPrice != 0)
            {
                int numShares = 0;
                while (numShares * priceToBuy < parameters.fractionInvest * cashAvailable)
                {
                    numShares++;
                }
                numShares--;

                if (numShares != 0)
                {
                    var tradeDetails = new SecurityTrade(TradeType.Buy, buy.StockName, day, numShares, priceToBuy, simulationParameters.TradeCost);
                    if (cashAvailable > tradeDetails.TotalCost)
                    {
                        if (!portfolio.Exists(Account.Security, buy.StockName))
                        {
                            _ = portfolio.TryAdd(Account.Security, new NameData(buy.StockName.Company, buy.StockName.Name, "GBP", buy.StockName.Url, new HashSet<string>()), ReportLogger);
                        }

                        // "Buy" the shares by adding the number of shares in the security desired. First must ensure we know the number of shares held.
                        double existingShares = portfolio.SecurityPrices(buy.StockName, day, SecurityDataStream.NumberOfShares);

                        _ = portfolio.TryAddOrEditDataToSecurity(buy.StockName, day, day, numShares + existingShares, priceToBuy, 1, tradeDetails, ReportLogger);

                        // Remove the cash used to buy the shares from the portfolio.
                        var value = new DailyValuation(day, cashAvailable - tradeDetails.TotalCost);
                        _ = portfolio.TryAddOrEditData(Account.BankAccount, new NameData("Cash", "Portfolio"), value, value, ReportLogger);
                    }
                }
            }

            base.BuyHolding(day, buy, stocks, portfolio, stats, parameters, simulationParameters);
        }
    }
}
