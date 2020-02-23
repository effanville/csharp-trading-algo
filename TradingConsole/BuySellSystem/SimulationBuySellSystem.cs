using FinancialStructures.Database;
using FinancialStructures.ReportingStructures;
using System;
using TradingConsole.Statistics;

namespace TradingConsole.BuySellSystem
{
    public class SimulationBuySellSystem : IBuySellSystem
    {
        public void BuySell(DateTime day, DecisionStatus status, ExchangeStocks stocks, Portfolio portfolio, TradingStatistics stats)
        {
            var sellDecisions = status.GetSellDecisions();

            foreach (var sell in sellDecisions)
            {
                SellHolding(day, sell, stocks, portfolio, stats);
            }

            var buyDecisions = status.GetBuyDecisions();

            foreach (var buy in buyDecisions)
            {
                BuyHolding(day, buy, stocks, portfolio, stats);
            }
        }

        private void SellHolding(DateTime day, Decision sell, ExchangeStocks stocks, Portfolio portfolio, TradingStatistics stats)
        {
            var reports = new ErrorReports();
            double price = stocks.GetValue(sell.StockName, day);
            portfolio.TryAddDataToSecurity(reports, sell.StockName.Company, sell.StockName.Name, day, 0.0, price);
            stats.AddTrade();
        }

        private void BuyHolding(DateTime day, Decision buy, ExchangeStocks stocks, Portfolio portfolio, TradingStatistics stats)
        {
            var reports = new ErrorReports();
            double price = stocks.GetValue(buy.StockName, day);
            double cashAvailable = portfolio.BankAccountValue(day);
            portfolio.TryAddDataToSecurity(reports, buy.StockName.Company, buy.StockName.Name, day, 0.0, price);
            stats.AddTrade();
        }
    }
}
