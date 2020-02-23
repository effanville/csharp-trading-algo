using FinancialStructures.Database;
using FinancialStructures.GUIFinanceStructures;
using FinancialStructures.ReportingStructures;
using System;
using TradingConsole.Statistics;
using TradingConsole.DecisionSystem;
using TradingConsole.StockStructures;

namespace TradingConsole.BuySellSystem
{
    public class IBClientTradingSystem : IBuySellSystem
    {
        public void BuySell(DateTime day, DecisionStatus status, ExchangeStocks stocks, Portfolio portfolio, TradingStatistics stats, BuySellParams parameters)
        {
            var sellDecisions = status.GetSellDecisions();

            foreach (var sell in sellDecisions)
            {
                SellHolding(day, sell, stocks, portfolio, stats, parameters);
            }

            var buyDecisions = status.GetBuyDecisions();

            foreach (var buy in buyDecisions)
            {
                BuyHolding(day, buy, stocks, portfolio, stats, parameters);
            }
        }

        private void SellHolding(DateTime day, Decision sell, ExchangeStocks stocks, Portfolio portfolio, TradingStatistics stats, BuySellParams parameters)
        {
            var reports = new ErrorReports();
            double price = stocks.GetValue(sell.StockName, day);
            portfolio.TryAddDataToSecurity(reports, sell.StockName.Company, sell.StockName.Name, day, 0.0, price);
            stats.AddTrade();
        }

        private void BuyHolding(DateTime day, Decision buy, ExchangeStocks stocks, Portfolio portfolio, TradingStatistics stats, BuySellParams parameters)
        {
            var reports = new ErrorReports();
            double price = stocks.GetValue(buy.StockName, day);
            double cashAvailable = portfolio.BankAccountValue(day);
            if (price != 0)
            {
                int numShares = 0;
                while (numShares * price < parameters.fractionInvest)
                {
                    numShares++;
                }
                numShares--;

                double costOfPurchase = numShares * price + parameters.tradeCost;
                if (cashAvailable > costOfPurchase)
                {
                    portfolio.TryAddDataToSecurity(reports, buy.StockName.Company, buy.StockName.Name, day, numShares, price);
                    portfolio.TryAddDataToBankAccount(new NameData("Cash", "Portfolio"), new DayValue_ChangeLogged(day, cashAvailable - numShares * costOfPurchase), reports);
                    stats.AddTrade();
                }
            }
        }
    }
}

