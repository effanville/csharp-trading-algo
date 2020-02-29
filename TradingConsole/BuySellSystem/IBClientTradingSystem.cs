using FinancialStructures.Database;
using FinancialStructures.GUIFinanceStructures;
using FinancialStructures.ReportingStructures;
using System;
using TradingConsole.Statistics;
using TradingConsole.DecisionSystem;
using TradingConsole.StockStructures;
using FinancialStructures.StockData;
using TradingConsole.Simulation;

namespace TradingConsole.BuySellSystem
{
    public class IBClientTradingSystem : BuySellBase
    {
        public override void SellHolding(DateTime day, Decision sell, ExchangeStocks stocks, Portfolio portfolio, TradingStatistics stats, BuySellParams parameters, SimulationParameters simulationParameters)
        {
            var reports = new ErrorReports();
            double price = stocks.GetValue(sell.StockName, day);
            portfolio.TryAddDataToSecurity(reports, sell.StockName.Company, sell.StockName.Name, day, 0.0, price);
            double numShares = portfolio.SecurityLatestShares(sell.StockName.Company, sell.StockName.Name);
            portfolio.TryAddDataToBankAccount(simulationParameters.bankAccData, new DayValue_ChangeLogged(day, numShares * price - simulationParameters.tradeCost), reports);
            stats.AddTrade(new TradeDetails(TradeType.Sell, "", sell.StockName.Company, sell.StockName.Name, day, numShares * price, numShares, price, simulationParameters.tradeCost));
        }

        public override void BuyHolding(DateTime day, Decision buy, ExchangeStocks stocks, Portfolio portfolio, TradingStatistics stats, BuySellParams parameters, SimulationParameters simulationParameters)
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

                double costOfPurchase = numShares * price + simulationParameters.tradeCost;
                if (cashAvailable > costOfPurchase)
                {
                    portfolio.TryAddDataToSecurity(reports, buy.StockName.Company, buy.StockName.Name, day, numShares, price);
                    portfolio.TryAddDataToBankAccount(new NameData("Cash", "Portfolio"), new DayValue_ChangeLogged(day, cashAvailable - numShares * costOfPurchase), reports);
                    stats.AddTrade(new TradeDetails(TradeType.Buy, "", buy.StockName.Company, buy.StockName.Name, day, numShares * price, numShares, price, simulationParameters.tradeCost));
                }
            }
        }
    }
}

