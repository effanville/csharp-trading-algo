using FinancialStructures.Database;
using FinancialStructures.DataStructures;
using FinancialStructures.NamingStructures;
using FinancialStructures.PortfolioAPI;
using FinancialStructures.Reporting;
using FinancialStructures.StockData;
using FinancialStructures.StockStructures;
using System;
using System.Collections.Generic;
using TradingConsole.DecisionSystem;
using TradingConsole.Simulation;
using TradingConsole.Statistics;

namespace TradingConsole.BuySellSystem
{
    /// <summary>
    /// Trading system for use in simulation systems.
    /// </summary>
    public class SimulationBuySellSystem : BuySellBase
    {
        public SimulationBuySellSystem(LogReporter reportLogger)
            : base(reportLogger)
        {
        }

        public override void SellHolding(DateTime day, Decision sell, ExchangeStocks stocks, Portfolio portfolio, TradingStatistics stats, BuySellParams parameters, SimulationParameters simulationParameters)
        {
            // First calculate price that one sells at. 
            // This is the open price of the stock, with a combat multiplier.
            double upDown = simulationParameters.randomNumbers.Next(0, 100) > 100 * simulationParameters.UpTickProbability ? 1 : -1;
            double valueModifier = 1 + simulationParameters.UpTickSize * upDown;
            double price = stocks.GetValue(sell.StockName, day, DataStream.Open) * valueModifier;

            // Now perform selling. This consists of removing the security at the specific value in our portfolio.
            // Note that the security in the portfolio does not take into account the cost of the trade
            portfolio.TryAddDataToSecurity(ReportLogger, sell.StockName, day, 0.0, price);

            // Now increase the amount in the bank account, i.e. free cash, held in the portfolio, to free it up to be used on other securities.
            double numShares = portfolio.SecurityShares(sell.StockName.Company, sell.StockName.Name, day);
            portfolio.TryAddData(AccountType.BankAccount, simulationParameters.bankAccData, new DayValue_ChangeLogged(day, numShares * price - simulationParameters.tradeCost), ReportLogger);

            // record the trade in the statistics of the run.
            stats.AddTrade(new TradeDetails(TradeType.Sell, "", sell.StockName.Company, sell.StockName.Name, day, numShares * price, numShares, price, simulationParameters.tradeCost));
        }

        public override void BuyHolding(DateTime day, Decision buy, ExchangeStocks stocks, Portfolio portfolio, TradingStatistics stats, BuySellParams parameters, SimulationParameters simulationParameters)
        {
            double openPrice = stocks.GetValue(buy.StockName, day, DataStream.Open);

            // we modify the price we buy at from the opening price, to simulate market movement.
            double upDown = simulationParameters.randomNumbers.Next(0, 100) > 100 * simulationParameters.UpTickProbability ? 1 : -1;
            double valueModifier = 1 + simulationParameters.UpTickSize * upDown;
            double priceToBuy = openPrice * valueModifier;

            double cashAvailable = portfolio.TotalValue(AccountType.BankAccount, day);
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
                    double costOfPurchase = numShares * priceToBuy + simulationParameters.tradeCost;
                    if (cashAvailable > costOfPurchase)
                    {
                        if (!portfolio.Exists(AccountType.Security, buy.StockName))
                        {
                            portfolio.TryAdd(AccountType.Security, new NameData(buy.StockName.Company, buy.StockName.Name, "GBP", buy.StockName.Url, new HashSet<string>()), ReportLogger);
                        }

                        // "Buy" the shares by adding the number of shares in the security desired. First must ensure we know the number of shares held.
                        double existingShares = portfolio.SecurityShares(buy.StockName.Company, buy.StockName.Name, day);
                        portfolio.TryAddDataToSecurity(ReportLogger, buy.StockName, day, numShares + existingShares, priceToBuy);

                        // Remove the cash used to buy the shares from the portfolio.
                        portfolio.TryAddData(AccountType.BankAccount, new NameData("Cash", "Portfolio"), new DayValue_ChangeLogged(day, cashAvailable - costOfPurchase), ReportLogger);

                        // Add a log of the trade in the statistics.
                        stats.AddTrade(new TradeDetails(TradeType.Buy, "", buy.StockName.Company, buy.StockName.Name, day, numShares * priceToBuy, numShares, priceToBuy, simulationParameters.tradeCost));
                    }
                }
            }
        }
    }
}
