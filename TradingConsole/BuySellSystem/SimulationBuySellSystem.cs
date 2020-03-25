using FinancialStructures.Database;
using FinancialStructures.PortfolioAPI;
using FinancialStructures.Reporting;
using System;
using TradingConsole.Statistics;
using TradingConsole.DecisionSystem;
using TradingConsole.StockStructures;
using TradingConsole.Simulation;
using FinancialStructures.StockData;
using FinancialStructures.DataStructures;
using FinancialStructures.NamingStructures;
using System.Collections.Generic;
using FinancialStructures.ReportLogging;

namespace TradingConsole.BuySellSystem
{
    public class SimulationBuySellSystem : BuySellBase
    {
        public SimulationBuySellSystem(LogReporter reportLogger)
            : base(reportLogger)
        {
        }

        public override void SellHolding(DateTime day, Decision sell, ExchangeStocks stocks, Portfolio portfolio, TradingStatistics stats, BuySellParams parameters, SimulationParameters simulationParameters)
        {
            var reports = new ErrorReports();
            double price = stocks.GetValue(sell.StockName, day);
            portfolio.TryAddDataToSecurity(ReportLogger, sell.StockName, day, 0.0, price);
            double numShares = portfolio.SecurityShares(sell.StockName.Company, sell.StockName.Name, day);
            portfolio.TryAddData(AccountType.BankAccount, simulationParameters.bankAccData, new DayValue_ChangeLogged(day, numShares * price - simulationParameters.tradeCost), ReportLogger);
            stats.AddTrade(new TradeDetails(TradeType.Sell, "", sell.StockName.Company, sell.StockName.Name, day, numShares * price, numShares, price, simulationParameters.tradeCost));
        }

        public override void BuyHolding(DateTime day, Decision buy, ExchangeStocks stocks, Portfolio portfolio, TradingStatistics stats, BuySellParams parameters, SimulationParameters simulationParameters)
        {
            var reports = new ErrorReports();
            double price = stocks.GetValue(buy.StockName, day);
            double cashAvailable = portfolio.TotalValue(AccountType.BankAccount, day);
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
                        if (!portfolio.Exists(AccountType.Security, buy.StockName))
                        {
                            portfolio.TryAdd(AccountType.Security, new NameData(buy.StockName.Company, buy.StockName.Name, "GBP", buy.StockName.Url, new HashSet<string>()), ReportLogger);
                        }
                        double existingShares = portfolio.SecurityShares(buy.StockName.Company, buy.StockName.Name, day);
                        portfolio.TryAddDataToSecurity(ReportLogger, buy.StockName, day, numShares + existingShares, price);
                        portfolio.TryAddData(AccountType.BankAccount, new NameData("Cash", "Portfolio"), new DayValue_ChangeLogged(day, cashAvailable - costOfPurchase), ReportLogger);
                        stats.AddTrade(new TradeDetails(TradeType.Buy, "", buy.StockName.Company, buy.StockName.Name, day, numShares * price, numShares, price, simulationParameters.tradeCost));
                    }
                }
            }
        }
    }
}
