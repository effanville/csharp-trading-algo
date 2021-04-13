using System;
using FinancialStructures.Database;
using FinancialStructures.NamingStructures;
using FinancialStructures.StockStructures;
using FinancialStructures.StockStructures.Implementation;
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

        public override void SellHolding(DateTime day, Decision sell, IStockExchange stocks, IPortfolio portfolio, TradingStatistics stats, BuySellParams parameters, SimulationParameters simulationParameters)
        {
            double price = stocks.GetValue(sell.StockName, day);
            _ = portfolio.TryAddOrEditDataToSecurity(sell.StockName, day, day, 0.0, price, 1.0, ReportLogger);
            double numShares = portfolio.SecurityPrices(sell.StockName, day, SecurityDataStream.NumberOfShares);
            var data = new DailyValuation(day, numShares * price - simulationParameters.tradeCost);
            _ = portfolio.TryAddOrEditData(Account.BankAccount, simulationParameters.bankAccData, data, data, ReportLogger);
            stats.AddTrade(new Trade(TradeType.Sell, "", sell.StockName.Company, sell.StockName.Name, day, numShares * price, numShares, price, simulationParameters.tradeCost));
        }

        public override void BuyHolding(DateTime day, Decision buy, IStockExchange stocks, IPortfolio portfolio, TradingStatistics stats, BuySellParams parameters, SimulationParameters simulationParameters)
        {
            double price = stocks.GetValue(buy.StockName, day);
            double cashAvailable = portfolio.TotalValue(Totals.BankAccount, day);
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
                    _ = portfolio.TryAddOrEditDataToSecurity(buy.StockName, day, day, numShares, price, 1, ReportLogger);
                    var data = new DailyValuation(day, cashAvailable - numShares * costOfPurchase);
                    _ = portfolio.TryAddOrEditData(Account.BankAccount, new NameData("Cash", "Portfolio"), data, data, ReportLogger);
                    stats.AddTrade(new Trade(TradeType.Buy, "", buy.StockName.Company, buy.StockName.Name, day, numShares * price, numShares, price, simulationParameters.tradeCost));
                }
            }
        }
    }
}

