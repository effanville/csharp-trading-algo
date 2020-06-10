using System;
using FinancialStructures.Database;
using FinancialStructures.NamingStructures;
using FinancialStructures.PortfolioAPI;
using FinancialStructures.StockData;
using FinancialStructures.StockStructures;
using StructureCommon.DataStructures;
using StructureCommon.Reporting;
using TradingConsole.DecisionSystem;
using TradingConsole.Simulation;
using TradingConsole.Statistics;

namespace TradingConsole.BuySellSystem
{
    public class IBClientTradingSystem : BuySellBase
    {
        public IBClientTradingSystem(IReportLogger reportLogger)
            : base(reportLogger)
        {
        }

        public override void SellHolding(DateTime day, Decision sell, ExchangeStocks stocks, Portfolio portfolio, TradingStatistics stats, BuySellParams parameters, SimulationParameters simulationParameters)
        {
            double price = stocks.GetValue(sell.StockName, day);
            _ = portfolio.TryAddDataToSecurity(sell.StockName, day, 0.0, price, 1.0, ReportLogger);
            double numShares = portfolio.SecurityPrices(sell.StockName, day, SecurityDataStream.NumberOfShares);
            _ = portfolio.TryAddData(AccountType.BankAccount, simulationParameters.bankAccData, new DailyValuation(day, numShares * price - simulationParameters.tradeCost), ReportLogger);
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
                    _ = portfolio.TryAddDataToSecurity(buy.StockName, day, numShares, price, 1, ReportLogger);
                    _ = portfolio.TryAddData(AccountType.BankAccount, new NameData("Cash", "Portfolio"), new DailyValuation(day, cashAvailable - numShares * costOfPurchase), ReportLogger);
                    stats.AddTrade(new TradeDetails(TradeType.Buy, "", buy.StockName.Company, buy.StockName.Name, day, numShares * price, numShares, price, simulationParameters.tradeCost));
                }
            }
        }
    }
}

