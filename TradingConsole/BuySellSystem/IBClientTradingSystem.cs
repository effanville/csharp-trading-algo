using FinancialStructures.Database;
using FinancialStructures.DataStructures;
using FinancialStructures.NamingStructures;
using FinancialStructures.PortfolioAPI;
using FinancialStructures.Reporting;
using FinancialStructures.StockData;
using FinancialStructures.StockStructures;
using System;
using TradingConsole.DecisionSystem;
using TradingConsole.Simulation;
using TradingConsole.Statistics;

namespace TradingConsole.BuySellSystem
{
    public class IBClientTradingSystem : BuySellBase
    {
        public IBClientTradingSystem(LogReporter reportLogger)
            : base(reportLogger)
        {
        }

        public override void SellHolding(DateTime day, Decision sell, ExchangeStocks stocks, Portfolio portfolio, TradingStatistics stats, BuySellParams parameters, SimulationParameters simulationParameters)
        {
            double price = stocks.GetValue(sell.StockName, day);
            portfolio.TryAddDataToSecurity(ReportLogger, sell.StockName, day, 0.0, price);
            double numShares = portfolio.SecurityShares(sell.StockName.Company, sell.StockName.Name, day);
            portfolio.TryAddData(AccountType.BankAccount, simulationParameters.bankAccData, new DayValue_ChangeLogged(day, numShares * price - simulationParameters.tradeCost), ReportLogger);
            stats.AddTrade(new TradeDetails(TradeType.Sell, "", sell.StockName.Company, sell.StockName.Name, day, numShares * price, numShares, price, simulationParameters.tradeCost));
        }

        public override void BuyHolding(DateTime day, Decision buy, ExchangeStocks stocks, Portfolio portfolio, TradingStatistics stats, BuySellParams parameters, SimulationParameters simulationParameters)
        {
            double price = stocks.GetValue(buy.StockName, day);
            double cashAvailable = portfolio.TotalValue(AccountType.BankAccount, day);
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
                    portfolio.TryAddDataToSecurity(ReportLogger, buy.StockName, day, numShares, price);
                    portfolio.TryAddData(AccountType.BankAccount, new NameData("Cash", "Portfolio"), new DayValue_ChangeLogged(day, cashAvailable - numShares * costOfPurchase), ReportLogger);
                    stats.AddTrade(new TradeDetails(TradeType.Buy, "", buy.StockName.Company, buy.StockName.Name, day, numShares * price, numShares, price, simulationParameters.tradeCost));
                }
            }
        }
    }
}

