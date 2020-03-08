using FinancialStructures.Database;
using FinancialStructures.GUIFinanceStructures;
using FinancialStructures.ReportingStructures;
using System;
using TradingConsole.Statistics;
using TradingConsole.DecisionSystem;
using TradingConsole.StockStructures;
using TradingConsole.Simulation;
using FinancialStructures.StockData;

namespace TradingConsole.BuySellSystem
{
    public class SimulationBuySellSystem : BuySellBase
    {
        public override void SellHolding(DateTime day, Decision sell, ExchangeStocks stocks, Portfolio portfolio, TradingStatistics stats, BuySellParams parameters, SimulationParameters simulationParameters)
        {
            var reports = new ErrorReports();
            double price = stocks.GetValue(sell.StockName, day);
            portfolio.TryAddDataToSecurity(reports, sell.StockName.Company, sell.StockName.Name, day, 0.0, price);
            double numShares = portfolio.SecurityShares(sell.StockName.Company, sell.StockName.Name, day);
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
                while (numShares * price < parameters.fractionInvest * cashAvailable)
                {
                    numShares++;
                }
                numShares--;

                if (numShares != 0)
                {
                    double costOfPurchase = numShares * price + simulationParameters.tradeCost;
                    if (cashAvailable > costOfPurchase)
                    {
                        if (!portfolio.DoesSecurityExist(buy.StockName.Company, buy.StockName.Name))
                        {
                            portfolio.TryAddSecurity(reports, buy.StockName.Company, buy.StockName.Name, "GBP", buy.StockName.Url, "");
                        }
                        double existingShares = portfolio.SecurityShares(buy.StockName.Company, buy.StockName.Name, day);
                        portfolio.TryAddDataToSecurity(reports, buy.StockName.Company, buy.StockName.Name, day, numShares + existingShares, price);
                        portfolio.TryAddDataToBankAccount(new NameData("Cash", "Portfolio"), new DayValue_ChangeLogged(day, cashAvailable - costOfPurchase), reports);
                        stats.AddTrade(new TradeDetails(TradeType.Buy, "", buy.StockName.Company, buy.StockName.Name, day, numShares * price, numShares, price, simulationParameters.tradeCost));
                    }
                }
            }
        }
    }
}
